import type { Server, User, Peer, Tariff, SystemError, DashboardOverview, CoreHealth } from './types'

export const mockDashboardOverview: DashboardOverview = {
  totalServers: 12,
  serversOnline: 10,
  totalActivePeers: 847,
  peersOnline: 312,
  avgVpnSpeedMbps: 124.5,
  avgInfraLatencyMs: 23.4,
  trafficTodayBytes: 1024 * 1024 * 1024 * 156, // 156 GB
  trafficLast24hBytes: 1024 * 1024 * 1024 * 423, // 423 GB
  totalUsers: 1247,
  activeSubscriptions: 892,
  expiringSoonDays3: 34,
  onlineUsersNow: 287
}

export const mockCoreHealth: CoreHealth = {
  rps: 1247.5,
  avgLatencyMs: 12.3,
  errorRatePct: 0.23,
  totalRequests: 4521893,
  totalErrors: 10412,
  windowStartUtc: new Date(Date.now() - 3600000).toISOString(),
  windowEndUtc: new Date().toISOString(),
  errorsByArea: {
    'auth': 3421,
    'vpn-connect': 2134,
    'billing': 1823,
    'server-sync': 1567,
    'peer-health': 1467
  }
}

export const mockServers: Server[] = [
  {
    id: 'srv-001',
    name: 'eu-west-1-prod',
    region: 'EU West',
    status: 'online',
    reachable: true,
    onlinePeers: 47,
    peersActive: 89,
    maxPeers: 200,
    vpnSpeedMbps: 142.3,
    infraLatencyMs: 18.2,
    trafficLast1hBytes: 1024 * 1024 * 1024 * 12,
    trafficLast24hBytes: 1024 * 1024 * 1024 * 89
  },
  {
    id: 'srv-002',
    name: 'us-east-1-prod',
    region: 'US East',
    status: 'online',
    reachable: true,
    onlinePeers: 112,
    peersActive: 156,
    maxPeers: 300,
    vpnSpeedMbps: 189.7,
    infraLatencyMs: 12.1,
    trafficLast1hBytes: 1024 * 1024 * 1024 * 24,
    trafficLast24hBytes: 1024 * 1024 * 1024 * 167
  },
  {
    id: 'srv-003',
    name: 'ap-south-1-prod',
    region: 'Asia Pacific',
    status: 'online',
    reachable: true,
    onlinePeers: 34,
    peersActive: 67,
    maxPeers: 150,
    vpnSpeedMbps: 98.4,
    infraLatencyMs: 45.3,
    trafficLast1hBytes: 1024 * 1024 * 1024 * 8,
    trafficLast24hBytes: 1024 * 1024 * 1024 * 54
  },
  {
    id: 'srv-004',
    name: 'eu-central-1-prod',
    region: 'EU Central',
    status: 'maintenance',
    reachable: false,
    onlinePeers: 0,
    peersActive: 0,
    maxPeers: 200,
    vpnSpeedMbps: 0,
    infraLatencyMs: 0,
    trafficLast1hBytes: 0,
    trafficLast24hBytes: 1024 * 1024 * 1024 * 34
  },
  {
    id: 'srv-005',
    name: 'us-west-2-prod',
    region: 'US West',
    status: 'online',
    reachable: true,
    onlinePeers: 78,
    peersActive: 123,
    maxPeers: 250,
    vpnSpeedMbps: 167.2,
    infraLatencyMs: 15.8,
    trafficLast1hBytes: 1024 * 1024 * 1024 * 18,
    trafficLast24hBytes: 1024 * 1024 * 1024 * 112
  },
  {
    id: 'srv-006',
    name: 'sa-east-1-prod',
    region: 'South America',
    status: 'offline',
    reachable: false,
    onlinePeers: null,
    peersActive: 0,
    maxPeers: 100,
    vpnSpeedMbps: 0,
    infraLatencyMs: 0,
    trafficLast1hBytes: 0,
    trafficLast24hBytes: 1024 * 1024 * 1024 * 12
  }
]

export const mockUsers: User[] = [
  {
    id: 'usr-001',
    email: 'john.doe@example.com',
    status: 'active',
    subscription: {
      tariffName: 'Premium',
      expiresAt: new Date(Date.now() + 30 * 24 * 3600000).toISOString(),
      status: 'active'
    },
    devices: 2,
    maxDevices: 5,
    createdAt: new Date(Date.now() - 180 * 24 * 3600000).toISOString(),
    lastActiveAt: new Date(Date.now() - 3600000).toISOString()
  },
  {
    id: 'usr-002',
    email: 'jane.smith@example.com',
    status: 'active',
    subscription: {
      tariffName: 'Basic',
      expiresAt: new Date(Date.now() + 2 * 24 * 3600000).toISOString(),
      status: 'expiring'
    },
    devices: 1,
    maxDevices: 2,
    createdAt: new Date(Date.now() - 90 * 24 * 3600000).toISOString(),
    lastActiveAt: new Date(Date.now() - 7200000).toISOString()
  },
  {
    id: 'usr-003',
    email: 'bob.wilson@example.com',
    status: 'suspended',
    subscription: null,
    devices: 0,
    maxDevices: 0,
    createdAt: new Date(Date.now() - 365 * 24 * 3600000).toISOString(),
    lastActiveAt: new Date(Date.now() - 30 * 24 * 3600000).toISOString()
  },
  {
    id: 'usr-004',
    email: 'alice.johnson@example.com',
    status: 'active',
    subscription: {
      tariffName: 'Enterprise',
      expiresAt: new Date(Date.now() + 365 * 24 * 3600000).toISOString(),
      status: 'active'
    },
    devices: 8,
    maxDevices: 10,
    createdAt: new Date(Date.now() - 60 * 24 * 3600000).toISOString(),
    lastActiveAt: new Date().toISOString()
  },
  {
    id: 'usr-005',
    email: 'charlie.brown@example.com',
    status: 'inactive',
    subscription: {
      tariffName: 'Basic',
      expiresAt: new Date(Date.now() - 10 * 24 * 3600000).toISOString(),
      status: 'expired'
    },
    devices: 0,
    maxDevices: 2,
    createdAt: new Date(Date.now() - 200 * 24 * 3600000).toISOString(),
    lastActiveAt: new Date(Date.now() - 15 * 24 * 3600000).toISOString()
  }
]

export const mockPeers: Peer[] = [
  {
    id: 'peer-001',
    userId: 'usr-001',
    userEmail: 'john.doe@example.com',
    serverId: 'srv-001',
    serverName: 'eu-west-1-prod',
    status: 'online',
    lastHandshakeAt: new Date(Date.now() - 120000).toISOString(),
    trafficUpBytes: 1024 * 1024 * 234,
    trafficDownBytes: 1024 * 1024 * 1024 * 2.3,
    createdAt: new Date(Date.now() - 7 * 24 * 3600000).toISOString()
  },
  {
    id: 'peer-002',
    userId: 'usr-001',
    userEmail: 'john.doe@example.com',
    serverId: 'srv-002',
    serverName: 'us-east-1-prod',
    status: 'offline',
    lastHandshakeAt: new Date(Date.now() - 3600000 * 3).toISOString(),
    trafficUpBytes: 1024 * 1024 * 89,
    trafficDownBytes: 1024 * 1024 * 456,
    createdAt: new Date(Date.now() - 14 * 24 * 3600000).toISOString()
  },
  {
    id: 'peer-003',
    userId: 'usr-002',
    userEmail: 'jane.smith@example.com',
    serverId: 'srv-001',
    serverName: 'eu-west-1-prod',
    status: 'online',
    lastHandshakeAt: new Date(Date.now() - 60000).toISOString(),
    trafficUpBytes: 1024 * 1024 * 567,
    trafficDownBytes: 1024 * 1024 * 1024 * 4.7,
    createdAt: new Date(Date.now() - 30 * 24 * 3600000).toISOString()
  },
  {
    id: 'peer-004',
    userId: 'usr-004',
    userEmail: 'alice.johnson@example.com',
    serverId: 'srv-002',
    serverName: 'us-east-1-prod',
    status: 'online',
    lastHandshakeAt: new Date(Date.now() - 30000).toISOString(),
    trafficUpBytes: 1024 * 1024 * 1024 * 1.2,
    trafficDownBytes: 1024 * 1024 * 1024 * 8.9,
    createdAt: new Date(Date.now() - 45 * 24 * 3600000).toISOString()
  },
  {
    id: 'peer-005',
    userId: 'usr-004',
    userEmail: 'alice.johnson@example.com',
    serverId: 'srv-005',
    serverName: 'us-west-2-prod',
    status: 'online',
    lastHandshakeAt: new Date(Date.now() - 45000).toISOString(),
    trafficUpBytes: 1024 * 1024 * 890,
    trafficDownBytes: 1024 * 1024 * 1024 * 5.6,
    createdAt: new Date(Date.now() - 20 * 24 * 3600000).toISOString()
  }
]

export const mockTariffs: Tariff[] = [
  {
    id: 'tar-001',
    name: 'Basic',
    price: 4.99,
    durationDays: 30,
    defaultMaxDevices: 2,
    status: 'active',
    createdAt: new Date(Date.now() - 365 * 24 * 3600000).toISOString()
  },
  {
    id: 'tar-002',
    name: 'Premium',
    price: 9.99,
    durationDays: 30,
    defaultMaxDevices: 5,
    status: 'active',
    createdAt: new Date(Date.now() - 365 * 24 * 3600000).toISOString()
  },
  {
    id: 'tar-003',
    name: 'Enterprise',
    price: 29.99,
    durationDays: 30,
    defaultMaxDevices: 10,
    status: 'active',
    createdAt: new Date(Date.now() - 180 * 24 * 3600000).toISOString()
  },
  {
    id: 'tar-004',
    name: 'Annual Basic',
    price: 49.99,
    durationDays: 365,
    defaultMaxDevices: 2,
    status: 'active',
    createdAt: new Date(Date.now() - 90 * 24 * 3600000).toISOString()
  },
  {
    id: 'tar-005',
    name: 'Trial',
    price: 0,
    durationDays: 7,
    defaultMaxDevices: 1,
    status: 'disabled',
    createdAt: new Date(Date.now() - 400 * 24 * 3600000).toISOString()
  }
]

export const mockErrors: SystemError[] = [
  {
    id: 'err-001',
    area: 'auth',
    message: 'Token refresh failed: invalid_grant',
    level: 'error',
    occurredAt: new Date(Date.now() - 1800000).toISOString(),
    count: 47
  },
  {
    id: 'err-002',
    area: 'vpn-connect',
    message: 'Handshake timeout exceeded 30s',
    level: 'warning',
    occurredAt: new Date(Date.now() - 3600000).toISOString(),
    count: 23
  },
  {
    id: 'err-003',
    area: 'server-sync',
    message: 'Agent unreachable: connection refused',
    level: 'critical',
    occurredAt: new Date(Date.now() - 7200000).toISOString(),
    count: 156
  },
  {
    id: 'err-004',
    area: 'billing',
    message: 'Payment provider webhook verification failed',
    level: 'error',
    occurredAt: new Date(Date.now() - 14400000).toISOString(),
    count: 12
  },
  {
    id: 'err-005',
    area: 'peer-health',
    message: 'Peer state inconsistency detected',
    level: 'warning',
    occurredAt: new Date(Date.now() - 21600000).toISOString(),
    count: 89
  },
  {
    id: 'err-006',
    area: 'auth',
    message: 'Rate limit exceeded for login attempts',
    level: 'warning',
    occurredAt: new Date(Date.now() - 28800000).toISOString(),
    count: 234
  }
]
