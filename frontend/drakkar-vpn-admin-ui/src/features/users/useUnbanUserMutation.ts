import { useMutation, useQueryClient } from '@tanstack/react-query'
import { USERS_LIST_QUERY_KEY, USERS_OVERVIEW_QUERY_KEY, userDetailsQueryKey } from '../../entities/users'
import { unbanUser } from './unban-user/api'

export function useUnbanUserMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (userId: string) => unbanUser(userId),
    onSuccess: async (_data, userId) => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: USERS_LIST_QUERY_KEY }),
        queryClient.invalidateQueries({ queryKey: USERS_OVERVIEW_QUERY_KEY }),
        queryClient.invalidateQueries({ queryKey: userDetailsQueryKey(userId) }),
      ])
    },
  })
}

