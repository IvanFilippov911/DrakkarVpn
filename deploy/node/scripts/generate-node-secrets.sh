#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ENV_FILE="${ROOT_DIR}/env/node.env"
EXAMPLE_FILE="${ROOT_DIR}/env/node.env.example"

# shellcheck source=./lib.sh
source "${ROOT_DIR}/scripts/lib.sh"

need_root

[[ -f "${ENV_FILE}" ]] || cp "${EXAMPLE_FILE}" "${ENV_FILE}"

set -a
source "${ENV_FILE}"
set +a

[[ -n "${NODE_ID:-}" ]] || die "NODE_ID is required in ${ENV_FILE}"
[[ -n "${NODE_REGION:-}" ]] || die "NODE_REGION is required in ${ENV_FILE}"
[[ -n "${NODE_PUBLIC_HOST:-}" ]] || die "NODE_PUBLIC_HOST is required in ${ENV_FILE}"
[[ -n "${NODE_MAX_PEERS:-}" ]] || die "NODE_MAX_PEERS is required in ${ENV_FILE}"

cmd_exists docker || die "docker is required (install via bootstrap-node.sh)"
docker info >/dev/null 2>&1 || die "Docker daemon is not running (systemctl start docker)"
cmd_exists wg || die "wg binary is required"
cmd_exists python3 || die "python3 is required"

# Same image as compose xray service; generate Reality keys without a host xray binary.
XRAY_CORE_IMAGE="${XRAY_CORE_IMAGE:-ghcr.io/xtls/xray-core:latest}"

# Image entrypoint is already /usr/local/bin/xray; args replace default CMD (-confdir ...).
xray_docker() {
  docker run --rm "${XRAY_CORE_IMAGE}" "$@"
}

update_env() {
  local key="$1"
  local value="$2"

  if grep -q "^${key}=" "${ENV_FILE}"; then
    sed -i "s#^${key}=.*#${key}=${value}#g" "${ENV_FILE}"
  else
    printf '%s=%s\n' "${key}" "${value}" >> "${ENV_FILE}"
  fi
}

log "Generating Xray UUID..."
if grep -q '^XRAY_CLIENT_UUID=replace_me$' "${ENV_FILE}" || ! grep -q '^XRAY_CLIENT_UUID=' "${ENV_FILE}"; then
  update_env "XRAY_CLIENT_UUID" "$(cat /proc/sys/kernel/random/uuid)"
fi

log "Generating Xray Reality keys (docker image ${XRAY_CORE_IMAGE})..."
if grep -q '^XRAY_REALITY_PRIVATE_KEY=replace_me$' "${ENV_FILE}" || ! grep -q '^XRAY_REALITY_PRIVATE_KEY=' "${ENV_FILE}"; then
  REALITY_PRIVATE_KEY="$(xray_docker x25519 | head -n1 | awk '{print $2}')"
  [[ -n "${REALITY_PRIVATE_KEY}" ]] || die "Failed to generate XRAY_REALITY_PRIVATE_KEY"
  update_env "XRAY_REALITY_PRIVATE_KEY" "${REALITY_PRIVATE_KEY}"

  X25519_OUT="$(xray_docker x25519 -i "${REALITY_PRIVATE_KEY}" 2>&1)"
  REALITY_PUBLIC_KEY="$(echo "${X25519_OUT}" | awk '{print $2}' | head -n1)"
  [[ -n "${REALITY_PUBLIC_KEY}" ]] || die "Failed to generate XRAY_REALITY_PUBLIC_KEY"
  update_env "XRAY_REALITY_PUBLIC_KEY" "${REALITY_PUBLIC_KEY}"
fi

log "Generating Xray short id..."
if grep -q '^XRAY_REALITY_SHORT_ID=replace_me$' "${ENV_FILE}" || ! grep -q '^XRAY_REALITY_SHORT_ID=' "${ENV_FILE}"; then
  update_env "XRAY_REALITY_SHORT_ID" "$(openssl rand -hex 8)"
fi

log "Generating agent HMAC secret..."
if grep -q '^DRAKKAR_AGENT_HMAC_SECRET=replace_me$' "${ENV_FILE}" || ! grep -q '^DRAKKAR_AGENT_HMAC_SECRET=' "${ENV_FILE}"; then
  update_env "DRAKKAR_AGENT_HMAC_SECRET" "$(openssl rand -hex 32)"
fi

if [[ "${WG_ENABLE:-false}" == "true" ]]; then
  log "Generating WireGuard values..."

  if grep -q '^WG_IP=$' "${ENV_FILE}" || ! grep -q '^WG_IP=' "${ENV_FILE}"; then
    update_env "WG_IP" "$(calc_wg_ip "${NODE_ID}")"
  fi

  umask 077
  WG_PRIV_FILE="/etc/wireguard/privatekey"
  WG_PUB_FILE="/etc/wireguard/publickey"

  if [[ ! -f "${WG_PRIV_FILE}" || ! -f "${WG_PUB_FILE}" ]]; then
    wg genkey | tee "${WG_PRIV_FILE}" | wg pubkey > "${WG_PUB_FILE}"
  fi

  update_env "WG_AGENT_PRIVATE_KEY" "$(cat "${WG_PRIV_FILE}")"
  update_env "WG_AGENT_PUBLIC_KEY" "$(cat "${WG_PUB_FILE}")"
fi

log "Secrets generation complete: ${ENV_FILE}"