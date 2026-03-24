import { keepPreviousData, useQuery } from '@tanstack/react-query'
import { getServers, getServersOverview } from './api'
import { mapAdminServerCardDto, mapServersOverviewDto } from './mappers'
import type { ServersListQuery } from './types'

const SERVERS_POLLING_INTERVAL_MS = 20_000

export const SERVERS_OVERVIEW_QUERY_KEY = ['serversOverview'] as const
export const SERVERS_LIST_QUERY_KEY = ['serversList'] as const

export function serversListQueryKey(query: ServersListQuery) {
  return [...SERVERS_LIST_QUERY_KEY, query] as const
}

export function useServersListQuery(query: ServersListQuery) {
  return useQuery({
    queryKey: serversListQueryKey(query),
    queryFn: () => getServers(query),
    refetchInterval: SERVERS_POLLING_INTERVAL_MS,
    placeholderData: keepPreviousData,
    select: (response) => ({
      ...response,
      items: response.items.map(mapAdminServerCardDto),
    }),
  })
}

export function useServersOverviewQuery() {
  return useQuery({
    queryKey: SERVERS_OVERVIEW_QUERY_KEY,
    queryFn: getServersOverview,
    select: mapServersOverviewDto,
    refetchInterval: SERVERS_POLLING_INTERVAL_MS,
  })
}

