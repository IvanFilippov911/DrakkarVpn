import { useMutation, useQueryClient } from '@tanstack/react-query'
import { USERS_LIST_QUERY_KEY, USERS_OVERVIEW_QUERY_KEY, userDetailsQueryKey } from '../../entities/users'
import { markUserInternal } from './mark-user-internal/api'
import type { MarkUserInternalRequest } from './types'

export function useMarkUserInternalMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ userId, payload }: { userId: string; payload: MarkUserInternalRequest }) =>
      markUserInternal(userId, payload),
    onSuccess: async (_data, { userId }) => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: USERS_LIST_QUERY_KEY }),
        queryClient.invalidateQueries({ queryKey: USERS_OVERVIEW_QUERY_KEY }),
        queryClient.invalidateQueries({ queryKey: userDetailsQueryKey(userId) }),
      ])
    },
  })
}

