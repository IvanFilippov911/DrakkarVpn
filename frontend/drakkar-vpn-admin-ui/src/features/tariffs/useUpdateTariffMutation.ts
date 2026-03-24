import { useMutation, useQueryClient } from '@tanstack/react-query'
import { tariffByIdQueryKey, updateTariff, TARIFFS_LIST_QUERY_KEY } from '../../entities/tariffs'
import type { UpdateTariffApiRequest } from '../../entities/tariffs'

type UpdateTariffPayload = {
  id: string
  payload: UpdateTariffApiRequest
}

export function useUpdateTariffMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, payload }: UpdateTariffPayload) => updateTariff(id, payload),
    onSuccess: async (_data, variables) => {
      const { id } = variables

      await Promise.all([
        queryClient.invalidateQueries({ queryKey: TARIFFS_LIST_QUERY_KEY }),
        queryClient.invalidateQueries({ queryKey: tariffByIdQueryKey(id) }),
      ])
    },
  })
}

