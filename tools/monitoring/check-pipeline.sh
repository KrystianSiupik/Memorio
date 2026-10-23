#!/usr/bin/env bash
# End-to-end check of observability (MEM-E0-PLAT-10) on a running local stack
# (docker compose --profile app --profile monitoring up -d): the worker's backlog metric (QS12) and the
# API's HTTP metrics reach Prometheus through the collector, the external probes see both hosts, the
# media bucket refuses anonymous reads (SD5), the alert rules are loaded, and the hosts' logs reach
# Loki. Waits up to CHECK_TIMEOUT seconds (default 240) for the first export.
set -euo pipefail

prometheus="${PROMETHEUS_URL:-http://localhost:9090}"
grafana="${GRAFANA_URL:-http://localhost:3000}"
grafana_auth="admin:${GRAFANA_PASSWORD:-memorio_local}"
timeout="${CHECK_TIMEOUT:-240}"

query() { curl -fsS --get "$prometheus/api/v1/query" --data-urlencode "query=$1"; }
has_result() { query "$1" | grep -q '"result":\[{'; }
loki_has() {
  curl -fsS -u "$grafana_auth" --get "$grafana/api/datasources/proxy/uid/loki/loki/api/v1/query_range" \
    --data-urlencode "query=$1" --data-urlencode "limit=1" | grep -q '"values":\[\['
}

eventually() {
  local description="$1"
  shift
  local deadline=$((SECONDS + timeout))
  until "$@"; do
    if ((SECONDS > deadline)); then
      echo "FAIL: $description"
      exit 1
    fi
    curl -fs -o /dev/null http://localhost:${API_PORT:-8080}/health || true   # some API traffic to measure
    sleep 5
  done
  echo "ok: $description"
}

eventually "worker backlog metric (QS12) in Prometheus" has_result 'memorio_messaging_pending_oldest_age_seconds{job="memorio-worker"}'
eventually "API HTTP metrics in Prometheus" has_result 'http_server_request_duration_seconds_count{job="memorio-api"}'
eventually "account count (SD14) in Prometheus" has_result 'memorio_accounts_count'
eventually "external probes see both hosts ready" has_result 'count(probe_success{job="ready"} == 1) == 2'
eventually "the media bucket refuses anonymous reads (SD5)" has_result 'count(probe_success{job="bucket-private"} == 1) == 2'
eventually "alert rules loaded" bash -c "curl -fsS '$prometheus/api/v1/rules' | grep -q MessageBacklogTooOld"
eventually "worker logs in Loki" loki_has '{service_name="memorio-worker"}'
eventually "API logs in Loki" loki_has '{service_name="memorio-api"}'
echo "Observability pipeline works."
