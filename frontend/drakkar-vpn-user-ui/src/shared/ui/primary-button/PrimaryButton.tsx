import type { ButtonHTMLAttributes, ReactNode } from 'react'

type PrimaryButtonProps = {
  children: ReactNode
} & ButtonHTMLAttributes<HTMLButtonElement>

export function PrimaryButton({ children, className = '', type = 'button', ...rest }: PrimaryButtonProps) {
  return (
    <button
      type={type}
      className={[
        'w-full rounded-lg border border-transparent bg-[var(--arctic-base)] px-8 py-5',
        'font-brand text-[15px] font-semibold uppercase tracking-[0.12em] text-[var(--primary-foreground)]',
        'transition-colors active:scale-[0.98] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[var(--arctic)] focus-visible:ring-offset-0',
        'hover:bg-[var(--arctic-hover)]',
        'disabled:pointer-events-none disabled:border-[var(--border)] disabled:bg-[var(--surface-2)] disabled:text-[var(--muted-strong)]',
        className,
      ]
        .filter(Boolean)
        .join(' ')}
      {...rest}
    >
      {children}
    </button>
  )
}
