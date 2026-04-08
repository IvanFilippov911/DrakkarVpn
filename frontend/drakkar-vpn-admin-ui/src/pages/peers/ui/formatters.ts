const DASH = '—'

export function formatNullable<T>(value: T | null | undefined, map: (v: T) => string): string {
  if (value === null || value === undefined) return DASH
  return map(value)
}

export function formatBytes(bytes: number | null | undefined): string {
  if (bytes === null || bytes === undefined) return DASH
  if (!Number.isFinite(bytes)) return DASH
  if (bytes === 0) return '0 B'

  const sign = bytes < 0 ? '-' : ''
  let n = Math.abs(bytes)
  const units = ['B', 'KB', 'MB', 'GB', 'TB'] as const
  let unitIndex = 0

  while (n >= 1024 && unitIndex < units.length - 1) {
    n /= 1024
    unitIndex++
  }

  const digits = n >= 100 ? 0 : n >= 10 ? 1 : 2
  return `${sign}${n.toFixed(digits)} ${units[unitIndex]}`
}

export function formatMbps(mbps: number | null | undefined): string {
  if (mbps === null || mbps === undefined) return DASH
  if (!Number.isFinite(mbps)) return DASH
  const digits = Math.abs(mbps) >= 100 ? 0 : 1
  return `${mbps.toFixed(digits)} Mbps`
}

export function formatMs(ms: number | null | undefined): string {
  if (ms === null || ms === undefined) return DASH
  if (!Number.isFinite(ms)) return DASH
  const digits = Math.abs(ms) >= 100 ? 0 : 1
  return `${ms.toFixed(digits)} ms`
}

export function formatUtcDateTime(isoUtc: string | null | undefined): string {
  if (!isoUtc) return DASH
  const d = new Date(isoUtc)
  if (Number.isNaN(d.getTime())) return DASH

  return new Intl.DateTimeFormat(undefined, {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  }).format(d)
}

