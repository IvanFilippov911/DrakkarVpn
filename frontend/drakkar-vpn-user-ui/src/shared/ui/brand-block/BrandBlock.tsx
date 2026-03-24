import type { ReactNode } from 'react'

type BrandBlockProps = {
  title?: string
  subtitle?: string
  icon?: ReactNode
}

export function BrandBlock({
  title = 'Drakkar Network',
  subtitle = 'Быстрое и приватное подключение',
  icon,
}: BrandBlockProps) {
  return (
    <div className="-mt-12 flex flex-col items-center text-center">
      <div className="mb-4 flex h-14 w-14 items-center justify-center text-[var(--arctic)]" aria-hidden>
        {icon ?? <ShieldMark />}
      </div>
      <h1 className="font-brand text-[2.25rem] leading-none font-medium tracking-[0.02em] text-[var(--foreground)]">
        {title}
      </h1>
      {subtitle ? (
        <p className="mt-2 max-w-xs text-sm font-normal tracking-[0.02em] text-[var(--muted-strong)]">
          {subtitle}
        </p>
      ) : null}
    </div>
  )
}

function ShieldMark() {
  return (
    <svg viewBox="0 0 48 48" fill="none" className="h-14 w-14" aria-hidden>
      <path
        d="M24 4L8 10v14c0 11.5 6.8 22.2 16 24 9.2-1.8 16-12.5 16-24V10L24 4z"
        stroke="currentColor"
        strokeWidth="2"
        strokeLinejoin="round"
      />
      <path
        d="M18 24l4 4 8-8"
        stroke="currentColor"
        strokeWidth="2"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  )
}
