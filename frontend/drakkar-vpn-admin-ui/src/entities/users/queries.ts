import { keepPreviousData, useQuery } from '@tanstack/react-query'
import { getUserDetails, getUsers, getUsersOverview } from './api'
import type { UsersListQuery } from './types'

const USERS_POLLING_INTERVAL_MS = 20_000

export const USERS_OVERVIEW_QUERY_KEY = ['usersOverview'] as const
export const USERS_LIST_QUERY_KEY = ['usersList'] as const

export function usersListQueryKey(query: UsersListQuery) {
  return [...USERS_LIST_QUERY_KEY, query] as const
}

export function userDetailsQueryKey(userId: string) {
  return ['users', 'details', userId] as const
}

export function useUsersOverviewQuery() {
  return useQuery({
    queryKey: USERS_OVERVIEW_QUERY_KEY,
    queryFn: getUsersOverview,
    refetchInterval: USERS_POLLING_INTERVAL_MS,
  })
}

export function useUsersListQuery(query: UsersListQuery) {
  return useQuery({
    queryKey: usersListQueryKey(query),
    queryFn: () => getUsers(query),
    refetchInterval: USERS_POLLING_INTERVAL_MS,
    placeholderData: keepPreviousData,
  })
}

export function useUserDetailsQuery(userId: string | undefined) {
  return useQuery({
    queryKey: userId ? userDetailsQueryKey(userId) : (['users', 'details'] as const),
    queryFn: () => {
      return getUserDetails(userId as string)
    },
    enabled: !!userId,
  })
}

