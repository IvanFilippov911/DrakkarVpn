import { useMutation, useQueryClient } from '@tanstack/react-query'
import { createTariff, tariffByIdQueryKey, TARIFFS_LIST_QUERY_KEY } from '../../entities/tariffs'
import type { CreateTariffApiRequest } from '../../entities/tariffs'

export function useCreateTariffMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: CreateTariffApiRequest) => createTariff(payload),
    onSuccess: async (createdTariffId) => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: TARIFFS_LIST_QUERY_KEY }),
        queryClient.invalidateQueries({ queryKey: tariffByIdQueryKey(createdTariffId) }),
      ])
    },
  })
}

