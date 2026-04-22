import { useEffect, useState, type MouseEvent as ReactMouseEvent } from 'react'
import { createPortal } from 'react-dom'
import { ADMIN_TABLE, ADMIN_TD, ADMIN_TH, EmptyState, InlineAlert, Skeleton } from '../../../shared/ui'
import type { TransportProfileRow, UiState } from './types'

const TRANSPORT_PROFILES_TABLE_HEADERS: { label: string; align: 'left' | 'right' }[] = [
  { label: 'Name', align: 'left' },
  { label: 'Transport', align: 'left' },
  { label: 'Security', align: 'left' },
  { label: 'Global Priority', align: 'right' },
  { label: 'Status', align: 'left' },
  { label: 'Updated', align: 'right' },
  { label: 'Actions', align: 'right' },
]

function TableSkeletonRows() {
  return (
    <>
      {Array.from({ length: 8 }).map((_, idx) => (
        <tr key={idx}>
          {Array.from({ length: 7 }).map((__, col) => (
            <td key={col} className={ADMIN_TD}>
              <Skeleton className={col === 0 ? 'h-4 w-56' : 'h-4 w-24'} />
            </td>
          ))}
        </tr>
      ))}
    </>
  )
}

function StatusBadge({
  label,
  enabled,
}: {
  label: string
  enabled: boolean
}) {
  return (
    <span
      className={[
        'inline-flex items-center rounded px-2 py-0.5 text-xs font-medium border',
        enabled
          ? 'bg-emerald-50 text-emerald-700 border-emerald-200'
          : 'bg-zinc-100 text-zinc-600 border-zinc-200',
      ].join(' ')}
    >
      {label}
    </span>
  )
}

function TypeBadge({
  type,
}: {
  type: TransportProfileRow['transportTypeValue']
}) {
  const isGrpc = type === 'Grpc'

  return (
    <span
      className={[
        'inline-flex items-center rounded px-2 py-0.5 text-xs font-medium border',
        isGrpc ? 'bg-blue-50 text-blue-700 border-blue-200' : 'bg-slate-100 text-slate-700 border-slate-200',
      ].join(' ')}
    >
      {isGrpc ? 'gRPC' : 'TCP'}
    </span>
  )
}

function SecurityBadge({ label }: { label: string }) {
  return (
    <span className="inline-flex items-center rounded px-2 py-0.5 text-xs font-medium border bg-violet-50 text-violet-700 border-violet-200">
      {label}
    </span>
  )
}

function TransportProfileTableRow({
  row,
  isMenuOpen,
  onOpenMenu,
}: {
  row: TransportProfileRow
  isMenuOpen: boolean
  onOpenMenu: (row: TransportProfileRow, trigger: HTMLButtonElement) => void
}) {
  return (
    <tr className="hover:bg-muted/50">
      <td className={[ADMIN_TD, 'min-w-0 text-left'].join(' ')}>
        <span className="block truncate" title={row.name}>
          {row.name}
        </span>
      </td>
      <td className={[ADMIN_TD, 'text-left'].join(' ')}>
        <TypeBadge type={row.transportTypeValue} />
      </td>
      <td className={[ADMIN_TD, 'text-left'].join(' ')}>
        <SecurityBadge label={row.securityType} />
      </td>
      <td className={[ADMIN_TD, 'text-right tabular-nums'].join(' ')}>{row.globalPriority}</td>
      <td className={[ADMIN_TD, 'text-left'].join(' ')}>
        <StatusBadge label={row.statusLabel} enabled={row.isEnabled} />
      </td>
      <td className={[ADMIN_TD, 'text-right tabular-nums text-muted-foreground'].join(' ')}>
        {row.updatedAt}
      </td>
      <td className={[ADMIN_TD, 'text-right'].join(' ')}>
        <div className="relative inline-block text-left">
          <button
            type="button"
            className="h-8 w-8 inline-flex items-center justify-center rounded-md hover:bg-accent"
            aria-haspopup="menu"
            aria-expanded={isMenuOpen}
            onClick={(event: ReactMouseEvent<HTMLButtonElement>) => {
              onOpenMenu(row, event.currentTarget)
            }}
          >
            <span className="text-lg leading-none">⋯</span>
            <span className="sr-only">Row actions</span>
          </button>
        </div>
      </td>
    </tr>
  )
}

export function TransportProfilesTableSection({
  state,
  rows,
  onView,
  onEdit,
  onEnable,
  onDisable,
  onDelete,
}: {
  state: UiState
  rows: TransportProfileRow[]
  onView: (id: string) => void
  onEdit: (id: string) => void
  onEnable: (id: string) => void
  onDisable: (id: string) => void
  onDelete: (id: string) => void
}) {
  const [menu, setMenu] = useState<{
    row: TransportProfileRow
    top: number
    left: number
  } | null>(null)

  useEffect(() => {
    if (!menu) return

    const close = () => setMenu(null)
    window.addEventListener('scroll', close, true)
    window.addEventListener('resize', close)
    return () => {
      window.removeEventListener('scroll', close, true)
      window.removeEventListener('resize', close)
    }
  }, [menu])

  function openMenu(row: TransportProfileRow, trigger: HTMLButtonElement) {
    const rect = trigger.getBoundingClientRect()
    const width = 176
    const left = Math.max(8, Math.min(rect.right - width, window.innerWidth - width - 8))

    setMenu({
      row,
      top: rect.bottom + 6,
      left,
    })
  }

  function closeMenu() {
    setMenu(null)
  }

  return (
    <>
      <div className="border rounded-lg overflow-x-auto">
        <table className={[ADMIN_TABLE, 'min-w-[62rem]'].join(' ')}>
          <thead>
            <tr>
              {TRANSPORT_PROFILES_TABLE_HEADERS.map((header) => (
                <th
                  key={header.label}
                  className={[ADMIN_TH, header.align === 'right' ? 'text-right' : 'text-left'].join(' ')}
                >
                  {header.label}
                </th>
              ))}
            </tr>
          </thead>

          <tbody>
            {state === 'loading' ? (
              <TableSkeletonRows />
            ) : state === 'error' ? (
              <tr>
                <td colSpan={7} className="p-4">
                  <InlineAlert message="Failed to load transport profiles." />
                </td>
              </tr>
            ) : state === 'empty' ? (
              <tr>
                <td colSpan={7} className="p-6">
                  <EmptyState
                    title="No transport profiles"
                    description="Create your first transport profile to see it here."
                  />
                </td>
              </tr>
            ) : (
              rows.map((row) => (
                <TransportProfileTableRow
                  key={row.id}
                  row={row}
                  isMenuOpen={menu?.row.id === row.id}
                  onOpenMenu={openMenu}
                />
              ))
            )}
          </tbody>
        </table>
      </div>

      {menu
        ? createPortal(
            <>
              <button
                type="button"
                aria-label="Close actions menu"
                className="fixed inset-0 z-40 cursor-default"
                onClick={closeMenu}
              />
              <div
                role="menu"
                className="fixed z-50 w-44 rounded-md border bg-popover shadow-sm"
                style={{ top: menu.top, left: menu.left }}
              >
                <button
                  type="button"
                  role="menuitem"
                  className="w-full text-left px-2 py-1.5 text-sm hover:bg-accent"
                  onClick={() => {
                    closeMenu()
                    onView(menu.row.id)
                  }}
                >
                  View details
                </button>
                <button
                  type="button"
                  role="menuitem"
                  className="w-full text-left px-2 py-1.5 text-sm hover:bg-accent"
                  onClick={() => {
                    closeMenu()
                    onEdit(menu.row.id)
                  }}
                >
                  Edit profile
                </button>
                <button
                  type="button"
                  role="menuitem"
                  disabled={menu.row.isEnabled}
                  className="w-full text-left px-2 py-1.5 text-sm hover:bg-accent disabled:opacity-50 disabled:cursor-not-allowed"
                  onClick={() => {
                    if (menu.row.isEnabled) return
                    closeMenu()
                    onEnable(menu.row.id)
                  }}
                >
                  Enable profile
                </button>
                <button
                  type="button"
                  role="menuitem"
                  disabled={!menu.row.isEnabled}
                  className="w-full text-left px-2 py-1.5 text-sm hover:bg-accent disabled:opacity-50 disabled:cursor-not-allowed"
                  onClick={() => {
                    if (!menu.row.isEnabled) return
                    closeMenu()
                    onDisable(menu.row.id)
                  }}
                >
                  Disable profile
                </button>
                <button
                  type="button"
                  role="menuitem"
                  className="w-full text-left px-2 py-1.5 text-sm hover:bg-accent text-destructive"
                  onClick={() => {
                    closeMenu()
                    onDelete(menu.row.id)
                  }}
                >
                  Delete profile
                </button>
              </div>
            </>,
            document.body,
          )
        : null}
    </>
  )
}
