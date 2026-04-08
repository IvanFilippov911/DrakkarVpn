export type Guid = string

export type BanUserRequest = {
  reason?: string | null
}

export type BulkBanUsersRequest = {
  userIds: Guid[]
  reason?: string | null
}

export type BulkUnbanUsersRequest = {
  userIds: Guid[]
}

export type MarkUserInternalRequest = {
  isInternal: boolean
}

export type BulkMarkUsersInternalRequest = {
  userIds: Guid[]
  isInternal: boolean
}

export type AdminGrantSubscriptionRequest = {
  tariffId: Guid
  deviceCount?: number | null
}

export type AdminBulkGrantSubscriptionsRequest = {
  userIds: Guid[]
  tariffId: Guid
  deviceCount?: number | null
}

export type BulkUsersOperationResponse = {
  succeededUserIds: Guid[]
  notFoundUserIds: Guid[]
  failedUserIds: Guid[]
  failureDetails?: unknown[] | null
}

export type BulkGrantSubscriptionsResponse = {
  succeeded: Guid[]
  notFound: Guid[]
  failed: Guid[]
  failureDetails?: unknown[] | null
}

