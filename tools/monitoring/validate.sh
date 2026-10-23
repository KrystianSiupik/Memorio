#!/usr/bin/env bash
# Checks every observability configuration (MEM-E0-PLAT-10) with the tools of the pinned images:
# Prometheus configuration and alert rules (with their unit tests), Alertmanager, the OpenTelemetry
# Collector and Loki. Run from the repository root; needs Docker. In Git Bash on Windows prefix with
# MSYS_NO_PATHCONV=1.
set -euo pipefail

root="$(cd "$(dirname "$0")/../.." && pwd)"
monitoring="$root/monitoring"
prometheus_image=$(grep -oE 'prom/prometheus:[^ ]+' "$root/compose.yaml")
alertmanager_image=$(grep -oE 'prom/alertmanager:[^ ]+' "$root/compose.yaml")
collector_image=$(grep -oE 'otel/opentelemetry-collector-contrib:[^ ]+' "$root/compose.yaml")
loki_image=$(grep -oE 'grafana/loki:[^ ]+' "$root/compose.yaml")

echo "Prometheus configuration and rules ($prometheus_image)"
docker run --rm --entrypoint promtool \
  -v "$monitoring/prometheus/prometheus.yml:/etc/prometheus/prometheus.yml:ro" \
  -v "$monitoring/prometheus/rules:/etc/prometheus/rules:ro" \
  -v "$monitoring/prometheus/targets/local:/etc/prometheus/targets:ro" \
  "$prometheus_image" check config /etc/prometheus/prometheus.yml

echo "Alert rule tests"
docker run --rm --entrypoint promtool -v "$monitoring/prometheus:/p:ro" \
  "$prometheus_image" test rules /p/tests/memorio.test.yml

echo "Alertmanager ($alertmanager_image)"
docker run --rm --entrypoint amtool \
  -v "$monitoring/alertmanager/alertmanager.yml:/etc/alertmanager/alertmanager.yml:ro" \
  "$alertmanager_image" check-config /etc/alertmanager/alertmanager.yml

echo "OpenTelemetry Collector ($collector_image)"
docker run --rm -e LOKI_OTLP_ENDPOINT=http://loki:3100/otlp \
  -v "$monitoring/otel-collector.yaml:/etc/otelcol/config.yaml:ro" \
  "$collector_image" validate --config=/etc/otelcol/config.yaml

echo "Loki ($loki_image)"
docker run --rm -v "$monitoring/loki/loki.yaml:/etc/loki/loki.yaml:ro" \
  "$loki_image" -config.file=/etc/loki/loki.yaml -verify-config

echo "All observability configurations are valid."
