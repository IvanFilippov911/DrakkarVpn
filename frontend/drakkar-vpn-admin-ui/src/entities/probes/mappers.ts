import type { ProbeNode, ProbeNodeApiResponse } from './types'

export function mapProbeNodeDto(dto: ProbeNodeApiResponse): ProbeNode {
  return {
    id: dto.id,
    name: dto.name,
    region: dto.region,
    host: dto.host,
    status: dto.status,
    isEnabled: dto.isEnabled,
    lastSeenAtUtc: dto.lastSeenAtUtc ? new Date(dto.lastSeenAtUtc) : null,
    createdAtUtc: new Date(dto.createdAtUtc),
    updatedAtUtc: new Date(dto.updatedAtUtc),
  }
}

