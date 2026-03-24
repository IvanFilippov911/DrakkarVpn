import type { ReactNode } from 'react'

export type StateCardVariant = 'default' | 'success' | 'warning' | 'error'

type StateCardProps = {
  title: string
  subtitle?: string
  icon?: ReactNode
  variant?: StateCardVariant
  footer?: ReactNode
}

const variantClass: Record<StateCardVariant, string> = {
  default: 'border-[rgba(35,33,30,0.8)] bg-[var(--surface-2)]',
  success: 'border-[oklch(0.55_0.15_240_/_0.3)] bg-[var(--surface-3)]',
  warning: 'border-[rgba(88,85,78,0.8)] bg-[var(--surface-3)]',
  error: 'border-[rgba(163,56,85,0.45)] bg-[var(--error-bg)]',
}

export function StateCard({
  title,
  subtitle,
  icon,
  variant = 'default',
  footer,
}: StateCardProps) {
  return (
    <div
      className={`flex max-w-md flex-row items-center gap-4 rounded-xl border px-6 py-4 ${variantClass[variant]}`}
    >
      {icon ? (
        <div className="flex h-11 w-11 shrink-0 items-center justify-center" aria-hidden>
          {icon}
        </div>
      ) : null}
      <div className="min-w-0 flex-1 text-left">
        <p className="text-sm font-medium tracking-[0.02em] text-[var(--foreground)]">{title}</p>
        {subtitle ? (
          <p className="mt-1 text-xs font-normal text-[var(--muted-soft)]">{subtitle}</p>
        ) : null}
        {footer}
      </div>
    </div>
  )
}
