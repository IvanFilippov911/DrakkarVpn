function deviceIdStorageKey(): string {
  const devBypass =
    import.meta.env.DEV && import.meta.env.VITE_TELEGRAM_DEV_BYPASS === 'true'
  if (devBypass) {
    const id =
      import.meta.env.VITE_DEV_TELEGRAM_ID != null &&
      String(import.meta.env.VITE_DEV_TELEGRAM_ID).trim() !== ''
        ? String(import.meta.env.VITE_DEV_TELEGRAM_ID).trim()
        : '7'
    return `drakkar_user_device_id_dev_${id}`
  }
  return 'drakkar_user_device_id'
}

function isBrowserEnvironment(): boolean {
  return typeof window !== 'undefined' && typeof window.localStorage !== 'undefined'
}

export function getStoredDeviceId(): string | null {
  if (!isBrowserEnvironment()) return null
  try {
    return window.localStorage.getItem(deviceIdStorageKey())
  } catch {
    return null
  }
}

export function setStoredDeviceId(deviceId: string): void {
  if (!isBrowserEnvironment()) return
  try {
    window.localStorage.setItem(deviceIdStorageKey(), deviceId)
  } catch {
    // ignore storage errors
  }
}

export function clearStoredDeviceId(): void {
  if (!isBrowserEnvironment()) return
  try {
    window.localStorage.removeItem(deviceIdStorageKey())
  } catch {
    // ignore storage errors
  }
}
