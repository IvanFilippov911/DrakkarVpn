export type TariffStatus = 'Active' | 'Disabled'

export type TariffKind = 'Standard' | 'Trial'

// DTO (API response)
export type TariffApiResponse = {
  id: string
  name: string
  price: number
  // Backend returns TimeSpan serialized as JSON; do not assume shape here.
  duration: unknown
  status: TariffStatus
  createdAt: string
  defaultMaxDevices: number
  kind: TariffKind
}

export type CreateTariffApiRequest = {
  name: string
  durationDays: number
  price: number
  defaultMaxDevices: number
  kind: TariffKind
}

export type UpdateTariffApiRequest = {
  name: string
  durationDays: number
  price: number
  defaultMaxDevices: number
}

