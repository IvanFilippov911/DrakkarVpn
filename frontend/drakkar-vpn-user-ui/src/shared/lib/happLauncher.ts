const HAPP_LAUNCH_TIMEOUT_MS = 1500

/** Синхронизировано с `deploy/app/.env.example` (VITE_HAPP_*). */
const DEFAULT_HAPP_ANDROID_URL = 'https://play.google.com/store/apps/details?id=com.happproxy'
const DEFAULT_HAPP_IOS_URL =
  'https://apps.apple.com/ru/app/happ-proxy-utility-plus/id6746188973'
/** В `.env.example` совпадает с iOS (App Store RU Plus). */
const DEFAULT_HAPP_MACOS_URL = DEFAULT_HAPP_IOS_URL
/** В `.env.example` нет отдельной ссылки: Web / Windows / Linux и пр. */
const DEFAULT_HAPP_FALLBACK_URL = 'https://happ.su'

/** Если хотя раз в этой вкладке WebView ушли в HApp (visibility), считаем что клиент есть — отмена deep link не показывает экран установки. */
const SESSION_STORAGE_HAPP_OPENED_OK = 'drakkar-vpn-happ-opened-ok'

export function markHappOpenedSuccessfully(): void {
  try {
    sessionStorage.setItem(SESSION_STORAGE_HAPP_OPENED_OK, '1')
  } catch {
    // private mode / disabled storage
  }
}

export function hasOpenedHappSuccessfullyInSession(): boolean {
  try {
    return sessionStorage.getItem(SESSION_STORAGE_HAPP_OPENED_OK) === '1'
  } catch {
    return false
  }
}

function resolveMacAppStoreInstallUrl(): string {
  return (
    import.meta.env.VITE_HAPP_MACOS_URL ||
    import.meta.env.VITE_HAPP_IOS_URL ||
    import.meta.env.VITE_V2RAYTUN_IOS_URL ||
    DEFAULT_HAPP_MACOS_URL
  )
}

/**
 * Telegram Desktop / Web на Mac часто дают platform вроде `tdesktop` или `webk` без `mac`.
 * Тогда ориентируемся на userAgent / navigator.platform.
 */
function isLikelyMacOsFromNavigator(): boolean {
  if (typeof navigator === 'undefined') return false
  const ua = navigator.userAgent ?? ''
  const plat = navigator.platform ?? ''
  if (/iPhone|iPod/i.test(ua)) return false
  if (/iPad/i.test(ua)) return false
  if (plat === 'MacIntel' && navigator.maxTouchPoints > 1) return false

  return /Mac OS X|Macintosh/i.test(ua) || /MacIntel/i.test(plat) || plat.toLowerCase().includes('mac')
}

/**
 * Tries to open a HApp deep-link and detects whether the app switch likely happened.
 * Returns false when browser stayed visible past timeout (likely no handler installed).
 */
export async function openHappLinkWithFallback(
  happLink: string,
  timeoutMs: number = HAPP_LAUNCH_TIMEOUT_MS,
): Promise<boolean> {
  if (!happLink) return false

  let didHide = false
  const onVisibilityChange = () => {
    if (document.hidden) didHide = true
  }

  document.addEventListener('visibilitychange', onVisibilityChange)

  try {
    window.location.href = happLink
    await new Promise((resolve) => window.setTimeout(resolve, timeoutMs))
    return didHide
  } finally {
    document.removeEventListener('visibilitychange', onVisibilityChange)
  }
}

export function getHappInstallUrl(platform?: string): string {
  const p = platform?.toLowerCase() ?? ''

  if (p.includes('android')) {
    return (
      import.meta.env.VITE_HAPP_ANDROID_URL ||
      import.meta.env.VITE_HIDDIFY_ANDROID_URL ||
      DEFAULT_HAPP_ANDROID_URL
    )
  }

  if (p.includes('ios') || p.includes('iphone') || p.includes('ipad')) {
    return (
      import.meta.env.VITE_HAPP_IOS_URL ||
      import.meta.env.VITE_V2RAYTUN_IOS_URL ||
      DEFAULT_HAPP_IOS_URL
    )
  }

  if (p.includes('mac')) {
    return resolveMacAppStoreInstallUrl()
  }

  if (isLikelyMacOsFromNavigator()) {
    return resolveMacAppStoreInstallUrl()
  }

  return (
    import.meta.env.VITE_HAPP_DESKTOP_URL ||
    import.meta.env.VITE_HIDDIFY_DESKTOP_URL ||
    DEFAULT_HAPP_FALLBACK_URL
  )
}
