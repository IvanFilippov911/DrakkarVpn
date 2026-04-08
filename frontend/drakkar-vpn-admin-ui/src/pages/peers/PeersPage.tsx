import { useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useServerPeersListQuery } from '../../entities/peers'
import { useRevokePeerMutation } from '../../features/peers/useRevokePeerMutation'
import { useRevokeServerPeersMutation } from '../../features/peers/useRevokeServerPeersMutation'
import { InlineAlert, PageHeader } from '../../shared/ui'
import type { PeerRow, UiState } from './ui/types'
import { formatBytes, formatMbps, formatMs, formatUtcDateTime } from './ui/formatters'
import { usePeersPageState } from './hooks/usePeersPageState'
import { ServerContextBlock } from './ui/ServerContextBlock'
import { PeersFiltersRow } from './ui/PeersFiltersRow'
import { PeersPagination } from './ui/PeersPagination'
import { PeersTableSection } from './ui/PeersTableSection'
import { RevokeAllPeersDialogShell } from './ui/RevokeAllPeersDialogShell'
import { RevokePeerDialogShell } from './ui/RevokePeerDialogShell'

export function PeersPage() {
  const navigate = useNavigate()
  const { serverId } = useParams<{ serverId: string }>()

  const pageState = usePeersPageState()
  const peersQuery = useServerPeersListQuery(serverId, pageState.query)

  const [revokePeerId, setRevokePeerId] = useState<string | null>(null)
  const [isRevokeAllOpen, setIsRevokeAllOpen] = useState(false)

  const revokePeerMutation = useRevokePeerMutation()
  const revokeServerPeersMutation = useRevokeServerPeersMutation()

  const rows = useMemo<PeerRow[]>(() => {
    if (!peersQuery.data) return []

    return peersQuery.data.items.map((p) => ({
      peerId: p.peerId,
      userId: p.userId,
      status: p.status,
      isOnline: p.isOnline,
      lastDataAtUtc: formatUtcDateTime(p.lastDataAtUtc),
      trafficLast1hBytes: formatBytes(p.trafficLast1hBytes),
      trafficLast24hBytes: formatBytes(p.trafficLast24hBytes),
      speedMbps: formatMbps(p.speedMbps),
      vpnLatencyMs: formatMs(p.vpnLatencyMs),
      createdAtUtc: formatUtcDateTime(p.createdAtUtc),
    }))
  }, [peersQuery.data])

  const tableState: UiState = !serverId
    ? 'error'
    : peersQuery.isLoading
      ? 'loading'
      : peersQuery.isError
        ? 'error'
        : peersQuery.data && peersQuery.data.items.length === 0
          ? 'empty'
          : 'success'

  const page = peersQuery.data?.page ?? pageState.state.page
  const totalPages = peersQuery.data?.totalPages ?? 1

  return (
    <div>
      <PageHeader
        title="Peers"
        description="Peers for selected server."
        action={
          <button
            type="button"
            onClick={() => setIsRevokeAllOpen(true)}
            className="h-9 px-4 rounded-md text-sm font-medium bg-destructive text-white hover:bg-destructive/90"
            disabled={!serverId}
          >
            Revoke All Peers
          </button>
        }
      />

      {!serverId ? (
        <InlineAlert message="Server context is missing. Open this page via /servers/{serverId}/peers." />
      ) : null}

      {serverId ? <ServerContextBlock serverId={serverId} /> : null}

      <PeersFiltersRow
        peerId={pageState.state.peerId}
        onlyOnline={pageState.state.onlyOnline}
        sortBy={pageState.state.sortBy}
        direction={pageState.state.direction}
        onPeerIdChange={pageState.setPeerId}
        onOnlyOnlineChange={pageState.setOnlyOnline}
        onSortByChange={pageState.setSortBy}
        onDirectionChange={pageState.setDirection}
      />

      <PeersTableSection
        state={tableState}
        rows={rows}
        onRevokePeer={(id) => setRevokePeerId(id)}
      />

      <PeersPagination
        page={page}
        totalPages={totalPages}
        onPrev={() => pageState.setPage(Math.max(1, page - 1))}
        onNext={() => pageState.setPage(Math.min(totalPages, page + 1))}
      />

      <RevokePeerDialogShell
        open={revokePeerId != null}
        peerId={revokePeerId ?? ''}
        serverId={serverId ?? ''}
        isPending={revokePeerMutation.isPending}
        errorMessage={revokePeerMutation.isError ? 'Failed to revoke peer.' : undefined}
        onCancel={() => setRevokePeerId(null)}
        onConfirm={() => {
          if (!serverId || !revokePeerId) return
          revokePeerMutation.mutate(
            { peerId: revokePeerId, serverId },
            {
              onSuccess: () => setRevokePeerId(null),
            },
          )
        }}
      />

      <RevokeAllPeersDialogShell
        open={isRevokeAllOpen}
        serverId={serverId ?? ''}
        isPending={revokeServerPeersMutation.isPending}
        errorMessage={revokeServerPeersMutation.isError ? 'Failed to revoke all peers.' : undefined}
        onCancel={() => setIsRevokeAllOpen(false)}
        onConfirm={() => {
          if (!serverId) return
          revokeServerPeersMutation.mutate(serverId, {
            onSuccess: () => setIsRevokeAllOpen(false),
          })
        }}
      />

      <div className="mt-6">
        <button
          type="button"
          className="h-8 px-3 rounded-md text-sm border bg-background hover:bg-accent"
          onClick={() => navigate('/servers')}
        >
          Back to Servers
        </button>
      </div>
    </div>
  )
}

