import type { HomeScreenState } from '../../entities/user'

export type HomePresentation = {
  subscriptionStatusLabel: 'Подписка неактивна' | 'Подписка активна'
  primaryCtaLabel: string | null
}

const active = 'Подписка активна' as const
const inactive = 'Подписка неактивна' as const

export const HOME_PRESENTATION: Record<HomeScreenState, HomePresentation> = {
  Blocked: {
    subscriptionStatusLabel: active,
    primaryCtaLabel: null,
  },
  DeviceLimitExceeded: {
    subscriptionStatusLabel: active,
    primaryCtaLabel: null,
  },
  NoSubscription: {
    subscriptionStatusLabel: inactive,
    primaryCtaLabel: 'Подключиться',
  },
  NotStarted: {
    subscriptionStatusLabel: active,
    primaryCtaLabel: 'Подключиться',
  },
  Pending: {
    subscriptionStatusLabel: active,
    primaryCtaLabel: null,
  },
  Ready: {
    subscriptionStatusLabel: active,
    primaryCtaLabel: 'Подключиться',
  },
}
