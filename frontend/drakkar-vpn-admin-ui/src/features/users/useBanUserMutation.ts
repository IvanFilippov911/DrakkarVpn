import { useMutation, useQueryClient } from '@tanstack/react-query'
import { USERS_LIST_QUERY_KEY, USERS_OVERVIEW_QUERY_KEY, userDetailsQueryKey } from '../../entities/users'
import { banUser } from './ban-user/api'
import type { BanUserRequest } from './types'

export function useBanUserMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ userId, payload }: { userId: string; payload: BanUserRequest }) =>
      banUser(userId, payload),
    onSuccess: async (_data, { userId }) => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: USERS_LIST_QUERY_KEY }),
        queryClient.invalidateQueries({ queryKey: USERS_OVERVIEW_QUERY_KEY }),
        queryClient.invalidateQueries({ queryKey: userDetailsQueryKey(userId) }),
      ])
    },
  })
}

