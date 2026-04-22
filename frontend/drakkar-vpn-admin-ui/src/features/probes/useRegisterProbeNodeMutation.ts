import { useMutation, useQueryClient } from '@tanstack/react-query'
import { registerProbeNode, type RegisterProbeNodeApiRequest } from './register-probe/api'
import { PROBES_LIST_QUERY_KEY } from '../../entities/probes'

export function useRegisterProbeNodeMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: RegisterProbeNodeApiRequest) => registerProbeNode(payload),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: PROBES_LIST_QUERY_KEY })
    },
  })
}

