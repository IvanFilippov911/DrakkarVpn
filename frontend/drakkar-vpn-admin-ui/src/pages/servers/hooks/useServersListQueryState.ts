import { useCallback, useMemo, useState } from 'react'
import type { ServersListQuery } from '../../../entities/server/types'

type ServersListQueryState = {
  region?: string
  status?: string
  page: number
  pageSize: number
}

const DEFAULT_PAGE = 1
const DEFAULT_PAGE_SIZE = 20

export function useServersListQueryState(initial?: Partial<ServersListQueryState>) {
  const [state, setState] = useState<ServersListQueryState>(() => ({
    region: initial?.region,
    status: initial?.status,
    page: initial?.page ?? DEFAULT_PAGE,
    pageSize: initial?.pageSize ?? DEFAULT_PAGE_SIZE,
  }))

  const setRegion = useCallback((region?: string) => {
    setState((prev) => ({ ...prev, region: region || undefined, page: DEFAULT_PAGE }))
  }, [])

  const setStatus = useCallback((status?: string) => {
    setState((prev) => ({ ...prev, status: status || undefined, page: DEFAULT_PAGE }))
  }, [])

  const setPage = useCallback((page: number) => {
    setState((prev) => ({ ...prev, page }))
  }, [])

  const setPageSize = useCallback((pageSize: number) => {
    setState((prev) => ({ ...prev, pageSize, page: DEFAULT_PAGE }))
  }, [])

  const query: ServersListQuery = useMemo(
    () => ({
      region: state.region,
      status: state.status,
      page: state.page,
      pageSize: state.pageSize,
    }),
    [state.page, state.pageSize, state.region, state.status],
  )

  return {
    state,
    query,
    setRegion,
    setStatus,
    setPage,
    setPageSize,
  }
}

