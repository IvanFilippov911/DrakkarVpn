import type { ReactNode } from 'react'

export function PageHeader({
  title,
  description,
  action,
}: {
  title: string
  description?: string
  action?: ReactNode
}) {
  return (
    <div className="flex items-center justify-between mb-6">
      <div>
        <div className="text-lg font-semibold">{title}</div>
        {description ? (
          <div className="text-sm text-muted-foreground mt-0.5">{description}</div>
        ) : null}
      </div>
      {action ? <div>{action}</div> : null}
    </div>
  )
}

