import type { ReactNode } from 'react'
import { useAdminOverview, useCoreHealth } from '../../features/dashboard'

export function DashboardPage() {
  const overviewQuery = useAdminOverview()
  const coreQuery = useCoreHealth()

  return (
    <div>
      <PageHeader
        title="Dashboard"
        description="System health overview and key metrics."
      />

      {overviewQuery.isLoading && !overviewQuery.data ? (
        <OverviewSkeleton />
      ) : overviewQuery.isError && !overviewQuery.data ? (
        <OverviewErrorCard message="Failed to load overview metrics." />
      ) : overviewQuery.data ? (
        <OverviewSection overview={overviewQuery.data} />
      ) : (
        <OverviewErrorCard message="Overview metrics are unavailable." />
      )}

      <div className="border rounded-lg bg-card">
        <div className="p-4 border-b">
          <div className="text-lg font-semibold">Core Metrics</div>
          <div className="text-sm text-muted-foreground mt-0.5">
            Window:{' '}
            {coreQuery.data
              ? formatUtcWindow(coreQuery.data.windowStartUtc, coreQuery.data.windowEndUtc)
              : '—'}
          </div>
        </div>

        <div className="p-4">
          {coreQuery.isLoading && !coreQuery.data ? (
            <CoreSkeleton />
          ) : coreQuery.isError && !coreQuery.data ? (
            <InlineAlert message="Failed to load core metrics." />
          ) : coreQuery.data ? (
            <CoreSection core={coreQuery.data} />
          ) : (
            <InlineAlert message="Core metrics are unavailable." />
          )}
        </div>
      </div>
    </div>
  )
}

function PageHeader({ title, description }: { title: string; description: string }) {
  return (
    <div className="flex items-center justify-between mb-6">
      <div>
        <div className="text-lg font-semibold">{title}</div>
        <div className="text-sm text-muted-foreground mt-0.5">{description}</div>
      </div>
    </div>
  )
}

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

function MetricCard({ title, children }: { title: string; children: ReactNode }) {
  return (
    <div className="rounded-lg border bg-card p-4">
      <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-3">
        {title}
      </div>
      <div className="space-y-2">{children}</div>
    </div>
  )
}

function MetricRow({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex items-center justify-between text-sm">
      <div className="text-muted-foreground">{label}</div>
      <div className="tabular-nums">{value}</div>
    </div>
  )
}

function CoreMetric({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide">
        {label}
      </div>
      <div className="text-lg font-semibold tabular-nums mt-1">{value}</div>
    </div>
  )
}

function OverviewSection({
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
        <KpiCard
          label="Active Peers"
          value={formatNumber(overview.totalActivePeers)}
        />
        <KpiCard
          label="Active Subscriptions"
          value={formatNumber(overview.activeSubscriptions)}
          isLast
        />
      </div>

      <div className="grid grid-cols-4 gap-4 mb-6">
        <MetricCard title="Infrastructure">
          <MetricRow label="Total Servers" value={formatNumber(overview.totalServers)} />
          <MetricRow
            label="Servers Online"
            value={formatNumber(overview.serversOnline)}
          />
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
          <MetricRow
            label="Today Traffic"
            value={formatBytes(overview.trafficTodayBytes)}
          />
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
          <MetricRow
            label="Expiring ≤3 days"
            value={formatNumber(overview.expiringSoonDays3)}
          />
          <MetricRow label="Online Now" value={formatNumber(overview.onlineUsersNow)} />
        </MetricCard>
      </div>
    </div>
  )
}

function OverviewSkeleton() {
  return (
    <div>
      <div className="flex border rounded-lg bg-card mb-6 overflow-hidden">
        {Array.from({ length: 4 }).map((_, idx) => (
          <div
            key={idx}
            className={['px-4 py-3 w-1/4', idx === 3 ? '' : 'border-r'].join(' ')}
          >
            <Skeleton className="h-3 w-24" />
            <Skeleton className="h-6 w-20 mt-2" />
          </div>
        ))}
      </div>

      <div className="grid grid-cols-4 gap-4 mb-6">
        {Array.from({ length: 4 }).map((_, idx) => (
          <div key={idx} className="rounded-lg border bg-card p-4">
            <Skeleton className="h-3 w-28 mb-4" />
            <div className="space-y-2">
              <Skeleton className="h-4 w-full" />
              <Skeleton className="h-4 w-5/6" />
              <Skeleton className="h-4 w-4/6" />
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

function OverviewErrorCard({ message }: { message: string }) {
  return (
    <div className="mb-6">
      <InlineAlert message={message} />
    </div>
  )
}

function CoreSection({
  core,
}: {
  core: {
    rps: number
    avgLatencyMs: number
    errorRatePct: number
    totalRequests: number
    totalErrors: number
    windowStartUtc: string
    windowEndUtc: string
    errorsByArea: Record<string, number>
  }
}) {
  return (
    <div>
      <div className="grid grid-cols-5 gap-6">
        <CoreMetric label="RPS" value={formatDecimal(core.rps)} />
        <CoreMetric label="Avg Latency" value={`${formatDecimal(core.avgLatencyMs)} ms`} />
        <CoreMetric label="Error Rate" value={`${formatDecimal(core.errorRatePct)}%`} />
        <CoreMetric label="Total Requests" value={formatNumber(core.totalRequests)} />
        <CoreMetric label="Total Errors" value={formatNumber(core.totalErrors)} />
      </div>

      <div className="mt-6">
        <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-3">
          Errors by area
        </div>

        <div className="space-y-2">
          {Object.entries(core.errorsByArea)
            .sort((a, b) => b[1] - a[1])
            .slice(0, 5)
            .map(([area, count]) => {
              const percent = core.totalErrors === 0 ? 0 : (count / core.totalErrors) * 100

              return (
                <div key={area} className="flex items-center justify-between text-sm">
                  <div className="text-muted-foreground">{area}</div>
                  <div className="tabular-nums">
                    {formatNumber(count)} ({formatPercent(percent)})
                  </div>
                </div>
              )
            })}
        </div>
      </div>
    </div>
  )
}

function CoreSkeleton() {
  return (
    <div>
      <div className="grid grid-cols-5 gap-6">
        {Array.from({ length: 5 }).map((_, idx) => (
          <div key={idx}>
            <Skeleton className="h-3 w-20" />
            <Skeleton className="h-6 w-16 mt-2" />
          </div>
        ))}
      </div>

      <div className="mt-6">
        <Skeleton className="h-3 w-32 mb-4" />
        <div className="space-y-2">
          {Array.from({ length: 5 }).map((_, idx) => (
            <div key={idx} className="flex items-center justify-between">
              <Skeleton className="h-4 w-32" />
              <Skeleton className="h-4 w-28" />
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}

function InlineAlert({ message }: { message: string }) {
  return (
    <div className="rounded-lg border border-destructive/30 bg-destructive/5 p-4">
      <div className="text-sm font-medium">Error</div>
      <div className="text-sm text-muted-foreground mt-1">{message}</div>
    </div>
  )
}

function Skeleton({ className }: { className: string }) {
  return <div className={['animate-pulse rounded-md bg-muted', className].join(' ')} />
}

function formatNumber(value: number): string {
  return new Intl.NumberFormat('en-US').format(value)
}

function formatDecimal(value: number): string {
  return new Intl.NumberFormat('en-US', { maximumFractionDigits: 2 }).format(value)
}

function formatPercent(percent: number): string {
  return `${formatDecimal(percent)}%`
}

function formatBytes(bytes: number): string {
  if (bytes === 0) return '0 B'

  const units = ['B', 'KB', 'MB', 'GB', 'TB'] as const
  const exponent = Math.min(Math.floor(Math.log(bytes) / Math.log(1024)), units.length - 1)
  const value = bytes / 1024 ** exponent

  return `${formatDecimal(value)} ${units[exponent]}`
}

function formatUtcWindow(startUtc: string, endUtc: string): string {
  return `${startUtc} → ${endUtc}`
}

