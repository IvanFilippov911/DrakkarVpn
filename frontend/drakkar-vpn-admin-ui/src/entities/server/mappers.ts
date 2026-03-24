import type { AdminServerCardApiResponse, ServerCard, ServersOverview, ServersWithTrafficOverviewDto } from './types'

export function mapAdminServerCardDto(dto: AdminServerCardApiResponse): ServerCard {
  return {
    id: dto.id,
    name: dto.name,
    region: dto.region,
    status: dto.status,
    reachable: dto.reachable,
    onlinePeers: dto.onlinePeers,
    peersActive: dto.peersActive,
    maxPeers: dto.maxPeers,
    vpnSpeedMbps: dto.vpnSpeedMbps,
    infraLatencyMs: dto.infraLatencyMs,
    trafficLast1hBytes: dto.trafficLast1hBytes,
    trafficLast24hBytes: dto.trafficLast24hBytes,
  }
}

export function mapServersOverviewDto(dto: ServersWithTrafficOverviewDto): ServersOverview {
  return {
    infrastructure: {
      totalServers: dto.servers.totalServers,
      serversOnline: dto.servers.serversOnline,
      totalActivePeers: dto.servers.totalActivePeers,
      peersOnline: dto.servers.peersOnline,
    },
    performance: {
      avgSpeedMbps: dto.traffic.avgSpeedMbps,
      avgInfraLatencyMs: dto.traffic.avgInfraLatencyMs,
    },
    traffic: {
      trafficTodayBytes: dto.traffic.trafficTodayBytes,
      trafficLast24hBytes: dto.traffic.trafficLast24hBytes,
    },
  }
}

