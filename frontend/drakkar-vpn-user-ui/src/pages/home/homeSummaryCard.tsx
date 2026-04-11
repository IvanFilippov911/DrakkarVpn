import { differenceInCalendarDays } from 'date-fns'
import { useUserSummary } from '../../entities/user'

function pluralizeDaysRu(n: number): string {
  const abs = Math.abs(n)
  const mod10 = abs % 10
  const mod100 = abs % 100
  if (mod100 >= 11 && mod100 <= 14) return 'дней'
  if (mod10 === 1) return 'день'
  if (mod10 >= 2 && mod10 <= 4) return 'дня'
  return 'дней'
}

const HOME_STATUS_CARD_SHELL =
  'mx-auto w-full max-w-sm rounded-2xl bg-[rgba(255,255,255,0.04)] p-4 shadow-[0_10px_40px_rgba(37,99,235,0.08)]'

function SummaryCardSkeleton() {
  return (
    <div className={HOME_STATUS_CARD_SHELL} aria-busy aria-hidden>
      <div className="flex items-center gap-3">
        <div className="h-5 w-5 shrink-0 rounded-full bg-[rgba(255,255,255,0.06)]" />
        <div className="min-w-0 flex-1 space-y-2">
          <div className="h-[17px] w-[72%] max-w-[16rem] rounded bg-[rgba(255,255,255,0.07)]" />
          <div className="h-[14px] w-[48%] max-w-[10rem] rounded bg-[rgba(255,255,255,0.05)]" />
        </div>
      </div>
    </div>
  )
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
    <div className={HOME_STATUS_CARD_SHELL}>
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

export function HomeSummaryCardSlot({
  homeContextReady,
  summary,
}: {
  homeContextReady: boolean
  summary: ReturnType<typeof useUserSummary>
}) {
  if (!homeContextReady) {
    return <SummaryCardSkeleton />
  }
  if (summary.data) {
    return (
      <SummaryCard
        subscriptionEndAtUtc={summary.data.subscriptionEndAtUtc}
        connectedDevices={summary.data.connectedDevices}
        maxDevices={summary.data.maxDevices}
      />
    )
  }
  return <SummaryCardSkeleton />
}
