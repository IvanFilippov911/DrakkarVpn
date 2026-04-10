import { useState } from 'react'
import type { ServerRow, UiState } from './types'
import { ADMIN_TABLE, ADMIN_TD, ADMIN_TH, EmptyState, InlineAlert, Skeleton, StatusDot } from '../../../shared/ui'

const SERVERS_TABLE_HEADERS: { label: string; align: 'left' | 'right' }[] = [
  { label: 'Server', align: 'left' },
  { label: 'Region', align: 'left' },
  { label: 'Status', align: 'left' },
  { label: 'Reachable', align: 'left' },
  { label: 'Online', align: 'right' },
  { label: 'Active', align: 'right' },
  { label: 'Max', align: 'right' },
  { label: 'Speed', align: 'right' },
  { label: 'Latency', align: 'right' },
  { label: '1h Traffic', align: 'right' },
  { label: '24h Traffic', align: 'right' },
  { label: 'Peers', align: 'right' },
  { label: 'Actions', align: 'right' },
]

function TableSkeletonRows() {
  return (
    <>
      {Array.from({ length: 8 }).map((_, idx) => (
        <tr key={idx}>
          {Array.from({ length: 13 }).map((__, col) => (
            <td key={col} className={ADMIN_TD}>
              <Skeleton className={col === 0 ? 'h-4 w-48' : 'h-4 w-20'} />
            </td>
          ))}
        </tr>
      ))}
    </>
  )
}

function ServerTableRow({
  row,
  onPeers,
  onDelete,
}: {
  row: ServerRow
  onPeers: () => void
  onDelete: () => void
}) {
  const [open, setOpen] = useState(false)

  return (
    <tr className="hover:bg-muted/50">
      <td className={[ADMIN_TD, 'min-w-0 text-left'].join(' ')}>
        <span className="block truncate" title={row.name}>
          {row.name}
        </span>
      </td>
      <td className={[ADMIN_TD, 'text-left text-muted-foreground'].join(' ')}>{row.region}</td>
      <td className={[ADMIN_TD, 'text-left'].join(' ')}>{row.status}</td>
      <td className={[ADMIN_TD, 'text-left'].join(' ')}>
        <StatusDot ok={row.reachable} label={row.reachable ? 'Yes' : 'No'} />
      </td>
      <td className={[ADMIN_TD, 'text-right tabular-nums'].join(' ')}>{row.online}</td>
      <td className={[ADMIN_TD, 'text-right tabular-nums'].join(' ')}>{row.active}</td>
      <td className={[ADMIN_TD, 'text-right tabular-nums'].join(' ')}>{row.max}</td>
      <td className={[ADMIN_TD, 'text-right tabular-nums'].join(' ')}>{row.speed}</td>
      <td className={[ADMIN_TD, 'text-right tabular-nums'].join(' ')}>{row.latency}</td>
      <td className={[ADMIN_TD, 'text-right tabular-nums'].join(' ')}>{row.traffic1h}</td>
      <td className={[ADMIN_TD, 'text-right tabular-nums'].join(' ')}>{row.traffic24h}</td>
      <td className={[ADMIN_TD, 'text-right'].join(' ')}>
        <button
          type="button"
          className="h-8 px-3 rounded-md text-sm border bg-background hover:bg-accent"
          onClick={onPeers}
        >
          Peers
        </button>
      </td>
      <td className={[ADMIN_TD, 'text-right'].join(' ')}>
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
            <div
              role="menu"
              className="absolute right-0 mt-1 w-40 rounded-md border bg-popover shadow-sm z-10"
            >
              <button
                type="button"
                role="menuitem"
                className="w-full text-left px-2 py-1.5 text-sm hover:bg-accent text-destructive"
                onClick={() => {
                  setOpen(false)
                  onDelete()
                }}
              >
                Delete
              </button>
            </div>
          ) : null}
        </div>
      </td>
    </tr>
  )
}

export function ServersTableSection({
  state,
  rows,
  onPeers,
  onDelete,
}: {
  state: UiState
  rows: ServerRow[]
  onPeers: (id: string) => void
  onDelete: (id: string) => void
}) {
  return (
    <div className="border rounded-lg overflow-x-auto">
      <table className={[ADMIN_TABLE, 'min-w-[70rem]'].join(' ')}>
        <thead>
          <tr>
            {SERVERS_TABLE_HEADERS.map((h) => (
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
              <td colSpan={13} className="p-4">
                <InlineAlert message="Failed to load servers list." />
              </td>
            </tr>
          ) : state === 'empty' ? (
            <tr>
              <td colSpan={13} className="p-6">
                <EmptyState
                  title="No servers"
                  description="Register your first server to see it here."
                />
              </td>
            </tr>
          ) : (
            rows.map((row) => (
              <ServerTableRow
                key={row.id}
                row={row}
                onPeers={() => onPeers(row.id)}
                onDelete={() => onDelete(row.id)}
              />
            ))
          )}
        </tbody>
      </table>
    </div>
  )
}

