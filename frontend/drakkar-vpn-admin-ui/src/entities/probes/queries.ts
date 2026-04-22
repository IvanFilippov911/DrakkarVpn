import { useQuery } from '@tanstack/react-query'
import { getProbeNodes } from './api'
import { mapProbeNodeDto } from './mappers'

export const PROBES_LIST_QUERY_KEY = ['probes', 'list'] as const

export function useProbeNodesQuery() {
  return useQuery({
    queryKey: PROBES_LIST_QUERY_KEY,
    queryFn: async () => {
      const dtos = await getProbeNodes()
      return dtos.map(mapProbeNodeDto)
    },
    refetchInterval: 20_000,
  })
}

