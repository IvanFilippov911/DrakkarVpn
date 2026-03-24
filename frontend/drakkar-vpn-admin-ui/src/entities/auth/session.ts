import { setAccessToken } from '../../shared/api/client'
import {
  clearStoredAccessToken,
  getStoredAccessToken,
  setStoredAccessToken,
} from '../../shared/lib/authTokenStorage'
import type { AuthResponseDto } from './types'

export function applyAccessTokenFromResponse(
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

