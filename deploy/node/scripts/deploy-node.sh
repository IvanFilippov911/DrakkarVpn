#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ENV_FILE="${ROOT_DIR}/env/node.env"
COMPOSE_FILE="${ROOT_DIR}/compose/docker-compose.node.yml"

# shellcheck source=./lib.sh
source "${ROOT_DIR}/scripts/lib.sh"

need_root
[[ -f "${ENV_FILE}" ]] || die "Missing ${ENV_FILE}"

set -a
source "${ENV_FILE}"
set +a

if [[ "${WG_ENABLE:-false}" == "true" ]]; then
  log "Starting WireGuard..."
  systemctl enable wg-quick@wg0 >/dev/null
  systemctl restart wg-quick@wg0

  ip -4 addr show dev wg0 | grep -q "${WG_IP}" || die "WireGuard wg0 did not get IP ${WG_IP}"
  ping -c 1 -W 1 "${WG_CORE_IP}" >/dev/null 2>&1 || warn "WG ping to core (${WG_CORE_IP}) failed"

  log "Allowing WireGuard UDP in UFW (${WG_AGENT_PORT:-51820})..."
  ufw allow "${WG_AGENT_PORT:-51820}/udp" comment drakkar-wg >/dev/null 2>&1 || warn "ufw allow WG UDP failed (configure firewall manually)"

  export AGENT_HOST_BIND="${WG_IP}"
else
  export AGENT_HOST_BIND="${AGENT_HOST_BIND:-127.0.0.1}"
fi

log "Starting docker compose stack..."
docker compose \
  --env-file "${ENV_FILE}" \
  -f "${COMPOSE_FILE}" \
  up -d

log "Running self-checks..."
ss -ltn | grep -q ':443' || die "xray not listening on :443"

if [[ "${WG_ENABLE:-false}" == "true" ]]; then
  AGENT_METRICS_URL="http://${WG_IP}:5000/metrics"
else
  AGENT_METRICS_URL="http://127.0.0.1:5000/metrics"
fi

for _ in {1..20}; do
  curl -fsS "${AGENT_METRICS_URL}" >/dev/null 2>&1 && break || true
  sleep 1
done

code="$(curl -sS -o /dev/null -w "%{http_code}" --max-time 2 "${AGENT_METRICS_URL}" || echo 000)"
if [[ "${code}" != "200" && "${code}" != "401" && "${code}" != "403" ]]; then
  warn "Agent HTTP check failed (code=${code})"
fi

AGENT_BASE_URL="http://127.0.0.1:5000"
if [[ "${WG_ENABLE:-false}" == "true" ]]; then
  AGENT_BASE_URL="http://${WG_IP}:5000"
fi

log "Manifest below:"
cat <<EOF
{
  "name": "${NODE_ID}",
  "region": "${NODE_REGION}",
  "publicHost": "${NODE_PUBLIC_HOST}",
  "agentBaseUrl": "${AGENT_BASE_URL}",
  "agentAuth": {
    "scheme": "${DRAKKAR_AGENT_AUTH_SCHEME}",
    "hmacSecret": "${DRAKKAR_AGENT_HMAC_SECRET}",
    "timestampSkewSec": ${DRAKKAR_AGENT_TIMESTAMP_SKEW_SEC}
  },
  "maxPeers": ${NODE_MAX_PEERS},
  "xray": {
    "transport": "reality",
    "port": 443,
    "uuid": "${XRAY_CLIENT_UUID}",
    "realityPublicKey": "${XRAY_REALITY_PUBLIC_KEY}",
    "realityShortId": "${XRAY_REALITY_SHORT_ID}",
    "realityDest": "${XRAY_REALITY_DEST}",
    "realitySni": "${XRAY_REALITY_SNI}",
    "inboundTag": "${XRAY_INBOUND_TAG}",
    "stats": {
      "enabled": true,
      "userLevel": 0,
      "perUser": ["uplink", "downlink", "online"],
      "perInbound": ["uplink", "downlink"]
    }
  }
}
EOF

if [[ "${WG_ENABLE:-false}" == "true" ]]; then
  log "Core peer snippet:"
  cat <<EOF
[Peer]
PublicKey = ${WG_AGENT_PUBLIC_KEY}
AllowedIPs = ${WG_IP}/32
PersistentKeepalive = 25
EOF
fi