import { ADMIN_TABLE, ADMIN_TD, ADMIN_TH, Skeleton } from '../../../shared/ui'
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
      className={[
        'block w-full min-w-0 text-left font-mono text-xs hover:underline',
        maxWidthClassName,
        'truncate',
      ].join(' ')}
      onClick={() => void copyToClipboard(value)}
      title={value}
    >
      {value}
    </button>
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
    <div className="border rounded-lg mb-6">
      <table className={ADMIN_TABLE}>
        <thead>
          <tr>
            <th className={[ADMIN_TH, 'text-left'].join(' ')}>Timestamp</th>
            <th className={[ADMIN_TH, 'text-left'].join(' ')}>Command</th>
            <th className={[ADMIN_TH, 'text-left'].join(' ')}>Area</th>
            <th className={[ADMIN_TH, 'text-left'].join(' ')}>Error Type</th>
            <th className={[ADMIN_TH, 'text-left'].join(' ')}>Domain Code</th>
            <th className={[ADMIN_TH, 'text-left'].join(' ')}>Message</th>
            <th className={[ADMIN_TH, 'text-left'].join(' ')}>Trace ID</th>
            <th className={[ADMIN_TH, 'text-left'].join(' ')}>User ID</th>
            <th className={[ADMIN_TH, 'text-left'].join(' ')}>Telegram ID</th>
            <th className={[ADMIN_TH, 'text-right'].join(' ')}>Actions</th>
          </tr>
        </thead>
        <tbody>
          {state === 'loading'
            ? Array.from({ length: 6 }).map((_, idx) => (
                <tr key={idx} className="hover:bg-muted/50">
                  {Array.from({ length: 10 }).map((__, cIdx) => (
                    <td key={cIdx} className={[ADMIN_TD, cIdx === 9 ? 'text-right' : ''].join(' ')}>
                      <Skeleton className={cIdx === 9 ? 'inline-block h-8 w-8' : 'h-4 w-full'} />
                    </td>
                  ))}
                </tr>
              ))
            : rows.map((r) => (
                <tr key={r.id} className="hover:bg-muted/50">
                  <td className={[ADMIN_TD, 'text-left whitespace-nowrap'].join(' ')}>{r.timestamp}</td>
                  <td className={[ADMIN_TD, 'min-w-0 text-left break-words'].join(' ')}>{r.command}</td>
                  <td className={[ADMIN_TD, 'min-w-0 text-left break-words'].join(' ')}>{r.area}</td>
                  <td className={[ADMIN_TD, 'min-w-0 text-left break-words'].join(' ')}>{r.errorType}</td>
                  <td className={[ADMIN_TD, 'min-w-0 text-left break-words'].join(' ')}>
                    {maybeValue(r.domainCode)}
                  </td>
                  <td className={[ADMIN_TD, 'min-w-0 text-left'].join(' ')}>
                    <div className="break-words line-clamp-3" title={r.message}>
                      {r.message}
                    </div>
                  </td>
                  <td className={[ADMIN_TD, 'min-w-0 text-left'].join(' ')}>
                    <CopyableId value={r.traceId} maxWidthClassName="max-w-full" />
                  </td>
                  <td className={[ADMIN_TD, 'min-w-0 text-left'].join(' ')}>
                    <CopyableId value={r.userId} maxWidthClassName="max-w-full" />
                  </td>
                  <td className={[ADMIN_TD, 'min-w-0 text-left'].join(' ')}>
                    <CopyableId value={r.telegramId} maxWidthClassName="max-w-full" />
                  </td>
                  <td className={[ADMIN_TD, 'text-right'].join(' ')}>
                    <button
                      type="button"
                      className="h-8 w-8 inline-flex items-center justify-center rounded-md text-muted-foreground hover:bg-accent hover:text-foreground disabled:opacity-50"
                      disabled={deleteDisabled}
                      aria-label="Delete error event"
                      title="Delete error event"
                      onClick={() => onDeleteClick(r.id)}
                    >
                      ⋯
                    </button>
                  </td>
                </tr>
              ))}
        </tbody>
      </table>
    </div>
  )
}
