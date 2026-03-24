import { useMutation, useQueryClient } from '@tanstack/react-query'
import { disableTariff, tariffByIdQueryKey, TARIFFS_LIST_QUERY_KEY } from '../../entities/tariffs'

export function useDisableTariffMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (tariffId: string) => disableTariff(tariffId),
    onSuccess: async (_data, tariffId) => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: TARIFFS_LIST_QUERY_KEY }),
        queryClient.invalidateQueries({ queryKey: tariffByIdQueryKey(tariffId) }),
      ])
    },
  })
}

