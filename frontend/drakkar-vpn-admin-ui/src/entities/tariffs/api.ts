import { apiClient } from '../../shared/api/client'
import type { CreateTariffApiRequest, TariffApiResponse, UpdateTariffApiRequest } from './types'

export async function getActiveTariffs(): Promise<TariffApiResponse[]> {
  const { data } = await apiClient.get<TariffApiResponse[]>('/api/admin/tariffs/active', {
    withCredentials: true,
  })

  return data
}

export async function getTariffById(id: string): Promise<TariffApiResponse> {
  const { data } = await apiClient.get<TariffApiResponse>(`/api/admin/tariffs/${id}`, {
    withCredentials: true,
  })

  return data
}

export async function createTariff(payload: CreateTariffApiRequest): Promise<string> {
  const { data } = await apiClient.post<string>('/api/admin/tariffs', payload, {
    withCredentials: true,
  })

  return data
}

export async function updateTariff(id: string, payload: UpdateTariffApiRequest): Promise<void> {
  await apiClient.put(`/api/admin/tariffs/${id}`, payload, {
    withCredentials: true,
  })
}

export async function enableTariff(id: string): Promise<void> {
  await apiClient.patch(
    `/api/admin/tariffs/${id}/enable`,
    null,
    {
      withCredentials: true,
    },
  )
}

export async function disableTariff(id: string): Promise<void> {
  await apiClient.patch(
    `/api/admin/tariffs/${id}/disable`,
    null,
    {
      withCredentials: true,
    },
  )
}

