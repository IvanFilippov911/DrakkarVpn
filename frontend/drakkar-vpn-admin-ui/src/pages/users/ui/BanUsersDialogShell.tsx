import { useEffect, useMemo, useState } from 'react'
import { InlineAlert, Overlay } from '../../../shared/ui'
import { useBanUserMutation } from '../../../features/users/useBanUserMutation'
import { useBanUsersMutation } from '../../../features/users/useBanUsersMutation'
import { getErrorMessage } from './getErrorMessage'

export function BanUsersDialogShell({
  open,
  userIds,
  onClose,
}: {
  open: boolean
  userIds: string[]
  onClose: () => void
}) {
  const singleBan = useBanUserMutation()
  const bulkBan = useBanUsersMutation()

  const [reason, setReason] = useState('')

  const isSingle = userIds.length === 1
  const isPending = singleBan.isPending || bulkBan.isPending

  useEffect(() => {
    if (!open) {
      singleBan.reset()
      bulkBan.reset()
      setReason('')
    }
  }, [open])

  const errorMessage = useMemo(() => {
    const err = singleBan.error ?? bulkBan.error
    if (!err) return null
    return getErrorMessage(err, 'Failed to ban user(s).')
  }, [bulkBan.error, singleBan.error])

  async function handleSubmit() {
    if (userIds.length === 0) return

    const payload = { reason: reason.trim().length > 0 ? reason.trim() : null }

    try {
      if (isSingle) {
        await singleBan.mutateAsync({ userId: userIds[0], payload })
      } else {
        await bulkBan.mutateAsync({ userIds, ...payload })
      }
      onClose()
    } catch {
      // Error is shown via `errorMessage`.
    }
  }

  return (
    <Overlay open={open} onClose={onClose}>
      <div className="p-4 border-b">
        <div className="text-lg font-semibold">{isSingle ? 'Ban user' : 'Ban users'}</div>
        <div className="text-sm text-muted-foreground mt-0.5">
          {isSingle
            ? 'This action will ban the selected user.'
            : `This action will ban ${userIds.length} selected users.`}
        </div>
      </div>

      <div className="p-4 space-y-4">
        {errorMessage ? <InlineAlert title="Ban failed" message={errorMessage} /> : null}

        <div>
          <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
            Reason (optional)
          </div>
          <textarea
            value={reason}
            onChange={(e) => setReason(e.target.value)}
            className="min-h-20 w-full rounded-md border bg-background px-3 py-2 text-sm"
            placeholder="e.g. chargeback / abuse / fraud"
            disabled={isPending}
          />
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
          className="h-8 px-3 rounded-md text-sm bg-destructive text-white hover:bg-destructive/90 disabled:opacity-50"
          onClick={() => void handleSubmit()}
          disabled={isPending || userIds.length === 0}
        >
          {isPending ? 'Banning…' : 'Ban'}
        </button>
      </div>
    </Overlay>
  )
}

