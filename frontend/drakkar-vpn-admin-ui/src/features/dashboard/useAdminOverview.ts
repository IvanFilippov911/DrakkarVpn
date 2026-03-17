import { useQuery } from '@tanstack/react-query'
import { getAdminOverview } from './api'

const ADMIN_OVERVIEW_QUERY_KEY = ['adminOverview'] as const

export function useAdminOverview() {
  return useQuery({
    queryKey: ADMIN_OVERVIEW_QUERY_KEY,
    queryFn: getAdminOverview,
    refetchInterval: 10_000,
  })
}

