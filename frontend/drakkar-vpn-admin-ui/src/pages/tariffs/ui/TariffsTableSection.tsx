import { useMemo, useState } from 'react'
import { EmptyState, InlineAlert, Skeleton } from '../../../shared/ui'
import { formatDecimal } from '../../../shared/lib/formatters'
import type { TariffRow, UiState } from './types'
import type { TariffStatus } from '../../../entities/tariffs'

function formatPrice(value: number) {
  // Currency is not provided by API contract; format decimal only.
  return formatDecimal(value)
}

function formatCreatedAt(value: string) {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value

  return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: '2-digit' })
}

function TariffStatusBadge({ status }: { status: TariffStatus }) {
  const active = status === 'Active'

  return (
    <span
      className={[
        'inline-flex items-center rounded px-2 py-0.5 text-xs font-medium border',
        active ? 'bg-emerald-50 text-emerald-700 border-emerald-200' : 'bg-zinc-100 text-zinc-600 border-zinc-200',
      ].join(' ')}
    >
      {status}
    </span>
  )
}

function TableSkeletonRows({ rowsCount = 8 }: { rowsCount?: number }) {
  const columnsCount = 8

  return (
    <>
      {Array.from({ length: rowsCount }).map((_, idx) => (
        <tr key={idx} className="border-b">
          {Array.from({ length: columnsCount }).map((__, col) => (
            <td key={col} className="p-2 align-middle">
              <Skeleton
                className={[
                  col === 0
                    ? 'h-4 w-48'
                    : col === 1
                      ? 'h-4 w-20'
                      : col === 2
                        ? 'h-4 w-28'
                        : col === 3
                          ? 'h-4 w-32'
                          : col === 4
                            ? 'h-4 w-24'
                            : col === 5
                              ? 'h-4 w-20'
                              : col === 6
                                ? 'h-4 w-28'
                                : 'h-4 w-10',
                ].join(' ')}
              />
            </td>
          ))}
        </tr>
      ))}
    </>
  )
}

function TariffTableRow({
  row,
  onEdit,
  onEnable,
  onDisable,
  onDelete,
}: {
  row: TariffRow
  onEdit: (id: string) => void
  onEnable: (id: string) => void
  onDisable: (id: string) => void
  onDelete: (id: string) => void
}) {
  const [open, setOpen] = useState(false)

  const statusActions = useMemo(() => {
    return {
      enable: row.status === 'Disabled',
      disable: row.status === 'Active',
    }
  }, [row.status])

  return (
    <tr className="border-b hover:bg-muted/50">
      <td className="p-2 align-middle">{row.name}</td>
      <td className="p-2 align-middle text-muted-foreground">{row.kind}</td>
      <td className="p-2 align-middle text-right tabular-nums">{formatPrice(row.price)}</td>
      <td className="p-2 align-middle">{row.duration}</td>
      <td className="p-2 align-middle text-right tabular-nums">{row.devices}</td>
      <td className="p-2 align-middle">
        <TariffStatusBadge status={row.status} />
      </td>
      <td className="p-2 align-middle text-muted-foreground">{formatCreatedAt(row.createdAt)}</td>
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
            <div
              role="menu"
              className="absolute right-0 mt-1 w-44 rounded-md border bg-popover shadow-sm z-10"
            >
              <button
                type="button"
                role="menuitem"
                className="w-full text-left px-2 py-1.5 text-sm hover:bg-accent"
                onClick={() => {
                  setOpen(false)
                  onEdit(row.id)
                }}
              >
                Edit tariff
              </button>

              <button
                type="button"
                role="menuitem"
                disabled={!statusActions.enable}
                className={[
                  'w-full text-left px-2 py-1.5 text-sm hover:bg-accent disabled:opacity-50 disabled:cursor-not-allowed',
                ].join(' ')}
                onClick={() => {
                  if (!statusActions.enable) return
                  setOpen(false)
                  onEnable(row.id)
                }}
              >
                Enable tariff
              </button>

              <button
                type="button"
                role="menuitem"
                disabled={!statusActions.disable}
                className={[
                  'w-full text-left px-2 py-1.5 text-sm hover:bg-accent disabled:opacity-50 disabled:cursor-not-allowed',
                ].join(' ')}
                onClick={() => {
                  if (!statusActions.disable) return
                  setOpen(false)
                  onDisable(row.id)
                }}
              >
                Disable tariff
              </button>

              <button
                type="button"
                role="menuitem"
                disabled
                className="w-full text-left px-2 py-1.5 text-sm hover:bg-accent disabled:opacity-50 disabled:cursor-not-allowed text-destructive"
                onClick={() => {
                  setOpen(false)
                  onDelete(row.id)
                }}
              >
                Delete tariff
              </button>
            </div>
          ) : null}
        </div>
      </td>
    </tr>
  )
}

export function TariffsTableSection({
  state,
  rows,
  onCreate,
  onEdit,
  onEnable,
  onDisable,
  onDelete,
}: {
  state: UiState
  rows: TariffRow[]
  onCreate: () => void
  onEdit: (id: string) => void
  onEnable: (id: string) => void
  onDisable: (id: string) => void
  onDelete: (id: string) => void
}) {
  return (
    <div className="border rounded-lg">
      <table className="w-full text-sm">
        <thead>
          <tr className="border-b">
            {['Name', 'Kind', 'Price', 'Duration', 'Devices', 'Status', 'Created', 'Actions'].map((h) => (
              <th
                key={h}
                className={[
                  'h-10 px-2 font-medium text-left whitespace-nowrap',
                  h === 'Actions' ? 'w-10 text-right' : '',
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
              <td colSpan={8} className="p-4">
                <InlineAlert message="Failed to load tariffs." />
              </td>
            </tr>
          ) : state === 'empty' ? (
            <tr>
              <td colSpan={8} className="p-6">
                <div className="space-y-4">
                  <EmptyState title="No tariffs yet" description="Create your first tariff to get started." />
                  <button
                    type="button"
                    className="h-9 px-4 rounded-md text-sm font-medium bg-primary text-primary-foreground hover:bg-primary/90"
                    onClick={onCreate}
                  >
                    Create first tariff
                  </button>
                </div>
              </td>
            </tr>
          ) : (
            rows.map((row) => (
              <TariffTableRow
                key={row.id}
                row={row}
                onEdit={onEdit}
                onEnable={onEnable}
                onDisable={onDisable}
                onDelete={onDelete}
              />
            ))
          )}
        </tbody>
      </table>
    </div>
  )
}

