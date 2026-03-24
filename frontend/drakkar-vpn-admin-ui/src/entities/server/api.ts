import { apiClient } from '../../shared/api/client'
import type { ServersListApiResponse, ServersListQuery, ServersWithTrafficOverviewDto } from './types'

export async function getServers(query: ServersListQuery): Promise<ServersListApiResponse> {
  const { data } = await apiClient.get<ServersListApiResponse>('/api/admin/servers', {
    params: query,
    withCredentials: true,
  })

  return data
}

export async function getServersOverview(): Promise<ServersWithTrafficOverviewDto> {
  const { data } = await apiClient.get<ServersWithTrafficOverviewDto>(
    '/api/admin/overview/servers',
    { withCredentials: true },
  )

  return data
}

