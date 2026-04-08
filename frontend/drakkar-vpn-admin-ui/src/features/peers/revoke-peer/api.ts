import { apiClient } from '../../../shared/api/client'

export async function revokePeer(peerId: string, serverId: string): Promise<void> {
  await apiClient.post(
    `/api/admin/peers/${peerId}/revoke`,
    undefined,
    {
      params: { serverId },
      withCredentials: true,
    },
  )
}

