export function formatBytes(bytes: number): string {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB', 'TB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + ' ' + sizes[i]
}

export function formatNumber(num: number | null | undefined, fallback = '—'): string {
  if (num === null || num === undefined) return fallback
  return num.toLocaleString()
}

export function formatDecimal(num: number | null | undefined, decimals = 1, fallback = '—'): string {
  if (num === null || num === undefined) return fallback
  return num.toFixed(decimals)
}

export function formatPercent(num: number | null | undefined, fallback = '—'): string {
  if (num === null || num === undefined) return fallback
  return num.toFixed(2) + '%'
}

export function formatDate(dateString: string | null | undefined, fallback = '—'): string {
  if (!dateString) return fallback
  const date = new Date(dateString)
  return date.toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric'
  })
}

export function formatDateTime(dateString: string | null | undefined, fallback = '—'): string {
  if (!dateString) return fallback
  const date = new Date(dateString)
  return date.toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

export function formatRelativeTime(dateString: string | null | undefined, fallback = '—'): string {
  if (!dateString) return fallback
  const date = new Date(dateString)
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffMins = Math.floor(diffMs / 60000)
  const diffHours = Math.floor(diffMs / 3600000)
  const diffDays = Math.floor(diffMs / 86400000)

  if (diffMins < 1) return 'just now'
  if (diffMins < 60) return `${diffMins}m ago`
  if (diffHours < 24) return `${diffHours}h ago`
  if (diffDays < 7) return `${diffDays}d ago`
  return formatDate(dateString)
}

export function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD'
  }).format(amount)
}
