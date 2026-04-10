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
    <header className="shrink-0 px-6 pb-4 pt-[max(2.75rem,calc(env(safe-area-inset-top,0px)+14px))]">
      <div className="flex items-center gap-3">
        {onBack ? (
          <button
            type="button"
            onClick={() => onBack()}
            className="min-w-12 shrink-0 text-left text-sm font-normal text-[var(--text-sub)] transition-colors hover:text-[var(--foreground)]"
          >
            {backLabel}
          </button>
        ) : (
          <div className="min-w-12 shrink-0" aria-hidden />
        )}
        <h1 className="min-w-0 flex-1 text-center font-sans text-xl font-bold tracking-[-0.02em] text-[var(--foreground)]">
          {title}
        </h1>
        <div className="flex min-w-12 justify-end">{trailing}</div>
      </div>
    </header>
  )
}
