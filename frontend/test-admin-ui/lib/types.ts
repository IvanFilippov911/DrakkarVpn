// Server types
export interface Server {
  id: string
  name: string
  region: string
  status: 'online' | 'offline' | 'maintenance'
  reachable: boolean
  onlinePeers: number | null
  peersActive: number
  maxPeers: number | null
  vpnSpeedMbps: number
  infraLatencyMs: number
  trafficLast1hBytes: number
  trafficLast24hBytes: number
}

// User types
export interface User {
  id: string
  email: string
  status: 'active' | 'inactive' | 'suspended'
  subscription: {
    tariffName: string
    expiresAt: string
    status: 'active' | 'expiring' | 'expired'
  } | null
  devices: number
  maxDevices: number
  createdAt: string
  lastActiveAt: string | null
}

// Peer types
export interface Peer {
  id: string
  userId: string
  userEmail: string
  serverId: string
  serverName: string
  status: 'online' | 'offline'
  lastHandshakeAt: string | null
  trafficUpBytes: number
  trafficDownBytes: number
  createdAt: string
}

// Tariff types
export interface Tariff {
  id: string
  name: string
  price: number
  durationDays: number
  defaultMaxDevices: number
  status: 'active' | 'disabled'
  createdAt: string
}

// Error types
export interface SystemError {
  id: string
  area: string
  message: string
  level: 'error' | 'warning' | 'critical'
  occurredAt: string
  count: number
}

// Dashboard types
export interface DashboardOverview {
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

export interface CoreHealth {
  rps: number
  avgLatencyMs: number
  errorRatePct: number
  totalRequests: number
  totalErrors: number
  windowStartUtc: string
  windowEndUtc: string
  errorsByArea: Record<string, number>
}
