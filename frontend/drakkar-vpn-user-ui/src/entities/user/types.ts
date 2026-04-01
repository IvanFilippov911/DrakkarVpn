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
 * Active tariff row from `GET /api/v1/user/tariffs`
 * (backend `UserTariffResponse`; `duration` is serialized `TimeSpan` string).
 */
export type TariffDto = {
  id: string
  name: string
  price: number
  duration: string
  status: string
  createdAt: string
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
  configRaw?: string | null
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
