import type { UsersFiltersState, UsersSortingState } from '../types'

export function UsersFiltersRow({
  filters,
  sorting,
  onSearchChange,
  onStatusChange,
  onSubscriptionStatusChange,
  onSortByChange,
  onDirectionChange,
}: {
  filters: UsersFiltersState
  sorting: UsersSortingState
  onSearchChange: (value: string) => void
  onStatusChange: (value: UsersFiltersState['status']) => void
  onSubscriptionStatusChange: (value: UsersFiltersState['subscriptionStatus']) => void
  onSortByChange: (value: UsersSortingState['sortBy']) => void
  onDirectionChange: (value: UsersSortingState['userSortDirection']) => void
}) {
  return (
    <div className="mb-6">
      <div className="flex flex-wrap items-end gap-3">
        <div className="min-w-56">
          <label className="block text-xs font-medium text-muted-foreground mb-1">Search</label>
          <input
            placeholder="Search users…"
            className="h-9 w-full rounded-md border bg-background px-3 text-sm"
            value={filters.search}
            onChange={(e) => onSearchChange(e.target.value)}
          />
        </div>

        <div className="min-w-44">
          <label className="block text-xs font-medium text-muted-foreground mb-1">Status</label>
          <select
            className="h-9 w-full rounded-md border bg-background px-2 text-sm"
            value={filters.status ?? ''}
            onChange={(e) => onStatusChange(e.target.value ? e.target.value : null)}
          >
            <option value="">All</option>
            <option value="Active">Active</option>
            <option value="Banned">Banned</option>
          </select>
        </div>

        <div className="min-w-52">
          <label className="block text-xs font-medium text-muted-foreground mb-1">
            Subscription Status
          </label>
          <select
            className="h-9 w-full rounded-md border bg-background px-2 text-sm"
            value={filters.subscriptionStatus ?? ''}
            onChange={(e) => onSubscriptionStatusChange(e.target.value ? e.target.value : null)}
          >
            <option value="">All</option>
            <option value="Active">Active</option>
            <option value="Expired">Expired</option>
            <option value="Cancelled">Cancelled</option>
          </select>
        </div>

        <div className="min-w-44">
          <label className="block text-xs font-medium text-muted-foreground mb-1">Sort By</label>
          <select
            className="h-9 w-full rounded-md border bg-background px-2 text-sm"
            value={sorting.sortBy}
            onChange={(e) => onSortByChange(e.target.value as UsersSortingState['sortBy'])}
          >
            <option value="CreatedAt">Created</option>
            <option value="LastSeen">Last Seen</option>
            <option value="SubscriptionEnd">Subscription End</option>
            <option value="Devices">Devices</option>
            <option value="Traffic24h">Traffic 24h</option>
          </select>
        </div>

        <div className="min-w-36">
          <label className="block text-xs font-medium text-muted-foreground mb-1">Direction</label>
          <select
            className="h-9 w-full rounded-md border bg-background px-2 text-sm"
            value={sorting.userSortDirection}
            onChange={(e) => onDirectionChange(e.target.value as UsersSortingState['userSortDirection'])}
          >
            <option value="Desc">Desc</option>
            <option value="Asc">Asc</option>
          </select>
        </div>
      </div>
    </div>
  )
}

