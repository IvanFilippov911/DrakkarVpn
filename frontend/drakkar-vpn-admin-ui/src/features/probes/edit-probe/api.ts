import { apiClient } from '../../../shared/api/client'

export type UpdateProbeNodeApiRequest = {
  name: string
  region: string
  host: string
}

export async function updateProbeNode(id: string, payload: UpdateProbeNodeApiRequest): Promise<void> {
  await apiClient.put(`/api/admin/network-monitoring/probe-nodes/${id}`, payload, {
    withCredentials: true,
  })
}

