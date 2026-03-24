import { useMemo, useState } from 'react'
import { isAxiosError } from 'axios'
import { InlineAlert, PageHeader } from '../../shared/ui'
import { CreateTariffShell } from '../../features/tariffs/create-tariff/ui/CreateTariffShell'
import { EditTariffShell } from '../../features/tariffs/edit-tariff/ui/EditTariffShell'
import {
  useDisableTariffMutation,
  useEnableTariffMutation,
} from '../../features/tariffs'
import { TariffsTableSection } from './ui/TariffsTableSection'
import type { TariffRow, UiState } from './ui/types'
import { useActiveTariffsQuery } from '../../entities/tariffs'
import { formatDecimal } from '../../shared/lib/formatters'

export function TariffsPage() {
  const query = useActiveTariffsQuery()
  const uiState: UiState =
    query.isLoading && !query.data
      ? 'loading'
      : query.isError && !query.data
        ? 'error'
        : query.data && query.data.length === 0
          ? 'empty'
          : 'success'

  const enableMutation = useEnableTariffMutation()
  const disableMutation = useDisableTariffMutation()

  const actionErrorMessage = useMemo(() => {
    const err = enableMutation.error ?? disableMutation.error
    if (!err) return null

    if (isAxiosError(err)) {
      const maybeMessage = getMessageFromUnknown(err.response?.data)
      if (typeof maybeMessage === 'string' && maybeMessage.trim().length > 0) {
        return maybeMessage
      }

      return err.response?.status
        ? `Request failed (${err.response.status}).`
        : 'Request failed.'
    }

    return err instanceof Error ? err.message : 'Action failed.'
  }, [disableMutation.error, enableMutation.error])

  const [isCreateOpen, setIsCreateOpen] = useState(false)
  const [editTariffId, setEditTariffId] = useState<string | null>(null)

  const rows = useMemo<TariffRow[]>(
    () =>
      (query.data ?? []).map((t) => ({
        id: t.id,
        name: t.name,
        price: t.price,
        duration: formatDurationInDays(t.duration),
        devices: t.defaultMaxDevices,
        status: t.status,
        createdAt: t.createdAt,
      })),
    [query.data],
  )

  const editTariff = useMemo(() => {
    if (!editTariffId) return null
    return rows.find((r) => r.id === editTariffId) ?? null
  }, [editTariffId, rows])

  async function handleEnable(tariffId: string) {
    try {
      await enableMutation.mutateAsync(tariffId)
    } catch {
      // Error is shown via `actionErrorMessage`.
    }
  }

  async function handleDisable(tariffId: string) {
    try {
      await disableMutation.mutateAsync(tariffId)
    } catch {
      // Error is shown via `actionErrorMessage`.
    }
  }

  return (
    <div>
      <PageHeader
        title="Tariffs"
        description="VPN subscription plans and pricing."
        action={
          <button
            type="button"
            onClick={() => setIsCreateOpen(true)}
            className="h-9 px-4 rounded-md text-sm font-medium bg-primary text-primary-foreground hover:bg-primary/90"
          >
            Create Tariff
          </button>
        }
      />

      {actionErrorMessage ? (
        <div className="mb-4">
          <InlineAlert title="Tariff action failed" message={actionErrorMessage} />
        </div>
      ) : null}

      <TariffsTableSection
        state={uiState}
        rows={rows}
        onCreate={() => setIsCreateOpen(true)}
        onEdit={(id) => setEditTariffId(id)}
        onEnable={(id) => {
          void handleEnable(id)
        }}
        onDisable={(id) => {
          void handleDisable(id)
        }}
        onDelete={(_id) => {
          // Delete is not supported in MVP.
        }}
      />

      <CreateTariffShell open={isCreateOpen} onClose={() => setIsCreateOpen(false)} />
      <EditTariffShell open={editTariffId != null} tariff={editTariff} onClose={() => setEditTariffId(null)} />
    </div>
  )
}

function formatDurationInDays(duration: unknown): string {
  const days = parseDurationDays(duration)
  if (days == null) return typeof duration === 'string' ? duration : '—'

  // The UI contract requires displaying duration in days.
  return `${formatDecimal(days)} days`
}

function parseDurationDays(duration: unknown): number | null {
  if (duration == null) return null

  if (typeof duration === 'number') return Number.isFinite(duration) ? duration : null

  if (typeof duration === 'string') {
    const s = duration.trim()
    if (!s) return null

    // ISO-8601 duration, e.g. "P30D"
    const iso = s.match(/^P([\d.]+)D$/i)
    if (iso) return Number(iso[1])

    // .NET TimeSpan in "c" format, e.g. "30.12:34:56.789"
    const daysFromDot = s.match(/^(-?\d+)\./)
    if (daysFromDot) return Number(daysFromDot[1])

    // "hh:mm:ss" (no day component) - convert to days.
    const hms = s.match(/^(-?\d+):(\d{1,2}):(\d{1,2})(\.\d+)?$/)
    if (hms) {
      const hours = Number(hms[1])
      const minutes = Number(hms[2])
      const seconds = Number(hms[3])
      const totalHours = hours + minutes / 60 + seconds / 3600
      return totalHours / 24
    }

    // Fallback: digits only
    const numeric = s.match(/^(-?\d+(?:\.\d+)?)$/)
    if (numeric) return Number(numeric[1])

    return null
  }

  if (typeof duration === 'object') {
    const d = duration as Record<string, unknown>

    const maybeDays = d.days ?? d.totalDays ?? d.Days ?? d.TotalDays
    if (typeof maybeDays === 'number') return maybeDays

    // .NET TimeSpan may be represented as ticks (100ns units).
    const maybeTicks = d.ticks ?? d.Ticks ?? d.totalTicks ?? d.TotalTicks
    if (typeof maybeTicks === 'number') {
      const TICKS_PER_DAY = 864_000_000_000
      return maybeTicks / TICKS_PER_DAY
    }

    return null
  }

  return null
}

function getMessageFromUnknown(value: unknown): string | undefined {
  if (!value || typeof value !== 'object') return undefined
  if (!('message' in value)) return undefined
  const msg = (value as { message?: unknown }).message
  return typeof msg === 'string' ? msg : undefined
}

