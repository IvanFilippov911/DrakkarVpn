import { useMutation, useQueryClient } from '@tanstack/react-query'
import { SERVER_PEERS_LIST_QUERY_KEY } from '../../entities/peers'
import { revokePeer } from './revoke-peer/api'

export function useRevokePeerMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ peerId, serverId }: { peerId: string; serverId: string }) =>
      revokePeer(peerId, serverId),
    onSuccess: async (_data, { serverId }) => {
      await queryClient.invalidateQueries({
        queryKey: [...SERVER_PEERS_LIST_QUERY_KEY, serverId],
      })
    },
  })
}

