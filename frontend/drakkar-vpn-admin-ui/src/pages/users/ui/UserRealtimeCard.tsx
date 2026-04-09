import type { ReactNode } from 'react'
import type { AdminUserRealtimeDto } from '../../../entities/users'
import { formatMaybeBytes, formatMaybeDateTimeUtc, formatMaybeNumber } from './formatters'

function OnlineBadge({ isOnline }: { isOnline: boolean }) {
  const variant = isOnline ? 'bg-emerald-50 text-emerald-700 border-emerald-200' : 'bg-zinc-100 text-zinc-600 border-zinc-200'
  const label = isOnline ? 'Online' : 'Offline'

  return (
    <span className={['inline-flex items-center rounded px-2 py-0.5 text-xs font-medium border', variant].join(' ')}>
      {label}
    </span>
  )
}

function SubscriptionActiveBadge({ active }: { active: boolean }) {
  const variant = active ? 'bg-emerald-50 text-emerald-700 border-emerald-200' : 'bg-zinc-100 text-zinc-600 border-zinc-200'
  const label = active ? 'Active' : 'Inactive'

  return (
    <span className={['inline-flex items-center rounded px-2 py-0.5 text-xs font-medium border', variant].join(' ')}>
      {label}
    </span>
  )
}

function DetailLabel({ children }: { children: string }) {
  return <dt className="text-xs font-medium text-muted-foreground">{children}</dt>
}

function DetailValue({ children }: { children: ReactNode }) {
  return <dd className="text-sm text-foreground min-w-0 break-words">{children}</dd>
}

export function UserRealtimeCard({ realtime }: { realtime: AdminUserRealtimeDto }) {
  return (
    <section className="border rounded-lg bg-card">
      <div className="px-4 py-3 border-b">
        <h2 className="text-base font-semibold">Realtime</h2>
      </div>
      <dl className="p-4 grid gap-3 sm:grid-cols-[minmax(8rem,auto)_1fr] sm:gap-x-6 sm:gap-y-2">
        <DetailLabel>Online</DetailLabel>
        <DetailValue>
          <OnlineBadge isOnline={realtime.isOnline} />
        </DetailValue>

        <DetailLabel>Device count</DetailLabel>
        <DetailValue>{formatMaybeNumber(realtime.deviceCount)}</DetailValue>

        <DetailLabel>Subscription max devices</DetailLabel>
        <DetailValue>{formatMaybeNumber(realtime.subscriptionMaxDevices)}</DetailValue>

        <DetailLabel>Subscription active</DetailLabel>
        <DetailValue>
          <SubscriptionActiveBadge active={realtime.isSubscriptionActive} />
        </DetailValue>

        <DetailLabel>Subscription ends</DetailLabel>
        <DetailValue>{formatMaybeDateTimeUtc(realtime.subscriptionEndUtc)}</DetailValue>

        <DetailLabel>Traffic (24h)</DetailLabel>
        <DetailValue>{formatMaybeBytes(realtime.traffic24hBytes)}</DetailValue>

        <DetailLabel>Updated at</DetailLabel>
        <DetailValue>{formatMaybeDateTimeUtc(realtime.updatedAtUtc)}</DetailValue>

        <DetailLabel>Last seen</DetailLabel>
        <DetailValue>{formatMaybeDateTimeUtc(realtime.lastSeenUtc)}</DetailValue>
      </dl>
    </section>
  )
}
