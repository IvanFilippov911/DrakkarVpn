/** Matches `HomeScreenStateDto` (JSON string enum). */
export type HomeScreenState =
  | 'Blocked'
  | 'DeviceLimitExceeded'
  | 'NoSubscription'
  | 'NotStarted'
  | 'Pending'
  | 'Ready'

/** Matches `VpnConfigStatusDto` (JSON string enum). */
export type VpnConfigStatus = 'Ready' | 'Pending' | 'NotStarted'

/** Matches `PeerProvisionStatus` (JSON string enum). */
export type PeerProvisionStatus = 'Pending' | 'Ready' | 'Failed'

export type RegisterResponse = {
  isNewUser: boolean
}

export type RegisterPayload = {
  telegramId: number
  publicPort?: number
  realityPublicKey?: string
  realityShortId?: string
  realitySni?: string
}

export type ConnectDevicePayload = {
  initData: string
  deviceName?: string
  platform?: string
  existingDeviceId?: string
}

export type ConnectDeviceResponse = {
  accessToken: string
  deviceId: string
}

export type HomeContextResponse = {
  state: HomeScreenState
  pendingProvisionJobId?: string | null
  pollUrl?: string | null
}

/**
 * User summary from `GET /api/v1/user/summary`.
 * (dates are ISO strings, serialized from backend `DateTime?`).
 */
export type UserSummaryResponse = {
  subscriptionEndAtUtc: string | null
  connectedDevices: number
  maxDevices: number | null
}

/**
 * Active tariff row from `GET /api/v1/user/tariffs`
 * (backend `UserTariffResponse`; `duration` is serialized `TimeSpan` string).
 */
export type TariffDto = {
  id: string
  name: string
  price: number
  duration: string
  defaultMaxDevices: number
}

export type PurchaseSubscriptionPayload = {
  telegramId: number
  tariffId: string
  devicesCount: number
  requestId: string
}

/**
 * `POST /api/v1/user/subscriptions/purchase` returns `SubscriptionDto`.
 */
export type PurchaseResponse = {
  id: string
  userId: string
  startAt: string
  endAt: string
  status: string
  maxDevices: number | null
}

/**
 * `StatusVpnConfigResponse` from current config / start provision endpoints.
 */
export type VpnConfigResponse = {
  statusDto: VpnConfigStatus
  /** Full Xray client JSON (same as GET configUrl). */
  configRaw?: string | null
  configVless?: string | null
  /** HTTPS URL that returns full Xray JSON. */
  configUrl?: string | null
  happLink?: string | null
  v2rayLink?: string | null
  jobId?: string | null
  pollUrl?: string | null
}

/**
 * `PeerProvisionJobResponse` from provision polling.
 */
export type ProvisionStatusResponse = {
  status: PeerProvisionStatus
  errorCode?: string | null
  errorMessage?: string | null
}
