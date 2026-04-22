import type {
  SortDirection,
  TransportProfilesSortBy,
  TransportType,
} from '../../../entities/transport-profiles'

export function TransportProfilesFiltersRow({
  search,
  isEnabled,
  transportType,
  sortBy,
  sortDirection,
  pageSize,
  onSearchChange,
  onIsEnabledChange,
  onTransportTypeChange,
  onSortByChange,
  onSortDirectionChange,
  onPageSizeChange,
}: {
  search?: string
  isEnabled?: boolean
  transportType?: TransportType
  sortBy: TransportProfilesSortBy
  sortDirection: SortDirection
  pageSize: number
  onSearchChange: (value?: string) => void
  onIsEnabledChange: (value?: boolean) => void
  onTransportTypeChange: (value?: TransportType) => void
  onSortByChange: (value: TransportProfilesSortBy) => void
  onSortDirectionChange: (value: SortDirection) => void
  onPageSizeChange: (value: number) => void
}) {
  return (
    <div className="grid grid-cols-1 lg:grid-cols-6 gap-3 mb-4">
      <div className="lg:col-span-2">
        <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">Search</div>
        <input
          value={search ?? ''}
          onChange={(e) => onSearchChange(e.target.value || undefined)}
          placeholder="Search by name"
          className="h-9 w-full rounded-md border bg-background px-3 text-sm"
        />
      </div>

      <div>
        <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">Status</div>
        <select
          value={isEnabled == null ? 'all' : isEnabled ? 'enabled' : 'disabled'}
          onChange={(e) => {
            const value = e.target.value
            onIsEnabledChange(value === 'all' ? undefined : value === 'enabled')
          }}
          className="h-9 w-full rounded-md border bg-background px-3 text-sm"
        >
          <option value="all">All</option>
          <option value="enabled">Enabled</option>
          <option value="disabled">Disabled</option>
        </select>
      </div>

      <div>
        <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">Transport</div>
        <select
          value={transportType ?? 'all'}
          onChange={(e) => {
            const value = e.target.value
            onTransportTypeChange(value === 'all' ? undefined : (value as TransportType))
          }}
          className="h-9 w-full rounded-md border bg-background px-3 text-sm"
        >
          <option value="all">All</option>
          <option value="Tcp">TCP</option>
          <option value="Grpc">gRPC</option>
        </select>
      </div>

      <div>
        <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">Sort by</div>
        <select
          value={sortBy}
          onChange={(e) => onSortByChange(e.target.value as TransportProfilesSortBy)}
          className="h-9 w-full rounded-md border bg-background px-3 text-sm"
        >
          <option value="GlobalPriority">Global priority</option>
          <option value="UpdatedAtUtc">Updated at</option>
        </select>
      </div>

      <div className="flex items-end gap-2">
        <div className="flex-1">
          <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">Direction</div>
          <select
            value={sortDirection}
            onChange={(e) => onSortDirectionChange(e.target.value as SortDirection)}
            className="h-9 w-full rounded-md border bg-background px-3 text-sm"
          >
            <option value="Asc">Asc</option>
            <option value="Desc">Desc</option>
          </select>
        </div>

        <div className="w-24">
          <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">Page size</div>
          <select
            value={pageSize}
            onChange={(e) => onPageSizeChange(Number(e.target.value))}
            className="h-9 w-full rounded-md border bg-background px-3 text-sm"
          >
            {[10, 20, 50].map((value) => (
              <option key={value} value={value}>
                {value}
              </option>
            ))}
          </select>
        </div>
      </div>
    </div>
  )
}
