import axios from 'axios'
import { getStoredAccessToken } from '../lib/authTokenStorage'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL

if (!API_BASE_URL) {
  console.warn(
    '[Drakkar Admin] VITE_API_BASE_URL is not defined. API client will use relative URLs.',
  )
}

export const apiClient = axios.create({
  baseURL: API_BASE_URL || undefined,
  withCredentials: true,
})

// Ensure every request has Authorization from storage (e.g. after reload before React effect runs).
apiClient.interceptors.request.use((config) => {
  const token = getStoredAccessToken()
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

export function setAccessToken(token: string | null) {
  if (!token) {
    delete apiClient.defaults.headers.common.Authorization
    return
  }

  apiClient.defaults.headers.common.Authorization = `Bearer ${token}`
}

