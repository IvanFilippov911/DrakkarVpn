import { useMutation, useQueryClient } from '@tanstack/react-query'
import { disableTransportProfile } from '../../entities/transport-profiles'
import { TRANSPORT_PROFILES_LIST_QUERY_KEY } from '../../entities/transport-profiles/queries'

export function useDisableTransportProfileMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (profileId: string) => disableTransportProfile(profileId),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: TRANSPORT_PROFILES_LIST_QUERY_KEY })
    },
  })
}
