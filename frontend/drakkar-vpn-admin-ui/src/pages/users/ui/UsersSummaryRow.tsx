import { InlineAlert, MetricCard, Skeleton } from '../../../shared/ui'
import type { UiState } from './types'

export type UsersSummaryRowData = {
  totalUsers: string
  activeSubscriptions: string
  expiringSoonDays3: string
  onlineUsersNow: string
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

export function UsersSummaryRow({
  state,
  data,
}: {
  state: UiState
  data?: UsersSummaryRowData
}) {
  if (state === 'loading') {
    return (
      <div className="grid grid-cols-4 gap-4 mb-6">
        {Array.from({ length: 4 }).map((_, idx) => (
          <div key={idx} className="rounded-lg border bg-card p-4">
            <Skeleton className="h-3 w-24 mb-4" />
            <div className="space-y-2">
              <Skeleton className="h-4 w-full" />
              <Skeleton className="h-4 w-5/6" />
            </div>
          </div>
        ))}
      </div>
    )
  }

  if (state === 'error') {
    return (
      <div className="grid grid-cols-4 gap-4 mb-6">
        <ErrorCard title="Total Users" message="Failed to load users summary." />
        <ErrorCard title="Active Subscriptions" message="Failed to load users summary." />
        <ErrorCard title="Expiring Soon" message="Failed to load users summary." />
        <ErrorCard title="Online Now" message="Failed to load users summary." />
      </div>
    )
  }

  if (!data) {
    return (
      <div className="grid grid-cols-4 gap-4 mb-6">
        <ErrorCard title="Total Users" message="Users summary is unavailable." />
        <ErrorCard title="Active Subscriptions" message="Users summary is unavailable." />
        <ErrorCard title="Expiring Soon" message="Users summary is unavailable." />
        <ErrorCard title="Online Now" message="Users summary is unavailable." />
      </div>
    )
  }

  return (
    <div className="grid grid-cols-4 gap-4 mb-6">
      <MetricCard title="Total Users">
        <div className="text-2xl font-semibold tabular-nums">{data.totalUsers}</div>
      </MetricCard>

      <MetricCard title="Active Subscriptions">
        <div className="text-2xl font-semibold tabular-nums">{data.activeSubscriptions}</div>
      </MetricCard>

      <MetricCard title="Expiring Soon">
        <div className="text-2xl font-semibold tabular-nums">{data.expiringSoonDays3}</div>
      </MetricCard>

      <MetricCard title="Online Now">
        <div className="text-2xl font-semibold tabular-nums">{data.onlineUsersNow}</div>
      </MetricCard>
    </div>
  )
}

