export type Guid = string
export type UtcDateTimeString = string

export type UsersOverviewDto = {
  totalUsers: number
  activeSubscriptions: number
  expiringSoonDays3: number
  onlineUsersNow: number
}

export type UserStatus = string
export type SubscriptionStatus = string

export type UsersSortBy = 'CreatedAt' | 'LastSeen' | 'SubscriptionEnd' | 'Devices' | 'Traffic24h'
export type UserSortDirection = 'Asc' | 'Desc'

export type UsersListQuery = {
  search?: string
  status?: UserStatus
  subscriptionStatus?: SubscriptionStatus
  sortBy: UsersSortBy
  userSortDirection: UserSortDirection
  page: number
  pageSize: number
}

export type AdminUserCardDto = {
  user: {
    id: Guid
    telegram: number | null
    createdAtUtc: UtcDateTimeString
    status: UserStatus
    isOnline: boolean
    deviceCount: number
    lastSeenUtc: UtcDateTimeString | null
  }
  subscription: {
    endAtUtc: UtcDateTimeString
    maxDevices: number
    lastSubscriptionStatus: SubscriptionStatus | null
  }
  trafficLast24hBytes: number | null
}

/** Matches backend `UserStatus` (JsonStringEnumConverter). */
export type AdminUserSummaryUserStatus = 'Active' | 'Banned'

/** Matches backend `DeviceStatus` (JsonStringEnumConverter). */
export type UserDeviceShortStatus = 'Active' | 'Revoked' | 'Registered'

export type AdminUserSummaryDetailDto = {
  userId: Guid
  telegramId: number
  username: string | null
  createdAtUtc: UtcDateTimeString
  status: AdminUserSummaryUserStatus
  isInternal: boolean
  banReason: string | null
  bannedAtUtc: UtcDateTimeString | null
}

export type AdminUserRealtimeDto = {
  isOnline: boolean
  deviceCount: number
  subscriptionMaxDevices: number
  isSubscriptionActive: boolean
  subscriptionEndUtc: UtcDateTimeString | null
  traffic24hBytes: number
  updatedAtUtc: UtcDateTimeString
  lastSeenUtc: UtcDateTimeString | null
}

export type AdminUserPeerShortDto = {
  peerId: Guid
  serverId: Guid
  agentPeerUuid: Guid
  isOnline: boolean
  traffic24hBytes: number
}

export type UserDeviceShortDto = {
  deviceId: string
  name: string | null
  platform: string | null
  createdAtUtc: UtcDateTimeString
  lastSeenUtc: UtcDateTimeString | null
  status: UserDeviceShortStatus
  peer: AdminUserPeerShortDto | null
}

export type AdminUserAlertDto = {
  alertId: Guid
  createdAtUtc: UtcDateTimeString
  isResolved: boolean
  severity: string
  title: string
  message: string
}

export type AdminUserDetailsDto = {
  user: AdminUserSummaryDetailDto
  realtime: AdminUserRealtimeDto
  devices: UserDeviceShortDto[]
  alerts: AdminUserAlertDto[]
}

