import { API_BASE_URL } from "../constants";

const base = (path: string) => `${API_BASE_URL}/api/v1/user${path}`;

export type VpnConfigStatus = "Ready" | "Pending";

export interface GetVpnConfigResponse {
  status: VpnConfigStatus;
  jobId?: string;
  happLink?: string;
  config?: string;
  expiresAt?: string;
}

export interface ConnectDeviceResponse {
  token: string;
  deviceId: string;
}

export interface ProvisionJobResponse {
  status: "Ready" | "Failed" | "Pending";
  happLink?: string;
  config?: string;
  error?: string;
}

export async function register(telegramId: number): Promise<void> {
  const res = await fetch(base("/register"), {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ telegramId }),
  });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || `Register failed: ${res.status}`);
  }
}

export async function purchaseSubscription(
  telegramId: number,
  tariffId: string,
  requestId: string
): Promise<void> {
  const res = await fetch(base("/subscriptions/purchase"), {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ telegramId, tariffId, requestId }),
  });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || `Purchase failed: ${res.status}`);
  }
}

export async function connectDevice(
  initData: string,
  deviceName: string,
  platform: string,
  existingDeviceId: string | null
): Promise<ConnectDeviceResponse> {
  const res = await fetch(base("/devices/connect"), {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      initData,
      deviceName,
      platform,
      existingDeviceId,
    }),
  });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || `Connect device failed: ${res.status}`);
  }
  return res.json() as Promise<ConnectDeviceResponse>;
}

export async function getVpnConfig(
  token: string
): Promise<GetVpnConfigResponse> {
  const res = await fetch(base("/vpn/config"), {
    method: "GET",
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!res.ok) {
    if (res.status === 401) throw new Error("Unauthorized");
    const text = await res.text();
    throw new Error(text || `Get config failed: ${res.status}`);
  }
  return res.json() as Promise<GetVpnConfigResponse>;
}

export async function getProvisionJob(
  jobId: string,
  token: string
): Promise<ProvisionJobResponse> {
  const res = await fetch(base(`/vpn/provision/${jobId}`), {
    method: "GET",
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || `Get provision failed: ${res.status}`);
  }
  return res.json() as Promise<ProvisionJobResponse>;
}
