#!/usr/bin/env node
// Builds the small base map the map benchmark serves (MEM-E0-PLAT-05): a PMTiles v3 archive with the
// real low-zoom tiles of a Protomaps world build (OpenStreetMap data, ODbL), read with range requests.
// The result is committed, so CI needs neither this script nor the network.
//
//   node tools/map-benchmark/make-fixture.mjs <yyyymmdd> [maxZoom=3]
import { mkdirSync, writeFileSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { createHash } from 'node:crypto';
import { fileURLToPath } from 'node:url';
import { gunzipSync, gzipSync } from 'node:zlib';

const [date, maxZoomArgument = '3'] = process.argv.slice(2);
if (!/^\d{8}$/.test(date ?? '')) {
  console.error('usage: make-fixture.mjs <yyyymmdd> [maxZoom]');
  process.exit(2);
}
const maxZoom = Number(maxZoomArgument);
const source = `https://build.protomaps.com/${date}.pmtiles`;
const output = join(dirname(fileURLToPath(import.meta.url)), '../../app/benchmark/map');

async function range(offset, length) {
  const response = await fetch(source, { headers: { Range: `bytes=${offset}-${offset + length - 1}` } });
  if (response.status !== 206) throw new Error(`${source}: HTTP ${response.status}`);
  return Buffer.from(await response.arrayBuffer());
}

function zxyToTileId(z, x, y) {
  const n = 2 ** z;
  let id = (4 ** z - 1) / 3;
  for (let s = n >> 1; s > 0; s >>= 1) {
    const rx = (x & s) > 0 ? 1 : 0;
    const ry = (y & s) > 0 ? 1 : 0;
    id += s * s * ((3 * rx) ^ ry);
    if (ry === 0) {
      if (rx === 1) [x, y] = [n - 1 - x, n - 1 - y];
      [x, y] = [y, x];
    }
  }
  return id;
}

function readDirectory(compressed) {
  const bytes = gunzipSync(compressed);
  let position = 0;
  const varint = () => {
    let value = 0;
    let scale = 1;
    for (;;) {
      const byte = bytes[position++];
      value += (byte & 0x7f) * scale;
      if (byte < 0x80) return value;
      scale *= 128;
    }
  };
  const count = varint();
  const entries = Array.from({ length: count }, () => ({}));
  let id = 0;
  for (const entry of entries) entry.tileId = id += varint();
  for (const entry of entries) entry.runLength = varint();
  for (const entry of entries) entry.length = varint();
  entries.forEach((entry, index) => {
    const value = varint();
    entry.offset = value === 0 ? entries[index - 1].offset + entries[index - 1].length : value - 1;
  });
  return entries;
}

function writeDirectory(entries) {
  const bytes = [];
  const varint = (value) => {
    while (value >= 0x80) {
      bytes.push((value % 128) | 0x80);
      value = Math.floor(value / 128);
    }
    bytes.push(value);
  };
  varint(entries.length);
  entries.forEach((entry, index) => varint(entry.tileId - (index === 0 ? 0 : entries[index - 1].tileId)));
  for (const entry of entries) varint(entry.runLength);
  for (const entry of entries) varint(entry.length);
  for (const entry of entries) varint(entry.offset + 1);
  return gzipSync(Buffer.from(bytes), { level: 9 });
}

const header = await range(0, 127);
if (header.subarray(0, 7).toString() !== 'PMTiles' || header[7] !== 3) throw new Error('not PMTiles v3');
const u64 = (offset) => Number(header.readBigUInt64LE(offset));
const leavesOffset = u64(40);
const tilesOffset = u64(56);
const root = readDirectory(await range(u64(8), u64(16)));

async function findTile(entries, id) {
  let found;
  for (const entry of entries) {
    if (entry.tileId > id) break;
    found = entry;
  }
  if (!found) return undefined;
  if (found.runLength === 0) {
    return findTile(readDirectory(await range(leavesOffset + found.offset, found.length)), id);
  }
  return id < found.tileId + found.runLength ? found : undefined;
}

// Oceans repeat, so the same content is stored once and addressed by several entries.
const contents = new Map();
const entries = [];
const tiles = [];
let offset = 0;
for (let z = 0; z <= maxZoom; z++) {
  for (let x = 0; x < 2 ** z; x++) {
    for (let y = 0; y < 2 ** z; y++) {
      const id = zxyToTileId(z, x, y);
      const entry = await findTile(root, id);
      if (!entry) continue;
      const tile = await range(tilesOffset + entry.offset, entry.length);
      const hash = createHash('sha256').update(tile).digest('hex');
      if (!contents.has(hash)) {
        contents.set(hash, offset);
        tiles.push(tile);
        offset += tile.length;
      }
      entries.push({ tileId: id, runLength: 1, length: tile.length, offset: contents.get(hash) });
    }
  }
}
entries.sort((a, b) => a.tileId - b.tileId);

const directory = writeDirectory(entries);
const metadata = gzipSync(Buffer.from(JSON.stringify({ attribution: '© OpenStreetMap contributors', source })));
const result = Buffer.alloc(127);
result.write('PMTiles');
result[7] = 3;
const set = (position, value) => result.writeBigUInt64LE(BigInt(value), position);
set(8, 127);
set(16, directory.length);
set(24, 127 + directory.length);
set(32, metadata.length);
set(40, 127 + directory.length + metadata.length);
set(48, 0);
set(56, 127 + directory.length + metadata.length);
set(64, offset);
set(72, entries.length);
set(80, entries.length);
set(88, tiles.length);
result[96] = 0; // not clustered: repeated contents point backwards
result[97] = 2; // directories in gzip
result[98] = header[98];
result[99] = header[99];
result[100] = 0;
result[101] = maxZoom;
result.writeInt32LE(-180e7, 102);
result.writeInt32LE(-85e7, 106);
result.writeInt32LE(180e7, 110);
result.writeInt32LE(85e7, 114);

mkdirSync(output, { recursive: true });
writeFileSync(join(output, 'basemap.pmtiles'), Buffer.concat([result, directory, metadata, ...tiles]));
console.log(`${entries.length} tiles, ${tiles.length} distinct, ${(offset / 1e6).toFixed(1)} MB, compression ${header[98]}`);
