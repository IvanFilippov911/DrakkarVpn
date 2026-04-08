import { Overlay } from '../../../shared/ui'

export function DeleteErrorEventDialog({
  open,
  id,
  pending,
  errorMessage,
  onClose,
  onConfirm,
}: {
  open: boolean
  id: string | null
  pending: boolean
  errorMessage: string | null
  onClose: () => void
  onConfirm: (id: string) => void
}) {
  return (
    <Overlay open={open} onClose={onClose}>
      <div className="p-4">
        <div className="text-sm font-medium">Delete error event</div>
        <div className="text-sm text-muted-foreground mt-1">
          This action is destructive and cannot be undone.
        </div>

        <div className="mt-3">
          <div className="text-xs text-muted-foreground mb-1">ID</div>
          <div className="font-mono text-xs break-all">{id ?? '—'}</div>
        </div>

        {errorMessage ? (
          <div className="mt-3 rounded-md border border-destructive/30 bg-destructive/5 p-3">
            <div className="text-sm font-medium">Delete failed</div>
            <div className="text-sm text-muted-foreground mt-1">{errorMessage}</div>
          </div>
        ) : null}

        <div className="mt-4 flex items-center justify-end gap-2">
          <button
            type="button"
            className="h-8 px-3 rounded-md text-sm font-medium text-muted-foreground hover:bg-accent hover:text-foreground disabled:opacity-60"
            onClick={onClose}
            disabled={pending}
          >
            Cancel
          </button>
          <button
            type="button"
            className="h-8 px-3 rounded-md text-sm font-medium bg-destructive text-white hover:bg-destructive/90 disabled:opacity-60"
            onClick={() => {
              if (!id) return
              onConfirm(id)
            }}
            disabled={pending || !id}
          >
            {pending ? 'Deleting…' : 'Delete'}
          </button>
        </div>
      </div>
    </Overlay>
  )
}

