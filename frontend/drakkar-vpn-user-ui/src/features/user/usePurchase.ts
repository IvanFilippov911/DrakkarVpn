import { useMutation, useQueryClient } from '@tanstack/react-query'
import type { PurchaseSubscriptionPayload } from '../../entities/user/types'
import { purchaseSubscription } from '../../entities/user/api'
import { userQueryKeys } from '../../entities/user/queryKeys'

export function usePurchase() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: PurchaseSubscriptionPayload) => purchaseSubscription(payload),
    retry: false,
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: userQueryKeys.homeContext })
    },
  })
}
