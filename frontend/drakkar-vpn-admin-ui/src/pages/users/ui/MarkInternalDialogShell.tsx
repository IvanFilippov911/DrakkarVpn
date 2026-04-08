import { useEffect, useMemo, useState } from 'react'
import { InlineAlert, Overlay } from '../../../shared/ui'
import { useMarkUserInternalMutation } from '../../../features/users/useMarkUserInternalMutation'
import { useMarkUsersInternalMutation } from '../../../features/users/useMarkUsersInternalMutation'
import { getErrorMessage } from './getErrorMessage'

export function MarkInternalDialogShell({
  open,
  userIds,
  onClose,
}: {
  open: boolean
  userIds: string[]
  onClose: () => void
}) {
  const single = useMarkUserInternalMutation()
  const bulk = useMarkUsersInternalMutation()

  const [isInternal, setIsInternal] = useState<'true' | 'false'>('true')

  const isSingle = userIds.length === 1
  const isPending = single.isPending || bulk.isPending

  useEffect(() => {
    if (!open) {
      single.reset()
      bulk.reset()
      setIsInternal('true')
    }
  }, [open])

  const errorMessage = useMemo(() => {
    const err = single.error ?? bulk.error
    if (!err) return null
    return getErrorMessage(err, 'Failed to update internal status.')
  }, [bulk.error, single.error])

  async function handleSubmit() {
    if (userIds.length === 0) return

    const payload = { isInternal: isInternal === 'true' }

    try {
      if (isSingle) {
        await single.mutateAsync({ userId: userIds[0], payload })
      } else {
        await bulk.mutateAsync({ userIds, ...payload })
      }
      onClose()
    } catch {
      // Error is shown via `errorMessage`.
    }
  }

  return (
    <Overlay open={open} onClose={onClose}>
      <div className="p-4 border-b">
        <div className="text-lg font-semibold">{isSingle ? 'Mark internal' : 'Mark internal (bulk)'}</div>
        <div className="text-sm text-muted-foreground mt-0.5">
          {isSingle
            ? 'Set internal status for the selected user.'
            : `Set internal status for ${userIds.length} selected users.`}
        </div>
      </div>

      <div className="p-4 space-y-4">
        {errorMessage ? <InlineAlert title="Update failed" message={errorMessage} /> : null}

        <div>
          <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
            Internal
          </div>
          <select
            className="h-9 w-full rounded-md border bg-background px-3 text-sm"
            value={isInternal}
            onChange={(e) => setIsInternal(e.target.value as 'true' | 'false')}
            disabled={isPending}
          >
            <option value="true">Set internal</option>
            <option value="false">Remove internal</option>
          </select>
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
          {isPending ? 'Saving…' : 'Save'}
        </button>
      </div>
    </Overlay>
  )
}

