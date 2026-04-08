type ErrorsFilters = {
  search: string
  area: string
  errorType: string
  command: string
  domainCode: string
  userId: string
  telegramId: string
  fromUtc: string
  toUtc: string
}

export function ErrorsFiltersRow({
  filters,
  onChange,
}: {
  filters: ErrorsFilters
  onChange: (next: ErrorsFilters) => void
}) {
  return (
    <div className="mb-6 border rounded-lg bg-card p-4">
      <div className="grid grid-cols-12 gap-3">
        <div className="col-span-12 md:col-span-4">
          <label className="block text-xs text-muted-foreground mb-1">Search</label>
          <input
            value={filters.search}
            onChange={(e) => onChange({ ...filters, search: e.target.value })}
            placeholder="Search message…"
            className="h-9 w-full rounded-md border bg-background px-3 text-sm"
          />
        </div>

        <div className="col-span-12 md:col-span-2">
          <label className="block text-xs text-muted-foreground mb-1">Area</label>
          <input
            value={filters.area}
            onChange={(e) => onChange({ ...filters, area: e.target.value })}
            placeholder="Area"
            className="h-9 w-full rounded-md border bg-background px-3 text-sm"
          />
        </div>

        <div className="col-span-12 md:col-span-2">
          <label className="block text-xs text-muted-foreground mb-1">Error Type</label>
          <input
            value={filters.errorType}
            onChange={(e) => onChange({ ...filters, errorType: e.target.value })}
            placeholder="Error type"
            className="h-9 w-full rounded-md border bg-background px-3 text-sm"
          />
        </div>

        <div className="col-span-12 md:col-span-2">
          <label className="block text-xs text-muted-foreground mb-1">Command</label>
          <input
            value={filters.command}
            onChange={(e) => onChange({ ...filters, command: e.target.value })}
            placeholder="Command"
            className="h-9 w-full rounded-md border bg-background px-3 text-sm"
          />
        </div>

        <div className="col-span-12 md:col-span-2">
          <label className="block text-xs text-muted-foreground mb-1">Domain Code</label>
          <input
            value={filters.domainCode}
            onChange={(e) => onChange({ ...filters, domainCode: e.target.value })}
            placeholder="Domain code"
            className="h-9 w-full rounded-md border bg-background px-3 text-sm"
          />
        </div>

        <div className="col-span-12 md:col-span-2">
          <label className="block text-xs text-muted-foreground mb-1">User ID</label>
          <input
            value={filters.userId}
            onChange={(e) => onChange({ ...filters, userId: e.target.value })}
            placeholder="User ID"
            className="h-9 w-full rounded-md border bg-background px-3 text-sm"
          />
        </div>

        <div className="col-span-12 md:col-span-2">
          <label className="block text-xs text-muted-foreground mb-1">Telegram ID</label>
          <input
            value={filters.telegramId}
            onChange={(e) => onChange({ ...filters, telegramId: e.target.value })}
            placeholder="Telegram ID"
            className="h-9 w-full rounded-md border bg-background px-3 text-sm"
          />
        </div>

        <div className="col-span-12 md:col-span-2">
          <label className="block text-xs text-muted-foreground mb-1">From</label>
          <input
            type="datetime-local"
            value={filters.fromUtc}
            onChange={(e) => onChange({ ...filters, fromUtc: e.target.value })}
            placeholder="From"
            className="h-9 w-full rounded-md border bg-background px-3 text-sm"
          />
        </div>

        <div className="col-span-12 md:col-span-2">
          <label className="block text-xs text-muted-foreground mb-1">To</label>
          <input
            type="datetime-local"
            value={filters.toUtc}
            onChange={(e) => onChange({ ...filters, toUtc: e.target.value })}
            placeholder="To"
            className="h-9 w-full rounded-md border bg-background px-3 text-sm"
          />
        </div>
      </div>
    </div>
  )
}

