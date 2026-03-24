import { MetricCard, MetricRow } from '../../../shared/ui'
import { formatBytes, formatDecimal, formatNumber } from '../../../shared/lib/formatters'

function KpiCard({
  label,
  value,
  isLast,
}: {
  label: string
  value: string
  isLast?: boolean
}) {
  return (
    <div className={['px-4 py-3', isLast ? '' : 'border-r'].join(' ')}>
      <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide">
        {label}
      </div>
      <div className="text-lg font-semibold tabular-nums mt-1">{value}</div>
    </div>
  )
}

export function DashboardOverviewSection({
  overview,
}: {
  overview: {
    totalServers: number
    serversOnline: number
    totalActivePeers: number
    peersOnline: number
    avgVpnSpeedMbps: number | null
    avgInfraLatencyMs: number | null
    trafficTodayBytes: number
    trafficLast24hBytes: number
    totalUsers: number
    activeSubscriptions: number
    expiringSoonDays3: number
    onlineUsersNow: number
  }
}) {
  return (
    <div>
      <div className="flex border rounded-lg bg-card mb-6">
        <KpiCard
          label="Servers Online"
          value={`${overview.serversOnline} / ${overview.totalServers}`}
        />
        <KpiCard label="Peers Online" value={formatNumber(overview.peersOnline)} />
        <KpiCard label="Active Peers" value={formatNumber(overview.totalActivePeers)} />
        <KpiCard
          label="Active Subscriptions"
          value={formatNumber(overview.activeSubscriptions)}
          isLast
        />
      </div>

      <div className="grid grid-cols-4 gap-4 mb-6">
        <MetricCard title="Infrastructure">
          <MetricRow label="Total Servers" value={formatNumber(overview.totalServers)} />
          <MetricRow label="Servers Online" value={formatNumber(overview.serversOnline)} />
          <MetricRow
            label="Total Active Peers"
            value={formatNumber(overview.totalActivePeers)}
          />
          <MetricRow label="Peers Online" value={formatNumber(overview.peersOnline)} />
        </MetricCard>

        <MetricCard title="Performance">
          <MetricRow
            label="Avg VPN Speed"
            value={
              overview.avgVpnSpeedMbps == null
                ? '—'
                : `${formatDecimal(overview.avgVpnSpeedMbps)} Mbps`
            }
          />
          <MetricRow
            label="Avg Latency"
            value={
              overview.avgInfraLatencyMs == null
                ? '—'
                : `${formatDecimal(overview.avgInfraLatencyMs)} ms`
            }
          />
        </MetricCard>

        <MetricCard title="Traffic">
          <MetricRow label="Today Traffic" value={formatBytes(overview.trafficTodayBytes)} />
          <MetricRow
            label="Last 24h Traffic"
            value={formatBytes(overview.trafficLast24hBytes)}
          />
        </MetricCard>

        <MetricCard title="Users">
          <MetricRow label="Total Users" value={formatNumber(overview.totalUsers)} />
          <MetricRow
            label="Active Subscriptions"
            value={formatNumber(overview.activeSubscriptions)}
          />
          <MetricRow label="Expiring ≤3 days" value={formatNumber(overview.expiringSoonDays3)} />
          <MetricRow label="Online Now" value={formatNumber(overview.onlineUsersNow)} />
        </MetricCard>
      </div>
    </div>
  )
}

