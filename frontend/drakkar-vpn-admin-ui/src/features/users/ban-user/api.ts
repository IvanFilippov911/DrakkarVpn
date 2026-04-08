import { apiClient } from '../../../shared/api/client'
import type { BanUserRequest, BulkBanUsersRequest, BulkUsersOperationResponse } from '../types'

export async function banUser(userId: string, payload: BanUserRequest): Promise<void> {
  await apiClient.post(`/api/admin/users/${userId}/ban`, payload, {
    withCredentials: true,
  })
}

export async function banUsers(payload: BulkBanUsersRequest): Promise<BulkUsersOperationResponse> {
  const { data } = await apiClient.post<BulkUsersOperationResponse>('/api/admin/users/ban', payload, {
    withCredentials: true,
  })

  return data
}

