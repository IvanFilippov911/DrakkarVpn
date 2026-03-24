import { apiClient } from '../../../shared/api/client'
import type { RegisterServerApiRequest, RegisterServerApiResponse } from './types'

export async function registerServer(
  payload: RegisterServerApiRequest,
): Promise<RegisterServerApiResponse> {
  const { data } = await apiClient.post<RegisterServerApiResponse>('/api/admin/servers', payload, {
    withCredentials: true,
  })

  return data
}

