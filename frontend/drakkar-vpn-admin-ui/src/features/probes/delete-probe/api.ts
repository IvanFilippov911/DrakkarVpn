import { apiClient } from '../../../shared/api/client'

export async function deleteProbeNode(probeNodeId: string): Promise<void> {
  await apiClient.delete(`/api/admin/network-monitoring/probe-nodes/${probeNodeId}`, {
    withCredentials: true,
  })
}

