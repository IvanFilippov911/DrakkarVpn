export type UiState = 'loading' | 'error' | 'empty' | 'success'

export type ServerRow = {
  id: string
  name: string
  region: string
  status: string
  reachable: boolean
  online: string
  active: string
  max: string
  speed: string
  latency: string
  traffic1h: string
  traffic24h: string
}

