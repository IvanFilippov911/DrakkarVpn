import { apiClient, setAccessToken } from './client'
import type {
  AuthResponseDto,
  CurrentAdminDto,
  LoginRequestDto,
} from '../../entities/auth/types'
import {
  clearStoredAccessToken,
  getStoredAccessToken,
  setStoredAccessToken,
} from '../lib/authTokenStorage'

function applyAccessTokenFromResponse(
  response: AuthResponseDto,
  persistent: boolean = true,
): void {
  const token = response.accessToken

  if (!token) {
    clearStoredAccessToken()
    setAccessToken(null)
    return
  }

  setStoredAccessToken(token, persistent)
  setAccessToken(token)
}

export async function login(
  email: LoginRequestDto['email'],
  password: LoginRequestDto['password'],
  rememberMe: boolean = true,
): Promise<AuthResponseDto> {
  const body: LoginRequestDto = { email, password }

  const { data } = await apiClient.post<AuthResponseDto>(
    '/api/admin/auth/login',
    body,
    {
      withCredentials: true,
    },
  )

  applyAccessTokenFromResponse(data, rememberMe)

  return data
}

export async function refresh(): Promise<AuthResponseDto> {
  const { data } = await apiClient.post<AuthResponseDto>(
    '/api/admin/auth/refresh',
    undefined,
    {
      withCredentials: true,
    },
  )

  applyAccessTokenFromResponse(data, true)

  return data
}

export async function logout(): Promise<void> {
  try {
    await apiClient.post('/api/admin/auth/logout', undefined, {
      withCredentials: true,
    })
  } finally {
    clearStoredAccessToken()
    setAccessToken(null)
  }
}

export async function getCurrentAdmin(): Promise<CurrentAdminDto> {
  const { data } = await apiClient.get<CurrentAdminDto>('/api/admin/auth/me', {
    withCredentials: true,
  })

  return data
}

export function initAuthFromStorage(): void {
  const token = getStoredAccessToken()

  if (!token) {
    setAccessToken(null)
    return
  }

  setAccessToken(token)
}

/**
 * Clears local auth session (token from storage and client). Does not call logout API.
 */
export function clearSession(): void {
  clearStoredAccessToken()
  setAccessToken(null)
}

