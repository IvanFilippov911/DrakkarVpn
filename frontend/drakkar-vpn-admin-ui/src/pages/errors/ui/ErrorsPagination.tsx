export function ErrorsPagination({
  page,
  pageSize,
  total,
  totalPages,
  onPrev,
  onNext,
}: {
  page: number
  pageSize: number
  total: number
  totalPages: number
  onPrev: () => void
  onNext: () => void
}) {
  const computedTotalPages = pageSize > 0 ? Math.ceil(total / pageSize) : 0
  const safeTotalPages = totalPages > 0 ? totalPages : computedTotalPages

  return (
    <div className="flex items-center justify-between">
      <div className="text-sm text-muted-foreground">
        Page {page} of {safeTotalPages || 1} · Total {total}
      </div>

      <div className="flex items-center gap-2">
        <button
          type="button"
          className="h-8 px-3 rounded-md text-sm font-medium text-muted-foreground hover:bg-accent hover:text-foreground disabled:opacity-60"
          onClick={onPrev}
          disabled={page <= 1}
        >
          Prev
        </button>
        <button
          type="button"
          className="h-8 px-3 rounded-md text-sm font-medium text-muted-foreground hover:bg-accent hover:text-foreground disabled:opacity-60"
          onClick={onNext}
          disabled={safeTotalPages > 0 ? page >= safeTotalPages : true}
        >
          Next
        </button>
      </div>
    </div>
  )
}

