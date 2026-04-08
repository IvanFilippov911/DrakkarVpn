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

export type UserSummaryDto = {
  id: Guid
  telegram: number | null
  createdAtUtc: UtcDateTimeString
  status: UserStatus
  isOnline: boolean
  deviceCount: number
  lastSeenUtc: UtcDateTimeString | null
}

export type SubscriptionSummaryDto = {
  endAtUtc: UtcDateTimeString
  maxDevices: number
  lastSubscriptionStatus: SubscriptionStatus | null
}

export type AdminUserDetailsDto = {
  user: UserSummaryDto
  subscription?: SubscriptionSummaryDto
  realtime: unknown
  devices: unknown[]
  alerts: unknown[]
}

