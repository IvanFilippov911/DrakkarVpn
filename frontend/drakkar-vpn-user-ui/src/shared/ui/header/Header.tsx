import type { ReactNode } from 'react'

type HeaderProps = {
  title: string
  /** Скелетон: можно оставить пустым, кнопка не навигирует. */
  onBack?: () => void
  backLabel?: string
  trailing?: ReactNode
}

export function Header({
  title,
  onBack,
  backLabel = 'Назад',
  trailing,
}: HeaderProps) {
  return (
    <header className="shrink-0 px-6 pt-6 pb-4">
      <div className="flex items-center gap-3">
        <button
          type="button"
          onClick={() => onBack?.()}
          className="min-w-12 text-left text-sm font-medium tracking-[0.02em] text-[var(--muted-strong)] transition-colors hover:text-[var(--foreground)]"
        >
          {backLabel}
        </button>
        <h1 className="min-w-0 flex-1 text-center font-brand text-xl font-semibold tracking-[0.02em] text-[var(--foreground)]">
          {title}
        </h1>
        <div className="flex min-w-12 justify-end">{trailing}</div>
      </div>
    </header>
  )
}
