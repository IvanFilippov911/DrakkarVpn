import type { TransportProfile } from '../../../entities/transport-profiles'
import { Overlay } from '../../../shared/ui'

function formatDateTime(value: Date): string {
  return value.toISOString().replace('T', ' ').slice(0, 19) + 'Z'
}

function formatNullable(value: string | null): string {
  return value ?? '—'
}

export function TransportProfileDetailsShell({
  open,
  profile,
  onClose,
}: {
  open: boolean
  profile: TransportProfile | null
  onClose: () => void
}) {
  return (
    <Overlay open={open} onClose={onClose}>
      <div className="p-4 border-b flex items-center justify-between">
        <div>
          <div className="text-lg font-semibold">Transport Profile Details</div>
          <div className="text-sm text-muted-foreground mt-0.5">
            Full profile snapshot without server usage analytics.
          </div>
        </div>
        <button
          type="button"
          className="h-8 w-8 inline-flex items-center justify-center rounded-md hover:bg-accent"
          onClick={onClose}
        >
          ×
        </button>
      </div>

      <div className="p-4 space-y-4">
        <div className="grid grid-cols-2 gap-3">
          <InfoRow label="Name" value={profile?.name ?? '—'} />
          <InfoRow label="Status" value={profile ? (profile.isEnabled ? 'Enabled' : 'Disabled') : '—'} />
        </div>

        <div className="grid grid-cols-2 gap-3">
          <InfoRow label="Transport Type" value={profile?.transportType ?? '—'} />
          <InfoRow label="Security Type" value={profile?.securityType ?? '—'} />
        </div>

        <InfoRow label="Global Priority" value={profile ? String(profile.globalPriority) : '—'} />

        <div className="grid grid-cols-2 gap-3">
          <InfoRow label="Reality SNI" value={formatNullable(profile?.realitySni ?? null)} />
          <InfoRow label="Reality Short ID" value={formatNullable(profile?.realityShortId ?? null)} />
        </div>

        <div className="grid grid-cols-2 gap-3">
          <InfoRow label="Reality Fingerprint" value={formatNullable(profile?.realityFingerprint ?? null)} />
          <InfoRow label="Reality Destination" value={formatNullable(profile?.realityDest ?? null)} />
        </div>

        <div className="grid grid-cols-2 gap-3">
          <InfoRow label="gRPC Service Name" value={formatNullable(profile?.grpcServiceName ?? null)} />
          <InfoRow label="gRPC Authority" value={formatNullable(profile?.grpcAuthority ?? null)} />
        </div>

        <div className="grid grid-cols-2 gap-3">
          <InfoRow label="Created At (UTC)" value={profile ? formatDateTime(profile.createdAtUtc) : '—'} />
          <InfoRow label="Updated At (UTC)" value={profile ? formatDateTime(profile.updatedAtUtc) : '—'} />
        </div>
      </div>
    </Overlay>
  )
}

function InfoRow({
  label,
  value,
}: {
  label: string
  value: string
}) {
  return (
    <div>
      <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">{label}</div>
      <div className="h-9 w-full rounded-md border bg-muted px-3 text-sm text-foreground flex items-center">
        <span className="truncate" title={value}>
          {value}
        </span>
      </div>
    </div>
  )
}
