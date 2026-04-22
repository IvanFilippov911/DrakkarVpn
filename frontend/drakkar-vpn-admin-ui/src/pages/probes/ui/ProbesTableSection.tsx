import { useState } from 'react'
import type { ProbeRow, UiState } from './types'
import { ADMIN_TABLE, ADMIN_TD, ADMIN_TH, EmptyState, InlineAlert, Skeleton, StatusDot } from '../../../shared/ui'

const PROBES_TABLE_HEADERS: { label: string; align: 'left' | 'right' }[] = [
  { label: 'Name', align: 'left' },
  { label: 'Region', align: 'left' },
  { label: 'Host', align: 'left' },
  { label: 'Status', align: 'left' },
  { label: 'Enabled', align: 'left' },
  { label: 'Last seen', align: 'right' },
  { label: 'Updated', align: 'right' },
  { label: 'Actions', align: 'right' },
]

function TableSkeletonRows() {
  return (
    <>
      {Array.from({ length: 8 }).map((_, idx) => (
        <tr key={idx}>
          {Array.from({ length: 8 }).map((__, col) => (
            <td key={col} className={ADMIN_TD}>
              <Skeleton className={col === 0 ? 'h-4 w-56' : 'h-4 w-24'} />
            </td>
          ))}
        </tr>
      ))}
    </>
  )
}

function ProbeTableRow({
  row,
  onEdit,
  onDelete,
}: {
  row: ProbeRow
  onEdit: (row: ProbeRow) => void
  onDelete: (id: string) => void
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
      <td className={[ADMIN_TD, 'text-left'].join(' ')}>
        <span className="block truncate" title={row.host}>
          {row.host}
        </span>
      </td>
      <td className={[ADMIN_TD, 'text-left'].join(' ')}>{row.status}</td>
      <td className={[ADMIN_TD, 'text-left'].join(' ')}>
        <StatusDot ok={row.enabled} label={row.enabled ? 'Yes' : 'No'} />
      </td>
      <td className={[ADMIN_TD, 'text-right tabular-nums text-muted-foreground'].join(' ')}>
        {row.lastSeen}
      </td>
      <td className={[ADMIN_TD, 'text-right tabular-nums text-muted-foreground'].join(' ')}>
        {row.updated}
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
              className="absolute right-0 mt-1 w-36 rounded-md border bg-popover shadow-sm z-10"
            >
              <button
                type="button"
                role="menuitem"
                className="w-full text-left px-2 py-1.5 text-sm hover:bg-accent"
                onClick={() => {
                  setOpen(false)
                  onEdit(row)
                }}
              >
                Edit
              </button>
              <button
                type="button"
                role="menuitem"
                className="w-full text-left px-2 py-1.5 text-sm hover:bg-accent text-destructive"
                onClick={() => {
                  setOpen(false)
                  onDelete(row.id)
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

export function ProbesTableSection({
  state,
  rows,
  onEdit,
  onDelete,
}: {
  state: UiState
  rows: ProbeRow[]
  onEdit: (row: ProbeRow) => void
  onDelete: (id: string) => void
}) {
  return (
    <div className="border rounded-lg overflow-x-auto">
      <table className={[ADMIN_TABLE, 'min-w-[70rem]'].join(' ')}>
        <thead>
          <tr>
            {PROBES_TABLE_HEADERS.map((h) => (
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
              <td colSpan={8} className="p-4">
                <InlineAlert message="Failed to load probe nodes." />
              </td>
            </tr>
          ) : state === 'empty' ? (
            <tr>
              <td colSpan={8} className="p-6">
                <EmptyState
                  title="No probes"
                  description="Register your first probe node to see it here."
                />
              </td>
            </tr>
          ) : (
            rows.map((row) => (
              <ProbeTableRow key={row.id} row={row} onEdit={onEdit} onDelete={onDelete} />
            ))
          )}
        </tbody>
      </table>
    </div>
  )
}

