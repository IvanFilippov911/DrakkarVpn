import { useQuery } from '@tanstack/react-query'
import { getAdminOverview, getCoreHealth } from './api'

const ADMIN_OVERVIEW_QUERY_KEY = ['adminOverview'] as const
const CORE_HEALTH_QUERY_KEY = ['coreHealth'] as const

export function useAdminOverviewQuery() {
  return useQuery({
    queryKey: ADMIN_OVERVIEW_QUERY_KEY,
    queryFn: getAdminOverview,
    refetchInterval: 10_000,
  })
}

export function useCoreHealthQuery() {
  return useQuery({
    queryKey: CORE_HEALTH_QUERY_KEY,
    queryFn: getCoreHealth,
    refetchInterval: 10_000,
  })
}

