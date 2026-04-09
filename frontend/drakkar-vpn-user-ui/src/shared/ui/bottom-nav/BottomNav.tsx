import { useQueryClient } from '@tanstack/react-query'
import { useEffect } from 'react'
import { NavLink } from 'react-router-dom'
import { getTariffs, getUserSummary } from '../../../entities/user/api'
import { userQueryKeys } from '../../../entities/user/queryKeys'

const linkClass =
  [
    'flex min-h-0 min-w-0 flex-1 basis-0 items-center justify-center',
    'no-underline outline-none touch-manipulation',
    'transition-transform duration-100 ease-out active:scale-95',
    'focus-visible:rounded-[18px] focus-visible:ring-2 focus-visible:ring-[rgba(255,255,255,0.18)] focus-visible:ring-offset-2 focus-visible:ring-offset-transparent',
  ].join(' ')

function tabInnerClass(isActive: boolean) {
  return [
    'inline-flex items-center justify-center',
    isActive
      ? 'rounded-[18px] bg-[rgba(255,255,255,0.08)] px-3 py-1.5 text-white'
      : 'px-2 py-2 text-[rgba(255,255,255,0.4)]',
  ].join(' ')
}

function IconHome({ className }: { className?: string }) {
  return (
    <svg className={className} viewBox="0 0 24 24" fill="none" aria-hidden>
      <path
        d="M4 10.5L12 4L20 10.5V19C20 19.5523 19.5523 20 19 20H5C4.44772 20 4 19.5523 4 19V10.5Z"
        stroke="currentColor"
        strokeWidth="1.5"
        strokeLinejoin="round"
      />
    </svg>
  )
}

function IconTariffs({ className }: { className?: string }) {
  return (
    <span
      className={[className, 'flex items-center justify-center font-sans text-[1.125rem] font-medium leading-none'].join(' ')}
      aria-hidden
    >
      $
    </span>
  )
}

function IconSupport({ className }: { className?: string }) {
  return (
    <svg className={className} viewBox="0 0 24 24" fill="none" aria-hidden>
      <path d="M4 12A8 8 0 0 1 20 12" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" />
      <rect x="3" y="12" width="4" height="6" rx="2" stroke="currentColor" strokeWidth="1.5" />
      <rect x="17" y="12" width="4" height="6" rx="2" stroke="currentColor" strokeWidth="1.5" />
    </svg>
  )
}

const iconClass = 'h-7 w-7 shrink-0 text-current'

export function BottomNav() {
  const queryClient = useQueryClient()

  useEffect(() => {
    void queryClient.prefetchQuery({
      queryKey: userQueryKeys.tariffs,
      queryFn: getTariffs,
      staleTime: 300_000,
    })
    void queryClient.prefetchQuery({
      queryKey: userQueryKeys.summary,
      queryFn: getUserSummary,
      staleTime: 120_000,
    })
  }, [queryClient])

  return (
    <nav className="fixed bottom-4 left-0 right-0 z-50 font-sans" aria-label="Основная навигация">
      <div
        className={[
          'flex w-full items-center justify-center gap-8 px-6',
          'bg-transparent',
          'pt-3 pb-[max(0.5rem,env(safe-area-inset-bottom))]',
        ].join(' ')}
      >
        <NavLink to="/" end className={linkClass} aria-label="Главная">
          {({ isActive }) => (
            <span className={tabInnerClass(isActive)}>
              <IconHome className={iconClass} />
            </span>
          )}
        </NavLink>
        <NavLink to="/tariffs" className={linkClass} aria-label="Тарифы">
          {({ isActive }) => (
            <span className={tabInnerClass(isActive)}>
              <IconTariffs className={iconClass} />
            </span>
          )}
        </NavLink>
        <NavLink to="/support" className={linkClass} aria-label="Поддержка">
          {({ isActive }) => (
            <span className={tabInnerClass(isActive)}>
              <IconSupport className={iconClass} />
            </span>
          )}
        </NavLink>
      </div>
    </nav>
  )
}
