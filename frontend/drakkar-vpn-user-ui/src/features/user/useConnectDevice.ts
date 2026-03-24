import { useMutation, useQueryClient } from '@tanstack/react-query'
import type { ConnectDevicePayload } from '../../entities/user/types'
import { connectDevice } from '../../entities/user/api'
import { userQueryKeys } from '../../entities/user/queryKeys'
import { setAccessToken } from '../../shared/api/client'
import { setStoredAccessToken } from '../../shared/lib/authTokenStorage'
import { setStoredDeviceId } from '../../shared/lib/deviceIdStorage'

export function useConnectDevice() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: ConnectDevicePayload) => connectDevice(payload),
    retry: false,
    onSuccess: (data) => {
      setAccessToken(data.accessToken)
      setStoredAccessToken(data.accessToken)
      setStoredDeviceId(data.deviceId)
      void queryClient.invalidateQueries({ queryKey: userQueryKeys.homeContext })
      void queryClient.invalidateQueries({ queryKey: userQueryKeys.currentConfig })
    },
  })
}
