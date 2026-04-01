import { useMutation, useQueryClient } from '@tanstack/react-query'
import type { PurchaseSubscriptionPayload } from '../../entities/user/types'
import { getHomeContext, purchaseSubscription } from '../../entities/user/api'
import { userQueryKeys } from '../../entities/user/queryKeys'

const HOME_AFTER_PURCHASE_ATTEMPTS = 8
const HOME_AFTER_PURCHASE_DELAY_MS = 120

export function usePurchase() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: PurchaseSubscriptionPayload) => purchaseSubscription(payload),
    retry: false,
    onSuccess: async () => {
      try {
        await queryClient.invalidateQueries({ queryKey: userQueryKeys.homeContext })
        for (let i = 0; i < HOME_AFTER_PURCHASE_ATTEMPTS; i++) {
          const home = await queryClient.fetchQuery({
            queryKey: userQueryKeys.homeContext,
            queryFn: getHomeContext,
          })
          if (home.state !== 'NoSubscription') break
          await new Promise((r) => setTimeout(r, HOME_AFTER_PURCHASE_DELAY_MS))
        }
      } catch {
        /* purchase already succeeded; HomePage can refetch */
      }
    },
  })
}
