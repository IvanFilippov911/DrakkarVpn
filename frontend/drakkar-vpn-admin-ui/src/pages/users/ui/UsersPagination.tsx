export function UsersPagination({
  page,
  pageSize,
  total,
  onPrev,
  onNext,
}: {
  page: number
  pageSize: number
  total: number
  onPrev: () => void
  onNext: () => void
}) {
  const from = total === 0 ? 0 : (page - 1) * pageSize + 1
  const to = Math.min(page * pageSize, total)
  const totalPages = Math.max(1, Math.ceil(total / pageSize))
  const canPrev = page > 1
  const canNext = page < totalPages

  return (
    <div className="mt-4 flex items-center justify-between">
      <div className="text-sm text-muted-foreground">
        Showing <span className="tabular-nums text-foreground">{from}</span>–{' '}
        <span className="tabular-nums text-foreground">{to}</span> of{' '}
        <span className="tabular-nums text-foreground">{total}</span>
      </div>

      <div className="flex items-center gap-2">
        <button
          type="button"
          className="h-8 px-3 rounded-md text-sm border hover:bg-accent disabled:opacity-50 disabled:cursor-not-allowed"
          disabled={!canPrev}
          onClick={onPrev}
        >
          Prev
        </button>
        <div className="text-sm text-muted-foreground">
          Page <span className="tabular-nums text-foreground">{page}</span> /{' '}
          <span className="tabular-nums text-foreground">{totalPages}</span>
        </div>
        <button
          type="button"
          className="h-8 px-3 rounded-md text-sm border hover:bg-accent disabled:opacity-50 disabled:cursor-not-allowed"
          disabled={!canNext}
          onClick={onNext}
        >
          Next
        </button>
      </div>
    </div>
  )
}

