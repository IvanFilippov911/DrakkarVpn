import type { PagedResponseDto } from '../../shared/api/types'

// DTO (API responses)
export type ServerPeerApiResponse = {
  peerId: string
  userId: string
  status: string
  isOnline: boolean
  lastDataAtUtc: string | null
  trafficLast1hBytes: number | null
  trafficLast24hBytes: number | null
  speedMbps: number | null
  vpnLatencyMs: number | null
  createdAtUtc: string
}

export type ServerPeerSortBy =
  | 'LastActivity'
  | 'Traffic1h'
  | 'Traffic24h'
  | 'Speed'
  | 'Latency'
  | 'CreatedAt'

export type SortDirection = 'Asc' | 'Desc'

export type GetServerPeersQuery = {
  page: number
  pageSize: number
  onlyOnline?: boolean
  peerId?: string
  sortBy?: ServerPeerSortBy
  peerSortDirection?: SortDirection
}

export type GetServerPeersApiResponse = PagedResponseDto<ServerPeerApiResponse>

