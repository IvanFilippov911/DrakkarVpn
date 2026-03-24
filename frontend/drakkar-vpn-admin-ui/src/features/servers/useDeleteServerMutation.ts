import { useMutation, useQueryClient } from '@tanstack/react-query'
import { deleteServer } from './delete-server/api'
import { SERVERS_LIST_QUERY_KEY, SERVERS_OVERVIEW_QUERY_KEY } from '../../entities/server'

export function useDeleteServerMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (serverId: string) => deleteServer(serverId),
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: SERVERS_LIST_QUERY_KEY }),
        queryClient.invalidateQueries({ queryKey: SERVERS_OVERVIEW_QUERY_KEY }),
      ])
    },
  })
}

