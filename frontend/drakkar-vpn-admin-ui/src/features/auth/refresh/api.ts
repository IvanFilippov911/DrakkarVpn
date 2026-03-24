import { apiClient } from '../../../shared/api/client'
import type { AuthResponseDto } from '../../../entities/auth/types'
import { applyAccessTokenFromResponse } from '../../../entities/auth/session'

export async function refresh(): Promise<AuthResponseDto> {
  const { data } = await apiClient.post<AuthResponseDto>('/api/admin/auth/refresh', undefined, {
    withCredentials: true,
  })

  applyAccessTokenFromResponse(data, true)
  return data
}

