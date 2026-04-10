import { useState } from 'react'
import { Link } from 'react-router-dom'
import { ADMIN_TABLE, ADMIN_TD, ADMIN_TH, EmptyState, InlineAlert, Skeleton, StatusDot } from '../../../shared/ui'
import type { UiState, UserRow } from './types'
import { UserRowActionsModal } from './UserRowActionsModal'

const USERS_TABLE_HEADERS: { label: string; align: 'left' | 'right' }[] = [
  { label: 'User ID', align: 'left' },
  { label: 'Telegram', align: 'left' },
  { label: 'Username', align: 'left' },
  { label: 'Created', align: 'left' },
  { label: 'Status', align: 'left' },
  { label: 'Online', align: 'left' },
  { label: 'Devices', align: 'right' },
  { label: 'Last Seen', align: 'left' },
  { label: 'Subscription status', align: 'left' },
  { label: 'Subscription End', align: 'left' },
  { label: 'Max Devices', align: 'right' },
  { label: 'Traffic 24h', align: 'right' },
  { label: 'Actions', align: 'right' },
  { label: 'Detail', align: 'right' },
]

function shortUserId(id: string) {
  const raw = id.trim()
  if (raw.length <= 16) return raw
  return `${raw.slice(0, 8)}…${raw.slice(-4)}`
}

function TableSkeletonRows({ rowsCount = 8 }: { rowsCount?: number }) {
  const columnsCount = 15

  return (
    <>
      {Array.from({ length: rowsCount }).map((_, idx) => (
        <tr key={idx}>
          {Array.from({ length: columnsCount }).map((__, col) => (
            <td key={col} className={ADMIN_TD}>
              <Skeleton
                className={[
                  col === 1 ? 'h-4 w-40' : col === 2 ? 'h-4 w-24' : col === 12 ? 'h-4 w-10' : 'h-4 w-20',
                ].join(' ')}
              />
            </td>
          ))}
        </tr>
      ))}
    </>
  )
}

function UserStatusBadge({ value }: { value: string }) {
  const lowered = value.toLowerCase()
  const variant =
    lowered.includes('ban') || lowered.includes('blocked')
      ? 'bg-red-50 text-red-700 border-red-200'
      : lowered.includes('active') || lowered.includes('ok')
        ? 'bg-emerald-50 text-emerald-700 border-emerald-200'
        : 'bg-zinc-100 text-zinc-600 border-zinc-200'

  return (
    <span className={['inline-flex items-center rounded px-2 py-0.5 text-xs font-medium border', variant].join(' ')}>
      {value}
    </span>
  )
}

function SubscriptionStatusBadge({ value }: { value: string }) {
  if (value === '—') {
    return <span className="text-muted-foreground tabular-nums">{value}</span>
  }

  const lowered = value.toLowerCase()
  const variant =
    lowered === 'active'
      ? 'bg-emerald-50 text-emerald-700 border-emerald-200'
      : lowered === 'cancelled'
        ? 'bg-amber-50 text-amber-800 border-amber-200'
        : lowered === 'expired'
          ? 'bg-zinc-100 text-zinc-600 border-zinc-200'
          : 'bg-zinc-100 text-zinc-600 border-zinc-200'

  return (
    <span className={['inline-flex items-center rounded px-2 py-0.5 text-xs font-medium border', variant].join(' ')}>
      {value}
    </span>
  )
}

function UsersTableRow({
  row,
  isSelected,
  onToggleSelect,
  onOpenActions,
}: {
  row: UserRow
  isSelected: boolean
  onToggleSelect: () => void
  onOpenActions: () => void
}) {
  return (
    <tr className={['hover:bg-muted/50', isSelected ? 'bg-muted' : ''].join(' ')}>
      <td className={[ADMIN_TD, 'text-center'].join(' ')}>
        <input type="checkbox" checked={isSelected} onChange={onToggleSelect} />
      </td>
      <td className={[ADMIN_TD, 'min-w-0'].join(' ')}>
        <div className="grid grid-cols-[minmax(0,1fr)_auto] items-center gap-x-2">
          <span className="min-w-0 truncate font-mono text-xs text-muted-foreground" title={row.id}>
            {shortUserId(row.id)}
          </span>
          <button
            type="button"
            className="shrink-0 text-xs text-muted-foreground hover:text-foreground underline-offset-2 hover:underline"
            title="Copy user id"
            onClick={async () => {
              try {
                await navigator.clipboard.writeText(row.id)
              } catch {
                // ignore
              }
            }}
          >
            Copy
          </button>
        </div>
      </td>
      <td className={[ADMIN_TD, 'text-left text-muted-foreground tabular-nums'].join(' ')}>{row.telegram}</td>
      <td className={[ADMIN_TD, 'text-left text-muted-foreground'].join(' ')}>{row.username}</td>
      <td className={[ADMIN_TD, 'text-left text-muted-foreground'].join(' ')}>{row.createdAt}</td>
      <td className={[ADMIN_TD, 'text-left'].join(' ')}>
        <UserStatusBadge value={row.status} />
      </td>
      <td className={[ADMIN_TD, 'text-left'].join(' ')}>
        <StatusDot ok={row.online} label={row.online ? 'Online' : 'Offline'} />
      </td>
      <td className={[ADMIN_TD, 'text-right tabular-nums'].join(' ')}>{row.devices}</td>
      <td className={[ADMIN_TD, 'text-left text-muted-foreground'].join(' ')}>{row.lastSeen}</td>
      <td className={[ADMIN_TD, 'text-left'].join(' ')}>
        <SubscriptionStatusBadge value={row.subscriptionStatus} />
      </td>
      <td className={[ADMIN_TD, 'text-left text-muted-foreground'].join(' ')}>{row.subscriptionEnd}</td>
      <td className={[ADMIN_TD, 'text-right tabular-nums'].join(' ')}>{row.maxDevices}</td>
      <td className={[ADMIN_TD, 'text-right tabular-nums'].join(' ')}>{row.traffic24h}</td>
      <td className={[ADMIN_TD, 'text-right'].join(' ')}>
        <button
          type="button"
          className="h-8 w-8 inline-flex items-center justify-center rounded-md hover:bg-accent"
          aria-haspopup="dialog"
          aria-label="Open user actions"
          onClick={onOpenActions}
        >
          <span className="text-lg leading-none">⋯</span>
        </button>
      </td>
      <td className={[ADMIN_TD, 'text-right'].join(' ')}>
        <Link
          to={`/users/${row.id}`}
          className="inline-flex h-8 items-center justify-center px-3 rounded-md text-sm border border-border/60 text-muted-foreground hover:bg-accent hover:text-foreground"
        >
          Detail
        </Link>
      </td>
    </tr>
  )
}

export function UsersTableSection({
  state,
  rows,
  selectedUserIds,
  onToggleRow,
  onSelectAllCurrentPage,
  onClearSelection,
  onRowAction,
}: {
  state: UiState
  rows: UserRow[]
  selectedUserIds: string[]
  onToggleRow: (userId: string) => void
  onSelectAllCurrentPage: () => void
  onClearSelection: () => void
  onRowAction: (userId: string, action: 'ban' | 'unban' | 'grantSubscription' | 'markInternal') => void
}) {
  const [actionsUserId, setActionsUserId] = useState<string | null>(null)
  const allOnPageSelected = rows.length > 0 && rows.every((r) => selectedUserIds.includes(r.id))

  return (
    <div className="border rounded-lg">
      <table className={ADMIN_TABLE}>
        <thead>
          <tr>
            <th className={[ADMIN_TH, 'text-center'].join(' ')}>
              <input
                type="checkbox"
                checked={allOnPageSelected}
                onChange={() => {
                  if (allOnPageSelected) onClearSelection()
                  else onSelectAllCurrentPage()
                }}
              />
            </th>
            {USERS_TABLE_HEADERS.map((h) => (
              <th
                key={h.label}
                className={[ADMIN_TH, h.align === 'right' ? 'text-right' : 'text-left'].join(' ')}
              >
                {h.label}
              </th>
            ))}
          </tr>
        </thead>

        <tbody>
          {state === 'loading' ? (
            <TableSkeletonRows />
          ) : state === 'error' ? (
            <tr>
              <td colSpan={15} className="p-4">
                <InlineAlert message="Failed to load users list." />
              </td>
            </tr>
          ) : state === 'empty' ? (
            <tr>
              <td colSpan={15} className="p-6">
                <EmptyState title="No users" description="No users match the current filters." />
              </td>
            </tr>
          ) : (
            rows.map((row) => (
              <UsersTableRow
                key={row.id}
                row={row}
                isSelected={selectedUserIds.includes(row.id)}
                onToggleSelect={() => onToggleRow(row.id)}
                onOpenActions={() => setActionsUserId(row.id)}
              />
            ))
          )}
        </tbody>
      </table>

      <UserRowActionsModal
        open={actionsUserId !== null}
        userId={actionsUserId}
        onClose={() => setActionsUserId(null)}
        onPickAction={(action) => {
          if (!actionsUserId) return
          const id = actionsUserId
          setActionsUserId(null)
          onRowAction(id, action)
        }}
      />
    </div>
  )
}

