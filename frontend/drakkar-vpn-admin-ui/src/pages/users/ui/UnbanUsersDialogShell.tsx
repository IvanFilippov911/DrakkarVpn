import { useEffect, useMemo } from 'react'
import { InlineAlert, Overlay } from '../../../shared/ui'
import { useUnbanUserMutation } from '../../../features/users/useUnbanUserMutation'
import { useUnbanUsersMutation } from '../../../features/users/useUnbanUsersMutation'
import { getErrorMessage } from './getErrorMessage'

export function UnbanUsersDialogShell({
  open,
  userIds,
  onClose,
}: {
  open: boolean
  userIds: string[]
  onClose: () => void
}) {
  const singleUnban = useUnbanUserMutation()
  const bulkUnban = useUnbanUsersMutation()

  const isSingle = userIds.length === 1
  const isPending = singleUnban.isPending || bulkUnban.isPending

  useEffect(() => {
    if (!open) {
      singleUnban.reset()
      bulkUnban.reset()
    }
  }, [open])

  const errorMessage = useMemo(() => {
    const err = singleUnban.error ?? bulkUnban.error
    if (!err) return null
    return getErrorMessage(err, 'Failed to unban user(s).')
  }, [bulkUnban.error, singleUnban.error])

  async function handleSubmit() {
    if (userIds.length === 0) return

    try {
      if (isSingle) {
        await singleUnban.mutateAsync(userIds[0])
      } else {
        await bulkUnban.mutateAsync({ userIds })
      }
      onClose()
    } catch {
      // Error is shown via `errorMessage`.
    }
  }

  return (
    <Overlay open={open} onClose={onClose}>
      <div className="p-4 border-b">
        <div className="text-lg font-semibold">{isSingle ? 'Unban user' : 'Unban users'}</div>
        <div className="text-sm text-muted-foreground mt-0.5">
          {isSingle
            ? 'This action will unban the selected user.'
            : `This action will unban ${userIds.length} selected users.`}
        </div>
      </div>

      <div className="p-4 space-y-4">
        {errorMessage ? <InlineAlert title="Unban failed" message={errorMessage} /> : null}
        <div className="text-sm text-muted-foreground">
          Confirm to proceed.
        </div>
      </div>

      <div className="p-4 border-t flex items-center justify-end gap-2">
        <button
          type="button"
          className="h-8 px-3 rounded-md text-sm border bg-background hover:bg-accent disabled:opacity-50"
          onClick={onClose}
          disabled={isPending}
        >
          Cancel
        </button>
        <button
          type="button"
          className="h-8 px-3 rounded-md text-sm bg-primary text-primary-foreground hover:bg-primary/90 disabled:opacity-50"
          onClick={() => void handleSubmit()}
          disabled={isPending || userIds.length === 0}
        >
          {isPending ? 'Unbanning…' : 'Unban'}
        </button>
      </div>
    </Overlay>
  )
}

