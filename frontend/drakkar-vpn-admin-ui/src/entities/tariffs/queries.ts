import { useQuery } from '@tanstack/react-query'
import { getActiveTariffs, getTariffById } from './api'
import type { TariffApiResponse } from './types'

export const TARIFFS_LIST_QUERY_KEY = ['tariffs', 'active'] as const

export const tariffByIdQueryKey = (id: string) => ['tariffs', 'byId', id] as const

export function useActiveTariffsQuery() {
  return useQuery({
    queryKey: TARIFFS_LIST_QUERY_KEY,
    queryFn: getActiveTariffs,
  })
}

export function useTariffByIdQuery(id: string | undefined) {
  return useQuery<TariffApiResponse>({
    queryKey: id ? tariffByIdQueryKey(id) : ['tariffs', 'byId'] as const,
    queryFn: () => {
      // `enabled` guarantees we never call this when id is undefined.
      return getTariffById(id as string)
    },
    enabled: !!id,
  })
}

