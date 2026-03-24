export function formatNumber(value: number): string {
  return new Intl.NumberFormat('en-US').format(value)
}

export function formatDecimal(value: number): string {
  return new Intl.NumberFormat('en-US', { maximumFractionDigits: 2 }).format(value)
}

export function formatPercent(percent: number): string {
  return `${formatDecimal(percent)}%`
}

export function formatBytes(bytes: number): string {
  if (bytes === 0) return '0 B'

  const units = ['B', 'KB', 'MB', 'GB', 'TB'] as const
  const exponent = Math.min(Math.floor(Math.log(bytes) / Math.log(1024)), units.length - 1)
  const value = bytes / 1024 ** exponent

  return `${formatDecimal(value)} ${units[exponent]}`
}

export function formatUtcWindow(startUtc: string, endUtc: string): string {
  return `${startUtc} → ${endUtc}`
}

