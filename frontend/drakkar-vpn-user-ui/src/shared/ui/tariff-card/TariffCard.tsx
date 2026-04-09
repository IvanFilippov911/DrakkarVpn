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
      aria-pressed={interactive ? selected : undefined}
      className={[
        'flex w-full flex-col rounded-2xl border p-6 text-left transition-[border-color,background-color]',
        selected
          ? 'border-[rgba(255,255,255,0.2)] bg-[rgba(255,255,255,0.04)]'
          : 'border-[var(--btn-primary-border)] bg-transparent hover:border-[rgba(255,255,255,0.1)]',
        interactive ? 'cursor-pointer focus-visible:outline-none' : '',
      ]
        .filter(Boolean)
        .join(' ')}
    >
      <div className="min-w-0">
        <div className="flex flex-wrap items-center gap-2">
          <h2 className="truncate font-sans text-lg font-semibold text-[var(--foreground)]">{name}</h2>
          {recommended ? <RecommendedBadge /> : null}
        </div>
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
    <span className="rounded-full bg-[rgba(255,255,255,0.06)] px-2 py-1 text-[12px] font-medium leading-none tracking-[-0.01em] text-[var(--foreground)]/75">
      Лучший выбор
    </span>
  )
}

