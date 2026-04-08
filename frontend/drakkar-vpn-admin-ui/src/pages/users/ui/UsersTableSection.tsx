import { useState } from 'react'
import { EmptyState, InlineAlert, Skeleton, StatusDot } from '../../../shared/ui'
import type { UiState, UserRow } from './types'

function shortUserId(id: string) {
  const raw = id.trim()
  if (raw.length <= 16) return raw
  return `${raw.slice(0, 8)}…${raw.slice(-4)}`
}

function TableSkeletonRows({ rowsCount = 8 }: { rowsCount?: number }) {
  const columnsCount = 14

  return (
    <>
      {Array.from({ length: rowsCount }).map((_, idx) => (
        <tr key={idx} className="border-b">
          {Array.from({ length: columnsCount }).map((__, col) => (
            <td key={col} className="p-2 align-middle">
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
  onAction,
}: {
  row: UserRow
  isSelected: boolean
  onToggleSelect: () => void
  onAction: (action: 'ban' | 'unban' | 'grantSubscription' | 'markInternal') => void
}) {
  const [open, setOpen] = useState(false)

  return (
    <tr className={['border-b hover:bg-muted/50', isSelected ? 'bg-muted' : ''].join(' ')}>
      <td className="p-2 align-middle">
        <input type="checkbox" checked={isSelected} onChange={onToggleSelect} />
      </td>
      <td className="p-2 align-middle">
        <div className="flex items-center gap-2">
          <span className="font-mono text-xs text-muted-foreground" title={row.id}>
            {shortUserId(row.id)}
          </span>
          <button
            type="button"
            className="text-xs text-muted-foreground hover:text-foreground underline-offset-2 hover:underline"
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
      <td className="p-2 align-middle text-muted-foreground tabular-nums">{row.telegram}</td>
      <td className="p-2 align-middle text-muted-foreground">{row.createdAt}</td>
      <td className="p-2 align-middle">
        <UserStatusBadge value={row.status} />
      </td>
      <td className="p-2 align-middle">
        <StatusDot ok={row.online} label={row.online ? 'Online' : 'Offline'} />
      </td>
      <td className="p-2 align-middle text-right tabular-nums">{row.devices}</td>
      <td className="p-2 align-middle text-muted-foreground">{row.lastSeen}</td>
      <td className="p-2 align-middle">
        <SubscriptionStatusBadge value={row.subscriptionStatus} />
      </td>
      <td className="p-2 align-middle text-muted-foreground">{row.subscriptionEnd}</td>
      <td className="p-2 align-middle text-right tabular-nums">{row.maxDevices}</td>
      <td className="p-2 align-middle text-right tabular-nums">{row.traffic24h}</td>
      <td className="p-2 align-middle text-right">
        <div className="relative">
          <button
            type="button"
            className="h-8 w-8 inline-flex items-center justify-center rounded-md hover:bg-accent"
            aria-haspopup="menu"
            aria-expanded={open}
            onClick={() => setOpen((v) => !v)}
          >
            <span className="text-lg leading-none">⋯</span>
            <span className="sr-only">Row actions</span>
          </button>

          {open ? (
            <div role="menu" className="absolute right-0 mt-1 w-56 rounded-md border bg-popover shadow-sm z-10">
              <button
                type="button"
                role="menuitem"
                className="w-full text-left px-2 py-1.5 text-sm hover:bg-accent text-destructive"
                onClick={() => {
                  setOpen(false)
                  onAction('ban')
                }}
              >
                Ban user
              </button>

              <button
                type="button"
                role="menuitem"
                className="w-full text-left px-2 py-1.5 text-sm hover:bg-accent"
                onClick={() => {
                  setOpen(false)
                  onAction('unban')
                }}
              >
                Unban user
              </button>

              <button
                type="button"
                role="menuitem"
                className="w-full text-left px-2 py-1.5 text-sm hover:bg-accent"
                onClick={() => {
                  setOpen(false)
                  onAction('grantSubscription')
                }}
              >
                Grant subscription
              </button>

              <button
                type="button"
                role="menuitem"
                className="w-full text-left px-2 py-1.5 text-sm hover:bg-accent"
                onClick={() => {
                  setOpen(false)
                  onAction('markInternal')
                }}
              >
                Mark internal
              </button>
            </div>
          ) : null}
        </div>
      </td>
      <td className="p-2 align-middle text-right">
        <button
          type="button"
          className="h-8 px-3 rounded-md text-sm border border-border/60 text-muted-foreground hover:bg-accent hover:text-foreground disabled:opacity-50 disabled:cursor-not-allowed"
          disabled
        >
          Detail
        </button>
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
  const allOnPageSelected = rows.length > 0 && rows.every((r) => selectedUserIds.includes(r.id))

  return (
    <div className="border rounded-lg">
      <table className="w-full text-sm">
        <thead>
          <tr className="border-b">
            <th className="h-10 px-2 font-medium text-left whitespace-nowrap">
              <input
                type="checkbox"
                checked={allOnPageSelected}
                onChange={() => {
                  if (allOnPageSelected) onClearSelection()
                  else onSelectAllCurrentPage()
                }}
              />
            </th>
            {[
              'User ID',
              'Telegram',
              'Created',
              'Status',
              'Online',
              'Devices',
              'Last Seen',
              'Subscription status',
              'Subscription End',
              'Max Devices',
              'Traffic 24h',
              'Actions',
              'Detail',
            ].map((h) => (
              <th
                key={h}
                className={[
                  'h-10 px-2 font-medium text-left whitespace-nowrap',
                  h === 'Actions' ? 'w-10 text-right' : '',
                  h === 'Detail' ? 'text-right' : '',
                ].join(' ')}
              >
                {h}
              </th>
            ))}
          </tr>
        </thead>

        <tbody>
          {state === 'loading' ? (
            <TableSkeletonRows />
          ) : state === 'error' ? (
            <tr>
              <td colSpan={14} className="p-4">
                <InlineAlert message="Failed to load users list." />
              </td>
            </tr>
          ) : state === 'empty' ? (
            <tr>
              <td colSpan={14} className="p-6">
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
                onAction={(action) => onRowAction(row.id, action)}
              />
            ))
          )}
        </tbody>
      </table>
    </div>
  )
}

