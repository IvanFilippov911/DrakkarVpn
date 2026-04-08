import { useEffect, useMemo, useState } from 'react'
import { InlineAlert, Overlay, Skeleton } from '../../../shared/ui'
import { useGrantUserSubscriptionMutation } from '../../../features/users/useGrantUserSubscriptionMutation'
import { useGrantUsersSubscriptionsMutation } from '../../../features/users/useGrantUsersSubscriptionsMutation'
import { useActiveTariffsQuery } from '../../../entities/tariffs'
import { getErrorMessage } from './getErrorMessage'

export function GrantSubscriptionDialogShell({
  open,
  userIds,
  onClose,
}: {
  open: boolean
  userIds: string[]
  onClose: () => void
}) {
  const single = useGrantUserSubscriptionMutation()
  const bulk = useGrantUsersSubscriptionsMutation()

  const tariffsQuery = useActiveTariffsQuery()

  const [tariffId, setTariffId] = useState('')
  const [deviceCount, setDeviceCount] = useState('')

  const isSingle = userIds.length === 1
  const isPending = single.isPending || bulk.isPending

  useEffect(() => {
    if (!open) {
      single.reset()
      bulk.reset()
      setTariffId('')
      setDeviceCount('')
    }
  }, [open])

  const errorMessage = useMemo(() => {
    const err = single.error ?? bulk.error ?? tariffsQuery.error
    if (!err) return null
    return getErrorMessage(err, 'Failed to grant subscription.')
  }, [bulk.error, single.error, tariffsQuery.error])

  const deviceCountParsed = useMemo(() => {
    const raw = deviceCount.trim()
    if (raw.length === 0) return { ok: true, value: null as number | null }
    if (!/^\d+$/.test(raw)) return { ok: false, value: null as number | null }
    const n = Number(raw)
    if (!Number.isInteger(n) || n <= 0) return { ok: false, value: null as number | null }
    return { ok: true, value: n }
  }, [deviceCount])

  const canSubmit =
    tariffId.trim().length > 0 &&
    userIds.length > 0 &&
    !tariffsQuery.isLoading &&
    deviceCountParsed.ok

  async function handleSubmit() {
    if (!canSubmit) return

    const payload = {
      tariffId: tariffId.trim(),
      deviceCount: deviceCountParsed.value,
    }

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
        <div className="text-lg font-semibold">
          {isSingle ? 'Grant subscription' : 'Grant subscription (bulk)'}
        </div>
        <div className="text-sm text-muted-foreground mt-0.5">
          {isSingle
            ? 'Grant a subscription to the selected user.'
            : `Grant a subscription to ${userIds.length} selected users.`}
        </div>
      </div>

      <div className="p-4 space-y-4">
        {errorMessage ? <InlineAlert title="Grant failed" message={errorMessage} /> : null}

        <div>
          <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
            Tariff
          </div>
          {tariffsQuery.isLoading ? (
            <Skeleton className="h-9 w-full" />
          ) : (
            <select
              className="h-9 w-full rounded-md border bg-background px-3 text-sm"
              value={tariffId}
              onChange={(e) => setTariffId(e.target.value)}
              disabled={isPending || tariffsQuery.isError}
            >
              <option value="">Select tariff…</option>
              {(tariffsQuery.data ?? []).map((t) => (
                <option key={t.id} value={t.id}>
                  {t.name}
                </option>
              ))}
            </select>
          )}
        </div>

        <div>
          <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
            Device count (optional)
          </div>
          <input
            value={deviceCount}
            onChange={(e) => setDeviceCount(e.target.value)}
            className="h-9 w-full rounded-md border bg-background px-3 text-sm"
            placeholder="e.g. 3"
            inputMode="numeric"
            disabled={isPending}
          />
          {!deviceCountParsed.ok ? (
            <div className="text-xs text-destructive mt-1">Device count must be a positive integer.</div>
          ) : null}
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
          disabled={isPending || !canSubmit}
        >
          {isPending ? 'Granting…' : 'Grant'}
        </button>
      </div>
    </Overlay>
  )
}

