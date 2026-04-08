import type { ButtonHTMLAttributes, ReactNode } from 'react'

type PrimaryButtonProps = {
  children: ReactNode
} & ButtonHTMLAttributes<HTMLButtonElement>

export function PrimaryButton({ children, className = '', type = 'button', ...rest }: PrimaryButtonProps) {
  return (
    <button
      type={type}
      className={[
        'w-full rounded-xl border border-[var(--btn-primary-border)] bg-[var(--btn-primary-bg)] px-8 py-3.5',
        'font-sans text-base font-semibold text-[var(--primary-foreground)] shadow-none sm:text-[1.125rem]',
        'transition-colors focus-visible:outline-none',
        'hover:bg-[var(--btn-primary-hover)]',
        'disabled:pointer-events-none disabled:border-[var(--border)] disabled:bg-[var(--surface-2)] disabled:text-[var(--text-status)] disabled:shadow-none',
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
