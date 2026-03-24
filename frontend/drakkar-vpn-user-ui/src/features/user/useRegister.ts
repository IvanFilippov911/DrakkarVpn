import { useMutation } from '@tanstack/react-query'
import { register } from '../../entities/user/api'

export function useRegister() {
  return useMutation({
    mutationFn: (telegramId: number) => register(telegramId),
    retry: false,
  })
}
