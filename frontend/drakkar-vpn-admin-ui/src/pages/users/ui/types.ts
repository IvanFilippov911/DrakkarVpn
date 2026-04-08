export type UiState = 'loading' | 'error' | 'empty' | 'success'

export type UserRow = {
  id: string
  telegram: string
  createdAt: string
  status: string
  online: boolean
  devices: string
  lastSeen: string
  subscriptionStatus: string
  subscriptionEnd: string
  maxDevices: string
  traffic24h: string
}

