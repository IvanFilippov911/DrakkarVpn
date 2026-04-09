import type { ReactNode } from 'react'
import type { AdminUserSummaryDetailDto } from '../../../entities/users'
import {
  formatMaybeDateTimeUtc,
  formatMaybeTelegram,
} from './formatters'

function UserSummaryStatusBadge({ value }: { value: AdminUserSummaryDetailDto['status'] }) {
  const lowered = value.toLowerCase()
  const variant =
    lowered === 'banned'
      ? 'bg-red-50 text-red-700 border-red-200'
      : lowered === 'active'
        ? 'bg-emerald-50 text-emerald-700 border-emerald-200'
        : 'bg-zinc-100 text-zinc-600 border-zinc-200'

  return (
    <span className={['inline-flex items-center rounded px-2 py-0.5 text-xs font-medium border', variant].join(' ')}>
      {value}
    </span>
  )
}

function InternalBadge({ isInternal }: { isInternal: boolean }) {
  const variant = isInternal ? 'bg-amber-50 text-amber-900 border-amber-200' : 'bg-zinc-100 text-zinc-600 border-zinc-200'
  const label = isInternal ? 'Yes' : 'No'

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

export function UserSummaryCard({ user }: { user: AdminUserSummaryDetailDto }) {
  return (
    <section className="border rounded-lg bg-card">
      <div className="px-4 py-3 border-b">
        <h2 className="text-base font-semibold">User summary</h2>
      </div>
      <dl className="p-4 grid gap-3 sm:grid-cols-[minmax(8rem,auto)_1fr] sm:gap-x-6 sm:gap-y-2">
        <DetailLabel>User ID</DetailLabel>
        <DetailValue>
          <span className="font-mono text-xs tabular-nums">{user.userId}</span>
        </DetailValue>

        <DetailLabel>Telegram ID</DetailLabel>
        <DetailValue>{formatMaybeTelegram(user.telegramId)}</DetailValue>

        <DetailLabel>Username</DetailLabel>
        <DetailValue>{user.username === null || user.username === '' ? '—' : user.username}</DetailValue>

        <DetailLabel>Created at</DetailLabel>
        <DetailValue>{formatMaybeDateTimeUtc(user.createdAtUtc)}</DetailValue>

        <DetailLabel>Status</DetailLabel>
        <DetailValue>
          <UserSummaryStatusBadge value={user.status} />
        </DetailValue>

        <DetailLabel>Internal</DetailLabel>
        <DetailValue>
          <InternalBadge isInternal={user.isInternal} />
        </DetailValue>

        <DetailLabel>Ban reason</DetailLabel>
        <DetailValue>{user.banReason === null || user.banReason === '' ? '—' : user.banReason}</DetailValue>

        <DetailLabel>Banned at</DetailLabel>
        <DetailValue>{formatMaybeDateTimeUtc(user.bannedAtUtc)}</DetailValue>
      </dl>
    </section>
  )
}
