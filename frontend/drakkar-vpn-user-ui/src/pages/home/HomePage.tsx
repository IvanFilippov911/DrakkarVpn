import { isAxiosError } from 'axios'
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useCurrentConfig, useHomeContext, useProvisionPolling } from '../../entities/user'
import { useStartProvision } from '../../features/user'
import { getHappInstallUrl, openHappLinkWithFallback } from '../../shared/lib/happLauncher'
import { AppContainer, BrandBlock, PrimaryButton, StateCard, StatusIndicator } from '../../shared/ui'
import { IconBlocked, IconPending } from '../../shared/ui/icons/homeStateIcons'
import { HOME_PRESENTATION } from './homePresentation'

type ReadyCtaError = 'maintenance' | 'session'
type IosClientFallback = {
  v2rayLink: string | null
  configRaw: string | null
  copied: boolean
}

export function HomePage() {
  const navigate = useNavigate()
  const home = useHomeContext()
  const startProvision = useStartProvision()
  const { refetch: fetchCurrentConfig, isFetching: isFetchingConfig } = useCurrentConfig({
    enabled: false,
  })
  const [happFallbackLink, setHappFallbackLink] = useState<string | null>(null)
  const [isLaunchingHapp, setIsLaunchingHapp] = useState(false)
  const [readyCtaError, setReadyCtaError] = useState<ReadyCtaError | null>(null)
  const [iosFallback, setIosFallback] = useState<IosClientFallback | null>(null)

  const state = home.data?.state
  const view = state != null ? HOME_PRESENTATION[state] : null

  const jobId = home.data?.pendingProvisionJobId
  useProvisionPolling(state === 'Pending' && jobId ? jobId : null)

  const platform = window.Telegram?.WebApp?.platform?.toLowerCase() ?? ''
  const isIos = platform.includes('ios') || platform.includes('iphone') || platform.includes('ipad')

  const launchHapp = async (link: string) => {
    setIsLaunchingHapp(true)
    try {
      const didOpen = await openHappLinkWithFallback(link)
      if (!didOpen) {
        if (isIos) {
          setIosFallback((prev) => ({
            v2rayLink: link,
            configRaw: prev?.configRaw ?? null,
            copied: prev?.copied ?? false,
          }))
        } else {
          setHappFallbackLink(link)
        }
        return
      }
      setHappFallbackLink(null)
      setIosFallback(null)
    } finally {
      setIsLaunchingHapp(false)
    }
  }

  const handlePrimary = () => {
    if (state === 'NoSubscription') {
      navigate('/tariffs')
      return
    }
    if (state === 'NotStarted') {
      startProvision.mutate()
      return
    }
    if (state === 'Ready') {
      void fetchCurrentConfig()
        .then((result) => {
          const link = isIos ? result.data?.v2rayLink : result.data?.happLink
          const configRaw = result.data?.configRaw ?? null
          if (link) {
            if (isIos) {
              setIosFallback((prev) => ({
                v2rayLink: link,
                configRaw,
                copied: prev?.copied ?? false,
              }))
            }
            void launchHapp(link)
            return
          }
          if (isIos && configRaw) {
            setIosFallback({ v2rayLink: null, configRaw, copied: false })
            return
          }
          setReadyCtaError('maintenance')
        })
        .catch((e: unknown) => {
          if (isAxiosError(e) && e.response?.status === 401) {
            setReadyCtaError('session')
            return
          }
          setReadyCtaError('maintenance')
        })
    }
  }

  if (home.isPending) {
    return (
      <AppContainer>
        <header className="px-6 pt-6 pb-4">
          <StatusIndicator tone="pending" label="подключение" />
        </header>
        <main className="flex flex-1 flex-col items-center justify-center px-6 pb-8">
          <div className="flex flex-1 flex-col items-center justify-center">
            <BrandBlock />
            <div className="h-20 shrink-0" aria-hidden />
            <StateCard
              variant="default"
              title="Загрузка"
              subtitle="Получаем состояние…"
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

  if (home.isError) {
    return (
      <AppContainer>
        <header className="px-6 pt-6 pb-4">
          <StatusIndicator tone="blocked" label="ошибка" />
        </header>
        <main className="flex flex-1 flex-col items-center justify-center px-6 pb-8">
          <div className="flex flex-1 flex-col items-center justify-center">
            <BrandBlock />
            <div className="h-20 shrink-0" aria-hidden />
            <StateCard
              variant="error"
              title="Не удалось загрузить"
              subtitle="Проверьте подключение и попробуйте снова."
              icon={<IconBlocked />}
            />
          </div>
        </main>
        <footer className="ui-safe-bottom px-6 pt-4">
          <PrimaryButton type="button" onClick={() => void home.refetch()}>
            Повторить
          </PrimaryButton>
        </footer>
      </AppContainer>
    )
  }

  if (home.isSuccess && home.data != null && view == null) {
    return (
      <AppContainer>
        <header className="px-6 pt-6 pb-4">
          <StatusIndicator tone="blocked" label="ошибка состояния" />
        </header>
        <main className="flex flex-1 flex-col items-center justify-center px-6 pb-8">
          <div className="flex flex-1 flex-col items-center justify-center">
            <BrandBlock />
            <div className="h-20 shrink-0" aria-hidden />
            <StateCard
              variant="error"
              title="Некорректный ответ сервера"
              subtitle="Обновите экран или попробуйте позже."
              icon={<IconBlocked />}
            />
          </div>
        </main>
        <footer className="ui-safe-bottom px-6 pt-4">
          <PrimaryButton type="button" onClick={() => void home.refetch()}>
            Повторить
          </PrimaryButton>
        </footer>
      </AppContainer>
    )
  }

  if (!view || state == null) {
    return null
  }

  if (iosFallback) {
    const installUrl = getHappInstallUrl(window.Telegram?.WebApp?.platform)

    const handleCopy = async () => {
      if (!iosFallback.configRaw) return
      try {
        await navigator.clipboard.writeText(iosFallback.configRaw)
        setIosFallback({ ...iosFallback, copied: true })
      } catch {
        // Clipboard may be unavailable in some webviews.
      }
    }

    return (
      <AppContainer>
        <header className="px-6 pt-6 pb-4">
          <StatusIndicator tone="inactive" label="настройка iOS" />
        </header>
        <main className="flex flex-1 flex-col items-center justify-center px-6 pb-8">
          <div className="flex flex-1 flex-col items-center justify-center">
            <BrandBlock />
            <div className="h-20 shrink-0" aria-hidden />
            <StateCard
              variant="warning"
              title="Установите v2RayTun"
              subtitle={
                iosFallback.copied
                  ? 'Конфиг скопирован. Откройте v2RayTun и добавьте конфигурацию из буфера.'
                  : 'Если ссылка не открывается, установите v2RayTun и добавьте конфигурацию вручную.'
              }
              icon={<IconPending />}
            />
          </div>
        </main>
        <footer className="ui-safe-bottom space-y-3 px-6 pt-4">
          <PrimaryButton
            type="button"
            onClick={() => window.open(installUrl, '_blank', 'noopener,noreferrer')}
          >
            Установить v2RayTun
          </PrimaryButton>
          {iosFallback.configRaw ? (
            <PrimaryButton
              type="button"
              className="bg-[var(--surface-2)] text-[var(--foreground)] hover:bg-[var(--surface-3)]"
              onClick={() => void handleCopy()}
            >
              {iosFallback.copied ? 'Скопировано' : 'Скопировать конфиг'}
            </PrimaryButton>
          ) : null}
          {iosFallback.v2rayLink ? (
            <PrimaryButton
              type="button"
              className="bg-[var(--surface-2)] text-[var(--foreground)] hover:bg-[var(--surface-3)]"
              onClick={() => void launchHapp(iosFallback.v2rayLink!)}
              disabled={isLaunchingHapp}
            >
              Я установил, открыть v2RayTun
            </PrimaryButton>
          ) : null}
          <PrimaryButton
            type="button"
            className="bg-[var(--surface-2)] text-[var(--foreground)] hover:bg-[var(--surface-3)]"
            onClick={() => setIosFallback(null)}
            disabled={isLaunchingHapp}
          >
            Назад
          </PrimaryButton>
        </footer>
      </AppContainer>
    )
  }

  if (readyCtaError) {
    const title =
      readyCtaError === 'session' ? 'Сессия истекла' : 'Временные технические работы'
    const subtitle =
      readyCtaError === 'session'
        ? 'Перезапустите приложение в Telegram и попробуйте снова.'
        : 'Попробуйте снова через 5 минут.'

    const handleRetry = () => {
      setReadyCtaError(null)

      void home.refetch()
      void fetchCurrentConfig()
        .then((result) => {
          const link = isIos ? result.data?.v2rayLink : result.data?.happLink
          const configRaw = result.data?.configRaw ?? null
          if (link) {
            if (isIos) {
              setIosFallback((prev) => ({
                v2rayLink: link,
                configRaw,
                copied: prev?.copied ?? false,
              }))
            }
            void launchHapp(link)
            return
          }
          if (isIos && configRaw) {
            setIosFallback({ v2rayLink: null, configRaw, copied: false })
            return
          }
          setReadyCtaError('maintenance')
        })
        .catch((e: unknown) => {
          if (isAxiosError(e) && e.response?.status === 401) {
            setReadyCtaError('session')
            return
          }
          setReadyCtaError('maintenance')
        })
    }

    return (
      <AppContainer>
        <header className="px-6 pt-6 pb-4">
          <StatusIndicator tone="blocked" label="ошибка" />
        </header>
        <main className="flex flex-1 flex-col items-center justify-center px-6 pb-8">
          <div className="flex flex-1 flex-col items-center justify-center">
            <BrandBlock />
            <div className="h-20 shrink-0" aria-hidden />
            <StateCard
              variant="error"
              title={title}
              subtitle={subtitle}
              icon={<IconBlocked />}
            />
          </div>
        </main>
        <footer className="ui-safe-bottom space-y-3 px-6 pt-4">
          <PrimaryButton
            type="button"
            onClick={handleRetry}
            disabled={isFetchingConfig || isLaunchingHapp}
          >
            Повторить
          </PrimaryButton>
          <PrimaryButton
            type="button"
            className="bg-[var(--surface-2)] text-[var(--foreground)] hover:bg-[var(--surface-3)]"
            onClick={() => setReadyCtaError(null)}
            disabled={isFetchingConfig || isLaunchingHapp}
          >
            Назад
          </PrimaryButton>
        </footer>
      </AppContainer>
    )
  }

  if (happFallbackLink) {
    return (
      <AppContainer>
        <header className="px-6 pt-6 pb-4">
          <StatusIndicator tone="inactive" label="требуется клиент" />
        </header>
        <main className="flex flex-1 flex-col items-center justify-center px-6 pb-8">
          <div className="flex flex-1 flex-col items-center justify-center">
            <BrandBlock />
            <div className="h-20 shrink-0" aria-hidden />
            <StateCard
              variant="warning"
              title="Установите HApp клиент"
              subtitle="Не удалось открыть VPN-клиент автоматически. Установите приложение и повторите попытку."
              icon={<IconPending />}
            />
          </div>
        </main>
        <footer className="ui-safe-bottom space-y-3 px-6 pt-4">
          <PrimaryButton
            type="button"
            onClick={() =>
              window.open(getHappInstallUrl(window.Telegram?.WebApp?.platform), '_blank', 'noopener,noreferrer')
            }
          >
            Установить клиент
          </PrimaryButton>
          <PrimaryButton
            type="button"
            className="bg-[var(--surface-2)] text-[var(--foreground)] hover:bg-[var(--surface-3)]"
            onClick={() => void launchHapp(happFallbackLink)}
            disabled={isLaunchingHapp}
          >
            Я установил, открыть VPN
          </PrimaryButton>
          <PrimaryButton
            type="button"
            className="bg-[var(--surface-2)] text-[var(--foreground)] hover:bg-[var(--surface-3)]"
            onClick={() => setHappFallbackLink(null)}
            disabled={isLaunchingHapp}
          >
            Назад
          </PrimaryButton>
        </footer>
      </AppContainer>
    )
  }

  const ctaDisabled =
    (state === 'NotStarted' && startProvision.isPending) ||
    (state === 'Ready' && (isFetchingConfig || isLaunchingHapp))

  return (
    <AppContainer>
      <header className="px-6 pt-6 pb-4">
        <StatusIndicator tone={toStatusTone(state)} label={toStatusLabel(state)} />
      </header>
      <main className="flex flex-1 flex-col items-center justify-center px-6 pb-8">
        <div className="flex flex-1 flex-col items-center justify-center">
          <BrandBlock />
          <div className="h-20 shrink-0" aria-hidden />
          <StateCard
            variant={view.variant}
            title={view.title}
            subtitle={view.subtitle}
            icon={view.icon}
            footer={view.footer}
          />
        </div>
      </main>
      <footer className="ui-safe-bottom px-6 pt-4">
        {view.primaryCtaLabel ? (
          <PrimaryButton
            type="button"
            disabled={ctaDisabled}
            onClick={handlePrimary}
          >
            {view.primaryCtaLabel}
          </PrimaryButton>
        ) : (
          <div className="h-14" aria-hidden />
        )}
      </footer>
    </AppContainer>
  )
}

function toStatusTone(state: keyof typeof HOME_PRESENTATION) {
  if (state === 'Ready') return 'ready'
  if (state === 'Pending') return 'pending'
  if (state === 'Blocked') return 'blocked'
  return 'inactive'
}

function toStatusLabel(state: keyof typeof HOME_PRESENTATION) {
  if (state === 'Ready') return 'готово'
  if (state === 'Pending') return 'подготовка'
  if (state === 'Blocked') return 'ограничено'
  if (state === 'NotStarted') return 'не запущено'
  return 'без подписки'
}
