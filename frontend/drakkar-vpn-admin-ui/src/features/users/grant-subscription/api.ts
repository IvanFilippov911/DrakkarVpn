import { apiClient } from '../../../shared/api/client'
import type {
  AdminBulkGrantSubscriptionsRequest,
  AdminGrantSubscriptionRequest,
  BulkGrantSubscriptionsResponse,
} from '../types'

export async function grantUserSubscription(
  userId: string,
  payload: AdminGrantSubscriptionRequest,
): Promise<unknown> {
  const { data } = await apiClient.post<unknown>(
    `/api/admin/users/${userId}/subscriptions/grant`,
    payload,
    {
      withCredentials: true,
    },
  )

  return data
}

export async function grantUsersSubscriptions(
  payload: AdminBulkGrantSubscriptionsRequest,
): Promise<BulkGrantSubscriptionsResponse> {
  const { data } = await apiClient.post<BulkGrantSubscriptionsResponse>(
    '/api/admin/users/subscriptions/grant',
    payload,
    {
      withCredentials: true,
    },
  )

  return data
}

