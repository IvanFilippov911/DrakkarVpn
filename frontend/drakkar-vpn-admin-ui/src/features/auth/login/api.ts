import { apiClient } from '../../../shared/api/client'
import type { AuthResponseDto, LoginRequestDto } from '../../../entities/auth/types'
import { applyAccessTokenFromResponse } from '../../../entities/auth/session'

export async function login(
  email: LoginRequestDto['email'],
  password: LoginRequestDto['password'],
  rememberMe: boolean = true,
): Promise<AuthResponseDto> {
  const body: LoginRequestDto = { email, password }

  const { data } = await apiClient.post<AuthResponseDto>('/api/admin/auth/login', body, {
    withCredentials: true,
  })

  applyAccessTokenFromResponse(data, rememberMe)
  return data
}

