import type { ReactNode } from 'react'

export function MetricCard({ title, children }: { title: string; children: ReactNode }) {
  return (
    <div className="rounded-lg border bg-card p-4">
      <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-3">
        {title}
      </div>
      <div className="space-y-2">{children}</div>
    </div>
  )
}

