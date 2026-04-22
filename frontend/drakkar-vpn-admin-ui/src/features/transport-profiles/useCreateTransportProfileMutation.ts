import { useMutation, useQueryClient } from '@tanstack/react-query'
import { createTransportProfile, type CreateTransportProfileApiRequest } from '../../entities/transport-profiles'
import { TRANSPORT_PROFILES_LIST_QUERY_KEY } from '../../entities/transport-profiles/queries'

export function useCreateTransportProfileMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: CreateTransportProfileApiRequest) => createTransportProfile(payload),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: TRANSPORT_PROFILES_LIST_QUERY_KEY })
    },
  })
}
