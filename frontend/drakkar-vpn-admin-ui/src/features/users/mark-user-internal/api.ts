import { apiClient } from '../../../shared/api/client'
import type {
  BulkMarkUsersInternalRequest,
  BulkUsersOperationResponse,
  MarkUserInternalRequest,
} from '../types'

export async function markUserInternal(userId: string, payload: MarkUserInternalRequest): Promise<void> {
  await apiClient.post(`/api/admin/users/${userId}/internal`, payload, {
    withCredentials: true,
  })
}

export async function markUsersInternal(
  payload: BulkMarkUsersInternalRequest,
): Promise<BulkUsersOperationResponse> {
  const { data } = await apiClient.post<BulkUsersOperationResponse>(
    '/api/admin/users/internal',
    payload,
    {
      withCredentials: true,
    },
  )

  return data
}

