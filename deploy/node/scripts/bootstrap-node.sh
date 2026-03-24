#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
# shellcheck source=./lib.sh
source "${ROOT_DIR}/scripts/lib.sh"

need_root
export DEBIAN_FRONTEND=noninteractive

log "apt update..."
apt-get update -y

log "Installing base packages..."
apt-get install -y \
  curl wget unzip htop git jq \
  ufw ca-certificates openssl python3 gettext-base \
  wireguard resolvconf docker.io || die "Failed to install docker.io"

if apt-get install -y docker-compose-v2; then
  log "Installed docker-compose-v2"
elif apt-get install -y docker-compose-plugin; then
  log "Installed docker-compose-plugin"
else
  warn "Compose plugin package not found, trying legacy docker-compose"
  apt-get install -y docker-compose || die "Failed to install any docker compose package"
fi

log "Applying sysctl tuning..."
cat > /etc/sysctl.d/99-drakkar.conf <<'EOF'
net.ipv4.ip_forward = 1
net.core.default_qdisc = fq
net.ipv4.tcp_congestion_control = bbr
net.ipv4.tcp_mtu_probing = 1
net.ipv4.tcp_slow_start_after_idle = 0
net.core.rmem_max = 2500000
net.core.wmem_max = 2500000
net.ipv4.tcp_rmem = 4096 87380 2500000
net.ipv4.tcp_wmem = 4096 16384 2500000
EOF

sysctl --system >/dev/null

log "Configuring UFW..."
ufw --force reset >/dev/null || true
ufw default deny incoming
ufw default allow outgoing
ufw allow 22/tcp
ufw allow 443/tcp
ufw --force enable >/dev/null

log "Ensuring docker is enabled..."
systemctl enable docker >/dev/null
systemctl restart docker

log "Ensuring required directories..."
mkdir -p /etc/drakkar
mkdir -p /etc/wireguard
chmod 700 /etc/wireguard

log "Bootstrap complete."