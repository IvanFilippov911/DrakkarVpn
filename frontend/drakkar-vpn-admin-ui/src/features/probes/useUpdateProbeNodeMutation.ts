import { useMutation, useQueryClient } from '@tanstack/react-query'
import { updateProbeNode, type UpdateProbeNodeApiRequest } from './edit-probe/api'
import { PROBES_LIST_QUERY_KEY } from '../../entities/probes'

type UpdateProbeNodePayload = {
  id: string
  payload: UpdateProbeNodeApiRequest
}

export function useUpdateProbeNodeMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, payload }: UpdateProbeNodePayload) => updateProbeNode(id, payload),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: PROBES_LIST_QUERY_KEY })
    },
  })
}

