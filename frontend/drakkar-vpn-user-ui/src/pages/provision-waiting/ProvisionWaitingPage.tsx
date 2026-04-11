import { useEffect, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useHomeContext, useProvisionPolling, useUserSummary } from '../../entities/user'
import { useStartProvision } from '../../features/user'
import { markConnectLaunchPending } from '../../shared/lib/connectLaunchPending'
import { consumeProvisionKickoff } from '../../shared/lib/provisionKickoff'
import { HomeBrandShell, PrimaryButton } from '../../shared/ui'
import { HomeSummaryCardSlot } from '../home/homeSummaryCard'

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

type ProvisionFailure = {
  errorCode?: string | null
  errorMessage?: string | null
}

export function ProvisionWaitingPage() {
  const navigate = useNavigate()
  const home = useHomeContext()
  const summary = useUserSummary()
  const startProvision = useStartProvision()

  const state = home.data?.state
  const jobId = home.data?.pendingProvisionJobId
  const provisionPolling = useProvisionPolling(state === 'Pending' && jobId ? jobId : null)

  const [provisionFailure, setProvisionFailure] = useState<ProvisionFailure | null>(null)
  const provisionKickoffDoneRef = useRef(false)

  useEffect(() => {
    void home.refetch()
  }, [home])

  /**
   * Старт provision только здесь: главная сразу navigate + флаг в sessionStorage.
   * Не вызываем mutate с Home — после unmount колбэк onSuccess теряется (второй тап).
   * sessionStorage переживает Strict Mode remount (флаг уже снят первым mount).
   */
  useEffect(() => {
    if (provisionKickoffDoneRef.current) return
    if (!consumeProvisionKickoff()) return
    provisionKickoffDoneRef.current = true
    startProvision.mutate()
  }, [startProvision])

  useEffect(() => {
    if (provisionPolling.data?.status !== 'Failed') return
    setProvisionFailure({
      errorCode: provisionPolling.data.errorCode,
      errorMessage: provisionPolling.data.errorMessage,
    })
  }, [provisionPolling.data])

  useEffect(() => {
    if (state !== 'Ready') return
    markConnectLaunchPending()
    navigate('/', { replace: true })
  }, [state, navigate])

  useEffect(() => {
    if (!home.isSuccess || state == null) return
    if (state === 'Blocked' || state === 'DeviceLimitExceeded' || state === 'NoSubscription') {
      navigate('/', { replace: true })
    }
  }, [home.isSuccess, state, navigate])

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

  if (provisionFailure) {
    const handleRetry = () => {
      setProvisionFailure(null)
      startProvision.mutate()
    }

    return (
      <HomeBrandShell
        footer={
          <div className="space-y-3">
            <PrimaryButton type="button" onClick={handleRetry}>
              Повторить подключение
            </PrimaryButton>
            <PrimaryButton
              type="button"
              className="border border-[var(--btn-primary-border)] bg-transparent text-[var(--foreground)] hover:bg-[var(--surface-2)]"
              onClick={() => navigate('/', { replace: true })}
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
        <HomeSummaryCardSlot homeContextReady={home.isSuccess && !summary.isPending} summary={summary} />
      }
      footer={
        <div className="mx-auto w-full max-w-sm">
          <PrimaryButton type="button" disabled className="shadow-[0_8px_30px_rgba(37,99,235,0.35)]">
            <span className="inline-flex items-center justify-center gap-2">
              <MiniSpinner />
              Подключение...
            </span>
          </PrimaryButton>
        </div>
      }
    />
  )
}
