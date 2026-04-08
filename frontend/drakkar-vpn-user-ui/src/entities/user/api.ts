import { apiClient } from '../../shared/api/client'
import type {
  ConnectDevicePayload,
  ConnectDeviceResponse,
  HomeContextResponse,
  ProvisionStatusResponse,
  PurchaseResponse,
  PurchaseSubscriptionPayload,
  RegisterPayload,
  RegisterResponse,
  TariffDto,
  UserSummaryResponse,
  VpnConfigResponse,
} from './types'

export async function register(payload: RegisterPayload): Promise<RegisterResponse> {
  const { data } = await apiClient.post<RegisterResponse>('/api/v1/user/register', payload)
  return data
}

export async function connectDevice(
  payload: ConnectDevicePayload,
): Promise<ConnectDeviceResponse> {
  const { data } = await apiClient.post<ConnectDeviceResponse>(
    '/api/v1/user/devices/connect',
    payload,
  )
  return data
}

function normalizeHomeScreenState(raw: string): HomeContextResponse['state'] {
  const map: Record<string, HomeContextResponse['state']> = {
    blocked: 'Blocked',
    deviceLimitExceeded: 'DeviceLimitExceeded',
    noSubscription: 'NoSubscription',
    notStarted: 'NotStarted',
    pending: 'Pending',
    ready: 'Ready',
    Blocked: 'Blocked',
    DeviceLimitExceeded: 'DeviceLimitExceeded',
    NoSubscription: 'NoSubscription',
    NotStarted: 'NotStarted',
    Pending: 'Pending',
    Ready: 'Ready',
  }
  return map[raw] ?? (raw as HomeContextResponse['state'])
}

export async function getHomeContext(): Promise<HomeContextResponse> {
  const { data } = await apiClient.get<HomeContextResponse>('/api/v1/user/home-context')
  const state = normalizeHomeScreenState(String(data.state))
  return { ...data, state }
}

export async function getTariffs(): Promise<TariffDto[]> {
  const { data } = await apiClient.get<TariffDto[]>('/api/v1/user/tariffs')
  return data
}

export async function getUserSummary(): Promise<UserSummaryResponse> {
  const { data } = await apiClient.get<UserSummaryResponse>('/api/v1/user/summary')
  return data
}

export async function purchaseSubscription(
  payload: PurchaseSubscriptionPayload,
): Promise<PurchaseResponse> {
  const { data } = await apiClient.post<PurchaseResponse>(
    '/api/v1/user/subscriptions/purchase',
    payload,
  )
  return data
}

export async function getCurrentConfig(): Promise<VpnConfigResponse> {
  const { data } = await apiClient.get<VpnConfigResponse>('/api/v1/user/vpn/config/current')
  return data
}

export async function startProvision(): Promise<VpnConfigResponse> {
  const { data } = await apiClient.post<VpnConfigResponse>(
    '/api/v1/user/vpn/config/provision',
  )
  return data
}

export async function pollProvision(jobId: string): Promise<ProvisionStatusResponse> {
  const { data } = await apiClient.get<ProvisionStatusResponse>(
    `/api/v1/user/vpn/config/provision/${jobId}`,
  )
  return data
}
