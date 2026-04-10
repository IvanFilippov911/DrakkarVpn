declare global {
  interface Window {
    Telegram?: {
      WebApp?: {
        ready: () => void
        expand: () => void
        /** Current visible Mini App height (px). Prefer over CSS 100vh in Telegram. */
        viewportHeight?: number
        /** Last stable visible height; changes after gestures finish. */
        viewportStableHeight?: number
        initData: string
        initDataUnsafe?: { user?: { id?: number } }
        platform?: string
        onEvent?: (eventType: 'viewportChanged', eventHandler: () => void) => void
        offEvent?: (eventType: 'viewportChanged', eventHandler: () => void) => void
        /** Открытие URL вне Mini App; для iOS WebView предпочтительно для happ:// / v2raytun:// вместо location.href */
        openLink?: (url: string, options?: { try_instant_view?: boolean }) => void
      }
    }
  }
}

export type TelegramBootOk = {
  ok: true
  initData: string
  telegramId: number
  platform?: string
}

export type TelegramBootErr = {
  ok: false
  message: string
}

export type TelegramBootResult = TelegramBootOk | TelegramBootErr

function devTelegramBypassEnabled(): boolean {
  return import.meta.env.DEV && import.meta.env.VITE_TELEGRAM_DEV_BYPASS === 'true'
}

function devBypassTelegramId(): number {
  const raw = import.meta.env.VITE_DEV_TELEGRAM_ID
  const n = raw != null && String(raw).trim() !== '' ? Number(raw) : NaN
  return Number.isFinite(n) && n > 0 ? Math.trunc(n) : 7
}

export function readTelegramBootContext(): TelegramBootResult {
  if (devTelegramBypassEnabled()) {
    const telegramId = devBypassTelegramId()
    const initData = `dev=1&auth_date=${Math.floor(Date.now() / 1000)}&user=${encodeURIComponent(JSON.stringify({ id: telegramId }))}`
    return {
      ok: true,
      initData,
      telegramId,
      platform: 'telegram_webapp',
    }
  }

  const tw = window.Telegram?.WebApp
  if (!tw) {
    return { ok: false, message: 'Откройте приложение внутри Telegram.' }
  }

  const initData = tw.initData?.trim() ?? ''
  if (!initData) {
    return { ok: false, message: 'Нет данных авторизации Telegram. Закройте и откройте WebApp снова.' }
  }

  const telegramId = tw.initDataUnsafe?.user?.id
  if (telegramId == null || telegramId <= 0) {
    return { ok: false, message: 'Не удалось определить пользователя Telegram.' }
  }

  return {
    ok: true,
    initData,
    telegramId,
    platform: tw.platform,
  }
}

/** Hides Telegram chrome loading state. expand() runs once at app entry (see telegramViewport.primeTelegramMiniAppViewport). */
export function initTelegramChrome(): void {
  window.Telegram?.WebApp?.ready()
}

/** Telegram user id из WebApp (после успешного boot совпадает с контекстом покупки). */
export function getTelegramUserId(): number | undefined {
  if (devTelegramBypassEnabled()) {
    return devBypassTelegramId()
  }
  const id = window.Telegram?.WebApp?.initDataUnsafe?.user?.id
  if (id == null || id <= 0) return undefined
  return id
}
