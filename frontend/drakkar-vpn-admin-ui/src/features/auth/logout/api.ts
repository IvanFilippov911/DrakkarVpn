import { apiClient } from '../../../shared/api/client'
import { clearSession } from '../../../entities/auth/session'

export async function logout(): Promise<void> {
  try {
    await apiClient.post('/api/admin/auth/logout', undefined, {
      withCredentials: true,
    })
  } finally {
    clearSession()
  }
}

