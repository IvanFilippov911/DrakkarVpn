import { apiClient } from '../../shared/api/client'
import type {
  CreateTransportProfileApiRequest,
  TransportProfilesListApiResponse,
  TransportProfilesListQuery,
  UpdateTransportProfileApiRequest,
} from './types'

export async function getTransportProfiles(
  query: TransportProfilesListQuery,
): Promise<TransportProfilesListApiResponse> {
  const { data } = await apiClient.get<TransportProfilesListApiResponse>(
    '/api/admin/transport-profiles',
    {
      params: query,
      withCredentials: true,
    },
  )

  return data
}

export async function createTransportProfile(payload: CreateTransportProfileApiRequest): Promise<string> {
  const { data } = await apiClient.post<{ id: string }>('/api/admin/transport-profiles', payload, {
    withCredentials: true,
  })

  return data.id
}

export async function updateTransportProfile(
  profileId: string,
  payload: UpdateTransportProfileApiRequest,
): Promise<void> {
  await apiClient.put(`/api/admin/transport-profiles/${profileId}`, payload, {
    withCredentials: true,
  })
}

export async function enableTransportProfile(profileId: string): Promise<void> {
  await apiClient.post(`/api/admin/transport-profiles/${profileId}/enable`, null, {
    withCredentials: true,
  })
}

export async function disableTransportProfile(profileId: string): Promise<void> {
  await apiClient.post(`/api/admin/transport-profiles/${profileId}/disable`, null, {
    withCredentials: true,
  })
}

export async function deleteTransportProfile(profileId: string): Promise<void> {
  await apiClient.delete(`/api/admin/transport-profiles/${profileId}`, {
    withCredentials: true,
  })
}
