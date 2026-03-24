import type { ServerCard, ServersOverview } from '../../../entities/server/types'
import type { ServerRow } from './types'
import type { ServersSummaryRowData } from './ServersSummaryRow'

function formatNumber(value: number): string {
  return new Intl.NumberFormat('en-US').format(value)
}

function formatDecimal(value: number): string {
  return new Intl.NumberFormat('en-US', { maximumFractionDigits: 2 }).format(value)
}

function formatNullableDecimal(
  value: number | null,
  formatter: (value: string) => string,
): string {
  if (value == null) return '—'
  return formatter(formatDecimal(value))
}

function formatBytes(bytes: number): string {
  if (bytes === 0) return '0 B'

  const units = ['B', 'KB', 'MB', 'GB', 'TB'] as const
  const exponent = Math.min(Math.floor(Math.log(bytes) / Math.log(1024)), units.length - 1)
  const value = bytes / 1024 ** exponent

  return `${formatDecimal(value)} ${units[exponent]}`
}

export function mapServersOverviewToSummaryData(overview: ServersOverview): ServersSummaryRowData {
  const { infrastructure, performance, traffic } = overview

  return {
    infrastructure: {
      totalServers: formatNumber(infrastructure.totalServers),
      serversOnline: `${formatNumber(infrastructure.serversOnline)} / ${formatNumber(
        infrastructure.totalServers,
      )}`,
      peersOnline: formatNumber(infrastructure.peersOnline),
      activePeers: formatNumber(infrastructure.totalActivePeers),
    },
    performance: {
      avgSpeed: formatNullableDecimal(performance.avgSpeedMbps, (v) => `${v} Mbps`),
      avgLatency: formatNullableDecimal(performance.avgInfraLatencyMs, (v) => `${v} ms`),
    },
    traffic: {
      today: formatBytes(traffic.trafficTodayBytes),
      last24h: formatBytes(traffic.trafficLast24hBytes),
    },
  }
}

export function mapServersListToRows(items: ServerCard[]): ServerRow[] {
  return items.map((item) => ({
    id: item.id,
    name: item.name,
    region: item.region,
    status: item.status,
    reachable: item.reachable,
    online: item.onlinePeers == null ? '—' : formatNumber(item.onlinePeers),
    active: formatNumber(item.peersActive),
    max: item.maxPeers == null ? '—' : formatNumber(item.maxPeers),
    speed: formatDecimal(item.vpnSpeedMbps) + ' Mbps',
    latency: formatDecimal(item.infraLatencyMs) + ' ms',
    traffic1h: formatBytes(item.trafficLast1hBytes),
    traffic24h: formatBytes(item.trafficLast24hBytes),
  }))
}

