import { useQuery } from '@tanstack/react-query'
import { getCurrentAdmin } from './api'

const CURRENT_ADMIN_QUERY_KEY = ['currentAdmin'] as const

export function useCurrentAdminQuery(options: { enabled: boolean } = { enabled: true }) {
  return useQuery({
    queryKey: CURRENT_ADMIN_QUERY_KEY,
    queryFn: getCurrentAdmin,
    enabled: options.enabled,
  })
}

