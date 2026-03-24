import { apiClient } from '../../shared/api/client'
import type { CurrentAdminDto } from './types'

export async function getCurrentAdmin(): Promise<CurrentAdminDto> {
  const { data } = await apiClient.get<CurrentAdminDto>('/api/admin/auth/me', {
    withCredentials: true,
  })

  return data
}

