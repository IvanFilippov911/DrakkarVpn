import { Overlay } from '../../../shared/ui'

export function UserRowActionsModal({
  open,
  userId,
  onClose,
  onPickAction,
}: {
  open: boolean
  userId: string | null
  onClose: () => void
  onPickAction: (action: 'ban' | 'unban' | 'grantSubscription' | 'markInternal') => void
}) {
  return (
    <Overlay open={open} onClose={onClose} maxWidthClassName="max-w-sm">
      <div className="p-4">
        <div className="text-sm font-medium">User actions</div>
        <div className="mt-1 text-xs text-muted-foreground font-mono break-all">{userId ?? '—'}</div>

        <div className="mt-4 flex flex-col gap-1">
          <button
            type="button"
            className="w-full rounded-md px-3 py-2 text-left text-sm text-destructive hover:bg-accent"
            onClick={() => onPickAction('ban')}
          >
            Ban user
          </button>
          <button
            type="button"
            className="w-full rounded-md px-3 py-2 text-left text-sm hover:bg-accent"
            onClick={() => onPickAction('unban')}
          >
            Unban user
          </button>
          <button
            type="button"
            className="w-full rounded-md px-3 py-2 text-left text-sm hover:bg-accent"
            onClick={() => onPickAction('grantSubscription')}
          >
            Grant subscription
          </button>
          <button
            type="button"
            className="w-full rounded-md px-3 py-2 text-left text-sm hover:bg-accent"
            onClick={() => onPickAction('markInternal')}
          >
            Mark internal
          </button>
        </div>

        <button
          type="button"
          className="mt-3 w-full h-9 rounded-md text-sm text-muted-foreground hover:bg-accent"
          onClick={onClose}
        >
          Cancel
        </button>
      </div>
    </Overlay>
  )
}
