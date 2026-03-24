#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ENV_FILE="${ROOT_DIR}/env/node.env"

# shellcheck source=./lib.sh
source "${ROOT_DIR}/scripts/lib.sh"

need_root

[[ -f "${ENV_FILE}" ]] || die "Missing ${ENV_FILE}"

set -a
source "${ENV_FILE}"
set +a

log "Rendering xray config..."
envsubst < "${ROOT_DIR}/configs/xray/config.json.template" > "${ROOT_DIR}/configs/xray/config.json"
python3 -m json.tool "${ROOT_DIR}/configs/xray/config.json" >/dev/null

if [[ "${WG_ENABLE:-false}" == "true" ]]; then
  log "Rendering wireguard config..."
  envsubst < "${ROOT_DIR}/configs/wg/wg0.conf.template" > "${ROOT_DIR}/configs/wg/wg0.conf"
  install -m 600 "${ROOT_DIR}/configs/wg/wg0.conf" /etc/wireguard/wg0.conf
fi

log "Config rendering complete."