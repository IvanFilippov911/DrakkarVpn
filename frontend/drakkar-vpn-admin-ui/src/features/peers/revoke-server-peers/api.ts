import { apiClient } from '../../../shared/api/client'
import type { AdminRevokeServerPeersApiResponse } from '../types'

export async function revokeServerPeers(serverId: string): Promise<AdminRevokeServerPeersApiResponse> {
  const { data } = await apiClient.post<AdminRevokeServerPeersApiResponse>(
    `/api/admin/servers/${serverId}/peers/revoke`,
    undefined,
    { withCredentials: true },
  )

  return data
}

