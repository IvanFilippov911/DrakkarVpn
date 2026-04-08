export function PeersPagination({
  page,
  totalPages,
  onPrev,
  onNext,
}: {
  page: number
  totalPages: number
  onPrev: () => void
  onNext: () => void
}) {
  const canPrev = page > 1
  const canNext = page < totalPages

  return (
    <div className="flex items-center justify-between mt-4">
      <div className="text-sm text-muted-foreground">
        Page <span className="tabular-nums">{page}</span> of{' '}
        <span className="tabular-nums">{totalPages}</span>
      </div>

      <div className="flex items-center gap-2">
        <button
          type="button"
          className="h-8 px-3 rounded-md text-sm border bg-background hover:bg-accent disabled:opacity-50"
          onClick={onPrev}
          disabled={!canPrev}
        >
          Previous
        </button>
        <button
          type="button"
          className="h-8 px-3 rounded-md text-sm border bg-background hover:bg-accent disabled:opacity-50"
          onClick={onNext}
          disabled={!canNext}
        >
          Next
        </button>
      </div>
    </div>
  )
}

