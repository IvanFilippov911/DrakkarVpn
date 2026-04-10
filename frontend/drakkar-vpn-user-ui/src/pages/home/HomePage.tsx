import { differenceInCalendarDays } from 'date-fns'
import type { ReactNode } from 'react'
import { createContext, useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react'
import { useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { useBottomNavControl } from '../../context/bottomNavContext'
import type { HomeScreenState, VpnConfigResponse } from '../../entities/user'
import {
  userQueryKeys,
  useCurrentConfig,
  useHomeContext,
  useProvisionPolling,
  useUserSummary,
} from '../../entities/user'
import { useStartProvision } from '../../features/user'
import {
  beginVpnDeeplinkAttempt,
  getHappInstallUrl,
  getV2RayTunInstallUrl,
  hasOpenedHappSuccessfullyInSession,
  markHappOpenedSuccessfully,
  openVpnClientDeeplinkSync,
  pickPrimaryVpnDeeplink,
} from '../../shared/lib/happLauncher'
import drakkarMark from '../../assets/drakkar-mark.png'
import { AppContainer, BrandBlock, HomeBrandShell, PrimaryButton } from '../../shared/ui'
import { HOME_PRESENTATION } from './homePresentation'

type ReadyCtaError = 'maintenance' | 'session'
type ProvisionFailure = {
  errorCode?: string | null
  errorMessage?: string | null
}

type ConnectOverlay =
  | { kind: 'addSubscription'; happLink: string; v2rayLink: string; copyTarget: string }
  | { kind: 'needInstallClient'; happLink: string; v2rayLink: string; copyTarget: string }

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

type InstallClientSelection = 'happ' | 'v2ray'

const flowPrimaryLargeClass =
  '!px-10 !py-[1.35rem] !text-[1.0625rem] !font-semibold transition-[transform,box-shadow] duration-300 sm:!py-6 sm:!text-[1.1875rem]'

const flowSecondaryCompactClass =
  'w-full rounded-full bg-[rgba(255,255,255,0.07)] px-8 py-3.5 font-sans text-[15px] font-semibold text-[#9ca0a5] shadow-none transition-[transform,background-color,color] duration-300 hover:bg-[rgba(255,255,255,0.11)] hover:text-[#c5c9ce] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[rgba(255,255,255,0.12)] focus-visible:ring-offset-2 focus-visible:ring-offset-[#000] active:scale-[0.99] disabled:pointer-events-none disabled:opacity-40 sm:text-[0.97rem]'

type NeedInstallClientContextValue = {
  copyTarget: string
  onClientInstalled: () => void
  selected: InstallClientSelection | null
  setSelected: (v: InstallClientSelection | null) => void
  storeOpened: boolean
  copyState: 'idle' | 'copied'
  openSelectedStore: () => void
  handleCopyConfig: () => Promise<void>
}

const NeedInstallClientContext = createContext<NeedInstallClientContextValue | null>(null)

function useNeedInstallClientContext() {
  const ctx = useContext(NeedInstallClientContext)
  if (!ctx) {
    throw new Error('Need install client UI must be used within NeedInstallClientProvider')
  }
  return ctx
}

function NeedInstallClientProvider({
  happInstallUrl,
  v2rayTunInstallUrl,
  copyTarget,
  onClientInstalled,
  children,
}: {
  happInstallUrl: string
  v2rayTunInstallUrl: string
  copyTarget: string
  onClientInstalled: () => void
  children: ReactNode
}) {
  const [selected, setSelected] = useState<InstallClientSelection | null>(null)
  const [storeOpened, setStoreOpened] = useState(false)
  const [copyState, setCopyState] = useState<'idle' | 'copied'>('idle')

  const openSelectedStore = useCallback(() => {
    if (selected == null) return
    const url = selected === 'happ' ? happInstallUrl : v2rayTunInstallUrl
    window.location.href = url
    setStoreOpened(true)
  }, [happInstallUrl, selected, v2rayTunInstallUrl])

  const handleCopyConfig = useCallback(async () => {
    const ok = await copyText(copyTarget)
    if (ok) {
      setCopyState('copied')
      window.setTimeout(() => setCopyState('idle'), 2000)
    }
  }, [copyTarget])

  const value = useMemo(
    () => ({
      copyTarget,
      onClientInstalled,
      selected,
      setSelected,
      storeOpened,
      copyState,
      openSelectedStore,
      handleCopyConfig,
    }),
    [
      copyState,
      copyTarget,
      handleCopyConfig,
      onClientInstalled,
      openSelectedStore,
      selected,
      storeOpened,
    ],
  )

  return <NeedInstallClientContext.Provider value={value}>{children}</NeedInstallClientContext.Provider>
}

function NeedInstallClientCards() {
  const { selected, setSelected } = useNeedInstallClientContext()

  const cardBase =
    'flex min-h-[102px] min-w-0 flex-1 flex-col items-center justify-center gap-1 rounded-[14px] px-2 py-3.5 text-center transition-[background-color,border-color,box-shadow,transform] duration-200 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[rgba(0,122,255,0.35)] focus-visible:ring-offset-2 focus-visible:ring-offset-[#000] disabled:pointer-events-none disabled:opacity-40'
  const cardIdle =
    'border border-[rgba(255,255,255,0.1)] bg-[rgba(255,255,255,0.04)] hover:border-[rgba(255,255,255,0.14)] hover:bg-[rgba(255,255,255,0.07)] active:scale-[0.98]'
  const cardActive =
    'border border-[rgba(0,122,255,0.5)] bg-[rgba(0,122,255,0.15)] shadow-[inset_0_1px_0_rgba(255,255,255,0.05)]'

  return (
    <div className="flex gap-3" role="radiogroup" aria-label="Клиент для установки">
      <button
        type="button"
        role="radio"
        aria-checked={selected === 'happ'}
        onClick={() => setSelected('happ')}
        className={`${cardBase} ${selected === 'happ' ? cardActive : cardIdle}`}
      >
        <span className="font-sans text-[15px] font-semibold tracking-[-0.02em] text-[var(--foreground)]">
          Happ
        </span>
        <span className="max-w-[9rem] text-[11px] font-medium leading-snug text-[#9ea3a8]">
          Рекомендуется
        </span>
      </button>
      <button
        type="button"
        role="radio"
        aria-checked={selected === 'v2ray'}
        onClick={() => setSelected('v2ray')}
        className={`${cardBase} ${selected === 'v2ray' ? cardActive : cardIdle}`}
      >
        <span className="font-sans text-[15px] font-semibold tracking-[-0.02em] text-[var(--foreground)]">
          v2RayTun
        </span>
        <span className="max-w-[9rem] text-[11px] font-medium leading-snug text-[#9ea3a8]">
          Если первый недоступен
        </span>
      </button>
    </div>
  )
}

function NeedInstallClientPanel() {
  const {
    selected,
    storeOpened,
    copyTarget,
    copyState,
    onClientInstalled,
    openSelectedStore,
    handleCopyConfig,
  } = useNeedInstallClientContext()

  return (
    <div className="rounded-[14px] bg-[rgba(255,255,255,0.03)] px-4 py-4">
      <div className="flex flex-col gap-3">
        <div className="flex flex-col gap-2.5">
          {!storeOpened ? (
            <>
              <PrimaryButton
                type="button"
                disabled={selected == null}
                onClick={openSelectedStore}
                className={flowPrimaryLargeClass}
              >
                Скачать
              </PrimaryButton>
              <button
                type="button"
                onClick={onClientInstalled}
                className={flowSecondaryCompactClass}
              >
                Продолжить подключение
              </button>
            </>
          ) : (
            <>
              <button
                type="button"
                disabled={selected == null}
                onClick={openSelectedStore}
                className={flowSecondaryCompactClass}
              >
                Скачать
              </button>
              <PrimaryButton
                type="button"
                onClick={onClientInstalled}
                className={flowPrimaryLargeClass}
              >
                Продолжить подключение
              </PrimaryButton>
            </>
          )}
        </div>

        <div className="h-px w-full bg-[rgba(255,255,255,0.06)]" aria-hidden />

        <div className="flex flex-col items-center gap-3 text-center">
          <p className="font-sans text-[14px] font-semibold tracking-[-0.01em] text-[var(--foreground)]">
            Ручное подключение
          </p>
          <button
            type="button"
            disabled={!copyTarget}
            onClick={() => void handleCopyConfig()}
            className="inline-flex h-11 w-full max-w-[20rem] items-center justify-center gap-2.5 whitespace-nowrap rounded-full bg-[rgba(255,255,255,0.07)] px-5 py-2.5 text-[14px] font-semibold text-[#d8d4cc] transition-[background-color,transform] hover:bg-[rgba(255,255,255,0.11)] active:scale-[0.99] disabled:pointer-events-none disabled:opacity-40"
          >
            <svg
              className="h-[18px] w-[18px] shrink-0 opacity-90"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
              aria-hidden
            >
              <rect x="9" y="9" width="13" height="13" rx="2" ry="2" />
              <path d="M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1" />
            </svg>
            <span className="min-w-[10.5rem] text-center">
              {copyState === 'copied' ? 'Скопировано' : 'Скопировать конфиг'}
            </span>
          </button>
        </div>
      </div>
    </div>
  )
}

function ConnectStepShell({
  title,
  subtitle,
  primary,
  secondary,
  tertiary,
  topBarLeading,
  /**
   * Hero сверху, действия снизу; свободная высота между ними, без скролла.
   */
  stackActionsBelowHero = false,
  /** Три зоны: hero — primary — secondary (равномерные промежутки по высоте). */
  stackActionsSplit = false,
}: {
  title: string
  subtitle: string
  primary: ReactNode
  secondary?: ReactNode
  tertiary?: ReactNode
  /** Слабый контроль «назад» над контентом (не трогает лого/заголовки в hero). */
  topBarLeading?: ReactNode
  stackActionsBelowHero?: boolean
  stackActionsSplit?: boolean
}) {
  const heroBlock = (
    <div className="mx-auto w-full max-w-sm text-center">
      <div className="mb-4 flex justify-center" aria-hidden>
        <img
          src={drakkarMark}
          alt=""
          className="h-[5.5rem] w-auto max-w-[12rem] object-contain select-none"
          draggable={false}
        />
      </div>
      <h2 className="font-sans text-xl font-bold leading-tight tracking-[-0.02em] text-[#e8e2d6]">{title}</h2>
      <p className="mt-2 text-base font-normal leading-normal text-[#8b8f94]">{subtitle}</p>
    </div>
  )

  const footerBlock = (
    <div className="mx-auto w-full max-w-sm shrink-0 space-y-3">
      {primary}
      {secondary}
      {tertiary}
    </div>
  )

  return (
    <AppContainer>
      <div className="relative flex min-h-0 w-full flex-1 flex-col overflow-hidden px-6 pb-[max(3rem,env(safe-area-inset-bottom))] pt-[max(1.25rem,env(safe-area-inset-top))]">
        {topBarLeading ? <div className="mb-2 shrink-0">{topBarLeading}</div> : null}
        {stackActionsBelowHero ? (
          stackActionsSplit ? (
            <div className="flex min-h-0 flex-1 flex-col justify-evenly overflow-hidden">
              <div className="shrink-0">{heroBlock}</div>
              <div className="mx-auto w-full max-w-sm shrink-0">{primary}</div>
              <div className="mx-auto w-full max-w-sm shrink-0">
                {secondary}
                {tertiary ? <div className="mt-3">{tertiary}</div> : null}
              </div>
            </div>
          ) : (
            <div className="flex min-h-0 flex-1 flex-col justify-evenly overflow-hidden">
              <div className="shrink-0">{heroBlock}</div>
              <div className="shrink-0">{footerBlock}</div>
            </div>
          )
        ) : (
          <>
            <div className="flex min-h-0 flex-1 flex-col items-center justify-center overflow-hidden text-center">
              {heroBlock}
            </div>
            <div className="mt-2">{footerBlock}</div>
          </>
        )}
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
    <div className="mx-auto w-full max-w-sm rounded-2xl bg-[rgba(255,255,255,0.04)] p-4 shadow-[0_10px_40px_rgba(37,99,235,0.08)]">
      <div className="flex items-center gap-3">
        <span className="shrink-0 leading-none" aria-hidden>
          {hasSubscription ? (
            <svg
              className="block h-5 w-5 text-[#22c55e]"
              viewBox="0 0 24 24"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M20 6L9 17l-5-5"
                stroke="currentColor"
                strokeWidth="2.25"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
            </svg>
          ) : (
            <svg
              className="block h-5 w-5 text-[#ef4444]"
              viewBox="0 0 24 24"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M18 6L6 18M6 6l12 12"
                stroke="currentColor"
                strokeWidth="2.25"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
            </svg>
          )}
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

async function copyText(text: string): Promise<boolean> {
  if (!text) return false
  try {
    await navigator.clipboard.writeText(text)
    return true
  } catch {
    return false
  }
}

function launchLinksFromConfig(data: VpnConfigResponse | undefined): { happ: string; v2ray: string } {
  const happ = (data?.happLink ?? '').trim()
  const v2ray = (data?.v2rayLink ?? '').trim()
  return { happ, v2ray }
}

function pickCopyTarget(data: VpnConfigResponse | undefined): string {
  if (!data) return ''
  return data.happLink ?? data.v2rayLink ?? data.configUrl ?? ''
}

export function HomePage() {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const home = useHomeContext()
  const summary = useUserSummary()
  const startProvision = useStartProvision()

  const state = home.data?.state
  const {
    data: vpnConfig,
    isPending: isVpnConfigPending,
    isFetching: isFetchingConfig,
  } = useCurrentConfig({
    enabled: state === 'Ready',
  })

  const [readyCtaError, setReadyCtaError] = useState<ReadyCtaError | null>(null)
  const [provisionFailure, setProvisionFailure] = useState<ProvisionFailure | null>(null)
  const [userConnectIntent, setUserConnectIntent] = useState(false)
  const [overlay, setOverlay] = useState<ConnectOverlay | null>(null)
  const [copyToast, setCopyToast] = useState<'idle' | 'copied'>('idle')

  const { setBottomNavHidden } = useBottomNavControl()

  const launchLockRef = useRef(false)
  const prevStateRef = useRef<HomeScreenState | undefined>(undefined)

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

  const runConnectLaunch = useCallback(() => {
    if (launchLockRef.current) return

    const data =
      queryClient.getQueryData<VpnConfigResponse>(userQueryKeys.currentConfig) ?? vpnConfig ?? undefined

    const { happ, v2ray } = launchLinksFromConfig(data)
    const configUrl = data?.configUrl ?? null
    const copyTarget = pickCopyTarget(data) || configUrl || ''

    if (!happ && !v2ray) {
      if (isVpnConfigPending && !data) {
        return
      }
      if (isIos && configUrl) {
        setOverlay({
          kind: 'addSubscription',
          happLink: '',
          v2rayLink: '',
          copyTarget,
        })
        return
      }
      setReadyCtaError('maintenance')
      return
    }

    const url = pickPrimaryVpnDeeplink(happ, v2ray)
    if (!url) {
      setReadyCtaError('maintenance')
      return
    }

    launchLockRef.current = true

    beginVpnDeeplinkAttempt({
      onLikelyOpened: () => {
        markHappOpenedSuccessfully()
        setUserConnectIntent(false)
        setOverlay(null)
        void home.refetch()
        launchLockRef.current = false
      },
      onStillVisible: () => {
        setUserConnectIntent(false)
        if (hasOpenedHappSuccessfullyInSession()) {
          setOverlay(null)
        } else {
          setOverlay({ kind: 'needInstallClient', happLink: happ, v2rayLink: v2ray, copyTarget })
        }
        launchLockRef.current = false
      },
    })

    openVpnClientDeeplinkSync(happ, v2ray)
  }, [home, isIos, isVpnConfigPending, queryClient, vpnConfig])

  useEffect(() => {
    const prev = prevStateRef.current
    if (
      prev !== undefined &&
      prev !== 'Ready' &&
      state === 'Ready' &&
      userConnectIntent &&
      !overlay
    ) {
      runConnectLaunch()
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
      runConnectLaunch()
    }
  }

  /** Спиннер и текст «Подключение…» только пока создаётся пир (джоба), не во время открытия HApp. */
  const showConnectLoading =
    state === 'Pending' || (state === 'NotStarted' && startProvision.isPending)

  const ctaDisabled =
    showConnectLoading ||
    (state === 'Ready' &&
      !overlay &&
      isVpnConfigPending &&
      !vpnConfig &&
      !queryClient.getQueryData<VpnConfigResponse>(userQueryKeys.currentConfig)) ||
    (state === 'Ready' && userConnectIntent && !overlay && isFetchingConfig)

  const tgPlatform = window.Telegram?.WebApp?.platform
  const happInstallUrl = getHappInstallUrl(tgPlatform)
  const v2rayTunInstallUrl = getV2RayTunInstallUrl(tgPlatform)

  if (home.isPending) {
    return (
      <HomeBrandShell
        brand={<BrandBlock />}
        statusCard={
          <div
            className="mx-auto w-full max-w-sm rounded-2xl bg-[rgba(255,255,255,0.04)] p-4 shadow-[0_10px_40px_rgba(37,99,235,0.08)]"
            aria-hidden
          >
            <div className="h-5 w-[72%] max-w-[16rem] rounded bg-[rgba(255,255,255,0.07)]" />
            <div className="mt-2.5 h-4 w-[48%] max-w-[10rem] rounded bg-[rgba(255,255,255,0.05)]" />
          </div>
        }
        footer={<div className="mx-auto h-14 w-full max-w-sm rounded-full bg-[rgba(255,255,255,0.06)]" aria-hidden />}
      />
    )
  }

  if (home.isError) {
    return (
      <HomeBrandShell
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
      <HomeBrandShell
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
      <HomeBrandShell
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
    const { happLink, v2rayLink, copyTarget } = overlay

    const handleInstalled = () => {
      beginVpnDeeplinkAttempt({
        onLikelyOpened: () => {
          markHappOpenedSuccessfully()
          setOverlay(null)
          setUserConnectIntent(false)
          void home.refetch()
        },
      })
      openVpnClientDeeplinkSync(happLink, v2rayLink)
    }

    return (
      <NeedInstallClientProvider
        happInstallUrl={happInstallUrl}
        v2rayTunInstallUrl={v2rayTunInstallUrl}
        copyTarget={copyTarget}
        onClientInstalled={handleInstalled}
      >
        <ConnectStepShell
          title="Установите VPN-клиент"
          subtitle='Установите один из клиентов, затем нажмите «Продолжить подключение»'
          stackActionsBelowHero
          stackActionsSplit
          topBarLeading={
            <button
              type="button"
              onClick={() => setOverlay(null)}
              className="-ml-1 flex items-center gap-1.5 py-1.5 pr-3 text-left text-[14px] font-medium text-[#8b8f94] transition-colors hover:text-[#d8d4cc] disabled:pointer-events-none disabled:opacity-45"
            >
              <svg
                className="h-[18px] w-[18px] shrink-0 opacity-90"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="2"
                strokeLinecap="round"
                strokeLinejoin="round"
                aria-hidden
              >
                <path d="M15 18l-6-6 6-6" />
              </svg>
              Назад
            </button>
          }
          primary={<NeedInstallClientCards />}
          secondary={<NeedInstallClientPanel />}
        />
      </NeedInstallClientProvider>
    )
  }

  if (overlay?.kind === 'addSubscription') {
    const { happLink, v2rayLink, copyTarget } = overlay

    const openHapp = () => {
      if (happLink || v2rayLink) {
        beginVpnDeeplinkAttempt({
          onLikelyOpened: () => {
            markHappOpenedSuccessfully()
            setOverlay(null)
            setUserConnectIntent(false)
            void home.refetch()
          },
          onStillVisible: () => {
            setUserConnectIntent(false)
            if (hasOpenedHappSuccessfullyInSession()) {
              setOverlay(null)
              return
            }
            setOverlay({ kind: 'needInstallClient', happLink, v2rayLink, copyTarget })
          },
        })
        openVpnClientDeeplinkSync(happLink, v2rayLink)
        return
      }
      window.location.href = happInstallUrl
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
          <PrimaryButton type="button" onClick={openHapp}>
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
      <HomeBrandShell
        statusText={view.subscriptionStatusLabel}
        footer={
          <div className="space-y-3">
            <PrimaryButton
              type="button"
              onClick={handleRetry}
              disabled={isFetchingConfig}
            >
              Повторить
            </PrimaryButton>
            <PrimaryButton
              type="button"
              className="border border-[var(--btn-primary-border)] bg-transparent text-[var(--foreground)] hover:bg-[var(--surface-2)]"
              onClick={() => setReadyCtaError(null)}
              disabled={isFetchingConfig}
            >
              Назад
            </PrimaryButton>
          </div>
        }
      />
    )
  }

  return (
    <HomeBrandShell
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
          <PrimaryButton
            type="button"
            disabled={ctaDisabled}
            onClick={handlePrimary}
            className="shadow-[0_8px_30px_rgba(37,99,235,0.35)]"
          >
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
