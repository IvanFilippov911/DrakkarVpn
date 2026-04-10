const HAPP_ADD_PREFIX = 'happ://add/'
const V2RAYTUN_IMPORT_PREFIX = 'v2raytun://import/'

/** Синхронизировано с `deploy/app/.env.example` (VITE_HAPP_*, VITE_V2RAYTUN_*). */
const DEFAULT_HAPP_ANDROID_URL = 'https://play.google.com/store/apps/details?id=com.happproxy'
const DEFAULT_HAPP_IOS_URL =
  'https://apps.apple.com/ru/app/happ-proxy-utility-plus/id6746188973'
/** В `.env.example` совпадает с iOS (App Store RU Plus). */
const DEFAULT_HAPP_MACOS_URL = DEFAULT_HAPP_IOS_URL
/** В `.env.example` нет отдельной ссылки: Web / Windows / Linux и пр. */
const DEFAULT_HAPP_FALLBACK_URL = 'https://happ.su'

/** v2RayTun — см. `deploy/app/.env.example` (VITE_V2RAYTUN_*). */
const DEFAULT_V2RAYTUN_IOS_URL = 'https://apps.apple.com/us/app/v2raytun/id6476628951?l=ru'
/** Play: v2RayTun (DATABRIDGES, `com.v2raytun.android`); иначе — `VITE_V2RAYTUN_ANDROID_URL`. */
const DEFAULT_V2RAYTUN_ANDROID_URL = 'https://play.google.com/store/apps/details?id=com.v2raytun.android'

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
  return import.meta.env.VITE_HAPP_MACOS_URL || import.meta.env.VITE_HAPP_IOS_URL || DEFAULT_HAPP_MACOS_URL
}

function resolveV2RayTunMacInstallUrl(): string {
  return (
    import.meta.env.VITE_V2RAYTUN_MACOS_URL ||
    import.meta.env.VITE_V2RAYTUN_IOS_URL ||
    DEFAULT_V2RAYTUN_IOS_URL
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
 * Happ ожидает RAW URL после `happ://add/` — БЕЗ encodeURIComponent.
 * См. https://github.com/MHSanaei/3x-ui/commit/ccd223a (Fix DeepLink for Happ, remove encoding URL).
 */
export function normalizeHappDeeplink(raw: string): string {
  const s = raw.trim()
  if (!s.toLowerCase().startsWith(HAPP_ADD_PREFIX)) {
    return s.replace(/\s+/g, '')
  }
  const rest = s.slice(HAPP_ADD_PREFIX.length).trim()
  if (!rest) return s

  let payload = rest
  try {
    payload = decodeURIComponent(rest)
  } catch {
    /* already raw */
  }
  return HAPP_ADD_PREFIX + payload.replace(/\s+/g, '')
}

/**
 * v2raytun://import/... с бэка уже с EscapeDataString; убираем только пробелы по краям.
 */
export function normalizeV2RayTunDeeplink(raw: string): string {
  const s = raw.trim().replace(/\s+/g, '')
  if (!s.toLowerCase().startsWith(V2RAYTUN_IMPORT_PREFIX)) {
    return s
  }
  const rest = s.slice(V2RAYTUN_IMPORT_PREFIX.length)
  if (!rest) return s
  try {
    const decoded = decodeURIComponent(rest)
    return V2RAYTUN_IMPORT_PREFIX + encodeURIComponent(decoded.replace(/\s+/g, ''))
  } catch {
    return V2RAYTUN_IMPORT_PREFIX + encodeURIComponent(rest)
  }
}

/**
 * Одна ссылка за клик: сначала Happ, иначе v2RayTun (как раньше с openHappThenV2Ray, но без второго жеста).
 */
export function pickPrimaryVpnDeeplink(
  happLink: string | null | undefined,
  v2rayLink: string | null | undefined,
): string | null {
  const h = (happLink ?? '').trim()
  const v = (v2rayLink ?? '').trim()
  if (h) return normalizeHappDeeplink(h)
  if (v) return normalizeV2RayTunDeeplink(v)
  return null
}

/**
 * Построить HTTPS-URL промежуточной страницы /open-app.html?url=<deeplink>.
 * Safari откроет эту страницу и выполнит `location.href = deeplink` —
 * из Safari кастомные схемы (happ://, v2raytun://) работают на iOS.
 */
function buildRedirectPageUrl(deeplink: string): string {
  return `${window.location.origin}/open-app.html?url=${encodeURIComponent(deeplink)}`
}

/**
 * Открытие кастомной схемы (happ://, v2raytun://).
 *
 * Telegram WebView (WKWebView на iOS) **не пробрасывает** кастомные схемы напрямую.
 * `openLink` с `happ://` даёт «Url protocol is not supported».
 *
 * Решение: через `openLink` открываем **HTTPS**-страницу /open-app.html в **Safari**
 * (try_instant_view: false), а она уже делает `location.href = deeplink`.
 * Из Safari кастомные схемы работают.
 *
 * Вне Telegram — пробуем напрямую через `<a>` / `location.assign`.
 */
export function openDeeplinkUrlSync(url: string): void {
  if (!url.trim()) return

  if (import.meta.env.DEV) {
    console.info('[drakkar-vpn] deeplink open', url.slice(0, 120))
  } else {
    console.info('[drakkar-vpn] deeplink open')
  }

  const tw = window.Telegram?.WebApp
  if (typeof tw?.openLink === 'function') {
    try {
      const httpsRedirect = buildRedirectPageUrl(url)
      tw.openLink(httpsRedirect, { try_instant_view: false })
      return
    } catch {
      // fallback ниже
    }
  }

  try {
    const a = document.createElement('a')
    a.href = url
    a.setAttribute('rel', 'noopener noreferrer')
    a.style.cssText = 'position:fixed;left:-9999px;top:0;opacity:0;pointer-events:none'
    document.body.appendChild(a)
    a.click()
    document.body.removeChild(a)
  } catch {
    window.location.assign(url)
  }
}

/**
 * Открытие VPN-клиента в том же синхронном стеке, что и user gesture (важно для iOS).
 */
export function openVpnClientDeeplinkSync(
  happLink: string | null | undefined,
  v2rayLink: string | null | undefined,
): void {
  const url = pickPrimaryVpnDeeplink(happLink, v2rayLink)
  if (!url) return
  openDeeplinkUrlSync(url)
}

export type VpnDeeplinkOutcomeOptions = {
  /** Документ скрылся сразу после навигации — клиент, скорее всего, открылся. */
  onLikelyOpened: () => void
  /** Остаёмся на странице после паузы — клиент, скорее всего, не установлен / ссылка не сработала. */
  onStillVisible?: () => void
  timeoutMs?: number
}

/**
 * Обработчики вешаем до `location.href`, без await до навигации.
 * Таймер только после попытки открытия (не перед ней).
 */
export function beginVpnDeeplinkAttempt(options: VpnDeeplinkOutcomeOptions): void {
  const timeoutMs = options.timeoutMs ?? 1500
  let finished = false

  const finish = (kind: 'opened' | 'still') => {
    if (finished) return
    finished = true
    document.removeEventListener('visibilitychange', onVisibilityChange)
    window.clearTimeout(timerId)
    if (kind === 'opened') {
      options.onLikelyOpened()
    } else {
      options.onStillVisible?.()
    }
  }

  const onVisibilityChange = () => {
    if (document.hidden) {
      finish('opened')
    }
  }

  document.addEventListener('visibilitychange', onVisibilityChange)

  const timerId = window.setTimeout(() => {
    finish('still')
  }, timeoutMs)
}

/**
 * Сначала Happ (если есть), после таймаута и если WebView всё ещё видим — v2RayTun (если есть).
 * Если оба исчерпаны и страница всё ещё на месте — onStillVisible (экран установки клиента и т.д.).
 */
export function beginSequentialVpnDeeplinkAttempt(
  happLink: string | null | undefined,
  v2rayLink: string | null | undefined,
  options: VpnDeeplinkOutcomeOptions,
): void {
  const h = (happLink ?? '').trim()
  const v = (v2rayLink ?? '').trim()
  const timeoutMs = options.timeoutMs ?? 1600

  if (!h && !v) {
    options.onStillVisible?.()
    return
  }

  const run = (which: 'happ' | 'v2ray'): void => {
    const url =
      which === 'happ' ? pickPrimaryVpnDeeplink(h, undefined) : pickPrimaryVpnDeeplink(undefined, v)
    if (!url) {
      if (which === 'happ' && v) {
        run('v2ray')
        return
      }
      options.onStillVisible?.()
      return
    }

    let finished = false
    let timerId = 0

    const cleanup = (onVis: () => void) => {
      document.removeEventListener('visibilitychange', onVis)
      window.clearTimeout(timerId)
    }

    const onLikelyOpened = () => {
      if (finished) return
      finished = true
      cleanup(onVis)
      options.onLikelyOpened()
    }

    const onStill = () => {
      if (finished) return
      finished = true
      cleanup(onVis)
      if (which === 'happ' && v) {
        run('v2ray')
        return
      }
      options.onStillVisible?.()
    }

    const onVis = () => {
      if (document.hidden) {
        onLikelyOpened()
      }
    }

    document.addEventListener('visibilitychange', onVis)
    timerId = window.setTimeout(() => {
      if (document.hidden) {
        onLikelyOpened()
      } else {
        onStill()
      }
    }, timeoutMs)

    openDeeplinkUrlSync(url)
  }

  run(h ? 'happ' : 'v2ray')
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
    return import.meta.env.VITE_HAPP_IOS_URL || DEFAULT_HAPP_IOS_URL
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

export function getV2RayTunInstallUrl(platform?: string): string {
  const p = platform?.toLowerCase() ?? ''

  if (p.includes('android')) {
    return import.meta.env.VITE_V2RAYTUN_ANDROID_URL || DEFAULT_V2RAYTUN_ANDROID_URL
  }

  if (p.includes('ios') || p.includes('iphone') || p.includes('ipad')) {
    return import.meta.env.VITE_V2RAYTUN_IOS_URL || DEFAULT_V2RAYTUN_IOS_URL
  }

  if (p.includes('mac')) {
    return resolveV2RayTunMacInstallUrl()
  }

  if (isLikelyMacOsFromNavigator()) {
    return resolveV2RayTunMacInstallUrl()
  }

  return (
    import.meta.env.VITE_V2RAYTUN_DESKTOP_URL ||
    import.meta.env.VITE_V2RAYTUN_IOS_URL ||
    DEFAULT_V2RAYTUN_IOS_URL
  )
}
