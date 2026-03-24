import { apiClient } from '../../../shared/api/client'

export async function deleteServer(serverId: string): Promise<void> {
  await apiClient.delete(`/api/admin/servers/${serverId}`, {
    withCredentials: true,
  })
}

