import { useMutation, useQueryClient } from '@tanstack/react-query'
import { startProvision } from '../../entities/user/api'
import { userQueryKeys } from '../../entities/user/queryKeys'

export function useStartProvision() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: () => startProvision(),
    retry: false,
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: userQueryKeys.homeContext })
      void queryClient.invalidateQueries({ queryKey: userQueryKeys.currentConfig })
    },
  })
}
