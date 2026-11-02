#!/usr/bin/env node
// Map benchmark (MEM-E0-PLAT-05, D-192): opens a web build of app/benchmark/map_benchmark_main.dart in
// headless Chrome and checks SD3, SD8, QS3 and the privacy rule of D-032. No dependencies: Node's HTTP
// server and WebSocket talking to Chrome over the DevTools protocol.
//
//   node tools/map-benchmark/run.mjs [--runs=20] [--check] [--gpu] [--out=dir]
//   node tools/map-benchmark/run.mjs --serve [--port=8791]    only serves the base map, for devices
//
// CHROME = path to the browser. --check exits with 1 when a budget below is exceeded.
import { spawn } from 'node:child_process';
import { existsSync, mkdirSync, mkdtempSync, readFileSync, readdirSync, rmSync, statSync, writeFileSync } from 'node:fs';
import { createServer } from 'node:http';
import { tmpdir } from 'node:os';
import { dirname, extname, join, normalize, relative } from 'node:path';
import { fileURLToPath } from 'node:url';
import { gzipSync } from 'node:zlib';

const options = Object.fromEntries(
  process.argv.slice(2).map((argument) => {
    const [key, value = 'true'] = argument.replace(/^--/, '').split('=');
    return [key, value];
  }),
);
const runs = Number(options.runs ?? 20);

// QS3 is the requirement and only means something on a GPU. Software rendering (CI) gets a guard against
// regressions; the bundle budgets are the measured size plus a margin (D-192).
const budgets = {
  firstImageP95Ms: options.gpu ? 2000 : 15000, // map screen opened → base map and points on screen, assets cached
  movingFps: 50, // only with --gpu
  initialGzipMb: 3.5, // SD3: what the first visit downloads before the map opens
  mapGzipMb: 0.35, // SD3: MapLibre GL JS and pmtiles, loaded when the map opens
};
const pointsMinimum = 1;
const here = dirname(fileURLToPath(import.meta.url));
const buildDirectory = join(here, '../../app/build/web');
const fixtureDirectory = join(here, '../../app/benchmark/map');
const outDirectory = options.out ?? join(here, '../../app/build/map-benchmark');

const types = {
  '.html': 'text/html',
  '.js': 'text/javascript',
  '.mjs': 'text/javascript',
  '.css': 'text/css',
  '.json': 'application/json',
  '.wasm': 'application/wasm',
  '.png': 'image/png',
  '.pbf': 'application/x-protobuf',
  '.pmtiles': 'application/octet-stream',
};
const compressible = new Set(['.html', '.js', '.mjs', '.css', '.json', '.wasm']);

function listFiles(directory) {
  return readdirSync(directory).flatMap((name) => {
    const path = join(directory, name);
    return statSync(path).isDirectory() ? listFiles(path) : [path];
  });
}

const gzipCache = new Map();
function gzipped(path) {
  if (!gzipCache.has(path)) gzipCache.set(path, gzipSync(readFileSync(path), { level: 6 }));
  return gzipCache.get(path);
}

/// Like nginx in front of the app: gzip for text and range requests for the archive.
function startServer(port = 0, host = '127.0.0.1') {
  const server = createServer((request, response) => {
    const url = decodeURIComponent(new URL(request.url, 'http://x').pathname);
    const root = url.startsWith('/map/') ? fixtureDirectory : buildDirectory;
    const inside = normalize(url.startsWith('/map/') ? url.slice(5) : url === '/' ? 'index.html' : url.slice(1));
    const path = join(root, inside);
    if (relative(root, path).startsWith('..') || !existsSync(path) || !statSync(path).isFile()) {
      response.writeHead(404).end();
      return;
    }
    const extension = extname(path);
    const headers = { 'Content-Type': types[extension] ?? 'application/octet-stream', 'Cache-Control': 'no-cache' };
    const content = readFileSync(path);
    const range = /^bytes=(\d+)-(\d*)$/.exec(request.headers.range ?? '');
    if (range) {
      const start = Number(range[1]);
      const end = Math.min(range[2] ? Number(range[2]) : content.length - 1, content.length - 1);
      response.writeHead(206, { ...headers, 'Content-Range': `bytes ${start}-${end}/${content.length}` });
      response.end(content.subarray(start, end + 1));
    } else if (compressible.has(extension) && /gzip/.test(request.headers['accept-encoding'] ?? '')) {
      response.writeHead(200, { ...headers, 'Content-Encoding': 'gzip' }).end(gzipped(path));
    } else {
      response.writeHead(200, headers).end(content);
    }
  });
  return new Promise((resolve) => server.listen(port, host, () => resolve(server)));
}

function findChrome() {
  const candidates = [
    process.env.CHROME,
    'C:/Program Files/Google/Chrome/Application/chrome.exe',
    '/usr/bin/google-chrome',
    '/usr/bin/chromium-browser',
  ];
  const found = candidates.find((path) => path && existsSync(path));
  if (!found) throw new Error('Chrome not found; set CHROME');
  return found;
}

async function startChrome() {
  const profile = mkdtempSync(join(tmpdir(), 'map-benchmark-'));
  const chrome = spawn(
    findChrome(),
    [
      '--headless=new',
      '--remote-debugging-port=0',
      `--user-data-dir=${profile}`,
      '--no-first-run',
      '--no-sandbox',
      // No GPU on CI runners: software rendering, so frame rates there say nothing about devices (D-192).
      ...(options.gpu ? ['--use-angle=default'] : ['--use-angle=swiftshader', '--enable-unsafe-swiftshader']),
      '--ignore-gpu-blocklist',
      '--window-size=1280,800',
      'about:blank',
    ],
    { stdio: ['ignore', 'ignore', 'pipe'] },
  );
  const endpoint = await new Promise((resolve, reject) => {
    let text = '';
    chrome.stderr.on('data', (data) => {
      text += data;
      const match = /DevTools listening on (ws:\S+)/.exec(text);
      if (match) resolve(match[1]);
    });
    chrome.on('exit', () => reject(new Error(`Chrome exited: ${text}`)));
  });
  return {
    endpoint,
    stop() {
      chrome.removeAllListeners('exit');
      chrome.kill();
      setTimeout(() => rmSync(profile, { recursive: true, force: true, maxRetries: 5 }), 500).unref();
    },
  };
}

/// A minimal DevTools protocol client.
async function connect(endpoint) {
  const socket = new WebSocket(endpoint);
  await new Promise((resolve, reject) => {
    socket.onopen = resolve;
    socket.onerror = reject;
  });
  let next = 1;
  const pending = new Map();
  const listeners = [];
  socket.onmessage = (event) => {
    const message = JSON.parse(event.data);
    if (message.id && pending.has(message.id)) {
      const { resolve, reject } = pending.get(message.id);
      pending.delete(message.id);
      if (message.error) reject(new Error(JSON.stringify(message.error)));
      else resolve(message.result);
    } else {
      for (const listener of listeners) listener(message);
    }
  };
  return {
    send(method, params = {}, sessionId) {
      const id = next++;
      return new Promise((resolve, reject) => {
        pending.set(id, { resolve, reject });
        socket.send(JSON.stringify({ id, method, params, sessionId }));
      });
    },
    on: (listener) => listeners.push(listener),
    close: () => socket.close(),
  };
}

const delay = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

const frameCounter = `
  window.mapBenchmarkFrames = [];
  (function tick(time) { window.mapBenchmarkFrames.push(time); requestAnimationFrame(tick); })(0);
`;

/// One page load in a fresh browser context, so that nothing is cached unless [warm].
async function measure(client, origin, { warm, keepOpen }) {
  const { browserContextId } = await client.send('Target.createBrowserContext');
  const { targetId } = await client.send('Target.createTarget', { url: 'about:blank', browserContextId });
  const { sessionId } = await client.send('Target.attachToTarget', { targetId, flatten: true });
  const send = (method, params) => client.send(method, params, sessionId);

  const requests = new Map();
  const errors = [];
  client.on((message) => {
    if (message.sessionId !== sessionId) return;
    const { method, params } = message;
    if (method === 'Network.requestWillBeSent') {
      requests.set(params.requestId, { url: params.request.url, bytes: 0 });
    } else if (method === 'Network.responseReceived' && requests.has(params.requestId)) {
      requests.get(params.requestId).status = params.response.status;
    } else if (method === 'Network.loadingFinished' && requests.has(params.requestId)) {
      requests.get(params.requestId).bytes = params.encodedDataLength;
    } else if (method === 'Runtime.exceptionThrown') {
      errors.push(params.exceptionDetails.exception?.description ?? params.exceptionDetails.text);
    } else if (method === 'Runtime.consoleAPICalled' && params.type === 'error') {
      errors.push(params.args.map((argument) => argument.value ?? argument.description).join(' '));
    }
  });
  await send('Page.enable');
  await send('Runtime.enable');
  await send('Network.enable');
  await send('Page.addScriptToEvaluateOnNewDocument', { source: frameCounter });

  const open = async () => {
    await send('Page.navigate', { url: `${origin}/` });
    const deadline = Date.now() + 90000;
    for (;;) {
      const { result } = await send('Runtime.evaluate', {
        expression: 'JSON.stringify(window.mapBenchmark ?? {})',
        returnByValue: true,
      });
      const state = JSON.parse(result.value);
      if (state.moveEndedAt) return state;
      if (Date.now() > deadline) throw new Error(`no map after 90 s: ${JSON.stringify({ state, errors })}`);
      await delay(100);
    }
  };

  if (warm) {
    await open();
    requests.clear();
    errors.length = 0;
  }
  const state = await open();
  const { result: frames } = await send('Runtime.evaluate', {
    expression: `(() => {
      const inMove = window.mapBenchmarkFrames.filter(
        (time) => time >= ${state.moveStartedAt} && time <= ${state.moveEndedAt});
      const gaps = inMove.slice(1).map((time, index) => time - inMove[index]).sort((a, b) => a - b);
      return JSON.stringify({
        fps: gaps.length / ((${state.moveEndedAt} - ${state.moveStartedAt}) / 1000),
        worstFrameMs: gaps.at(-1) ?? 0,
      });
    })()`,
    returnByValue: true,
  });
  const result = {
    ...state,
    ...JSON.parse(frames.value),
    requests: [...requests.values()],
    errors: [...errors],
  };
  if (keepOpen) {
    result.screenshot = async () => {
      const { data } = await send('Page.captureScreenshot', { format: 'png' });
      const { result: pixel } = await send('Runtime.evaluate', {
        expression: `(async () => {
          const image = new Image();
          image.src = 'data:image/png;base64,${data}';
          await image.decode();
          const canvas = document.createElement('canvas');
          canvas.width = image.width;
          canvas.height = image.height;
          const context = canvas.getContext('2d');
          context.drawImage(image, 0, 0);
          const at = (x, y) => [...context.getImageData(x, y, 1, 1).data].slice(0, 3);
          return JSON.stringify({ corner: at(20, 90), centre: at(image.width / 2, image.height / 2) });
        })()`,
        awaitPromise: true,
        returnByValue: true,
      });
      return { png: Buffer.from(data, 'base64'), pixels: JSON.parse(pixel.value) };
    };
    result.close = () => client.send('Target.disposeBrowserContext', { browserContextId });
  } else {
    await client.send('Target.disposeBrowserContext', { browserContextId });
  }
  return result;
}

const percentile = (values, p) => {
  const sorted = [...values].sort((a, b) => a - b);
  return sorted[Math.min(sorted.length - 1, Math.ceil((p / 100) * sorted.length) - 1)];
};
const megabytes = (bytes) => bytes / 1e6;

if (options.serve) {
  const port = Number(options.port ?? 8791);
  await startServer(port, '0.0.0.0');
  console.log(`Base map on port ${port}, at /map/`);
  await new Promise(() => {});
}

if (!existsSync(join(buildDirectory, 'main.dart.js'))) {
  console.error('app/build/web is missing; run flutter build web (see app/benchmark/README.md)');
  process.exit(2);
}

const server = await startServer();
const origin = `http://127.0.0.1:${server.address().port}`;
const chrome = await startChrome();
const client = await connect(chrome.endpoint);
const failures = [];
const check = (name, ok, detail) => {
  console.log(`${ok ? 'ok  ' : 'FAIL'} ${name}: ${detail}`);
  if (!ok) failures.push(name);
};

try {
  mkdirSync(outDirectory, { recursive: true });

  const first = await measure(client, origin, { warm: false, keepOpen: true });
  const external = first.requests.filter(({ url }) => !url.startsWith(origin) && !/^(data|blob):/.test(url));
  const archive = first.requests.filter(({ url }) => url.includes('basemap.pmtiles'));
  const isMap = ({ url }) => /\/vendor\/maplibre\//.test(url);
  const isApp = (request) => !isMap(request) && !/\/map\//.test(request.url) && request.url.startsWith(origin);
  const sum = (requests) => requests.reduce((total, { bytes }) => total + bytes, 0);
  const { png, pixels } = await first.screenshot();
  writeFileSync(join(outDirectory, 'map.png'), png);
  await first.close();

  const warmTimes = [];
  const styleAndPoints = [];
  const idleTimes = [];
  const coldTimes = [];
  const fps = [];
  const worst = [];
  for (let run = 0; run < runs; run++) {
    const warm = await measure(client, origin, { warm: true });
    warmTimes.push(warm.pointsVisibleAt - warm.mapOpenedAt);
    styleAndPoints.push(warm.pointsAddedAt - warm.mapOpenedAt);
    idleTimes.push(warm.mapIdleAt - warm.mapOpenedAt);
    fps.push(warm.fps);
    worst.push(warm.worstFrameMs);
    console.log(`run ${run + 1}/${runs}: ${Math.round(warmTimes.at(-1))} ms, ${Math.round(warm.fps)} fps`);
  }
  for (let run = 0; run < Math.min(runs, 5); run++) {
    const cold = await measure(client, origin, { warm: false });
    coldTimes.push(cold.pointsVisibleAt);
  }

  const result = {
    runs,
    firstImageMs: { p50: percentile(warmTimes, 50), p95: percentile(warmTimes, 95) },
    mapOpenedToPointsAddedMs: { p50: percentile(styleAndPoints, 50), p95: percentile(styleAndPoints, 95) },
    mapIdleMs: { p50: percentile(idleTimes, 50), p95: percentile(idleTimes, 95) },
    coldLoadToPointsMs: { p50: percentile(coldTimes, 50), max: Math.max(...coldTimes) },
    fps: { p50: percentile(fps, 50), minimum: Math.min(...fps) },
    worstFrameMs: Math.max(...worst),
    renderedFeatures: first.renderedFeatures,
    initialGzipMb: megabytes(sum(first.requests.filter(isApp))),
    mapGzipMb: megabytes(sum(first.requests.filter(isMap))),
    tilesMb: megabytes(sum(archive)),
    rangeRequests: archive.filter(({ status }) => status === 206).length,
    pixels,
    errors: first.errors,
  };
  writeFileSync(join(outDirectory, 'result.json'), JSON.stringify(result, null, 2));
  console.log(JSON.stringify(result, null, 2));

  check('QS3/SD8 first image p95', result.firstImageMs.p95 <= budgets.firstImageP95Ms,
    `${Math.round(result.firstImageMs.p95)} ms (budget ${budgets.firstImageP95Ms})`);
  if (options.gpu) {
    check('QS3 smooth movement', result.fps.p50 >= budgets.movingFps,
      `${Math.round(result.fps.p50)} fps (budget ${budgets.movingFps}), worst frame ${Math.round(result.worstFrameMs)} ms`);
  }
  check('SD3 first visit', result.initialGzipMb <= budgets.initialGzipMb,
    `${result.initialGzipMb.toFixed(2)} MB gzip (budget ${budgets.initialGzipMb})`);
  check('SD3 map libraries', result.mapGzipMb <= budgets.mapGzipMb,
    `${result.mapGzipMb.toFixed(2)} MB gzip (budget ${budgets.mapGzipMb})`);
  check('points drawn', result.renderedFeatures >= pointsMinimum, `${result.renderedFeatures} clusters on screen`);
  check('base map by range requests', result.rangeRequests > 0, `${result.rangeRequests} responses 206`);
  check('D-032 nothing leaves the origin', external.length === 0,
    external.length === 0 ? 'no other host contacted' : external.map(({ url }) => url).join(', '));
  const ocean = [0xbc, 0xd9, 0xe6];
  check('globe on the web', pixels.corner.join() !== ocean.join(),
    `corner of the map outside the globe is rgb(${pixels.corner}), ocean is rgb(${ocean})`);
  check('no errors in the page', first.errors.length === 0, first.errors.join(' | ') || 'none');
} finally {
  client.close();
  chrome.stop();
  server.close();
}

if (options.check && failures.length > 0) {
  console.error(`Failed: ${failures.join(', ')}`);
  process.exit(1);
}
process.exit(0);
