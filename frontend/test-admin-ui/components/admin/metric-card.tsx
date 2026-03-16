import { cn } from '@/lib/utils'
import { Skeleton } from '@/components/ui/skeleton'

interface MetricCardProps {
  title: string
  children: React.ReactNode
  className?: string
}

export function MetricCard({ title, children, className }: MetricCardProps) {
  return (
    <div className={cn('rounded-lg border bg-card p-4', className)}>
      <h3 className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-3">{title}</h3>
      <div className="space-y-2">
        {children}
      </div>
    </div>
  )
}

interface MetricRowProps {
  label: string
  value: string | number | React.ReactNode
  secondary?: string
  highlight?: boolean
}

export function MetricRow({ label, value, secondary, highlight }: MetricRowProps) {
  return (
    <div className="flex items-center justify-between text-sm">
      <span className="text-muted-foreground">{label}</span>
      <div className="text-right">
        <span className={cn('font-medium tabular-nums', highlight && 'text-emerald-600')}>{value}</span>
        {secondary && <span className="text-muted-foreground ml-1 text-xs">{secondary}</span>}
      </div>
    </div>
  )
}

interface KpiCardProps {
  label: string
  value: string | number
  subValue?: string
  className?: string
}

export function KpiCard({ label, value, subValue, className }: KpiCardProps) {
  return (
    <div className={cn('px-4 py-3 border-r last:border-r-0', className)}>
      <div className="text-xs text-muted-foreground mb-0.5">{label}</div>
      <div className="flex items-baseline gap-1.5">
        <span className="text-xl font-semibold tabular-nums">{value}</span>
        {subValue && <span className="text-sm text-muted-foreground">{subValue}</span>}
      </div>
    </div>
  )
}

export function MetricCardSkeleton() {
  return (
    <div className="rounded-lg border bg-card p-4">
      <Skeleton className="h-3 w-24 mb-3" />
      <div className="space-y-2">
        <div className="flex justify-between">
          <Skeleton className="h-4 w-20" />
          <Skeleton className="h-4 w-16" />
        </div>
        <div className="flex justify-between">
          <Skeleton className="h-4 w-24" />
          <Skeleton className="h-4 w-12" />
        </div>
        <div className="flex justify-between">
          <Skeleton className="h-4 w-16" />
          <Skeleton className="h-4 w-20" />
        </div>
      </div>
    </div>
  )
}

export function KpiRowSkeleton() {
  return (
    <div className="flex border rounded-lg">
      {[...Array(4)].map((_, i) => (
        <div key={i} className="px-4 py-3 border-r last:border-r-0 flex-1">
          <Skeleton className="h-3 w-16 mb-1" />
          <Skeleton className="h-6 w-20" />
        </div>
      ))}
    </div>
  )
}
