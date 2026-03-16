import { useMutation } from '@tanstack/react-query'
import { login } from '../../shared/api/auth'

type LoginVariables = {
  email: string
  password: string
  rememberMe: boolean
}

export function useLoginMutation() {
  return useMutation({
    mutationFn: (variables: LoginVariables) =>
      login(variables.email, variables.password, variables.rememberMe),
  })
}
