import { useState } from 'react'
import type { ReactNode } from 'react'
import type { AdminUserPeerShortDto, UserDeviceShortDto } from '../../../entities/users'
import { useRevokePeerMutation } from '../../../features/peers/useRevokePeerMutation'
import { EmptyState } from '../../../shared/ui'
import { RevokePeerDialogShell } from '../../peers/ui/RevokePeerDialogShell'
import { formatMaybeBytes, formatMaybeDateTimeUtc } from './formatters'

function shortId(id: string) {
  const raw = id.trim()
  if (raw.length <= 16) return raw
  return `${raw.slice(0, 8)}…${raw.slice(-4)}`
}

function TruncatedCopyId({ id, copyLabel = 'Copy' }: { id: string; copyLabel?: string }) {
  return (
    <div className="flex items-center gap-2 min-w-0">
      <span className="font-mono text-xs tabular-nums truncate" title={id}>
        {shortId(id)}
      </span>
      <button
        type="button"
        className="shrink-0 text-xs text-muted-foreground hover:text-foreground underline-offset-2 hover:underline"
        title={`Copy ${id}`}
        onClick={async () => {
          try {
            await navigator.clipboard.writeText(id)
          } catch {
            // ignore
          }
        }}
      >
        {copyLabel}
      </button>
    </div>
  )
}

function maybeText(value: string | null): string {
  if (value === null || value === '') return '—'
  return value
}

function DeviceStatusBadge({ value }: { value: UserDeviceShortDto['status'] }) {
  const lowered = value.toLowerCase()
  const variant =
    lowered === 'revoked'
      ? 'bg-red-50 text-red-700 border-red-200'
      : lowered === 'active'
        ? 'bg-emerald-50 text-emerald-700 border-emerald-200'
        : lowered === 'registered'
          ? 'bg-sky-50 text-sky-800 border-sky-200'
          : 'bg-zinc-100 text-zinc-600 border-zinc-200'

  return (
    <span className={['inline-flex items-center rounded px-2 py-0.5 text-xs font-medium border', variant].join(' ')}>
      {value}
    </span>
  )
}

function PeerOnlineBadge({ isOnline }: { isOnline: boolean }) {
  const variant = isOnline ? 'bg-emerald-50 text-emerald-700 border-emerald-200' : 'bg-zinc-100 text-zinc-600 border-zinc-200'
  const label = isOnline ? 'Online' : 'Offline'

  return (
    <span className={['inline-flex items-center rounded px-2 py-0.5 text-xs font-medium border', variant].join(' ')}>
      {label}
    </span>
  )
}

function DetailLabel({ children }: { children: string }) {
  return <dt className="text-xs font-medium text-muted-foreground">{children}</dt>
}

function DetailValue({ children }: { children: ReactNode }) {
  return <dd className="text-sm text-foreground min-w-0">{children}</dd>
}

function PeerBlock({ peer, onRevoke }: { peer: AdminUserPeerShortDto; onRevoke: () => void }) {
  return (
    <div className="mt-4 pt-4 border-t">
      <div className="flex flex-wrap items-center justify-between gap-2 mb-3">
        <div className="text-xs font-semibold text-muted-foreground uppercase tracking-wide">Peer</div>
        <button
          type="button"
          className="h-8 px-3 rounded-md text-sm font-medium border border-destructive/40 text-destructive hover:bg-destructive/10 disabled:opacity-50 disabled:cursor-not-allowed"
          onClick={onRevoke}
        >
          Revoke peer
        </button>
      </div>
      <dl className="grid gap-3 sm:grid-cols-[minmax(8rem,auto)_1fr] sm:gap-x-6 sm:gap-y-2">
        <DetailLabel>Peer ID</DetailLabel>
        <DetailValue>
          <TruncatedCopyId id={peer.peerId} />
        </DetailValue>

        <DetailLabel>Server ID</DetailLabel>
        <DetailValue>
          <TruncatedCopyId id={peer.serverId} />
        </DetailValue>

        <DetailLabel>Agent peer UUID</DetailLabel>
        <DetailValue>
          <TruncatedCopyId id={peer.agentPeerUuid} />
        </DetailValue>

        <DetailLabel>Online</DetailLabel>
        <DetailValue>
          <PeerOnlineBadge isOnline={peer.isOnline} />
        </DetailValue>

        <DetailLabel>Traffic (24h)</DetailLabel>
        <DetailValue>{formatMaybeBytes(peer.traffic24hBytes)}</DetailValue>
      </dl>
    </div>
  )
}

function DeviceCard({
  device,
  onRequestRevokePeer,
}: {
  device: UserDeviceShortDto
  onRequestRevokePeer: (peer: AdminUserPeerShortDto) => void
}) {
  return (
    <article className="border rounded-lg bg-card p-4">
      <dl className="grid gap-3 sm:grid-cols-[minmax(8rem,auto)_1fr] sm:gap-x-6 sm:gap-y-2">
        <DetailLabel>Device ID</DetailLabel>
        <DetailValue>
          <TruncatedCopyId id={device.deviceId} />
        </DetailValue>

        <DetailLabel>Name</DetailLabel>
        <DetailValue>{maybeText(device.name)}</DetailValue>

        <DetailLabel>Platform</DetailLabel>
        <DetailValue>{maybeText(device.platform)}</DetailValue>

        <DetailLabel>Created at</DetailLabel>
        <DetailValue>{formatMaybeDateTimeUtc(device.createdAtUtc)}</DetailValue>

        <DetailLabel>Last seen</DetailLabel>
        <DetailValue>{formatMaybeDateTimeUtc(device.lastSeenUtc)}</DetailValue>

        <DetailLabel>Status</DetailLabel>
        <DetailValue>
          <DeviceStatusBadge value={device.status} />
        </DetailValue>
      </dl>

      {device.peer ? (
        <PeerBlock
          peer={device.peer}
          onRevoke={() => {
            const p = device.peer
            if (p) onRequestRevokePeer(p)
          }}
        />
      ) : (
        <p className="mt-4 pt-4 border-t text-sm text-muted-foreground">No peer</p>
      )}
    </article>
  )
}

export function UserDevicesSection({ devices, userId }: { devices: UserDeviceShortDto[]; userId: string }) {
  const [revokeTarget, setRevokeTarget] = useState<AdminUserPeerShortDto | null>(null)
  const revokeMutation = useRevokePeerMutation()

  return (
    <section className="border rounded-lg bg-card mt-6">
      <div className="px-4 py-3 border-b">
        <h2 className="text-base font-semibold">Devices</h2>
      </div>
      <div className="p-4">
        {devices.length === 0 ? (
          <EmptyState title="No devices" description="This user has no registered devices." />
        ) : (
          <div className="flex flex-col gap-4">
            {devices.map((device) => (
              <DeviceCard
                key={device.deviceId}
                device={device}
                onRequestRevokePeer={(peer) => setRevokeTarget(peer)}
              />
            ))}
          </div>
        )}
      </div>

      <RevokePeerDialogShell
        open={revokeTarget != null}
        peerId={revokeTarget?.peerId ?? ''}
        serverId={revokeTarget?.serverId ?? ''}
        isPending={revokeMutation.isPending}
        errorMessage={revokeMutation.isError ? 'Failed to revoke peer.' : undefined}
        onCancel={() => {
          revokeMutation.reset()
          setRevokeTarget(null)
        }}
        onConfirm={() => {
          if (!revokeTarget) return
          revokeMutation.mutate(
            { peerId: revokeTarget.peerId, serverId: revokeTarget.serverId, userId },
            {
              onSuccess: () => {
                setRevokeTarget(null)
                revokeMutation.reset()
              },
            },
          )
        }}
      />
    </section>
  )
}
