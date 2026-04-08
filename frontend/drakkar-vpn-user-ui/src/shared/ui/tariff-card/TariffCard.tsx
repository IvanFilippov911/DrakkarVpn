export type TariffCardProps = {
  name: string
  price: string
  durationLabel: string
  features: string[]
  recommended?: boolean
  selected?: boolean
  onSelect?: () => void
}

export function TariffCard({
  name,
  price,
  durationLabel,
  features,
  recommended,
  selected = false,
  onSelect,
}: TariffCardProps) {
  const interactive = onSelect != null

  return (
    <button
      type="button"
      disabled={!interactive}
      onClick={onSelect}
      className={[
        'flex w-full flex-col rounded-2xl border border-[var(--btn-primary-border)] bg-transparent p-6 text-left transition-colors',
        selected
          ? 'border-[var(--btn-primary-bg)] bg-[rgba(255,255,255,0.03)]'
          : 'hover:border-[rgba(255,255,255,0.1)]',
        interactive ? 'cursor-pointer focus-visible:outline-none' : '',
      ]
        .filter(Boolean)
        .join(' ')}
    >
      <div className="flex items-start justify-between gap-3">
        <div className="min-w-0">
          <div className="flex flex-wrap items-center gap-2">
            <h2 className="truncate font-sans text-lg font-semibold text-[var(--foreground)]">{name}</h2>
            {recommended ? <RecommendedBadge /> : null}
          </div>
        </div>
        <Radio selected={selected} />
      </div>

      <div className="mt-4">
        <p className="font-sans text-3xl font-bold tracking-[-0.03em] text-[var(--foreground)]">{durationLabel}</p>
        <p className="mt-1 text-base font-semibold text-[var(--muted-soft)]">{price}</p>
      </div>

      {features.length > 0 ? (
        <ul className="mt-4 flex flex-col gap-2">
          {features.map((f) => (
            <li key={f} className="text-[12px] leading-snug text-[var(--muted-strong)]">
              {f}
            </li>
          ))}
        </ul>
      ) : null}
    </button>
  )
}

function RecommendedBadge() {
  return (
    <span className="rounded-full border border-[rgba(255,255,255,0.08)] bg-[rgba(255,255,255,0.04)] px-2 py-0.5 text-[10px] font-semibold tracking-[-0.01em] text-[var(--foreground)]/80">
      Лучший выбор
    </span>
  )
}

function Radio({ selected }: { selected: boolean }) {
  return (
    <div
      className={[
        'flex h-5 w-5 shrink-0 items-center justify-center rounded-full border transition-colors',
        selected
          ? 'border-[var(--btn-primary-bg)] bg-[var(--btn-primary-bg)]'
          : 'border-[rgba(255,255,255,0.18)] bg-transparent',
      ].join(' ')}
      aria-hidden
    >
      {selected ? <span className="h-1.5 w-1.5 rounded-full bg-[var(--primary-foreground)]" /> : null}
    </div>
  )
}
