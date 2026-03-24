import { useMutation, useQueryClient } from '@tanstack/react-query'
import { enableTariff, tariffByIdQueryKey, TARIFFS_LIST_QUERY_KEY } from '../../entities/tariffs'

export function useEnableTariffMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (tariffId: string) => enableTariff(tariffId),
    onSuccess: async (_data, tariffId) => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: TARIFFS_LIST_QUERY_KEY }),
        queryClient.invalidateQueries({ queryKey: tariffByIdQueryKey(tariffId) }),
      ])
    },
  })
}

