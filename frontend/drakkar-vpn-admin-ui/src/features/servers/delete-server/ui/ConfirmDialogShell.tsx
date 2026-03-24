import { useEffect, useMemo } from 'react'
import { isAxiosError } from 'axios'
import { InlineAlert, Overlay } from '../../../../shared/ui'
import { useDeleteServerMutation } from '../../useDeleteServerMutation'

export function ConfirmDialogShell({
  open,
  serverId,
  title,
  description,
  confirmLabel,
  onCancel,
}: {
  open: boolean
  serverId: string
  title: string
  description: string
  confirmLabel: string
  onCancel: () => void
}) {
  const deleteMutation = useDeleteServerMutation()

  // Clear previous errors when opening/closing the dialog.
  useEffect(() => {
    if (!open) {
      deleteMutation.reset()
    }
  }, [deleteMutation, open])

  const errorMessage = useMemo(() => {
    const err = deleteMutation.error
    if (!err) return null

    if (isAxiosError(err)) {
      const status = err.response?.status
      const maybeMessage = getMessageFromUnknown(err.response?.data)

      if (status === 409) {
        return (
          maybeMessage ??
          'Delete conflict (409): server cannot be deleted in its current state.'
        )
      }

      if (typeof maybeMessage === 'string' && maybeMessage.trim().length > 0) {
        return maybeMessage
      }

      return status ? `Request failed (${status}).` : 'Request failed.'
    }

    return err instanceof Error ? err.message : 'Failed to delete server.'
  }, [deleteMutation.error])

  async function handleConfirm() {
    if (!serverId) return

    try {
      await deleteMutation.mutateAsync(serverId)
      onCancel()
    } catch {
      // Error is displayed via `deleteMutation.error`.
    }
  }

  return (
    <Overlay open={open} onClose={onCancel}>
      <div className="p-4 border-b">
        <div className="text-lg font-semibold">{title}</div>
        <div className="text-sm text-muted-foreground mt-0.5">{description}</div>
      </div>

      <div className="p-4">
        {errorMessage ? <InlineAlert title="Delete failed" message={errorMessage} /> : null}
      </div>

      <div className="p-4 border-t flex items-center justify-end gap-2">
        <button
          type="button"
          className="h-8 px-3 rounded-md text-sm border bg-background hover:bg-accent disabled:opacity-50"
          onClick={onCancel}
          disabled={deleteMutation.isPending}
        >
          Cancel
        </button>
        <button
          type="button"
          className="h-8 px-3 rounded-md text-sm bg-destructive text-white hover:bg-destructive/90 disabled:opacity-50"
          onClick={() => {
            void handleConfirm()
          }}
          disabled={deleteMutation.isPending || !serverId}
        >
          {deleteMutation.isPending ? 'Deleting…' : confirmLabel}
        </button>
      </div>
    </Overlay>
  )
}

function getMessageFromUnknown(value: unknown): string | undefined {
  if (!value || typeof value !== 'object') return undefined
  if (!('message' in value)) return undefined
  const msg = (value as { message?: unknown }).message
  return typeof msg === 'string' ? msg : undefined
}

