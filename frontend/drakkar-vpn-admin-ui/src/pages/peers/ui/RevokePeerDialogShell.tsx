import { InlineAlert, Overlay } from '../../../shared/ui'

export function RevokePeerDialogShell({
  open,
  peerId,
  serverId,
  isPending = false,
  errorMessage,
  onCancel,
  onConfirm,
}: {
  open: boolean
  peerId: string
  serverId: string
  isPending?: boolean
  errorMessage?: string
  onCancel: () => void
  onConfirm: () => void
}) {
  return (
    <Overlay open={open} onClose={onCancel}>
      <div className="p-4">
        <div className="text-sm font-medium">Revoke peer?</div>
        <div className="text-sm text-muted-foreground mt-1">
          This action is destructive.
        </div>

        <div className="mt-3 rounded-md border bg-muted/30 p-3 text-xs font-mono">
          <div>peerId: {peerId}</div>
          <div>serverId: {serverId}</div>
        </div>

        {errorMessage ? (
          <div className="mt-3">
            <InlineAlert message={errorMessage} />
          </div>
        ) : null}

        <div className="mt-4 flex items-center justify-end gap-2">
          <button
            type="button"
            className="h-8 px-3 rounded-md text-sm border bg-background hover:bg-accent"
            onClick={onCancel}
            disabled={isPending}
          >
            Cancel
          </button>
          <button
            type="button"
            className="h-8 px-3 rounded-md text-sm font-medium bg-destructive text-white hover:bg-destructive/90"
            onClick={onConfirm}
            disabled={isPending}
          >
            {isPending ? 'Revoking…' : 'Revoke'}
          </button>
        </div>
      </div>
    </Overlay>
  )
}

