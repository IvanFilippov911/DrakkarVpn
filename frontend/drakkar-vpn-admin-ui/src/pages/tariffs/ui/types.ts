import type { TariffStatus } from '../../../entities/tariffs'

export type UiState = 'loading' | 'empty' | 'error' | 'success'

export type TariffRow = {
  id: string
  name: string
  price: number
  // For UI skeleton we use a preformatted string.
  duration: string
  devices: number
  status: TariffStatus
  createdAt: string
}

