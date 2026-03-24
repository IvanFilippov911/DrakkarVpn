export type StatusIndicatorTone = 'ready' | 'pending' | 'blocked' | 'inactive'

type StatusIndicatorProps = {
  label: string
  tone: StatusIndicatorTone
}

const toneClasses: Record<StatusIndicatorTone, { dot: string; ring: string; text: string }> = {
  ready: {
    dot: 'bg-[oklch(0.52_0.12_240)] animate-status-breathe',
    ring: 'bg-[oklch(0.5_0.1_240_/_0.25)]',
    text: 'text-[oklch(0.58_0.12_240)]',
  },
  pending: {
    dot: 'bg-[oklch(0.5_0.1_240)]',
    ring: 'bg-[oklch(0.5_0.1_240_/_0.4)] animate-underwater-pulse',
    text: 'text-[oklch(0.55_0.1_240)]',
  },
  blocked: {
    dot: 'bg-[oklch(0.5_0.15_25)]',
    ring: 'bg-transparent',
    text: 'text-[oklch(0.55_0.12_25)]',
  },
  inactive: {
    dot: 'bg-[var(--steel)]',
    ring: 'bg-transparent',
    text: 'text-[var(--steel-light)]',
  },
}

export function StatusIndicator({ label, tone }: StatusIndicatorProps) {
  const toneClass = toneClasses[tone]

  return (
    <div className="inline-flex items-center gap-2">
      <span className="relative inline-flex h-2 w-2 items-center justify-center" aria-hidden>
        <span className={`absolute h-2 w-2 rounded-full ${toneClass.ring}`} />
        <span className={`relative h-1.5 w-1.5 rounded-full ${toneClass.dot}`} />
      </span>
      <span className={`text-[11px] tracking-wide lowercase ${toneClass.text}`}>{label}</span>
    </div>
  )
}
