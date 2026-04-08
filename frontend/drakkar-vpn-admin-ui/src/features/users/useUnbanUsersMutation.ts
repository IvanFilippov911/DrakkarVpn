import { useMutation, useQueryClient } from '@tanstack/react-query'
import { USERS_LIST_QUERY_KEY, USERS_OVERVIEW_QUERY_KEY } from '../../entities/users'
import { unbanUsers } from './unban-user/api'
import type { BulkUnbanUsersRequest } from './types'

export function useUnbanUsersMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: BulkUnbanUsersRequest) => unbanUsers(payload),
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: USERS_LIST_QUERY_KEY }),
        queryClient.invalidateQueries({ queryKey: USERS_OVERVIEW_QUERY_KEY }),
      ])
    },
  })
}

