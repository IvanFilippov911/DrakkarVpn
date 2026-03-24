import { useState } from 'react'
import type { ServerRow, UiState } from './types'
import { EmptyState, InlineAlert, Skeleton, StatusDot } from '../../../shared/ui'

function TableSkeletonRows() {
  return (
    <>
      {Array.from({ length: 8 }).map((_, idx) => (
        <tr key={idx} className="border-b">
          {Array.from({ length: 12 }).map((__, col) => (
            <td key={col} className="p-2 align-middle">
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
  onDelete,
}: {
  row: ServerRow
  onDelete: () => void
}) {
  const [open, setOpen] = useState(false)

  return (
    <tr className="border-b hover:bg-muted/50">
      <td className="p-2 align-middle">{row.name}</td>
      <td className="p-2 align-middle text-muted-foreground">{row.region}</td>
      <td className="p-2 align-middle">{row.status}</td>
      <td className="p-2 align-middle">
        <StatusDot ok={row.reachable} label={row.reachable ? 'Yes' : 'No'} />
      </td>
      <td className="p-2 align-middle text-right tabular-nums">{row.online}</td>
      <td className="p-2 align-middle text-right tabular-nums">{row.active}</td>
      <td className="p-2 align-middle text-right tabular-nums">{row.max}</td>
      <td className="p-2 align-middle text-right tabular-nums">{row.speed}</td>
      <td className="p-2 align-middle text-right tabular-nums">{row.latency}</td>
      <td className="p-2 align-middle text-right tabular-nums">{row.traffic1h}</td>
      <td className="p-2 align-middle text-right tabular-nums">{row.traffic24h}</td>
      <td className="p-2 align-middle">
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
  onDelete,
}: {
  state: UiState
  rows: ServerRow[]
  onDelete: (id: string) => void
}) {
  return (
    <div className="border rounded-lg">
      <table className="w-full text-sm">
        <thead>
          <tr className="border-b">
            {[
              'Server',
              'Region',
              'Status',
              'Reachable',
              'Online',
              'Active',
              'Max',
              'Speed',
              'Latency',
              '1h Traffic',
              '24h Traffic',
              'Actions',
            ].map((h) => (
              <th
                key={h}
                className={[
                  'h-10 px-2 font-medium text-left whitespace-nowrap',
                  h === 'Actions' ? 'w-10' : '',
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
              <td colSpan={12} className="p-4">
                <InlineAlert message="Failed to load servers list." />
              </td>
            </tr>
          ) : state === 'empty' ? (
            <tr>
              <td colSpan={12} className="p-6">
                <EmptyState
                  title="No servers"
                  description="Register your first server to see it here."
                />
              </td>
            </tr>
          ) : (
            rows.map((row) => (
              <ServerTableRow key={row.id} row={row} onDelete={() => onDelete(row.id)} />
            ))
          )}
        </tbody>
      </table>
    </div>
  )
}

