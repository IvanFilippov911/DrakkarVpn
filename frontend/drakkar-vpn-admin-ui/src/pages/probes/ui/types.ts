export type UiState = 'loading' | 'error' | 'empty' | 'success'

export type ProbeRow = {
  id: string
  name: string
  region: string
  host: string
  status: string
  enabled: boolean
  lastSeen: string
  updated: string
}

