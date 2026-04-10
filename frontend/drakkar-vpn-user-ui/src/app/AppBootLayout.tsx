import { isAxiosError } from 'axios'
import { useCallback, useEffect, useRef, useState } from 'react'
import type { ReactNode } from 'react'
import { useQueryClient, type QueryClient } from '@tanstack/react-query'
import { RoutedPageOutlet } from './RoutedPageOutlet'
import { getCurrentConfig, getHomeContext, getUserSummary } from '../entities/user/api'
import { userQueryKeys } from '../entities/user/queryKeys'
import type { HomeContextResponse } from '../entities/user/types'
import { useConnectDevice, useRegister } from '../features/user'
import { setAccessToken } from '../shared/api/client'
import { getStoredAccessToken, clearStoredAccessToken } from '../shared/lib/authTokenStorage'
import { getStoredDeviceId } from '../shared/lib/deviceIdStorage'
import { initTelegramChrome, readTelegramBootContext } from '../shared/lib/telegramContext'
import { BottomNavProvider } from '../context/bottomNavContext'
import { AppContainer, BrandBlock, PrimaryButton } from '../shared/ui'

type BootPhase = 'running' | 'ready' | 'error'

/**
 * Warm cache for the home tab before first paint so brand, summary card, and CTA
 * resolve together (same pattern as navigating back from other tabs).
 */
async function prefetchHomeRouteData(qc: QueryClient): Promise<void> {
  try {
    await qc.prefetchQuery({
      queryKey: userQueryKeys.homeContext,
      queryFn: getHomeContext,
    })
  } catch {
    return
  }

  try {
    await qc.prefetchQuery({
      queryKey: userQueryKeys.summary,
      queryFn: getUserSummary,
    })
  } catch {
    /* non-fatal: HomePage will refetch */
  }

  const home = qc.getQueryData<HomeContextResponse>(userQueryKeys.homeContext)
  if (home?.state === 'Ready') {
    try {
      await qc.prefetchQuery({
        queryKey: userQueryKeys.currentConfig,
        queryFn: getCurrentConfig,
      })
    } catch {
      /* non-fatal */
    }
  }
}

function BootShell({ footer }: { footer: ReactNode }) {
  return (
    <div className="flex h-[var(--app-viewport-height,var(--tg-viewport-height,100dvh))] max-h-[var(--app-viewport-height,var(--tg-viewport-height,100dvh))] min-h-0 flex-col overflow-hidden">
      <AppContainer>
        <main className="flex min-h-0 flex-1 flex-col items-center justify-center px-6 pb-3 pt-[max(1.5rem,env(safe-area-inset-top))]">
          <div className="flex w-full min-h-0 flex-1 flex-col items-center justify-center">
            <BrandBlock />
            <div className="h-8 shrink-0" aria-hidden />
          </div>
        </main>
        <footer className="w-full shrink-0 px-6 pb-[max(0.75rem,env(safe-area-inset-bottom))] pt-1">
          {footer}
        </footer>
      </AppContainer>
    </div>
  )
}

export function AppBootLayout() {
  const queryClient = useQueryClient()
  const queryClientRef = useRef(queryClient)
  const [phase, setPhase] = useState<BootPhase>('running')
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

      const existingToken = getStoredAccessToken()
      if (existingToken) {
        setAccessToken(existingToken)
        try {
          const home = await getHomeContext()
          if (gen !== bootGenRef.current) return
          queryClientRef.current.setQueryData(userQueryKeys.homeContext, home)
          initTelegramChrome()
          await prefetchHomeRouteData(queryClientRef.current)
          if (gen !== bootGenRef.current) return
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
        await prefetchHomeRouteData(queryClientRef.current)
        if (gen !== bootGenRef.current) return
        setPhase('ready')
      } catch {
        if (gen !== bootGenRef.current) return
        setPhase('error')
      }
    }

    void runBoot()
  }, [bootAttempt])

  if (phase === 'error') {
    return (
      <BootShell
        footer={<PrimaryButton type="button" onClick={handleRetry}>Повторить</PrimaryButton>}
      />
    )
  }

  if (phase !== 'ready') {
    return (
      <BootShell
        footer={<div className="h-14" aria-hidden />}
      />
    )
  }

  return (
    <BottomNavProvider>
      <RoutedPageOutlet />
    </BottomNavProvider>
  )
}
