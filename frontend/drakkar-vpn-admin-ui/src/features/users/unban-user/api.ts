import { apiClient } from '../../../shared/api/client'
import type { BulkUnbanUsersRequest, BulkUsersOperationResponse } from '../types'

export async function unbanUser(userId: string): Promise<void> {
  await apiClient.post(`/api/admin/users/${userId}/unban`, null, {
    withCredentials: true,
  })
}

export async function unbanUsers(payload: BulkUnbanUsersRequest): Promise<BulkUsersOperationResponse> {
  const { data } = await apiClient.post<BulkUsersOperationResponse>(
    '/api/admin/users/unban',
    payload,
    {
      withCredentials: true,
    },
  )

  return data
}

