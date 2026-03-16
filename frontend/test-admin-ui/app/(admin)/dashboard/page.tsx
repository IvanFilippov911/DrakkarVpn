import { PageHeader } from '@/components/admin/page-header'
import { MetricCard, MetricRow, KpiCard } from '@/components/admin/metric-card'
import { mockDashboardOverview, mockCoreHealth } from '@/lib/mock-data'
import { formatBytes, formatDecimal, formatPercent, formatNumber, formatDateTime } from '@/lib/format'

export default function DashboardPage() {
  const overview = mockDashboardOverview
  const coreHealth = mockCoreHealth

  // Sort errors by area
  const errorsByArea = Object.entries(coreHealth.errorsByArea)
    .sort(([, a], [, b]) => b - a)
    .slice(0, 5)

  return (
    <div>
      <PageHeader 
        title="Dashboard" 
        description="System health overview and key metrics"
      />

      {/* KPI Row */}
      <div className="flex border rounded-lg mb-6 bg-card">
        <KpiCard 
          label="Servers Online" 
          value={overview.serversOnline} 
          subValue={`/ ${overview.totalServers}`}
        />
        <KpiCard 
          label="Peers Online" 
          value={formatNumber(overview.peersOnline)} 
        />
        <KpiCard 
          label="Active Peers" 
          value={formatNumber(overview.totalActivePeers)} 
        />
        <KpiCard 
          label="Active Subscriptions" 
          value={formatNumber(overview.activeSubscriptions)} 
        />
      </div>

      {/* Main Grid */}
      <div className="grid grid-cols-4 gap-4 mb-6">
        <MetricCard title="Infrastructure">
          <MetricRow 
            label="Total Servers" 
            value={overview.totalServers} 
          />
          <MetricRow 
            label="Servers Online" 
            value={overview.serversOnline} 
            highlight 
          />
          <MetricRow 
            label="Total Active Peers" 
            value={formatNumber(overview.totalActivePeers)} 
          />
          <MetricRow 
            label="Peers Online" 
            value={formatNumber(overview.peersOnline)} 
            highlight 
          />
        </MetricCard>

        <MetricCard title="Performance">
          <MetricRow 
            label="Avg VPN Speed" 
            value={formatDecimal(overview.avgVpnSpeedMbps)} 
            secondary="Mbps" 
          />
          <MetricRow 
            label="Avg Latency" 
            value={formatDecimal(overview.avgInfraLatencyMs)} 
            secondary="ms" 
          />
        </MetricCard>

        <MetricCard title="Traffic">
          <MetricRow 
            label="Today" 
            value={formatBytes(overview.trafficTodayBytes)} 
          />
          <MetricRow 
            label="Last 24h" 
            value={formatBytes(overview.trafficLast24hBytes)} 
          />
        </MetricCard>

        <MetricCard title="Users">
          <MetricRow 
            label="Total Users" 
            value={formatNumber(overview.totalUsers)} 
          />
          <MetricRow 
            label="Active Subscriptions" 
            value={formatNumber(overview.activeSubscriptions)} 
          />
          <MetricRow 
            label="Expiring ≤3 days" 
            value={overview.expiringSoonDays3} 
          />
          <MetricRow 
            label="Online Now" 
            value={overview.onlineUsersNow} 
            highlight 
          />
        </MetricCard>
      </div>

      {/* Core Health */}
      <div className="border rounded-lg bg-card">
        <div className="p-4 border-b">
          <div className="flex items-center justify-between">
            <div>
              <h3 className="font-medium">Core Metrics</h3>
              <p className="text-xs text-muted-foreground mt-0.5">
                {formatDateTime(coreHealth.windowStartUtc)} — {formatDateTime(coreHealth.windowEndUtc)}
              </p>
            </div>
          </div>
        </div>
        <div className="p-4">
          <div className="grid grid-cols-5 gap-6">
            <div>
              <div className="text-xs text-muted-foreground mb-1">RPS</div>
              <div className="text-2xl font-semibold tabular-nums">{formatDecimal(coreHealth.rps)}</div>
            </div>
            <div>
              <div className="text-xs text-muted-foreground mb-1">Avg Latency</div>
              <div className="text-2xl font-semibold tabular-nums">{formatDecimal(coreHealth.avgLatencyMs)} <span className="text-sm font-normal text-muted-foreground">ms</span></div>
            </div>
            <div>
              <div className="text-xs text-muted-foreground mb-1">Error Rate</div>
              <div className="text-2xl font-semibold tabular-nums">{formatPercent(coreHealth.errorRatePct)}</div>
            </div>
            <div>
              <div className="text-xs text-muted-foreground mb-1">Total Requests</div>
              <div className="text-2xl font-semibold tabular-nums">{formatNumber(coreHealth.totalRequests)}</div>
            </div>
            <div>
              <div className="text-xs text-muted-foreground mb-1">Total Errors</div>
              <div className="text-2xl font-semibold tabular-nums">{formatNumber(coreHealth.totalErrors)}</div>
            </div>
          </div>

          <div className="mt-6 pt-4 border-t">
            <div className="text-xs text-muted-foreground mb-3 uppercase tracking-wide">Errors by Area</div>
            <div className="grid grid-cols-5 gap-4">
              {errorsByArea.map(([area, count]) => {
                const percent = coreHealth.totalErrors > 0 
                  ? (count / coreHealth.totalErrors * 100).toFixed(1) 
                  : '0'
                return (
                  <div key={area} className="text-sm">
                    <div className="text-muted-foreground capitalize">{area.replace('-', ' ')}</div>
                    <div className="font-medium tabular-nums">{formatNumber(count)} <span className="text-muted-foreground font-normal">({percent}%)</span></div>
                  </div>
                )
              })}
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
