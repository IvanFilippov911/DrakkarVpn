import type { PagedResponseDto } from '../../shared/api/types'

// DTO (API responses)
export type AdminServerCardApiResponse = {
  id: string
  name: string
  region: string
  status: string
  reachable: boolean
  onlinePeers: number | null
  peersActive: number
  maxPeers: number | null
  vpnSpeedMbps: number
  infraLatencyMs: number
  trafficLast1hBytes: number
  trafficLast24hBytes: number
}

export type ServersWithTrafficOverviewDto = {
  servers: {
    totalServers: number
    serversOnline: number
    totalActivePeers: number
    peersOnline: number
  }
  traffic: {
    avgSpeedMbps: number | null
    avgInfraLatencyMs: number | null
    trafficTodayBytes: number
    trafficLast24hBytes: number
  }
}

export type ServersListQuery = {
  region?: string
  status?: string
  page: number
  pageSize: number
}

export type ServersListApiResponse = PagedResponseDto<AdminServerCardApiResponse>

// UI models (used by the UI layer, never use DTO directly)
export type ServerCard = {
  id: string
  name: string
  region: string
  status: string
  reachable: boolean
  onlinePeers: number | null
  peersActive: number
  maxPeers: number | null
  vpnSpeedMbps: number
  infraLatencyMs: number
  trafficLast1hBytes: number
  trafficLast24hBytes: number
}

export type ServersOverview = {
  infrastructure: {
    totalServers: number
    serversOnline: number
    totalActivePeers: number
    peersOnline: number
  }
  performance: {
    avgSpeedMbps: number | null
    avgInfraLatencyMs: number | null
  }
  traffic: {
    trafficTodayBytes: number
    trafficLast24hBytes: number
  }
}

