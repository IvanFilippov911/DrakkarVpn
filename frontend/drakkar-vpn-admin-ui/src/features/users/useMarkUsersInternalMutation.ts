import { useMutation, useQueryClient } from '@tanstack/react-query'
import { USERS_LIST_QUERY_KEY, USERS_OVERVIEW_QUERY_KEY } from '../../entities/users'
import { markUsersInternal } from './mark-user-internal/api'
import type { BulkMarkUsersInternalRequest } from './types'

export function useMarkUsersInternalMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: BulkMarkUsersInternalRequest) => markUsersInternal(payload),
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: USERS_LIST_QUERY_KEY }),
        queryClient.invalidateQueries({ queryKey: USERS_OVERVIEW_QUERY_KEY }),
      ])
    },
  })
}

