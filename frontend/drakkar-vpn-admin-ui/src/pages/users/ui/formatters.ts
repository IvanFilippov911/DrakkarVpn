import { formatBytes, formatNumber } from '../../../shared/lib/formatters'

export function formatMaybeNumber(value: number | null | undefined): string {
  if (value == null) return '—'
  if (!Number.isFinite(value)) return '—'
  return formatNumber(value)
}

export function formatMaybeBytes(value: number | null | undefined): string {
  if (value == null) return '—'
  if (!Number.isFinite(value) || value < 0) return '—'
  return formatBytes(value)
}

export function formatMaybeTelegram(value: number | null | undefined): string {
  if (value == null) return '—'
  if (!Number.isFinite(value)) return '—'
  return String(Math.trunc(value))
}

/** Backend sends enum as string (Active | Expired | Cancelled) via JsonStringEnumConverter. */
export function formatMaybeSubscriptionStatus(value: string | null | undefined): string {
  if (value == null || value === '') return '—'
  return value
}

export function formatMaybeDateTimeUtc(value: string | null | undefined): string {
  if (!value) return '—'

  const d = new Date(value)
  if (Number.isNaN(d.getTime())) return value

  // Treat UnixEpoch as "missing data" for this page.
  if (d.getTime() === 0) return '—'

  return d.toLocaleString('en-US', {
    year: 'numeric',
    month: 'short',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  })
}

