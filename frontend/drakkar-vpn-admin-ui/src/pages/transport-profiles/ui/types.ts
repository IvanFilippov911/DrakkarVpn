import type { TransportProfile } from '../../../entities/transport-profiles'

export type UiState = 'loading' | 'error' | 'empty' | 'success'

export type TransportProfileRow = {
  id: string
  name: string
  transportType: string
  securityType: string
  globalPriority: string
  statusLabel: string
  updatedAt: string
  isEnabled: boolean
  transportTypeValue: TransportProfile['transportType']
}
