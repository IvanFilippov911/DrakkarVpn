import type { AdminUserAlertDto } from '../../../entities/users'
import { EmptyState } from '../../../shared/ui'
import { formatMaybeDateTimeUtc } from './formatters'

const ALERTS_LIMIT_NOTE = 'Last 20 alerts'

function SeverityBadge({ severity }: { severity: string }) {
  const raw = severity.trim()
  if (raw === '') {
    return (
      <span className="inline-flex items-center rounded px-2 py-0.5 text-xs font-medium border bg-zinc-100 text-zinc-600 border-zinc-200">
        —
      </span>
    )
  }

  const lowered = raw.toLowerCase()
  const variant =
    lowered.includes('critical') || lowered.includes('fatal')
      ? 'bg-red-50 text-red-800 border-red-200'
      : lowered.includes('error') || lowered.includes('err')
        ? 'bg-red-50 text-red-700 border-red-200'
        : lowered.includes('warn')
          ? 'bg-amber-50 text-amber-900 border-amber-200'
          : lowered.includes('info')
            ? 'bg-sky-50 text-sky-800 border-sky-200'
            : 'bg-zinc-100 text-zinc-700 border-zinc-200'

  return (
    <span className={['inline-flex items-center rounded px-2 py-0.5 text-xs font-medium border', variant].join(' ')}>
      {raw}
    </span>
  )
}

function ResolvedBadge({ isResolved }: { isResolved: boolean }) {
  const variant = isResolved ? 'bg-emerald-50 text-emerald-800 border-emerald-200' : 'bg-zinc-100 text-zinc-600 border-zinc-200'
  const label = isResolved ? 'Resolved' : 'Open'

  return (
    <span className={['inline-flex items-center rounded px-2 py-0.5 text-xs font-medium border', variant].join(' ')}>
      {label}
    </span>
  )
}

export function UserAlertsSection({ alerts }: { alerts: AdminUserAlertDto[] }) {
  return (
    <section className="border rounded-lg bg-card mt-6">
      <div className="px-4 py-3 border-b">
        <div className="flex flex-col gap-0.5 sm:flex-row sm:items-baseline sm:justify-between sm:gap-4">
          <h2 className="text-base font-semibold">Alerts</h2>
          <p className="text-xs text-muted-foreground">{ALERTS_LIMIT_NOTE}</p>
        </div>
      </div>
      <div className="p-4">
        {alerts.length === 0 ? (
          <EmptyState title="No alerts" description="There are no recent alerts for this user." />
        ) : (
          <ul className="flex flex-col gap-4">
            {alerts.map((alert) => (
              <li key={alert.alertId} className="border rounded-md bg-background p-4">
                <div className="flex flex-wrap items-center gap-2 gap-y-2 mb-2">
                  <time className="text-xs text-muted-foreground tabular-nums" dateTime={alert.createdAtUtc}>
                    {formatMaybeDateTimeUtc(alert.createdAtUtc)}
                  </time>
                  <SeverityBadge severity={alert.severity} />
                  <ResolvedBadge isResolved={alert.isResolved} />
                </div>
                <div className="text-sm font-medium text-foreground">{alert.title}</div>
                <div
                  className={[
                    'mt-2 text-sm text-muted-foreground',
                    'whitespace-pre-wrap break-words',
                    'max-h-48 overflow-y-auto rounded border border-border/50 bg-muted/30 px-2 py-1.5',
                  ].join(' ')}
                  title={alert.message}
                >
                  {alert.message}
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>
    </section>
  )
}
