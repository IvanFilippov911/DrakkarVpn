import { useNavigate } from 'react-router-dom'
import { useCurrentConfig, useHomeContext, useProvisionPolling } from '../../entities/user'
import { useStartProvision } from '../../features/user'
import { AppContainer, BrandBlock, PrimaryButton, StateCard, StatusIndicator } from '../../shared/ui'
import { IconBlocked, IconPending } from '../../shared/ui/icons/homeStateIcons'
import { HOME_PRESENTATION } from './homePresentation'

export function HomePage() {
  const navigate = useNavigate()
  const home = useHomeContext()
  const startProvision = useStartProvision()
  const { refetch: fetchCurrentConfig, isFetching: isFetchingConfig } = useCurrentConfig({
    enabled: false,
  })

  const state = home.data?.state
  const view = state != null ? HOME_PRESENTATION[state] : null

  const jobId = home.data?.pendingProvisionJobId
  useProvisionPolling(state === 'Pending' && jobId ? jobId : null)

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
      void fetchCurrentConfig().then((result) => {
        const link = result.data?.happLink
        if (link) {
          window.open(link, '_blank', 'noopener,noreferrer')
        }
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

  const ctaDisabled =
    (state === 'NotStarted' && startProvision.isPending) ||
    (state === 'Ready' && isFetchingConfig)

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
