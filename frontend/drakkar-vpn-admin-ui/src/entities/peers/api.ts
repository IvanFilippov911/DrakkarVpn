import { apiClient } from '../../shared/api/client'
import type { GetServerPeersApiResponse, GetServerPeersQuery } from './types'

export async function getServerPeers(
  serverId: string,
  query: GetServerPeersQuery,
): Promise<GetServerPeersApiResponse> {
  const { data } = await apiClient.get<GetServerPeersApiResponse>(`/api/admin/${serverId}/peers`, {
    params: query,
    withCredentials: true,
  })

  return data
}

