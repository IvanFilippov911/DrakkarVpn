import type { TransportProfile, TransportProfileApiResponse } from './types'

export function mapTransportProfileDto(dto: TransportProfileApiResponse): TransportProfile {
  return {
    id: dto.id,
    name: dto.name,
    transportType: dto.transportType,
    securityType: dto.securityType,
    realitySni: dto.realitySni,
    realityShortId: dto.realityShortId,
    realityFingerprint: dto.realityFingerprint,
    realityDest: dto.realityDest,
    grpcServiceName: dto.grpcServiceName,
    grpcAuthority: dto.grpcAuthority,
    globalPriority: dto.globalPriority,
    isEnabled: dto.isEnabled,
    createdAtUtc: new Date(dto.createdAtUtc),
    updatedAtUtc: new Date(dto.updatedAtUtc),
    version: dto.version,
  }
}
