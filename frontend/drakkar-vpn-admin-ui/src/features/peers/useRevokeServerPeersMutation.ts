import { useMutation, useQueryClient } from '@tanstack/react-query'
import { SERVER_PEERS_LIST_QUERY_KEY } from '../../entities/peers'
import { revokeServerPeers } from './revoke-server-peers/api'

export function useRevokeServerPeersMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (serverId: string) => revokeServerPeers(serverId),
    onSuccess: async (_data, serverId) => {
      await queryClient.invalidateQueries({
        queryKey: [...SERVER_PEERS_LIST_QUERY_KEY, serverId],
      })
    },
  })
}

