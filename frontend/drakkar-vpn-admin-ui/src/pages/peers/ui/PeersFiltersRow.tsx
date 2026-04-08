import type { PeerSortBy, SortDirection } from './types'

export function PeersFiltersRow({
  peerId,
  onlyOnline,
  sortBy,
  direction,
  onPeerIdChange,
  onOnlyOnlineChange,
  onSortByChange,
  onDirectionChange,
}: {
  peerId: string
  onlyOnline: 'all' | 'online'
  sortBy: PeerSortBy
  direction: SortDirection
  onPeerIdChange: (value: string) => void
  onOnlyOnlineChange: (value: 'all' | 'online') => void
  onSortByChange: (value: PeerSortBy) => void
  onDirectionChange: (value: SortDirection) => void
}) {
  return (
    <div className="flex items-end gap-3 mb-4">
      <div>
        <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
          Peer ID
        </div>
        <input
          value={peerId}
          onChange={(e) => onPeerIdChange(e.target.value)}
          placeholder="Search by peer ID"
          className="h-9 w-80 rounded-md border bg-background px-3 text-sm font-mono"
        />
      </div>

      <div>
        <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
          Online Status
        </div>
        <select
          value={onlyOnline}
          onChange={(e) => onOnlyOnlineChange(e.target.value as 'all' | 'online')}
          className="h-9 rounded-md border bg-background px-3 text-sm"
        >
          <option value="all">All</option>
          <option value="online">Online only</option>
        </select>
      </div>

      <div className="ml-auto flex items-end gap-3">
        <div>
          <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
            Sort By
          </div>
          <select
            value={sortBy}
            onChange={(e) => onSortByChange(e.target.value as PeerSortBy)}
            className="h-9 rounded-md border bg-background px-3 text-sm"
          >
            <option value="LastActivity">Last Activity</option>
            <option value="Traffic1h">Traffic (1h)</option>
            <option value="Traffic24h">Traffic (24h)</option>
            <option value="Speed">Speed</option>
            <option value="Latency">Latency</option>
            <option value="CreatedAt">Created</option>
          </select>
        </div>

        <div>
          <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
            Direction
          </div>
          <select
            value={direction}
            onChange={(e) => onDirectionChange(e.target.value as SortDirection)}
            className="h-9 rounded-md border bg-background px-3 text-sm"
          >
            <option value="Asc">Asc</option>
            <option value="Desc">Desc</option>
          </select>
        </div>
      </div>
    </div>
  )
}

