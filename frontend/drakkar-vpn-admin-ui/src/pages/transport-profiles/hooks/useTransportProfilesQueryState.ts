import { useCallback, useMemo, useState } from 'react'
import type {
  SortDirection,
  TransportProfilesListQuery,
  TransportProfilesSortBy,
  TransportType,
} from '../../../entities/transport-profiles'

type TransportProfilesListState = {
  page: number
  pageSize: number
  search?: string
  isEnabled?: boolean
  transportType?: TransportType
  sortBy: TransportProfilesSortBy
  sortDirection: SortDirection
}

const DEFAULT_PAGE = 1
const DEFAULT_PAGE_SIZE = 20

export function useTransportProfilesQueryState(initial?: Partial<TransportProfilesListState>) {
  const [state, setState] = useState<TransportProfilesListState>(() => ({
    page: initial?.page ?? DEFAULT_PAGE,
    pageSize: initial?.pageSize ?? DEFAULT_PAGE_SIZE,
    search: initial?.search,
    isEnabled: initial?.isEnabled,
    transportType: initial?.transportType,
    sortBy: initial?.sortBy ?? 'GlobalPriority',
    sortDirection: initial?.sortDirection ?? 'Asc',
  }))

  const setSearch = useCallback((search?: string) => {
    setState((prev) => ({ ...prev, search: search || undefined, page: DEFAULT_PAGE }))
  }, [])

  const setIsEnabled = useCallback((isEnabled?: boolean) => {
    setState((prev) => ({ ...prev, isEnabled, page: DEFAULT_PAGE }))
  }, [])

  const setTransportType = useCallback((transportType?: TransportType) => {
    setState((prev) => ({ ...prev, transportType, page: DEFAULT_PAGE }))
  }, [])

  const setSortBy = useCallback((sortBy: TransportProfilesSortBy) => {
    setState((prev) => ({ ...prev, sortBy, page: DEFAULT_PAGE }))
  }, [])

  const setSortDirection = useCallback((sortDirection: SortDirection) => {
    setState((prev) => ({ ...prev, sortDirection, page: DEFAULT_PAGE }))
  }, [])

  const setPage = useCallback((page: number) => {
    setState((prev) => ({ ...prev, page }))
  }, [])

  const setPageSize = useCallback((pageSize: number) => {
    setState((prev) => ({ ...prev, pageSize, page: DEFAULT_PAGE }))
  }, [])

  const query: TransportProfilesListQuery = useMemo(
    () => ({
      page: state.page,
      pageSize: state.pageSize,
      search: state.search,
      isEnabled: state.isEnabled,
      transportType: state.transportType,
      sortBy: state.sortBy,
      sortDirection: state.sortDirection,
    }),
    [
      state.isEnabled,
      state.page,
      state.pageSize,
      state.search,
      state.sortBy,
      state.sortDirection,
      state.transportType,
    ],
  )

  return {
    state,
    query,
    setSearch,
    setIsEnabled,
    setTransportType,
    setSortBy,
    setSortDirection,
    setPage,
    setPageSize,
  }
}
