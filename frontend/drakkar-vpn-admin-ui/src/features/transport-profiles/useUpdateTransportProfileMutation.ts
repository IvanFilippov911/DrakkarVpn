import { useMutation, useQueryClient } from '@tanstack/react-query'
import { updateTransportProfile, type UpdateTransportProfileApiRequest } from '../../entities/transport-profiles'
import { TRANSPORT_PROFILES_LIST_QUERY_KEY } from '../../entities/transport-profiles/queries'

type UpdateTransportProfilePayload = {
  id: string
  payload: UpdateTransportProfileApiRequest
}

export function useUpdateTransportProfileMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, payload }: UpdateTransportProfilePayload) => updateTransportProfile(id, payload),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: TRANSPORT_PROFILES_LIST_QUERY_KEY })
    },
  })
}
