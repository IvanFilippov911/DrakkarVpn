import type { PagedResponseDto } from '../../shared/api/types'

export type TransportType = 'Tcp' | 'Grpc'
export type SecurityType = 'Reality' | 'Tls'
export type SortDirection = 'Asc' | 'Desc'
export type TransportProfilesSortBy = 'GlobalPriority' | 'UpdatedAtUtc'

// DTO (API responses)
export type TransportProfileApiResponse = {
  id: string
  name: string
  transportType: TransportType
  securityType: SecurityType
  realitySni: string | null
  realityShortId: string | null
  realityFingerprint: string | null
  realityDest: string | null
  grpcServiceName: string | null
  grpcAuthority: string | null
  globalPriority: number
  isEnabled: boolean
  createdAtUtc: string
  updatedAtUtc: string
  version: number
}

export type TransportProfilesListQuery = {
  page: number
  pageSize: number
  search?: string
  isEnabled?: boolean
  transportType?: TransportType
  sortBy: TransportProfilesSortBy
  sortDirection: SortDirection
}

export type TransportProfilesListApiResponse = PagedResponseDto<TransportProfileApiResponse>

export type CreateTransportProfileApiRequest = {
  name: string
  transportType: TransportType
  securityType: SecurityType
  realitySni: string
  realityShortId: string
  realityFingerprint: string
  realityDest: string
  grpcServiceName: string | null
  grpcAuthority: string | null
  globalPriority: number
}

export type UpdateTransportProfileApiRequest = {
  name: string
  realitySni: string
  realityShortId: string
  realityFingerprint: string
  realityDest: string
  grpcServiceName: string | null
  grpcAuthority: string | null
  globalPriority: number
}

// UI models (used by UI layer, never use DTO directly)
export type TransportProfile = {
  id: string
  name: string
  transportType: TransportType
  securityType: SecurityType
  realitySni: string | null
  realityShortId: string | null
  realityFingerprint: string | null
  realityDest: string | null
  grpcServiceName: string | null
  grpcAuthority: string | null
  globalPriority: number
  isEnabled: boolean
  createdAtUtc: Date
  updatedAtUtc: Date
  version: number
}
