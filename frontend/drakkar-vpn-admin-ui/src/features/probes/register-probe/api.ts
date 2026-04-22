import { apiClient } from '../../../shared/api/client'

export type RegisterProbeNodeApiRequest = {
  name: string
  region: string
  host: string
}

export type RegisterProbeNodeApiResponse = {
  id: string
}

export async function registerProbeNode(
  payload: RegisterProbeNodeApiRequest,
): Promise<RegisterProbeNodeApiResponse> {
  const { data } = await apiClient.post<RegisterProbeNodeApiResponse>(
    '/api/admin/network-monitoring/probe-nodes/register',
    payload,
    { withCredentials: true },
  )

  return data
}

