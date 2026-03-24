import type { ReactNode } from 'react'
import type { HomeScreenState } from '../../entities/user'
import type { StateCardVariant } from '../../shared/ui/state-card/StateCard'
import {
  IconBlocked,
  IconNoSubscription,
  IconNotStarted,
  IconPending,
  IconReady,
  PendingDots,
} from '../../shared/ui/icons/homeStateIcons'

export type HomePresentation = {
  variant: StateCardVariant
  title: string
  subtitle: string
  icon: ReactNode
  footer?: ReactNode
  primaryCtaLabel: string | null
}

export const HOME_PRESENTATION: Record<HomeScreenState, HomePresentation> = {
  Blocked: {
    variant: 'error',
    title: 'Доступ ограничен',
    subtitle: 'Аккаунт заблокирован. Если это ошибка, свяжитесь с поддержкой.',
    icon: <IconBlocked />,
    primaryCtaLabel: null,
  },
  NoSubscription: {
    variant: 'default',
    title: 'Нет активной подписки',
    subtitle: 'Оформите подписку, чтобы подключить VPN.',
    icon: <IconNoSubscription />,
    primaryCtaLabel: 'Купить подписку',
  },
  NotStarted: {
    variant: 'default',
    title: 'Подписка активна',
    subtitle: 'Запустите подготовку конфигурации VPN.',
    icon: <IconNotStarted />,
    primaryCtaLabel: 'Подключиться',
  },
  Pending: {
    variant: 'warning',
    title: 'Подготовка VPN',
    subtitle: 'Это займёт несколько секунд. Не закрывайте приложение.',
    icon: <IconPending />,
    footer: <PendingDots />,
    primaryCtaLabel: null,
  },
  Ready: {
    variant: 'success',
    title: 'VPN готов',
    subtitle: 'Можно открыть конфигурацию и подключиться.',
    icon: <IconReady />,
    primaryCtaLabel: 'Открыть VPN',
  },
}
