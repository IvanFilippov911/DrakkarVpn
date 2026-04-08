import { keepPreviousData, useQuery } from '@tanstack/react-query'
import { getCoreErrorEvents } from './api'
import type { CoreErrorEventsListQuery } from './types'

const ERRORS_POLLING_INTERVAL_MS = 20_000

export const CORE_ERROR_EVENTS_LIST_QUERY_KEY = ['coreErrorEventsList'] as const

export function coreErrorEventsListQueryKey(query: CoreErrorEventsListQuery) {
  return [...CORE_ERROR_EVENTS_LIST_QUERY_KEY, query] as const
}

export function useCoreErrorEventsQuery(query: CoreErrorEventsListQuery) {
  return useQuery({
    queryKey: coreErrorEventsListQueryKey(query),
    queryFn: () => getCoreErrorEvents(query),
    refetchInterval: ERRORS_POLLING_INTERVAL_MS,
    placeholderData: keepPreviousData,
  })
}

