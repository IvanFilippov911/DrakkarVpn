import { useMutation, useQueryClient } from '@tanstack/react-query'
import { deleteTransportProfile } from '../../entities/transport-profiles'
import { TRANSPORT_PROFILES_LIST_QUERY_KEY } from '../../entities/transport-profiles/queries'

export function useDeleteTransportProfileMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (profileId: string) => deleteTransportProfile(profileId),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: TRANSPORT_PROFILES_LIST_QUERY_KEY })
    },
  })
}
