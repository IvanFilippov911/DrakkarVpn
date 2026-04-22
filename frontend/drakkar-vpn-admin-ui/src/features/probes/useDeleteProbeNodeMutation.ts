import { useMutation, useQueryClient } from '@tanstack/react-query'
import { deleteProbeNode } from './delete-probe/api'
import { PROBES_LIST_QUERY_KEY } from '../../entities/probes'

export function useDeleteProbeNodeMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (probeNodeId: string) => deleteProbeNode(probeNodeId),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: PROBES_LIST_QUERY_KEY })
    },
  })
}

