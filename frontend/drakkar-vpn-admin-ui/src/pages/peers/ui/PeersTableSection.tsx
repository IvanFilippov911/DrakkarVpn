import { useState } from 'react'
import { ADMIN_TABLE, ADMIN_TD, ADMIN_TH, EmptyState, InlineAlert, Skeleton, StatusDot } from '../../../shared/ui'
import type { PeerRow, UiState } from './types'

const PEERS_TABLE_HEADERS: { label: string; align: 'left' | 'right' }[] = [
  { label: 'Peer ID', align: 'left' },
  { label: 'User ID', align: 'left' },
  { label: 'Status', align: 'left' },
  { label: 'Online', align: 'left' },
  { label: 'Last Activity', align: 'left' },
  { label: 'Traffic 1h', align: 'right' },
  { label: 'Traffic 24h', align: 'right' },
  { label: 'Speed', align: 'right' },
  { label: 'Latency', align: 'right' },
  { label: 'Created', align: 'left' },
  { label: 'Actions', align: 'right' },
]

function shortId(id: string) {
  const raw = id.trim()
  if (raw.length <= 18) return raw
  return `${raw.slice(0, 8)}…${raw.slice(-4)}`
}

function PeerStatusBadge({ value }: { value: string }) {
  const lowered = value.toLowerCase()
  const variant =
    lowered === 'active'
      ? 'bg-emerald-50 text-emerald-700 border-emerald-200'
      : lowered === 'revoked'
        ? 'bg-zinc-100 text-zinc-600 border-zinc-200'
        : lowered === 'expired'
          ? 'bg-amber-50 text-amber-800 border-amber-200'
          : 'bg-zinc-100 text-zinc-600 border-zinc-200'

  return (
    <span className={['inline-flex items-center rounded px-2 py-0.5 text-xs font-medium border', variant].join(' ')}>
      {value}
    </span>
  )
}

function TableSkeletonRows({ rowsCount = 8 }: { rowsCount?: number }) {
  const columnsCount = 11

  return (
    <>
      {Array.from({ length: rowsCount }).map((_, idx) => (
        <tr key={idx}>
          {Array.from({ length: columnsCount }).map((__, col) => (
            <td key={col} className={ADMIN_TD}>
              <Skeleton
                className={[
                  col === 0 ? 'h-4 w-44' : col === 1 ? 'h-4 w-36' : col === 10 ? 'h-4 w-10' : 'h-4 w-20',
                ].join(' ')}
              />
            </td>
          ))}
        </tr>
      ))}
    </>
  )
}

function PeerRowActions({
  peerId,
  onRevoke,
}: {
  peerId: string
  onRevoke: () => void
}) {
  const [open, setOpen] = useState(false)

  return (
    <div className="relative inline-block text-left">
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
              onRevoke()
            }}
          >
            Revoke peer
          </button>

          <div className="px-2 py-1 text-xs text-muted-foreground">
            peerId: <span className="font-mono">{shortId(peerId)}</span>
          </div>
        </div>
      ) : null}
    </div>
  )
}

export function PeersTableSection({
  state,
  rows,
  onRevokePeer,
}: {
  state: UiState
  rows: PeerRow[]
  onRevokePeer: (peerId: string) => void
}) {
  return (
    <div className="border rounded-lg overflow-x-auto">
      <table className={[ADMIN_TABLE, 'min-w-[64rem]'].join(' ')}>
        <thead>
          <tr>
            {PEERS_TABLE_HEADERS.map((h) => (
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
              <td colSpan={11} className="p-4">
                <InlineAlert message="Failed to load peers list." />
              </td>
            </tr>
          ) : state === 'empty' ? (
            <tr>
              <td colSpan={11} className="p-6">
                <EmptyState title="No peers found" description="No peers match the current filters." />
              </td>
            </tr>
          ) : (
            rows.map((row) => (
              <tr key={row.peerId} className="hover:bg-muted/50">
                <td className={[ADMIN_TD, 'min-w-0'].join(' ')}>
                  <div className="grid grid-cols-[minmax(0,1fr)_auto] items-center gap-x-2">
                    <span className="min-w-0 truncate font-mono text-xs text-muted-foreground" title={row.peerId}>
                      {shortId(row.peerId)}
                    </span>
                    <button
                      type="button"
                      className="shrink-0 text-xs text-muted-foreground hover:text-foreground underline-offset-2 hover:underline"
                      title="Copy peer id"
                      onClick={async () => {
                        try {
                          await navigator.clipboard.writeText(row.peerId)
                        } catch {
                          // ignore
                        }
                      }}
                    >
                      Copy
                    </button>
                  </div>
                </td>
                <td className={[ADMIN_TD, 'min-w-0'].join(' ')}>
                  <span className="block truncate font-mono text-xs text-muted-foreground" title={row.userId}>
                    {shortId(row.userId)}
                  </span>
                </td>
                <td className={[ADMIN_TD, 'text-left'].join(' ')}>
                  <PeerStatusBadge value={row.status} />
                </td>
                <td className={[ADMIN_TD, 'text-left'].join(' ')}>
                  <StatusDot ok={row.isOnline} label={row.isOnline ? 'Online' : 'Offline'} />
                </td>
                <td className={[ADMIN_TD, 'text-left text-muted-foreground tabular-nums'].join(' ')}>
                  {row.lastDataAtUtc}
                </td>
                <td className={[ADMIN_TD, 'text-right text-muted-foreground tabular-nums'].join(' ')}>
                  {row.trafficLast1hBytes}
                </td>
                <td className={[ADMIN_TD, 'text-right text-muted-foreground tabular-nums'].join(' ')}>
                  {row.trafficLast24hBytes}
                </td>
                <td className={[ADMIN_TD, 'text-right text-muted-foreground tabular-nums'].join(' ')}>{row.speedMbps}</td>
                <td className={[ADMIN_TD, 'text-right text-muted-foreground tabular-nums'].join(' ')}>
                  {row.vpnLatencyMs}
                </td>
                <td className={[ADMIN_TD, 'text-left text-muted-foreground tabular-nums'].join(' ')}>{row.createdAtUtc}</td>
                <td className={[ADMIN_TD, 'text-right'].join(' ')}>
                  <PeerRowActions peerId={row.peerId} onRevoke={() => onRevokePeer(row.peerId)} />
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  )
}

