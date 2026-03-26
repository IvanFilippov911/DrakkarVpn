import { useCallback, useEffect, useState } from 'react'
import { Outlet } from 'react-router-dom'
import { useConnectDevice, useRegister } from '../features/user'
import { IconBlocked, IconPending } from '../shared/ui/icons/homeStateIcons'
import { getStoredDeviceId } from '../shared/lib/deviceIdStorage'
import { initTelegramChrome, readTelegramBootContext } from '../shared/lib/telegramContext'
import { AppContainer, BrandBlock, PrimaryButton, StateCard, StatusIndicator } from '../shared/ui'

type BootPhase = 'running' | 'ready' | 'error'

export function AppBootLayout() {
  const [phase, setPhase] = useState<BootPhase>('running')
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [bootAttempt, setBootAttempt] = useState(0)

  const { mutateAsync: registerAsync } = useRegister()
  const { mutateAsync: connectAsync } = useConnectDevice()

  const waitForTelegramWebApp = async (timeoutMs: number): Promise<boolean> => {
    const start = Date.now()
    // window.Telegram.WebApp иногда появляется чуть позже инициализации React.
    while (Date.now() - start < timeoutMs) {
      if (window.Telegram?.WebApp) return true
      await new Promise((r) => window.setTimeout(r, 50))
    }
    return Boolean(window.Telegram?.WebApp)
  }

  const runBoot = useCallback(async () => {
    setPhase('running')
    setErrorMessage(null)
    initTelegramChrome()

    await waitForTelegramWebApp(2000)
    // На всякий случай ещё раз активируем ready/expand.
    initTelegramChrome()

    const ctx = readTelegramBootContext()
    if (!ctx.ok) {
      setErrorMessage(ctx.message)
      setPhase('error')
      return
    }

    try {
      await registerAsync({ telegramId: ctx.telegramId })
      await connectAsync({
        initData: ctx.initData,
        deviceName: typeof navigator !== 'undefined' ? navigator.userAgent.slice(0, 120) : undefined,
        platform: ctx.platform ?? 'telegram_webapp',
        existingDeviceId: getStoredDeviceId() ?? undefined,
      })
      setPhase('ready')
    } catch {
      setErrorMessage('Не удалось подключиться. Проверьте сеть и попробуйте снова.')
      setPhase('error')
    }
  }, [connectAsync, registerAsync])

  useEffect(() => {
    const id = window.setTimeout(() => {
      void runBoot()
    }, 0)
    return () => window.clearTimeout(id)
  }, [runBoot, bootAttempt])

  const handleRetry = () => {
    setBootAttempt((n) => n + 1)
  }

  if (phase === 'error' && errorMessage) {
    return (
      <AppContainer>
        <header className="px-6 pt-6 pb-4">
          <StatusIndicator tone="blocked" label="ошибка запуска" />
        </header>
        <main className="flex flex-1 flex-col items-center justify-center px-6 pb-8">
          <div className="flex flex-1 flex-col items-center justify-center">
            <BrandBlock />
            <div className="h-20 shrink-0" aria-hidden />
            <StateCard
              variant="error"
              title="Ошибка запуска"
              subtitle={errorMessage}
              icon={<IconBlocked />}
            />
          </div>
        </main>
        <footer className="ui-safe-bottom px-6 pt-4">
          <PrimaryButton type="button" onClick={handleRetry}>
            Повторить
          </PrimaryButton>
        </footer>
      </AppContainer>
    )
  }

  if (phase !== 'ready') {
    return (
      <AppContainer>
        <header className="px-6 pt-6 pb-4">
          <StatusIndicator tone="pending" label="инициализация" />
        </header>
        <main className="flex flex-1 flex-col items-center justify-center px-6 pb-8">
          <div className="flex flex-1 flex-col items-center justify-center">
            <BrandBlock />
            <div className="h-20 shrink-0" aria-hidden />
            <StateCard
              variant="default"
              title="Загрузка"
              subtitle="Подключаемся к серверу…"
              icon={<IconPending />}
            />
          </div>
        </main>
        <footer className="ui-safe-bottom px-6 pt-4">
          <div className="h-14" aria-hidden />
        </footer>
      </AppContainer>
    )
  }

  return <Outlet />
}
