import { useMutation, useQueryClient } from '@tanstack/react-query'
import { registerServer } from './register-server/api'
import type { RegisterServerApiRequest } from './register-server/types'
import { SERVERS_LIST_QUERY_KEY, SERVERS_OVERVIEW_QUERY_KEY } from '../../entities/server'

export function useRegisterServerMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: RegisterServerApiRequest) => registerServer(payload),
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: SERVERS_LIST_QUERY_KEY }),
        queryClient.invalidateQueries({ queryKey: SERVERS_OVERVIEW_QUERY_KEY }),
      ])
    },
  })
}

