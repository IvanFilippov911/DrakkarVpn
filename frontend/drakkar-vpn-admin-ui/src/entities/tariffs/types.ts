export type TariffStatus = 'Active' | 'Disabled'

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
}

export type CreateTariffApiRequest = {
  name: string
  durationDays: number
  price: number
  defaultMaxDevices: number
}

export type UpdateTariffApiRequest = {
  name: string
  durationDays: number
  price: number
  defaultMaxDevices: number
}

