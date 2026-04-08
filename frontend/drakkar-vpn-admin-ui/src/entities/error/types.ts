export type Guid = string
export type UtcDateTimeString = string

export type CoreErrorEventListItemDto = {
  id: Guid
  timestampUtc: UtcDateTimeString
  command: string
  area: string
  errorType: string
  domainCode: string | null
  message: string
  traceId: string
  userId: string | null
  telegramId: string | null
}

export type CoreErrorEventsListQuery = {
  page: number
  pageSize: number
  area?: string
  errorType?: string
  command?: string
  domainCode?: string
  userId?: string
  telegramId?: string
  search?: string
  fromUtc?: UtcDateTimeString
  toUtc?: UtcDateTimeString
}

