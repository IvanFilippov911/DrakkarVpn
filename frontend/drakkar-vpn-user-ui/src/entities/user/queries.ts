import { isAxiosError } from 'axios'
import { useEffect } from 'react'
import { useQuery, useQueryClient } from '@tanstack/react-query'
import { getCurrentConfig, getHomeContext, getTariffs, pollProvision } from './api'
import { userQueryKeys } from './queryKeys'

const READ_RETRY_COUNT = 3

function readRetryDelay(attemptIndex: number): number {
  return Math.min(1000 * 2 ** attemptIndex, 30_000)
}

export type UseHomeContextOptions = {
  /** Gate the query until boot finished (e.g. after connect + token). Default true. */
  enabled?: boolean
}

/**
 * Home screen business state must come only from this query’s `data` (home-context API).
 * Do not infer home UI state from `useCurrentConfig` or provision polling.
 */
export function useHomeContext(options?: UseHomeContextOptions) {
  const enabled = options?.enabled ?? true

  return useQuery({
    queryKey: userQueryKeys.homeContext,
    queryFn: getHomeContext,
    enabled,
    retry: READ_RETRY_COUNT,
    retryDelay: readRetryDelay,
  })
}

export function useTariffs(options?: { enabled?: boolean }) {
  const enabled = options?.enabled ?? true

  return useQuery({
    queryKey: userQueryKeys.tariffs,
    queryFn: getTariffs,
    enabled,
    retry: READ_RETRY_COUNT,
    retryDelay: readRetryDelay,
  })
}

/**
 * Operational VPN config (payload / pending job). Not the home screen state source.
 */
export function useCurrentConfig(options?: { enabled?: boolean }) {
  const enabled = options?.enabled ?? true

  return useQuery({
    queryKey: userQueryKeys.currentConfig,
    queryFn: getCurrentConfig,
    enabled,
    retry: READ_RETRY_COUNT,
    retryDelay: readRetryDelay,
  })
}

const PROVISION_POLL_MS = 1500

function provisionPollRetry(failureCount: number, error: unknown): boolean {
  if (isAxiosError(error) && error.response?.status === 404) {
    return false
  }
  return failureCount < READ_RETRY_COUNT
}

/**
 * Polls a single provision job. Stops interval when status is Ready or Failed.
 * On terminal status, invalidates home-context (and current config) so UI stays aligned with backend.
 */
const provisionPollIdleKey = ['user', 'provision-poll', '__idle__'] as const

export function useProvisionPolling(jobId: string | null | undefined) {
  const queryClient = useQueryClient()
  const enabled = Boolean(jobId)

  const query = useQuery({
    queryKey: enabled ? userQueryKeys.provisionPoll(jobId!) : provisionPollIdleKey,
    queryFn: () => pollProvision(jobId!),
    enabled,
    refetchOnWindowFocus: false,
    refetchInterval: (q) => {
      const status = q.state.data?.status
      if (status === 'Ready' || status === 'Failed') {
        return false
      }
      return PROVISION_POLL_MS
    },
    retry: provisionPollRetry,
    retryDelay: readRetryDelay,
  })

  const status = query.data?.status

  useEffect(() => {
    if (status !== 'Ready' && status !== 'Failed') {
      return
    }
    void queryClient.invalidateQueries({ queryKey: userQueryKeys.homeContext })
    void queryClient.invalidateQueries({ queryKey: userQueryKeys.currentConfig })
  }, [status, queryClient])

  return query
}
