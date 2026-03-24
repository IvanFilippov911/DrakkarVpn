import type { UiState } from './types'
import { InlineAlert, MetricCard, MetricRow, Skeleton } from '../../../shared/ui'

export type ServersSummaryRowData = {
  infrastructure: {
    totalServers: string
    serversOnline: string
    peersOnline: string
    activePeers: string
  }
  performance: {
    avgSpeed: string
    avgLatency: string
  }
  traffic: {
    today: string
    last24h: string
  }
}

function ErrorCard({ title, message }: { title: string; message: string }) {
  return (
    <div className="rounded-lg border bg-card p-4">
      <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-3">
        {title}
      </div>
      <InlineAlert message={message} />
    </div>
  )
}

export function ServersSummaryRow({
  state,
  data,
}: {
  state: UiState
  data?: ServersSummaryRowData
}) {
  if (state === 'loading') {
    return (
      <div className="grid grid-cols-3 gap-4 mb-6">
        {Array.from({ length: 3 }).map((_, idx) => (
          <div key={idx} className="rounded-lg border bg-card p-4">
            <Skeleton className="h-3 w-24 mb-4" />
            <div className="space-y-2">
              <Skeleton className="h-4 w-full" />
              <Skeleton className="h-4 w-5/6" />
              <Skeleton className="h-4 w-4/6" />
            </div>
          </div>
        ))}
      </div>
    )
  }

  if (state === 'error') {
    return (
      <div className="grid grid-cols-3 gap-4 mb-6">
        <ErrorCard title="Infrastructure" message="Failed to load infrastructure summary." />
        <ErrorCard title="Performance" message="Failed to load performance summary." />
        <ErrorCard title="Traffic" message="Failed to load traffic summary." />
      </div>
    )
  }

  if (!data) {
    return (
      <div className="grid grid-cols-3 gap-4 mb-6">
        <ErrorCard title="Infrastructure" message="Infrastructure summary is unavailable." />
        <ErrorCard title="Performance" message="Performance summary is unavailable." />
        <ErrorCard title="Traffic" message="Traffic summary is unavailable." />
      </div>
    )
  }

  return (
    <div className="grid grid-cols-3 gap-4 mb-6">
      <MetricCard title="Infrastructure">
        <MetricRow label="Total Servers" value={data.infrastructure.totalServers} />
        <MetricRow label="Servers Online" value={data.infrastructure.serversOnline} />
        <MetricRow label="Peers Online" value={data.infrastructure.peersOnline} />
        <MetricRow label="Active Peers" value={data.infrastructure.activePeers} />
      </MetricCard>

      <MetricCard title="Performance">
        <MetricRow label="Avg VPN Speed" value={data.performance.avgSpeed} />
        <MetricRow label="Avg Latency" value={data.performance.avgLatency} />
      </MetricCard>

      <MetricCard title="Traffic">
        <MetricRow label="Today Traffic" value={data.traffic.today} />
        <MetricRow label="Last 24h Traffic" value={data.traffic.last24h} />
      </MetricCard>
    </div>
  )
}

