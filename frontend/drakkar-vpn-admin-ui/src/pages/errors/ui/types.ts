export type ErrorsUiState = 'loading' | 'success' | 'empty' | 'error'

export type ErrorEventRow = {
  id: string
  timestamp: string
  command: string
  area: string
  errorType: string
  domainCode: string | null
  message: string
  traceId: string
  userId: string | null
  telegramId: string | null
}

