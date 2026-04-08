import type { UsersBulkAction } from '../types'

export function UsersBulkActionsBar({
  selectedCount,
  selectedAction,
  onActionChange,
  onUpdate,
  updateDisabled,
}: {
  selectedCount: number
  selectedAction: UsersBulkAction | ''
  onActionChange: (value: UsersBulkAction | '') => void
  onUpdate: () => void
  updateDisabled: boolean
}) {
  return (
    <div className="mb-3">
      <div className="flex flex-wrap items-center gap-2 rounded-lg border bg-muted/30 px-3 py-2">
        <div className="text-xs text-muted-foreground">
          Selected <span className="tabular-nums text-foreground">{selectedCount}</span>
        </div>

        <div className="w-64 max-w-full">
          <select
            className="h-8 w-full rounded-md border border-border/60 bg-background/60 px-2 text-sm shadow-sm focus:bg-background disabled:opacity-60"
            disabled={selectedCount === 0}
            value={selectedAction}
            onChange={(e) => onActionChange(e.target.value as UsersBulkAction | '')}
          >
            <option value="">Bulk action…</option>
            <option value="ban">Ban</option>
            <option value="unban">Unban</option>
            <option value="grantSubscription">Grant Subscription</option>
            <option value="markInternal">Mark Internal</option>
          </select>
        </div>

        <div className="flex-1" />

        <button
          type="button"
          className="h-8 px-3 rounded-md text-sm font-medium bg-secondary text-secondary-foreground hover:bg-secondary/80 disabled:opacity-50 disabled:cursor-not-allowed"
          disabled={updateDisabled}
          onClick={onUpdate}
        >
          Update
        </button>
      </div>
    </div>
  )
}

