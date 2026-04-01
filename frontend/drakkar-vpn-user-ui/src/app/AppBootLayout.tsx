import { isAxiosError } from 'axios'
import { useCallback, useEffect, useRef, useState } from 'react'
import { useQueryClient } from '@tanstack/react-query'
import { Outlet } from 'react-router-dom'
import { getHomeContext } from '../entities/user/api'
import { userQueryKeys } from '../entities/user/queryKeys'
import { useConnectDevice, useRegister } from '../features/user'
import { setAccessToken } from '../shared/api/client'
import { IconBlocked, IconPending } from '../shared/ui/icons/homeStateIcons'
import { getStoredAccessToken, clearStoredAccessToken } from '../shared/lib/authTokenStorage'
import { getStoredDeviceId } from '../shared/lib/deviceIdStorage'
import { initTelegramChrome, readTelegramBootContext } from '../shared/lib/telegramContext'
import { AppContainer, BrandBlock, PrimaryButton, StateCard, StatusIndicator } from '../shared/ui'

type BootPhase = 'running' | 'ready' | 'error'

export function AppBootLayout() {
  const queryClient = useQueryClient()
  const queryClientRef = useRef(queryClient)
  const [phase, setPhase] = useState<BootPhase>('running')
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [bootAttempt, setBootAttempt] = useState(0)
  const bootGenRef = useRef(0)

  const { mutateAsync: registerAsync } = useRegister()
  const { mutateAsync: connectAsync } = useConnectDevice()

  const registerAsyncRef = useRef(registerAsync)
  const connectAsyncRef = useRef(connectAsync)

  useEffect(() => {
    registerAsyncRef.current = registerAsync
    connectAsyncRef.current = connectAsync
  }, [registerAsync, connectAsync])

  useEffect(() => {
    queryClientRef.current = queryClient
  }, [queryClient])

  const waitForTelegramWebApp = async (timeoutMs: number): Promise<boolean> => {
    const start = Date.now()
    while (Date.now() - start < timeoutMs) {
      if (window.Telegram?.WebApp) return true
      await new Promise((r) => window.setTimeout(r, 50))
    }
    return Boolean(window.Telegram?.WebApp)
  }

  const handleRetry = useCallback(() => {
    setBootAttempt((n) => n + 1)
  }, [])

  useEffect(() => {
    const gen = ++bootGenRef.current

    const runBoot = async () => {
      setPhase('running')
      setErrorMessage(null)

      const existingToken = getStoredAccessToken()
      if (existingToken) {
        setAccessToken(existingToken)
        try {
          const home = await getHomeContext()
          if (gen !== bootGenRef.current) return
          queryClientRef.current.setQueryData(userQueryKeys.homeContext, home)
          initTelegramChrome()
          setPhase('ready')
          return
        } catch (err) {
          if (gen !== bootGenRef.current) return
          if (
            isAxiosError(err) &&
            (err.response?.status === 401 || err.response?.status === 403)
          ) {
            clearStoredAccessToken()
            setAccessToken(null)
          }
        }
      }

      initTelegramChrome()

      let ctx = readTelegramBootContext()
      if (!ctx.ok) {
        await waitForTelegramWebApp(2000)
        initTelegramChrome()
        ctx = readTelegramBootContext()
      }

      if (!ctx.ok) {
        if (gen !== bootGenRef.current) return
        setErrorMessage(ctx.message)
        setPhase('error')
        return
      }

      try {
        await registerAsyncRef.current({ telegramId: ctx.telegramId })
        if (gen !== bootGenRef.current) return
        await connectAsyncRef.current({
          initData: ctx.initData,
          deviceName: typeof navigator !== 'undefined' ? navigator.userAgent.slice(0, 120) : undefined,
          platform: ctx.platform ?? 'telegram_webapp',
          existingDeviceId: getStoredDeviceId() ?? undefined,
        })
        if (gen !== bootGenRef.current) return
        setPhase('ready')
      } catch {
        if (gen !== bootGenRef.current) return
        setErrorMessage('Не удалось подключиться. Проверьте сеть и попробуйте снова.')
        setPhase('error')
      }
    }

    void runBoot()
  }, [bootAttempt])

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
