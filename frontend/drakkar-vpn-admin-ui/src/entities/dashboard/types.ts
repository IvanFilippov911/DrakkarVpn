export type AdminOverviewApiResponse = {
  totalServers: number
  serversOnline: number
  totalActivePeers: number
  peersOnline: number
  avgVpnSpeedMbps: number | null
  avgInfraLatencyMs: number | null
  trafficTodayBytes: number
  trafficLast24hBytes: number
  totalUsers: number
  activeSubscriptions: number
  expiringSoonDays3: number
  onlineUsersNow: number
}

export type CoreHealthApiResponse = {
  rps: number
  avgLatencyMs: number
  errorRatePct: number
  totalRequests: number
  totalErrors: number
  windowStartUtc: string
  windowEndUtc: string
  errorsByArea: Record<string, number>
}

