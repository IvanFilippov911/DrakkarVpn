import type { SubscriptionStatus, UserSortDirection, UserStatus, UsersSortBy } from '../../entities/users'

export type UsersFiltersState = {
  search: string
  status: UserStatus | null
  subscriptionStatus: SubscriptionStatus | null
}

export type UsersSortingState = {
  sortBy: UsersSortBy
  userSortDirection: UserSortDirection
}

export type UsersPaginationState = {
  page: number
  pageSize: number
}

export type UsersBulkAction =
  | 'ban'
  | 'unban'
  | 'grantSubscription'
  | 'markInternal'

