import { useMutation, useQueryClient } from '@tanstack/react-query'
import { USERS_LIST_QUERY_KEY, USERS_OVERVIEW_QUERY_KEY } from '../../entities/users'
import { grantUsersSubscriptions } from './grant-subscription/api'
import type { AdminBulkGrantSubscriptionsRequest } from './types'

export function useGrantUsersSubscriptionsMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: AdminBulkGrantSubscriptionsRequest) => grantUsersSubscriptions(payload),
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: USERS_LIST_QUERY_KEY }),
        queryClient.invalidateQueries({ queryKey: USERS_OVERVIEW_QUERY_KEY }),
      ])
    },
  })
}

