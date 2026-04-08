import { keepPreviousData, useQuery } from '@tanstack/react-query'
import { getServerPeers } from './api'
import type { GetServerPeersQuery } from './types'

const PEERS_POLLING_INTERVAL_MS = 20_000

export const SERVER_PEERS_LIST_QUERY_KEY = ['serverPeersList'] as const

export function serverPeersListQueryKey(serverId: string, query: GetServerPeersQuery) {
  return [...SERVER_PEERS_LIST_QUERY_KEY, serverId, query] as const
}

export function useServerPeersListQuery(serverId: string | undefined, query: GetServerPeersQuery) {
  return useQuery({
    queryKey: serverId ? serverPeersListQueryKey(serverId, query) : SERVER_PEERS_LIST_QUERY_KEY,
    queryFn: () => getServerPeers(serverId as string, query),
    enabled: !!serverId,
    refetchInterval: PEERS_POLLING_INTERVAL_MS,
    placeholderData: keepPreviousData,
  })
}

