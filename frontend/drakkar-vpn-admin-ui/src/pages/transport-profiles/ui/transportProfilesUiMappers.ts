import type { TransportProfile } from '../../../entities/transport-profiles'
import type { TransportProfileRow } from './types'

function formatDateTime(value: Date): string {
  return value.toISOString().replace('T', ' ').slice(0, 16) + 'Z'
}

function transportTypeLabel(type: TransportProfile['transportType']): string {
  return type === 'Grpc' ? 'gRPC' : 'TCP'
}

export function mapTransportProfilesToRows(items: TransportProfile[]): TransportProfileRow[] {
  return items.map((item) => ({
    id: item.id,
    name: item.name,
    transportType: transportTypeLabel(item.transportType),
    securityType: item.securityType,
    globalPriority: String(item.globalPriority),
    statusLabel: item.isEnabled ? 'Enabled' : 'Disabled',
    updatedAt: formatDateTime(item.updatedAtUtc),
    isEnabled: item.isEnabled,
    transportTypeValue: item.transportType,
  }))
}
