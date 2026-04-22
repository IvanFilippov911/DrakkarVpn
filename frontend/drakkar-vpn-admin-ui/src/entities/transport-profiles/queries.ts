import { keepPreviousData, useQuery } from '@tanstack/react-query'
import { getTransportProfiles } from './api'
import { mapTransportProfileDto } from './mappers'
import type { TransportProfilesListQuery } from './types'

const TRANSPORT_PROFILES_POLLING_INTERVAL_MS = 20_000

export const TRANSPORT_PROFILES_LIST_QUERY_KEY = ['transportProfiles', 'list'] as const

export function transportProfilesListQueryKey(query: TransportProfilesListQuery) {
  return [...TRANSPORT_PROFILES_LIST_QUERY_KEY, query] as const
}

export function useTransportProfilesListQuery(query: TransportProfilesListQuery) {
  return useQuery({
    queryKey: transportProfilesListQueryKey(query),
    queryFn: () => getTransportProfiles(query),
    refetchInterval: TRANSPORT_PROFILES_POLLING_INTERVAL_MS,
    placeholderData: keepPreviousData,
    select: (response) => ({
      ...response,
      items: response.items.map(mapTransportProfileDto),
    }),
  })
}
