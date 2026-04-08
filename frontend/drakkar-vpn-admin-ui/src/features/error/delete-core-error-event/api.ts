import { apiClient } from '../../../shared/api/client'

export async function deleteCoreErrorEvent(id: string): Promise<void> {
  await apiClient.delete(`/admin/core/errors/${id}`, {
    withCredentials: true,
  })
}

