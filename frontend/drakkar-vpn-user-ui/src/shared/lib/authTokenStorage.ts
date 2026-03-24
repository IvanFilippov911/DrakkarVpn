const ACCESS_TOKEN_STORAGE_KEY = 'drakkar_user_access_token'

function isBrowserEnvironment(): boolean {
  return (
    typeof window !== 'undefined' &&
    typeof window.localStorage !== 'undefined' &&
    typeof window.sessionStorage !== 'undefined'
  )
}

export function getStoredAccessToken(): string | null {
  if (!isBrowserEnvironment()) {
    return null
  }

  try {
    const fromLocal = window.localStorage.getItem(ACCESS_TOKEN_STORAGE_KEY)
    if (fromLocal) return fromLocal
    const fromSession = window.sessionStorage.getItem(ACCESS_TOKEN_STORAGE_KEY)
    return fromSession || null
  } catch {
    return null
  }
}

export function setStoredAccessToken(
  token: string,
  persistent: boolean = true,
): void {
  if (!isBrowserEnvironment()) {
    return
  }

  try {
    const storage = persistent ? window.localStorage : window.sessionStorage
    const other = persistent ? window.sessionStorage : window.localStorage
    other.removeItem(ACCESS_TOKEN_STORAGE_KEY)
    storage.setItem(ACCESS_TOKEN_STORAGE_KEY, token)
  } catch {
    // ignore storage errors
  }
}

export function clearStoredAccessToken(): void {
  if (!isBrowserEnvironment()) {
    return
  }

  try {
    window.localStorage.removeItem(ACCESS_TOKEN_STORAGE_KEY)
    window.sessionStorage.removeItem(ACCESS_TOKEN_STORAGE_KEY)
  } catch {
    // ignore storage errors
  }
}
