import { useEffect, useRef, useState } from 'react'
import { Skeleton } from '../../../shared/ui'
import type { ErrorEventRow, ErrorsUiState } from './types'

function maybeValue(v: string | null) {
  return v && v.trim().length > 0 ? v : '—'
}

async function copyToClipboard(value: string) {
  try {
    await navigator.clipboard.writeText(value)
  } catch {
    // ignore
  }
}

function CopyableId({
  value,
  maxWidthClassName,
}: {
  value: string | null
  maxWidthClassName: string
}) {
  if (!value) return <span className="text-muted-foreground">—</span>

  return (
    <button
      type="button"
      className={['text-left font-mono text-xs hover:underline', maxWidthClassName, 'truncate'].join(' ')}
      onClick={() => void copyToClipboard(value)}
      title={value}
    >
      {value}
    </button>
  )
}

function RowActions({
  id,
  onDeleteClick,
  disabled,
}: {
  id: string
  onDeleteClick: (id: string) => void
  disabled?: boolean
}) {
  const [open, setOpen] = useState(false)
  const rootRef = useRef<HTMLDivElement | null>(null)

  useEffect(() => {
    if (!open) return

    const onDocClick = (e: MouseEvent) => {
      if (!rootRef.current) return
      if (e.target instanceof Node && rootRef.current.contains(e.target)) return
      setOpen(false)
    }

    document.addEventListener('mousedown', onDocClick)
    return () => document.removeEventListener('mousedown', onDocClick)
  }, [open])

  return (
    <div className="relative" ref={rootRef}>
      <button
        type="button"
        className="h-8 w-8 rounded-md text-muted-foreground hover:bg-accent hover:text-foreground"
        aria-haspopup="menu"
        aria-expanded={open}
        onClick={() => setOpen((v) => !v)}
        disabled={disabled}
      >
        ⋯
      </button>

      {open ? (
        <div
          role="menu"
          className="absolute right-0 mt-1 w-40 rounded-md border bg-background shadow-sm"
        >
          <button
            type="button"
            role="menuitem"
            className="w-full text-left px-2 py-1.5 text-sm hover:bg-accent text-destructive"
            onClick={() => {
              setOpen(false)
              onDeleteClick(id)
            }}
            disabled={disabled}
          >
            Delete
          </button>
        </div>
      ) : null}
    </div>
  )
}

export function ErrorsTableSection({
  state,
  rows,
  onDeleteClick,
  deleteDisabled,
}: {
  state: ErrorsUiState
  rows: ErrorEventRow[]
  onDeleteClick: (id: string) => void
  deleteDisabled?: boolean
}) {
  return (
    <div className="border rounded-lg overflow-hidden mb-6">
      <table className="w-full text-sm">
        <thead>
          <tr className="border-b">
            <th className="h-10 px-2 font-medium text-left whitespace-nowrap">Timestamp</th>
            <th className="h-10 px-2 font-medium text-left whitespace-nowrap">Command</th>
            <th className="h-10 px-2 font-medium text-left whitespace-nowrap">Area</th>
            <th className="h-10 px-2 font-medium text-left whitespace-nowrap">Error Type</th>
            <th className="h-10 px-2 font-medium text-left whitespace-nowrap">Domain Code</th>
            <th className="h-10 px-2 font-medium text-left whitespace-nowrap">Message</th>
            <th className="h-10 px-2 font-medium text-left whitespace-nowrap">Trace ID</th>
            <th className="h-10 px-2 font-medium text-left whitespace-nowrap">User ID</th>
            <th className="h-10 px-2 font-medium text-left whitespace-nowrap">Telegram ID</th>
            <th className="h-10 px-2 font-medium text-left whitespace-nowrap w-10">Actions</th>
          </tr>
        </thead>
        <tbody>
          {state === 'loading'
            ? Array.from({ length: 6 }).map((_, idx) => (
                <tr key={idx} className="border-b hover:bg-muted/50">
                  {Array.from({ length: 9 }).map((__, cIdx) => (
                    <td key={cIdx} className="p-2 align-middle">
                      <Skeleton className="h-4 w-full" />
                    </td>
                  ))}
                  <td className="p-2 align-middle">
                    <Skeleton className="h-8 w-8" />
                  </td>
                </tr>
              ))
            : rows.map((r) => (
                <tr key={r.id} className="border-b hover:bg-muted/50">
                  <td className="p-2 align-middle whitespace-nowrap">{r.timestamp}</td>
                  <td className="p-2 align-middle whitespace-nowrap">{r.command}</td>
                  <td className="p-2 align-middle whitespace-nowrap">{r.area}</td>
                  <td className="p-2 align-middle whitespace-nowrap">{r.errorType}</td>
                  <td className="p-2 align-middle whitespace-nowrap">{maybeValue(r.domainCode)}</td>
                  <td className="p-2 align-middle">
                    <div className="max-w-[480px] truncate" title={r.message}>
                      {r.message}
                    </div>
                  </td>
                  <td className="p-2 align-middle whitespace-nowrap">
                    <CopyableId value={r.traceId} maxWidthClassName="max-w-[220px]" />
                  </td>
                  <td className="p-2 align-middle whitespace-nowrap">
                    <CopyableId value={r.userId} maxWidthClassName="max-w-[220px]" />
                  </td>
                  <td className="p-2 align-middle whitespace-nowrap">
                    <CopyableId value={r.telegramId} maxWidthClassName="max-w-[220px]" />
                  </td>
                  <td className="p-2 align-middle whitespace-nowrap w-10">
                    <RowActions
                      id={r.id}
                      disabled={deleteDisabled}
                      onDeleteClick={onDeleteClick}
                    />
                  </td>
                </tr>
              ))}
        </tbody>
      </table>
    </div>
  )
}

