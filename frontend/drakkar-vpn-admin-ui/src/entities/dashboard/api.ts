import { apiClient } from '../../shared/api/client'
import type { AdminOverviewApiResponse, CoreHealthApiResponse } from './types'

export async function getAdminOverview(): Promise<AdminOverviewApiResponse> {
  const { data } = await apiClient.get<AdminOverviewApiResponse>('/api/admin/overview', {
    withCredentials: true,
  })

  return data
}

export async function getCoreHealth(): Promise<CoreHealthApiResponse> {
  const { data } = await apiClient.get<CoreHealthApiResponse>('/api/admin/core/health', {
    withCredentials: true,
  })

  return data
}

