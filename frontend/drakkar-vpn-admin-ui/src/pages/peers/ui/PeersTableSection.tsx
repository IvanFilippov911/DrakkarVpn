import { useState } from 'react'
import { EmptyState, InlineAlert, Skeleton, StatusDot } from '../../../shared/ui'
import type { PeerRow, UiState } from './types'

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
        <tr key={idx} className="border-b">
          {Array.from({ length: columnsCount }).map((__, col) => (
            <td key={col} className="p-2 align-middle">
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
    <div className="border rounded-lg">
      <table className="w-full text-sm">
        <thead>
          <tr className="border-b">
            {[
              'Peer ID',
              'User ID',
              'Status',
              'Online',
              'Last Activity',
              'Traffic 1h',
              'Traffic 24h',
              'Speed',
              'Latency',
              'Created',
              'Actions',
            ].map((h) => (
              <th
                key={h}
                className={[
                  'h-10 px-2 font-medium text-left whitespace-nowrap',
                  h === 'Actions' ? 'w-10 text-right' : '',
                  ['Traffic 1h', 'Traffic 24h', 'Speed', 'Latency'].includes(h) ? 'text-right' : '',
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
              <tr key={row.peerId} className="border-b hover:bg-muted/50">
                <td className="p-2 align-middle">
                  <div className="flex items-center gap-2">
                    <span className="font-mono text-xs text-muted-foreground" title={row.peerId}>
                      {shortId(row.peerId)}
                    </span>
                    <button
                      type="button"
                      className="text-xs text-muted-foreground hover:text-foreground underline-offset-2 hover:underline"
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
                <td className="p-2 align-middle">
                  <span className="font-mono text-xs text-muted-foreground" title={row.userId}>
                    {shortId(row.userId)}
                  </span>
                </td>
                <td className="p-2 align-middle">
                  <PeerStatusBadge value={row.status} />
                </td>
                <td className="p-2 align-middle">
                  <StatusDot ok={row.isOnline} label={row.isOnline ? 'Online' : 'Offline'} />
                </td>
                <td className="p-2 align-middle text-muted-foreground tabular-nums">{row.lastDataAtUtc}</td>
                <td className="p-2 align-middle text-right text-muted-foreground tabular-nums">
                  {row.trafficLast1hBytes}
                </td>
                <td className="p-2 align-middle text-right text-muted-foreground tabular-nums">
                  {row.trafficLast24hBytes}
                </td>
                <td className="p-2 align-middle text-right text-muted-foreground tabular-nums">{row.speedMbps}</td>
                <td className="p-2 align-middle text-right text-muted-foreground tabular-nums">{row.vpnLatencyMs}</td>
                <td className="p-2 align-middle text-muted-foreground tabular-nums">{row.createdAtUtc}</td>
                <td className="p-2 align-middle text-right">
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

