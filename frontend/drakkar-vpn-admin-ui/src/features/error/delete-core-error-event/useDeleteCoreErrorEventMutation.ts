import { useMutation, useQueryClient } from '@tanstack/react-query'
import { CORE_ERROR_EVENTS_LIST_QUERY_KEY } from '../../../entities/error'
import { deleteCoreErrorEvent } from './api'

export function useDeleteCoreErrorEventMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => deleteCoreErrorEvent(id),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: CORE_ERROR_EVENTS_LIST_QUERY_KEY })
    },
  })
}

