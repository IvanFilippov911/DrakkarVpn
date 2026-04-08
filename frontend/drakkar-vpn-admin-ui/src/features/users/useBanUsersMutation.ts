import { useMutation, useQueryClient } from '@tanstack/react-query'
import { USERS_LIST_QUERY_KEY, USERS_OVERVIEW_QUERY_KEY } from '../../entities/users'
import { banUsers } from './ban-user/api'
import type { BulkBanUsersRequest } from './types'

export function useBanUsersMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: BulkBanUsersRequest) => banUsers(payload),
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: USERS_LIST_QUERY_KEY }),
        queryClient.invalidateQueries({ queryKey: USERS_OVERVIEW_QUERY_KEY }),
      ])
    },
  })
}

