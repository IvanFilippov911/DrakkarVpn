import { useMutation } from '@tanstack/react-query'
import { logout } from '../../shared/api/auth'

export function useLogoutMutation() {
  return useMutation({
    mutationFn: logout,
  })
}
