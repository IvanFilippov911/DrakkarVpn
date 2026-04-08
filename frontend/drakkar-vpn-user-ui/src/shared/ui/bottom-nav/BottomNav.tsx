import { NavLink } from 'react-router-dom'

const navItemClass = ({ isActive }: { isActive: boolean }) =>
  [
    'flex min-w-0 flex-1 basis-0 flex-col items-center justify-center gap-0.5 py-2 text-center text-[11px] font-normal leading-tight transition-[color,opacity] touch-manipulation',
    'no-underline focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-[var(--btn-primary-bg)]/35 focus-visible:ring-offset-2 focus-visible:ring-offset-[#0b0f14]',
    isActive ? 'text-[var(--btn-primary-bg)]' : 'text-[#e8e2d6]/50',
  ].join(' ')

function IconHome({ className }: { className?: string }) {
  return (
    <svg className={className} viewBox="0 0 24 24" fill="none" aria-hidden>
      <path
        d="M4 10.5L12 4l8 6.5V19a1 1 0 01-1 1h-4v-6H9v6H5a1 1 0 01-1-1v-8.5z"
        stroke="currentColor"
        strokeWidth="1.75"
        strokeLinejoin="round"
      />
    </svg>
  )
}

function IconTariffs({ className }: { className?: string }) {
  return (
    <svg className={className} viewBox="0 0 24 24" fill="none" aria-hidden>
      <path
        d="M5 7h14M5 12h14M5 17h9"
        stroke="currentColor"
        strokeWidth="1.75"
        strokeLinecap="round"
      />
    </svg>
  )
}

function IconSupport({ className }: { className?: string }) {
  return (
    <svg className={className} viewBox="0 0 24 24" fill="none" aria-hidden>
      <path
        d="M12 21a9 9 0 100-18 9 9 0 000 18z"
        stroke="currentColor"
        strokeWidth="1.75"
      />
      <path
        d="M9.5 9.75a2.5 2.5 0 015 0c0 2-2.5 1.75-2.5 4M12 17h.01"
        stroke="currentColor"
        strokeWidth="1.75"
        strokeLinecap="round"
      />
    </svg>
  )
}

export function BottomNav() {
  const iconClass = 'h-6 w-6 shrink-0 text-current'

  return (
    <nav
      className="fixed bottom-0 left-0 right-0 z-50 flex w-full border-t border-[rgba(255,255,255,0.06)] bg-[#0b0f14] pb-[env(safe-area-inset-bottom)] font-sans"
      aria-label="Основная навигация"
    >
      <NavLink to="/" end className={navItemClass}>
        <IconHome className={iconClass} />
        Главная
      </NavLink>
      <NavLink to="/tariffs" className={navItemClass}>
        <IconTariffs className={iconClass} />
        Тарифы
      </NavLink>
      <NavLink to="/support" className={navItemClass}>
        <IconSupport className={iconClass} />
        Поддержка
      </NavLink>
    </nav>
  )
}
