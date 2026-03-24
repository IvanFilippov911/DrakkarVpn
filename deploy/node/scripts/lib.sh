#!/usr/bin/env bash

log()  { echo "[$(date -u +'%Y-%m-%dT%H:%M:%SZ')] $*"; }
warn() { echo "[$(date -u +'%Y-%m-%dT%H:%M:%SZ')] WARN: $*" >&2; }
die()  { echo "[$(date -u +'%Y-%m-%dT%H:%M:%SZ')] ERROR: $*" >&2; exit 1; }

need_root() {
  [[ "${EUID:-$(id -u)}" -eq 0 ]] || die "Run as root"
}

cmd_exists() {
  command -v "$1" >/dev/null 2>&1
}

backup_file() {
  local f="$1"
  [[ -f "$f" ]] || return 0
  local ts
  ts="$(date -u +'%Y%m%d%H%M%S')"
  cp -a "$f" "${f}.bak.${ts}"
}

calc_wg_ip() {
  local id="$1"
  local h
  h="$(printf '%s' "$id" | sha256sum | awk '{print $1}')"

  local b1=$(( 0x${h:0:2} ))
  local b2=$(( 0x${h:2:2} ))

  local o3=$(( (b1 % 249) + 2 ))
  local o4=$(( (b2 % 249) + 2 ))

  echo "10.10.${o3}.${o4}"
}