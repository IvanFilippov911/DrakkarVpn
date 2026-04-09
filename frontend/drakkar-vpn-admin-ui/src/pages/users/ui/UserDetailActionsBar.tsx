import type { AdminUserSummaryDetailDto } from '../../../entities/users'

export function UserDetailActionsBar({
  user,
  onBan,
  onUnban,
  onMarkInternal,
  onGrantSubscription,
}: {
  user: AdminUserSummaryDetailDto
  onBan: () => void
  onUnban: () => void
  onMarkInternal: () => void
  onGrantSubscription: () => void
}) {
  const isBanned = user.status === 'Banned'

  return (
    <div className="flex flex-wrap items-center justify-end gap-2">
      <button
        type="button"
        className="h-8 px-3 rounded-md text-sm font-medium border border-destructive/40 text-destructive hover:bg-destructive/10 disabled:opacity-50 disabled:cursor-not-allowed"
        disabled={isBanned}
        onClick={onBan}
      >
        Ban
      </button>
      <button
        type="button"
        className="h-8 px-3 rounded-md text-sm font-medium bg-secondary text-secondary-foreground hover:bg-secondary/80 disabled:opacity-50 disabled:cursor-not-allowed"
        disabled={!isBanned}
        onClick={onUnban}
      >
        Unban
      </button>
      <button
        type="button"
        className="h-8 px-3 rounded-md text-sm font-medium border border-border/60 bg-background hover:bg-accent"
        onClick={onMarkInternal}
      >
        Mark internal
      </button>
      <button
        type="button"
        className="h-8 px-3 rounded-md text-sm font-medium bg-primary text-primary-foreground hover:bg-primary/90"
        onClick={onGrantSubscription}
      >
        Grant subscription
      </button>
    </div>
  )
}
