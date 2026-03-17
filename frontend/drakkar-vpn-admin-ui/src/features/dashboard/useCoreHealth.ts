import { useQuery } from '@tanstack/react-query'
import { getCoreHealth } from './api'

const CORE_HEALTH_QUERY_KEY = ['coreHealth'] as const

export function useCoreHealth() {
  return useQuery({
    queryKey: CORE_HEALTH_QUERY_KEY,
    queryFn: getCoreHealth,
    refetchInterval: 10_000,
  })
}

