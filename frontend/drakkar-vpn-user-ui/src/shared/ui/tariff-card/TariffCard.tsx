export type TariffCardProps = {
  name: string
  price: string
  period: string
  features: string[]
  popular?: boolean
  selected?: boolean
  onSelect?: () => void
}

export function TariffCard({
  name,
  price,
  period,
  features,
  popular,
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
        'flex w-full flex-col rounded-xl border p-5 text-left transition-colors',
        selected
          ? 'border-[oklch(0.55_0.15_240_/_0.3)] bg-[var(--surface-3)]'
          : 'border-[rgba(35,33,30,0.8)] bg-[var(--surface-2)] hover:border-[var(--border)]',
        interactive ? 'cursor-pointer focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[var(--arctic)] focus-visible:ring-offset-0' : '',
      ]
        .filter(Boolean)
        .join(' ')}
    >
      <div className="flex items-start justify-between gap-3">
        <div className="min-w-0">
          <div className="flex flex-wrap items-center gap-2">
            <h2 className="font-brand text-lg font-semibold text-[var(--foreground)]">{name}</h2>
            {popular ? <PopularBadge /> : null}
          </div>
        </div>
        <SelectionDot selected={selected} />
      </div>
      <div className="mt-4">
        <p className="font-brand text-3xl font-semibold tracking-[0.02em] text-[var(--foreground)]">{price}</p>
        <p className="mt-1 text-sm text-[var(--muted-soft)]">{period}</p>
      </div>
      {features.length > 0 ? (
        <ul className="mt-4 flex flex-col gap-2">
          {features.map((f) => (
            <li key={f} className="text-[11px] tracking-wide text-[var(--muted-strong)]">
              {f}
            </li>
          ))}
        </ul>
      ) : null}
    </button>
  )
}

function PopularBadge() {
  return (
    <span className="rounded px-2 py-0.5 text-[10px] font-medium uppercase tracking-[0.12em] text-[var(--muted-strong)]">
      Popular
    </span>
  )
}

function SelectionDot({ selected }: { selected: boolean }) {
  return (
    <div
      className={[
        'flex h-5 w-5 shrink-0 items-center justify-center rounded-full border transition-colors',
        selected
          ? 'border-[var(--arctic)] bg-[var(--arctic)]'
          : 'border-[var(--steel)] bg-transparent',
      ].join(' ')}
      aria-hidden
    >
      {selected ? <span className="h-1.5 w-1.5 rounded-full bg-[var(--primary-foreground)]" /> : null}
    </div>
  )
}
