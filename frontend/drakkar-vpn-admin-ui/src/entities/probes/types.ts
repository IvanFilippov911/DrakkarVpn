// DTO (API responses)
export type ProbeNodeStatus = 1 | 2 | 3

export type ProbeNodeApiResponse = {
  id: string
  name: string
  region: string
  host: string
  status: ProbeNodeStatus
  isEnabled: boolean
  lastSeenAtUtc: string | null
  createdAtUtc: string
  updatedAtUtc: string
}

// UI models (used by the UI layer, never use DTO directly)
export type ProbeNode = {
  id: string
  name: string
  region: string
  host: string
  status: ProbeNodeStatus
  isEnabled: boolean
  lastSeenAtUtc: Date | null
  createdAtUtc: Date
  updatedAtUtc: Date
}

