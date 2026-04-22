import type { ProbeNode, ProbeNodeStatus } from '../../../entities/probes'
import type { ProbeRow } from './types'

function statusLabel(status: ProbeNodeStatus): string {
  switch (status) {
    case 1:
      return 'Healthy'
    case 2:
      return 'Offline'
    case 3:
      return 'Disabled'
    default:
      return 'Unknown'
  }
}

function formatDateTime(value: Date | null): string {
  if (!value) return '—'
  // compact and readable; no locale-specific seconds spam
  return value.toISOString().replace('T', ' ').slice(0, 16) + 'Z'
}

export function mapProbesToRows(items: ProbeNode[]): ProbeRow[] {
  return items.map((x) => ({
    id: x.id,
    name: x.name,
    region: x.region,
    host: x.host,
    status: statusLabel(x.status),
    enabled: x.isEnabled,
    lastSeen: formatDateTime(x.lastSeenAtUtc),
    updated: formatDateTime(x.updatedAtUtc),
  }))
}

