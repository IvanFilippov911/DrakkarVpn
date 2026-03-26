import { useMutation } from '@tanstack/react-query'
import { register } from '../../entities/user/api'
import type { RegisterPayload } from '../../entities/user'

export function useRegister() {
  return useMutation({
    mutationFn: (payload: RegisterPayload) => register(payload),
    retry: false,
  })
}
