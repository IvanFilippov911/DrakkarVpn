declare global {
  interface Window {
    Telegram?: {
      WebApp?: {
        ready: () => void
        expand: () => void
        initData: string
        initDataUnsafe?: { user?: { id?: number } }
        platform?: string
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

export function readTelegramBootContext(): TelegramBootResult {
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

export function initTelegramChrome(): void {
  window.Telegram?.WebApp?.ready()
  window.Telegram?.WebApp?.expand()
}

/** Telegram user id из WebApp (после успешного boot совпадает с контекстом покупки). */
export function getTelegramUserId(): number | undefined {
  const id = window.Telegram?.WebApp?.initDataUnsafe?.user?.id
  if (id == null || id <= 0) return undefined
  return id
}
