const DEVICE_ID_STORAGE_KEY = 'drakkar_user_device_id'

function isBrowserEnvironment(): boolean {
  return typeof window !== 'undefined' && typeof window.localStorage !== 'undefined'
}

export function getStoredDeviceId(): string | null {
  if (!isBrowserEnvironment()) return null
  try {
    return window.localStorage.getItem(DEVICE_ID_STORAGE_KEY)
  } catch {
    return null
  }
}

export function setStoredDeviceId(deviceId: string): void {
  if (!isBrowserEnvironment()) return
  try {
    window.localStorage.setItem(DEVICE_ID_STORAGE_KEY, deviceId)
  } catch {
    // ignore storage errors
  }
}

export function clearStoredDeviceId(): void {
  if (!isBrowserEnvironment()) return
  try {
    window.localStorage.removeItem(DEVICE_ID_STORAGE_KEY)
  } catch {
    // ignore storage errors
  }
}
