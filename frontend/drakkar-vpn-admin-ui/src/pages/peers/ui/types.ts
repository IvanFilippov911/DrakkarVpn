export type UiState = 'loading' | 'error' | 'empty' | 'success'

export type PeerRow = {
  peerId: string
  userId: string
  status: 'Active' | 'Revoked' | 'Expired' | string
  isOnline: boolean
  lastDataAtUtc: string
  trafficLast1hBytes: string
  trafficLast24hBytes: string
  speedMbps: string
  vpnLatencyMs: string
  createdAtUtc: string
}

export type PeerSortBy =
  | 'LastActivity'
  | 'Traffic1h'
  | 'Traffic24h'
  | 'Speed'
  | 'Latency'
  | 'CreatedAt'

export type SortDirection = 'Asc' | 'Desc'

