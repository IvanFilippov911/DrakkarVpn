import { useMutation, useQueryClient } from '@tanstack/react-query'
import { SERVER_PEERS_LIST_QUERY_KEY } from '../../entities/peers'
import { userDetailsQueryKey } from '../../entities/users'
import { revokePeer } from './revoke-peer/api'

type RevokePeerVariables = {
  peerId: string
  serverId: string
  /** When set (e.g. User Detail), refreshes that user's details after revoke. */
  userId?: string
}

export function useRevokePeerMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ peerId, serverId }: RevokePeerVariables) => revokePeer(peerId, serverId),
    onSuccess: async (_data, { serverId, userId }) => {
      await queryClient.invalidateQueries({
        queryKey: [...SERVER_PEERS_LIST_QUERY_KEY, serverId],
      })
      if (userId) {
        await queryClient.invalidateQueries({ queryKey: userDetailsQueryKey(userId) })
      }
    },
  })
}

