import { isAxiosError } from 'axios'
import { differenceInCalendarDays } from 'date-fns'
import type { ReactNode } from 'react'
import { useCallback, useEffect, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useBottomNavControl } from '../../context/bottomNavContext'
import type { HomeScreenState, VpnConfigResponse } from '../../entities/user'
import { useCurrentConfig, useHomeContext, useProvisionPolling, useUserSummary } from '../../entities/user'
import { useStartProvision } from '../../features/user'
import {
  getHappInstallUrl,
  hasOpenedHappSuccessfullyInSession,
  markHappOpenedSuccessfully,
  openHappLinkWithFallback,
} from '../../shared/lib/happLauncher'
import { AppContainer, BrandBlock, PrimaryButton } from '../../shared/ui'
import { HOME_PRESENTATION } from './homePresentation'

type ReadyCtaError = 'maintenance' | 'session'
type ProvisionFailure = {
  errorCode?: string | null
  errorMessage?: string | null
}

type ConnectOverlay =
  | { kind: 'addSubscription'; deepLink: string; copyTarget: string }
  | { kind: 'needInstallClient'; deepLink: string; copyTarget: string }

function MiniSpinner({ className = 'h-4 w-4' }: { className?: string }) {
  return (
    <svg className={`animate-spin text-current ${className}`} viewBox="0 0 24 24" fill="none" aria-hidden>
      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="3" />
      <path
        className="opacity-80"
        fill="currentColor"
        d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
      />
    </svg>
  )
}

function ConnectStepShell({
  title,
  subtitle,
  primary,
  secondary,
  tertiary,
}: {
  title: string
  subtitle: string
  primary: ReactNode
  secondary: ReactNode
  tertiary?: ReactNode
}) {
  return (
    <AppContainer>
      <div className="flex h-full min-h-0 flex-col overflow-hidden px-6 pb-3 pt-[max(1.25rem,env(safe-area-inset-top))]">
        <div className="flex min-h-0 flex-1 flex-col items-center justify-center overflow-hidden text-center">
          <div className="w-full max-w-sm">
            <div className="mb-4 flex justify-center" aria-hidden>
              <img
                src="/drakkar-mark.png"
                alt=""
                className="h-20 w-auto max-w-[12rem] object-contain select-none"
                draggable={false}
              />
            </div>
            <h2 className="font-sans text-xl font-bold leading-tight tracking-[-0.02em] text-[#e8e2d6]">{title}</h2>
            <p className="mt-2 text-base font-normal leading-normal text-[#8b8f94]">{subtitle}</p>
          </div>
        </div>
        <div className="mx-auto w-full max-w-sm shrink-0 space-y-3 pb-2">
          {primary}
          {secondary}
          {tertiary}
        </div>
      </div>
    </AppContainer>
  )
}

function pluralizeDaysRu(n: number): string {
  const abs = Math.abs(n)
  const mod10 = abs % 10
  const mod100 = abs % 100
  if (mod100 >= 11 && mod100 <= 14) return 'дней'
  if (mod10 === 1) return 'день'
  if (mod10 >= 2 && mod10 <= 4) return 'дня'
  return 'дней'
}

function SummaryCard({
  subscriptionEndAtUtc,
  connectedDevices,
  maxDevices,
}: {
  subscriptionEndAtUtc: string | null
  connectedDevices: number
  maxDevices: number | null
}) {
  const hasSubscription = subscriptionEndAtUtc != null && subscriptionEndAtUtc.trim() !== '' && maxDevices != null
  const endDate = hasSubscription ? new Date(subscriptionEndAtUtc!) : null
  const daysLeft =
    endDate && Number.isFinite(endDate.getTime())
      ? Math.max(0, differenceInCalendarDays(endDate, new Date()))
      : null

  return (
    <div className="mx-auto w-full max-w-sm rounded-2xl bg-[rgba(255,255,255,0.04)] p-4">
      <div className="flex items-center gap-3">
        <span
          className={[
            'shrink-0 text-xl leading-none',
            hasSubscription ? 'text-green-500' : 'text-red-500',
          ].join(' ')}
          aria-hidden
        >
          {hasSubscription ? '✔' : '✖'}
        </span>
        <div className="min-w-0">
          {hasSubscription ? (
            <>
              <p className="truncate font-sans text-[15px] font-semibold text-[var(--foreground)]">
                Подписка активна
                {daysLeft != null ? ` · ${daysLeft} ${pluralizeDaysRu(daysLeft)}` : ''}
              </p>
              <p className="mt-0.5 text-[13px] font-normal text-[#8b8f94]">
                Устройства: {connectedDevices} из {maxDevices}
              </p>
            </>
          ) : (
            <>
              <p className="truncate font-sans text-[15px] font-semibold text-[var(--foreground)]">
                Подписка неактивна
              </p>
              <p className="mt-0.5 text-[13px] font-normal text-[#8b8f94]">
                Подключите тариф для доступа
              </p>
            </>
          )}
        </div>
      </div>
    </div>
  )
}

function HomeShell({
  statusText,
  statusCard,
  footer,
}: {
  statusText?: string | null
  statusCard?: ReactNode
  footer: ReactNode
}) {
  return (
    <AppContainer>
      <main className="flex min-h-0 flex-1 flex-col items-center justify-center px-6 pb-3 pt-[max(1.25rem,env(safe-area-inset-top))]">
        <div className="flex w-full min-w-0 min-h-0 flex-1 flex-col items-center justify-center overflow-hidden">
          <BrandBlock />
          <div className="h-6 shrink-0" aria-hidden />
        </div>
      </main>
      <footer className="w-full shrink-0 px-6 pt-2">
        {statusCard ? <div className="mb-3">{statusCard}</div> : null}
        {statusText ? (
          <p className="mb-2 w-full text-center text-[14px] font-normal leading-normal text-[#a1a6aa]">{statusText}</p>
        ) : null}
        {footer}
      </footer>
    </AppContainer>
  )
}

async function copyText(text: string): Promise<boolean> {
  if (!text) return false
  try {
    await navigator.clipboard.writeText(text)
    return true
  } catch {
    return false
  }
}

function pickDeepLink(data: VpnConfigResponse | undefined, isIos: boolean): string | null {
  if (!data) return null
  if (isIos) {
    return data.v2rayLink ?? data.happLink ?? null
  }
  return data.happLink ?? data.v2rayLink ?? null
}

function pickCopyTarget(data: VpnConfigResponse | undefined): string {
  if (!data) return ''
  return data.happLink ?? data.v2rayLink ?? data.configUrl ?? ''
}

export function HomePage() {
  const navigate = useNavigate()
  const home = useHomeContext()
  const summary = useUserSummary()
  const startProvision = useStartProvision()
  const { refetch: fetchCurrentConfig, isFetching: isFetchingConfig } = useCurrentConfig({
    enabled: false,
  })
  const [isLaunchingHapp, setIsLaunchingHapp] = useState(false)
  const [readyCtaError, setReadyCtaError] = useState<ReadyCtaError | null>(null)
  const [provisionFailure, setProvisionFailure] = useState<ProvisionFailure | null>(null)
  const [userConnectIntent, setUserConnectIntent] = useState(false)
  const [overlay, setOverlay] = useState<ConnectOverlay | null>(null)
  const [copyToast, setCopyToast] = useState<'idle' | 'copied'>('idle')

  const { setBottomNavHidden } = useBottomNavControl()

  const launchLockRef = useRef(false)
  const prevStateRef = useRef<HomeScreenState | undefined>(undefined)

  const state = home.data?.state
  const view = state != null ? HOME_PRESENTATION[state] : null

  const jobId = home.data?.pendingProvisionJobId
  const provisionPolling = useProvisionPolling(state === 'Pending' && jobId ? jobId : null)

  useEffect(() => {
    if (provisionPolling.data?.status !== 'Failed') return
    setProvisionFailure({
      errorCode: provisionPolling.data.errorCode,
      errorMessage: provisionPolling.data.errorMessage,
    })
  }, [provisionPolling.data])

  useEffect(() => {
    if (state === 'Ready') {
      setProvisionFailure(null)
    }
  }, [state])

  useEffect(() => {
    setBottomNavHidden(overlay !== null)
    return () => setBottomNavHidden(false)
  }, [overlay, setBottomNavHidden])

  const platform = window.Telegram?.WebApp?.platform?.toLowerCase() ?? ''
  const isIos = platform.includes('ios') || platform.includes('iphone') || platform.includes('ipad')

  const runConnectLaunch = useCallback(async () => {
    if (launchLockRef.current) return
    launchLockRef.current = true
    try {
      const result = await fetchCurrentConfig()
      const data = result.data

      const deepLink = pickDeepLink(data, isIos)
      const configUrl = data?.configUrl ?? null
      const copyTarget = pickCopyTarget(data) || configUrl || ''

      if (!deepLink) {
        if (isIos && configUrl) {
          setOverlay({
            kind: 'addSubscription',
            deepLink: '',
            copyTarget,
          })
          return
        }
        setReadyCtaError('maintenance')
        return
      }

      setIsLaunchingHapp(true)
      try {
        const opened = await openHappLinkWithFallback(deepLink)
        if (opened) {
          markHappOpenedSuccessfully()
          setUserConnectIntent(false)
          setOverlay(null)
          void home.refetch()
          return
        }
        setUserConnectIntent(false)
        if (hasOpenedHappSuccessfullyInSession()) {
          setOverlay(null)
          return
        }
        setOverlay({ kind: 'needInstallClient', deepLink, copyTarget })
      } finally {
        setIsLaunchingHapp(false)
      }
    } catch (e: unknown) {
      if (isAxiosError(e) && e.response?.status === 401) {
        setReadyCtaError('session')
        return
      }
      setReadyCtaError('maintenance')
    } finally {
      launchLockRef.current = false
    }
  }, [fetchCurrentConfig, home, isIos])

  useEffect(() => {
    const prev = prevStateRef.current
    if (
      prev !== undefined &&
      prev !== 'Ready' &&
      state === 'Ready' &&
      userConnectIntent &&
      !overlay
    ) {
      void runConnectLaunch()
    }
    prevStateRef.current = state
  }, [state, userConnectIntent, overlay, runConnectLaunch])

  const handlePrimary = () => {
    if (state === 'NoSubscription') {
      navigate('/tariffs')
      return
    }
    if (state === 'NotStarted') {
      setUserConnectIntent(true)
      startProvision.mutate()
      return
    }
    if (state === 'Ready') {
      setUserConnectIntent(true)
      void runConnectLaunch()
    }
  }

  /** Спиннер и текст «Подключение…» только пока создаётся пир (джоба), не во время открытия HApp. */
  const showConnectLoading =
    state === 'Pending' || (state === 'NotStarted' && startProvision.isPending)

  const ctaDisabled =
    showConnectLoading ||
    (state === 'Ready' &&
      userConnectIntent &&
      !overlay &&
      (isFetchingConfig || isLaunchingHapp))

  const installUrl = getHappInstallUrl(window.Telegram?.WebApp?.platform)

  if (home.isPending) {
    return (
      <HomeShell
        footer={
          <div className="h-14" aria-hidden />
        }
      />
    )
  }

  if (home.isError) {
    return (
      <HomeShell
        footer={
          <PrimaryButton type="button" onClick={() => void home.refetch()}>
            Повторить
          </PrimaryButton>
        }
      />
    )
  }

  if (home.isSuccess && home.data != null && view == null) {
    return (
      <HomeShell
        footer={
          <PrimaryButton type="button" onClick={() => void home.refetch()}>
            Повторить
          </PrimaryButton>
        }
      />
    )
  }

  if (!view || state == null) {
    return null
  }

  if (provisionFailure) {
    const handleRetryProvision = () => {
      setProvisionFailure(null)
      setUserConnectIntent(true)
      startProvision.mutate()
    }

    return (
      <HomeShell
        statusText={view.subscriptionStatusLabel}
        footer={
          <div className="space-y-3">
            <PrimaryButton
              type="button"
              onClick={handleRetryProvision}
              disabled={startProvision.isPending}
            >
              Повторить подключение
            </PrimaryButton>
            <PrimaryButton
              type="button"
              className="border border-[var(--btn-primary-border)] bg-transparent text-[var(--foreground)] hover:bg-[var(--surface-2)]"
              onClick={() => setProvisionFailure(null)}
              disabled={startProvision.isPending}
            >
              Назад
            </PrimaryButton>
          </div>
        }
      />
    )
  }

  if (overlay?.kind === 'needInstallClient') {
    const { deepLink, copyTarget } = overlay
    const storeUrl = installUrl

    const handleInstalled = async () => {
      setIsLaunchingHapp(true)
      try {
        const opened = await openHappLinkWithFallback(deepLink)
        if (opened) {
          markHappOpenedSuccessfully()
          setOverlay(null)
          setUserConnectIntent(false)
          void home.refetch()
        }
      } finally {
        setIsLaunchingHapp(false)
      }
    }

    return (
      <ConnectStepShell
        title="Нужно установить приложение"
        subtitle="Установите HApp, затем вернитесь сюда и продолжите подключение."
        primary={
          <div className="space-y-2">
            <div className="flex justify-center">
              <button
                type="button"
                className="text-center text-[12px] font-medium text-[#a1a6aa] underline underline-offset-4 hover:text-[var(--foreground)] disabled:cursor-not-allowed disabled:opacity-60"
                disabled={!copyTarget}
                onClick={async () => {
                  const ok = await copyText(copyTarget)
                  if (ok) {
                    setCopyToast('copied')
                    window.setTimeout(() => setCopyToast('idle'), 2000)
                  }
                }}
              >
                {copyToast === 'copied'
                  ? 'Скопировано'
                  : 'Скопировать конфиг и вставить в клиент вручную'}
              </button>
            </div>
            <PrimaryButton
              type="button"
              onClick={() => window.open(storeUrl, '_blank', 'noopener,noreferrer')}
            >
              Установить клиент
            </PrimaryButton>
          </div>
        }
        secondary={
          <PrimaryButton
            type="button"
            className="border border-[var(--btn-primary-border)] bg-transparent text-[var(--foreground)] hover:bg-[var(--surface-2)]"
            onClick={() => void handleInstalled()}
            disabled={isLaunchingHapp}
          >
            Я установил
          </PrimaryButton>
        }
        tertiary={
          <PrimaryButton
            type="button"
            className="border border-[var(--btn-primary-border)] bg-transparent text-[var(--foreground)] hover:bg-[var(--surface-2)]"
            onClick={() => setOverlay(null)}
            disabled={isLaunchingHapp}
          >
            Назад
          </PrimaryButton>
        }
      />
    )
  }

  if (overlay?.kind === 'addSubscription') {
    const { deepLink, copyTarget } = overlay

    const openHapp = async () => {
      if (deepLink) {
        setIsLaunchingHapp(true)
        try {
          const opened = await openHappLinkWithFallback(deepLink)
          if (opened) {
            markHappOpenedSuccessfully()
            setOverlay(null)
            setUserConnectIntent(false)
            void home.refetch()
            return
          }
          setUserConnectIntent(false)
          if (hasOpenedHappSuccessfullyInSession()) {
            setOverlay(null)
            return
          }
          setOverlay({ kind: 'needInstallClient', deepLink, copyTarget })
        } finally {
          setIsLaunchingHapp(false)
        }
        return
      }
      window.open(installUrl, '_blank', 'noopener,noreferrer')
    }

    const handleCopy = async () => {
      const ok = await copyText(copyTarget)
      if (ok) {
        setCopyToast('copied')
        window.setTimeout(() => setCopyToast('idle'), 2000)
      }
    }

    return (
      <ConnectStepShell
        title="Добавьте подписку"
        subtitle="Откроем HApp и передадим конфигурацию."
        primary={
          <PrimaryButton type="button" onClick={() => void openHapp()} disabled={isLaunchingHapp}>
            Открыть HApp
          </PrimaryButton>
        }
        secondary={
          <PrimaryButton
            type="button"
            className="border border-[var(--btn-primary-border)] bg-transparent text-[var(--foreground)] hover:bg-[var(--surface-2)]"
            onClick={() => void handleCopy()}
          >
            {copyToast === 'copied' ? 'Скопировано' : 'Скопировать ссылку'}
          </PrimaryButton>
        }
      />
    )
  }

  if (readyCtaError) {
    const handleRetry = () => {
      setReadyCtaError(null)
      setUserConnectIntent(true)
      void home.refetch().then(() => runConnectLaunch())
    }

    return (
      <HomeShell
        statusText={view.subscriptionStatusLabel}
        footer={
          <div className="space-y-3">
            <PrimaryButton
              type="button"
              onClick={handleRetry}
              disabled={isFetchingConfig || isLaunchingHapp}
            >
              Повторить
            </PrimaryButton>
            <PrimaryButton
              type="button"
              className="border border-[var(--btn-primary-border)] bg-transparent text-[var(--foreground)] hover:bg-[var(--surface-2)]"
              onClick={() => setReadyCtaError(null)}
              disabled={isFetchingConfig || isLaunchingHapp}
            >
              Назад
            </PrimaryButton>
          </div>
        }
      />
    )
  }

  return (
    <HomeShell
      statusText={null}
      statusCard={
        summary.data ? (
          <SummaryCard
            subscriptionEndAtUtc={summary.data.subscriptionEndAtUtc}
            connectedDevices={summary.data.connectedDevices}
            maxDevices={summary.data.maxDevices}
          />
        ) : null
      }
      footer={
        view.primaryCtaLabel ? (
          <PrimaryButton type="button" disabled={ctaDisabled} onClick={handlePrimary}>
            {showConnectLoading ? (
              <span className="inline-flex items-center justify-center gap-2">
                <MiniSpinner />
                Подключение...
              </span>
            ) : (
              view.primaryCtaLabel
            )}
          </PrimaryButton>
        ) : showConnectLoading ? (
          <PrimaryButton type="button" disabled>
            <span className="inline-flex items-center justify-center gap-2">
              <MiniSpinner />
              Подключение...
            </span>
          </PrimaryButton>
        ) : (
          <div className="h-14" aria-hidden />
        )
      }
    />
  )
}
