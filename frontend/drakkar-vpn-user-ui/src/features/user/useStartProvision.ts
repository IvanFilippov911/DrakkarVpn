import { useMutation, useQueryClient } from '@tanstack/react-query'
import { getHomeContext, startProvision } from '../../entities/user/api'
import { userQueryKeys } from '../../entities/user/queryKeys'

export function useStartProvision() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: () => startProvision(),
    retry: false,
    onSuccess: () => {
      void queryClient.fetchQuery({
        queryKey: userQueryKeys.homeContext,
        queryFn: getHomeContext,
        staleTime: 0,
      })
      void queryClient.invalidateQueries({ queryKey: userQueryKeys.currentConfig })
    },
  })
}
