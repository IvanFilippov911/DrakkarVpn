import { cn } from '@/lib/utils'

interface StatusBadgeProps {
  status: string
  variant?: 'default' | 'dot'
}

const statusColors: Record<string, { bg: string; text: string; dot: string }> = {
  online: { bg: 'bg-emerald-50', text: 'text-emerald-700', dot: 'bg-emerald-500' },
  active: { bg: 'bg-emerald-50', text: 'text-emerald-700', dot: 'bg-emerald-500' },
  offline: { bg: 'bg-zinc-100', text: 'text-zinc-600', dot: 'bg-zinc-400' },
  inactive: { bg: 'bg-zinc-100', text: 'text-zinc-600', dot: 'bg-zinc-400' },
  disabled: { bg: 'bg-zinc-100', text: 'text-zinc-600', dot: 'bg-zinc-400' },
  maintenance: { bg: 'bg-amber-50', text: 'text-amber-700', dot: 'bg-amber-500' },
  expiring: { bg: 'bg-amber-50', text: 'text-amber-700', dot: 'bg-amber-500' },
  warning: { bg: 'bg-amber-50', text: 'text-amber-700', dot: 'bg-amber-500' },
  suspended: { bg: 'bg-red-50', text: 'text-red-700', dot: 'bg-red-500' },
  expired: { bg: 'bg-red-50', text: 'text-red-700', dot: 'bg-red-500' },
  error: { bg: 'bg-red-50', text: 'text-red-700', dot: 'bg-red-500' },
  critical: { bg: 'bg-red-50', text: 'text-red-700', dot: 'bg-red-500' },
}

export function StatusBadge({ status, variant = 'default' }: StatusBadgeProps) {
  const colors = statusColors[status.toLowerCase()] || { bg: 'bg-zinc-100', text: 'text-zinc-600', dot: 'bg-zinc-400' }

  if (variant === 'dot') {
    return (
      <span className="flex items-center gap-1.5">
        <span className={cn('size-1.5 rounded-full', colors.dot)} />
        <span className={cn('text-sm capitalize', colors.text)}>{status}</span>
      </span>
    )
  }

  return (
    <span
      className={cn(
        'inline-flex items-center gap-1.5 px-2 py-0.5 rounded text-xs font-medium capitalize',
        colors.bg,
        colors.text
      )}
    >
      <span className={cn('size-1.5 rounded-full', colors.dot)} />
      {status}
    </span>
  )
}

export function ReachableBadge({ reachable }: { reachable: boolean }) {
  return (
    <span
      className={cn(
        'inline-flex items-center gap-1.5 px-2 py-0.5 rounded text-xs font-medium',
        reachable ? 'bg-emerald-50 text-emerald-700' : 'bg-red-50 text-red-700'
      )}
    >
      <span className={cn('size-1.5 rounded-full', reachable ? 'bg-emerald-500' : 'bg-red-500')} />
      {reachable ? 'Yes' : 'No'}
    </span>
  )
}
