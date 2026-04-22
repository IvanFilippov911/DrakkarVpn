import { apiClient } from '../../shared/api/client'
import type { ProbeNodeApiResponse } from './types'

export async function getProbeNodes(): Promise<ProbeNodeApiResponse[]> {
  const { data } = await apiClient.get<ProbeNodeApiResponse[]>(
    '/api/admin/network-monitoring/probe-nodes',
    { withCredentials: true },
  )

  return data
}

