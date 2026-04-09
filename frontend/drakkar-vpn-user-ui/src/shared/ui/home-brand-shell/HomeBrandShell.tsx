import type { ReactNode } from 'react'
import { AppContainer } from '../app-container/AppContainer'
import { BrandBlock } from '../brand-block/BrandBlock'

/** Невидимая строка той же высоты, что «@username» над CTA — выравнивание с поддержкой. */
function FooterRailSpacer() {
  return (
    <div className="w-full text-center" aria-hidden>
      <p className="invisible pointer-events-none select-none text-[14px] font-normal leading-normal">
        @drakkar_network_help
      </p>
    </div>
  )
}

/** Фантом карточки статуса (без внешнего mb — его даёт оболочка). */
export function HomeFooterStatusCardSpace() {
  return (
    <div className="invisible pointer-events-none select-none mx-auto w-full max-w-sm rounded-2xl bg-[rgba(255,255,255,0.04)] p-4">
      <div className="flex items-center gap-3">
        <span className="h-5 w-5 shrink-0" />
        <div className="min-w-0">
          <p className="truncate font-sans text-[15px] font-semibold text-[var(--foreground)]">
            Подписка активна · 888 дней
          </p>
          <p className="mt-0.5 text-[13px] font-normal text-[#8b8f94]">Устройства: 99 из 999</p>
        </div>
      </div>
    </div>
  )
}

export type HomeBrandShellProps = {
  brand?: ReactNode
  statusText?: string | null
  statusCard?: ReactNode
  /** Между карточкой и CTA, если нет statusText (напр. @username на поддержке). */
  footerMiddle?: ReactNode
  footer: ReactNode
}

export function HomeBrandShell({
  brand = <BrandBlock />,
  statusText,
  statusCard,
  footerMiddle,
  footer,
}: HomeBrandShellProps) {
  const reserveMiddleUnderCard = statusCard != null && statusText == null

  return (
    <AppContainer>
      <main className="flex min-h-0 flex-1 flex-col items-center justify-center px-6 pb-1 pt-[max(1.25rem,env(safe-area-inset-top))]">
        <div className="flex w-full min-w-0 min-h-0 flex-1 flex-col items-center justify-center overflow-hidden">
          {brand}
          <div className="h-2 shrink-0" aria-hidden />
        </div>
      </main>
      <footer className="w-full shrink-0 px-6 pt-0">
        {statusCard ? <div className="mb-2">{statusCard}</div> : null}
        {statusText ? (
          <p className="mb-1.5 w-full text-center text-[14px] font-normal leading-normal text-[#a1a6aa]">
            {statusText}
          </p>
        ) : reserveMiddleUnderCard ? (
          <div className="mb-1.5">{footerMiddle ?? <FooterRailSpacer />}</div>
        ) : null}
        {footer}
      </footer>
    </AppContainer>
  )
}
