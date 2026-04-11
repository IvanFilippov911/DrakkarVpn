import type { ReactNode } from 'react'
import { createContext, useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react'
import { createPortal } from 'react-dom'
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
  consumeConnectLaunchPending,
  peekConnectLaunchPending,
} from '../../shared/lib/connectLaunchPending'
import { markProvisionKickoff } from '../../shared/lib/provisionKickoff'
import {
  beginSequentialVpnDeeplinkAttempt,
  getHappInstallUrl,
  getV2RayTunInstallUrl,
  markHappOpenedSuccessfully,
  pickPrimaryVpnDeeplink,
} from '../../shared/lib/happLauncher'
import drakkarMark from '../../assets/drakkar-mark.png'
import { AppContainer, HomeBrandShell, PrimaryButton } from '../../shared/ui'
import { HomeSummaryCardSlot } from './homeSummaryCard'
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

function HomePrimaryCtaPlaceholder() {
  return (
    <div
      className="mx-auto flex min-h-[3.75rem] w-full items-center justify-center rounded-full border border-transparent bg-[rgba(255,255,255,0.06)] px-10 py-5"
      aria-hidden
    >
      <div className="h-4 w-32 max-w-[60%] rounded bg-[rgba(255,255,255,0.08)]" />
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

function ConnectClientProbePortal() {
  return createPortal(
    <div
      role="status"
      aria-live="polite"
      aria-busy="true"
      className="fixed inset-0 z-[70] flex flex-col items-center justify-center bg-[rgba(0,0,0,0.86)] px-6 pt-[max(2rem,env(safe-area-inset-top))] pb-[max(2rem,env(safe-area-inset-bottom))]"
    >
      <p className="max-w-[20rem] text-center font-sans text-[17px] font-medium leading-relaxed text-[var(--foreground)]">
        Проверяем наличие клиента
        <span className="inline-flex items-baseline gap-[0.12em] pl-0.5" aria-hidden>
          <span className="connect-client-ellipsis-dot" />
          <span className="connect-client-ellipsis-dot" />
          <span className="connect-client-ellipsis-dot" />
        </span>
      </p>
    </div>,
    document.body,
  )
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
  const [connectClientProbeOpen, setConnectClientProbeOpen] = useState(false)

  const { setBottomNavHidden } = useBottomNavControl()

  const launchLockRef = useRef(false)
  const prevStateRef = useRef<HomeScreenState | undefined>(undefined)
  /**
   * После успешного auto-connect по флагу — блокируем повторный runConnectLaunch при refetch.
   * Сбрасываем, когда state уходит с Ready (следующий цикл provisioning).
   */
  const postProvisionAutoConnectDoneRef = useRef(false)

  useEffect(() => {
    if (state !== 'Ready') {
      postProvisionAutoConnectDoneRef.current = false
    }
  }, [state])

  const view = state != null ? HOME_PRESENTATION[state] : null

  const jobId = home.data?.pendingProvisionJobId
  const provisionPolling = useProvisionPolling(state === 'Pending' && jobId ? jobId : null)

  useEffect(() => {
    if (provisionPolling.data?.status !== 'Failed') return
    const snap = provisionPolling.data
    queueMicrotask(() => {
      setProvisionFailure({
        errorCode: snap.errorCode,
        errorMessage: snap.errorMessage,
      })
    })
  }, [provisionPolling.data])

  useEffect(() => {
    if (state === 'Ready') {
      queueMicrotask(() => setProvisionFailure(null))
    }
  }, [state])

  useEffect(() => {
    if (state === 'Ready' && (startProvision.isSuccess || startProvision.isError)) {
      startProvision.reset()
    }
  }, [state, startProvision])

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
        setUserConnectIntent(false)
        return
      }
      if (isIos && configUrl) {
        setUserConnectIntent(false)
        setOverlay({
          kind: 'addSubscription',
          happLink: '',
          v2rayLink: '',
          copyTarget,
        })
        return
      }
      setUserConnectIntent(false)
      setReadyCtaError('maintenance')
      return
    }

    const url = pickPrimaryVpnDeeplink(happ, v2ray)
    if (!url) {
      setUserConnectIntent(false)
      setReadyCtaError('maintenance')
      return
    }

    launchLockRef.current = true
    setConnectClientProbeOpen(true)
    const failsafeUnlock = window.setTimeout(() => {
      launchLockRef.current = false
      setConnectClientProbeOpen(false)
    }, 12_000)

    beginSequentialVpnDeeplinkAttempt(happ, v2ray, {
      onLikelyOpened: () => {
        window.clearTimeout(failsafeUnlock)
        setConnectClientProbeOpen(false)
        markHappOpenedSuccessfully()
        setUserConnectIntent(false)
        setOverlay(null)
        void home.refetch()
        launchLockRef.current = false
      },
      onStillVisible: () => {
        window.clearTimeout(failsafeUnlock)
        setConnectClientProbeOpen(false)
        setUserConnectIntent(false)
        setOverlay({ kind: 'needInstallClient', happLink: happ, v2rayLink: v2ray, copyTarget })
        launchLockRef.current = false
      },
    })
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

  /**
   * После provisioning: с /provision выставлен markConnectLaunchPending → здесь один раз
   * продолжаем connect (runConnectLaunch), без второго тапа. userConnectIntent на главной
   * теряется при unmount — флаг в sessionStorage переносит намерение.
   */
  useEffect(() => {
    if (state !== 'Ready' || overlay != null) return
    if (postProvisionAutoConnectDoneRef.current) return
    if (!peekConnectLaunchPending()) return

    const data =
      queryClient.getQueryData<VpnConfigResponse>(userQueryKeys.currentConfig) ?? vpnConfig ?? undefined
    if (!data && isVpnConfigPending) return

    if (!consumeConnectLaunchPending()) return
    postProvisionAutoConnectDoneRef.current = true
    runConnectLaunch()
    setUserConnectIntent(true)
  }, [state, overlay, queryClient, vpnConfig, isVpnConfigPending, runConnectLaunch])

  const handlePrimary = () => {
    if (state === 'NoSubscription') {
      navigate('/tariffs')
      return
    }
    if (state === 'NotStarted') {
      setUserConnectIntent(true)
      markProvisionKickoff()
      navigate('/provision')
      return
    }
    if (state === 'Ready') {
      void consumeConnectLaunchPending()
      // Deeplink должен вызываться до setState: на iOS жест пользователя не должен «обрываться» ре-рендером.
      runConnectLaunch()
      setUserConnectIntent(true)
    }
  }

  /** Спиннер на главной только для Pending; старт provision уходит на /provision с первого тапа. */
  const showConnectLoading = state === 'Pending'

  const mainStateReady = home.isSuccess && state != null && view != null

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

  const summaryStillLoading = home.isSuccess && summary.isPending
  /**
   * На NotStarted никогда не подменяем CTA плейсхолдером — иначе первый тап попадает в «пустышку».
   * Для Pending тоже показываем реальный футер (спиннер), а не скелетон.
   */
  const deferHomeFooterForSyncPaint =
    state === 'NotStarted' || state === 'Pending'
      ? false
      : home.isPending || summaryStillLoading

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

  if (!home.isPending && home.isSuccess && (!view || state == null)) {
    return null
  }

  if (mainStateReady && provisionFailure) {
    const handleRetryProvision = () => {
      setProvisionFailure(null)
      setUserConnectIntent(true)
      markProvisionKickoff()
      navigate('/provision')
    }

    return (
      <HomeBrandShell
        statusText={view.subscriptionStatusLabel}
        footer={
          <div className="space-y-3">
            <PrimaryButton type="button" onClick={handleRetryProvision}>
              Повторить подключение
            </PrimaryButton>
            <PrimaryButton
              type="button"
              className="border border-[var(--btn-primary-border)] bg-transparent text-[var(--foreground)] hover:bg-[var(--surface-2)]"
              onClick={() => setProvisionFailure(null)}
            >
              Назад
            </PrimaryButton>
          </div>
        }
      />
    )
  }

  if (mainStateReady && overlay?.kind === 'needInstallClient') {
    const { happLink, v2rayLink, copyTarget } = overlay

    const handleInstalled = () => {
      beginSequentialVpnDeeplinkAttempt(happLink, v2rayLink, {
        onLikelyOpened: () => {
          markHappOpenedSuccessfully()
          setOverlay(null)
          setUserConnectIntent(false)
          void home.refetch()
        },
      })
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

  if (mainStateReady && overlay?.kind === 'addSubscription') {
    const { happLink, v2rayLink, copyTarget } = overlay

    const openHapp = () => {
      if (happLink || v2rayLink) {
        beginSequentialVpnDeeplinkAttempt(happLink, v2rayLink, {
          onLikelyOpened: () => {
            markHappOpenedSuccessfully()
            setOverlay(null)
            setUserConnectIntent(false)
            void home.refetch()
          },
          onStillVisible: () => {
            setUserConnectIntent(false)
            setOverlay({ kind: 'needInstallClient', happLink, v2rayLink, copyTarget })
          },
        })
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

  if (mainStateReady && readyCtaError) {
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

  const showMainHomeChrome = home.isPending || mainStateReady
  if (!showMainHomeChrome) {
    return null
  }

  return (
    <>
      {connectClientProbeOpen ? <ConnectClientProbePortal /> : null}
      <HomeBrandShell
      statusText={null}
      statusCard={
        <HomeSummaryCardSlot homeContextReady={home.isSuccess && !summary.isPending} summary={summary} />
      }
      footer={
        deferHomeFooterForSyncPaint ? (
          <HomePrimaryCtaPlaceholder />
        ) : view!.primaryCtaLabel ? (
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
              view!.primaryCtaLabel
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
    </>
  )
}
