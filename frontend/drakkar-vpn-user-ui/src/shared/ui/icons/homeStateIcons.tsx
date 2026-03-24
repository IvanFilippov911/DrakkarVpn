export function IconBlocked() {
  return (
    <svg viewBox="0 0 24 24" fill="none" className="h-7 w-7 text-[var(--error-text)]" aria-hidden>
      <circle cx="12" cy="12" r="9" stroke="currentColor" strokeWidth="1.75" />
      <path d="M8 8l8 8M16 8l-8 8" stroke="currentColor" strokeWidth="1.75" strokeLinecap="round" />
    </svg>
  )
}

export function IconNoSubscription() {
  return (
    <svg viewBox="0 0 24 24" fill="none" className="h-7 w-7 text-[var(--muted-strong)]" aria-hidden>
      <path
        d="M12 3v18M7 8h10M9 12h6"
        stroke="currentColor"
        strokeWidth="1.75"
        strokeLinecap="round"
      />
    </svg>
  )
}

export function IconNotStarted() {
  return (
    <svg viewBox="0 0 24 24" fill="none" className="h-7 w-7 text-[var(--arctic)]" aria-hidden>
      <path
        d="M12 5v8l5 3"
        stroke="currentColor"
        strokeWidth="1.75"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <circle cx="12" cy="12" r="9" stroke="currentColor" strokeWidth="1.75" />
    </svg>
  )
}

export function IconPending() {
  return (
    <svg viewBox="0 0 24 24" fill="none" className="h-7 w-7 text-[var(--steel-light)]" aria-hidden>
      <circle cx="12" cy="12" r="9" stroke="currentColor" strokeWidth="1.75" />
      <path
        d="M12 7v5l3 2"
        stroke="currentColor"
        strokeWidth="1.75"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  )
}

export function IconReady() {
  return (
    <svg viewBox="0 0 24 24" fill="none" className="h-7 w-7 text-[var(--arctic)]" aria-hidden>
      <circle cx="12" cy="12" r="9" stroke="currentColor" strokeWidth="1.75" />
      <path
        d="M8 12l3 3 5-6"
        stroke="currentColor"
        strokeWidth="1.75"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  )
}

export function PendingDots() {
  return (
    <div className="mt-3 flex gap-1.5" aria-hidden>
      <span className="inline-block h-1.5 w-1.5 animate-pulse rounded-full bg-[var(--steel-light)]" />
      <span className="inline-block h-1.5 w-1.5 animate-pulse rounded-full bg-[var(--steel-light)]" />
      <span className="inline-block h-1.5 w-1.5 animate-pulse rounded-full bg-[var(--steel-light)]" />
    </div>
  )
}
