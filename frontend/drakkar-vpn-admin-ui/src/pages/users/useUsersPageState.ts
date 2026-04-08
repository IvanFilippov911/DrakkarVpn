import { useCallback, useMemo, useState } from 'react'
import type { UsersListQuery } from '../../entities/users'
import type {
  UsersBulkAction,
  UsersFiltersState,
  UsersPaginationState,
  UsersSortingState,
} from './types'

const DEFAULT_PAGE_SIZE = 25

const DEFAULT_FILTERS: UsersFiltersState = {
  search: '',
  status: null,
  subscriptionStatus: null,
}

const DEFAULT_SORTING: UsersSortingState = {
  sortBy: 'CreatedAt',
  userSortDirection: 'Desc',
}

const DEFAULT_PAGINATION: UsersPaginationState = {
  page: 1,
  pageSize: DEFAULT_PAGE_SIZE,
}

export type UsersPageState = {
  filters: UsersFiltersState
  setSearch: (value: string) => void
  setStatus: (value: UsersFiltersState['status']) => void
  setSubscriptionStatus: (value: UsersFiltersState['subscriptionStatus']) => void
  resetFilters: () => void

  sorting: UsersSortingState
  setSortBy: (value: UsersSortingState['sortBy']) => void
  setUserSortDirection: (value: UsersSortingState['userSortDirection']) => void
  setSorting: (value: UsersSortingState) => void

  pagination: UsersPaginationState
  setPage: (value: number) => void
  setPageSize: (value: number) => void
  setPagination: (value: UsersPaginationState) => void

  query: UsersListQuery

  selection: {
    selectedUserIds: string[]
    isSelected: (userId: string) => boolean
    selectRow: (userId: string) => void
    unselectRow: (userId: string) => void
    toggleRow: (userId: string) => void
    selectAllOnCurrentPage: (userIds: string[]) => void
    clearSelection: () => void
  }

  bulkAction: {
    selectedAction: UsersBulkAction | null
    setSelectedAction: (value: UsersBulkAction | null) => void
    isUpdateDisabled: boolean
  }
}

export function useUsersPageState(): UsersPageState {
  const [filters, setFilters] = useState<UsersFiltersState>(DEFAULT_FILTERS)
  const [sorting, setSorting] = useState<UsersSortingState>(DEFAULT_SORTING)
  const [pagination, setPagination] = useState<UsersPaginationState>(DEFAULT_PAGINATION)

  const [selectedIds, setSelectedIds] = useState<Set<string>>(() => new Set())
  const [selectedAction, setSelectedAction] = useState<UsersBulkAction | null>(null)

  const selectedUserIds = useMemo(() => Array.from(selectedIds), [selectedIds])

  const query = useMemo<UsersListQuery>(
    () => ({
      search: filters.search.trim().length > 0 ? filters.search.trim() : undefined,
      status: filters.status ?? undefined,
      subscriptionStatus: filters.subscriptionStatus ?? undefined,
      sortBy: sorting.sortBy,
      userSortDirection: sorting.userSortDirection,
      page: pagination.page,
      pageSize: pagination.pageSize,
    }),
    [filters.search, filters.status, filters.subscriptionStatus, sorting, pagination],
  )

  const clearSelection = useCallback(() => {
    setSelectedIds(new Set())
  }, [])

  const isSelected = useCallback((userId: string) => selectedIds.has(userId), [selectedIds])

  const selectRow = useCallback((userId: string) => {
    setSelectedIds((prev) => {
      const next = new Set(prev)
      next.add(userId)
      return next
    })
  }, [])

  const unselectRow = useCallback((userId: string) => {
    setSelectedIds((prev) => {
      if (!prev.has(userId)) return prev
      const next = new Set(prev)
      next.delete(userId)
      return next
    })
  }, [])

  const toggleRow = useCallback((userId: string) => {
    setSelectedIds((prev) => {
      const next = new Set(prev)
      if (next.has(userId)) next.delete(userId)
      else next.add(userId)
      return next
    })
  }, [])

  const selectAllOnCurrentPage = useCallback((userIds: string[]) => {
    setSelectedIds((prev) => {
      const next = new Set(prev)
      for (const id of userIds) next.add(id)
      return next
    })
  }, [])

  const setSearch = useCallback((value: string) => {
    setFilters((prev) => ({ ...prev, search: value }))
    setPagination((prev) => ({ ...prev, page: 1 }))
  }, [])

  const setStatus = useCallback((value: UsersFiltersState['status']) => {
    setFilters((prev) => ({ ...prev, status: value }))
    setPagination((prev) => ({ ...prev, page: 1 }))
  }, [])

  const setSubscriptionStatus = useCallback((value: UsersFiltersState['subscriptionStatus']) => {
    setFilters((prev) => ({ ...prev, subscriptionStatus: value }))
    setPagination((prev) => ({ ...prev, page: 1 }))
  }, [])

  const resetFilters = useCallback(() => {
    setFilters(DEFAULT_FILTERS)
    setPagination((prev) => ({ ...prev, page: 1 }))
  }, [])

  const setSortBy = useCallback((value: UsersSortingState['sortBy']) => {
    setSorting((prev) => ({ ...prev, sortBy: value }))
    setPagination((prev) => ({ ...prev, page: 1 }))
  }, [])

  const setUserSortDirection = useCallback((value: UsersSortingState['userSortDirection']) => {
    setSorting((prev) => ({ ...prev, userSortDirection: value }))
    setPagination((prev) => ({ ...prev, page: 1 }))
  }, [])

  const setPage = useCallback((value: number) => {
    setPagination((prev) => ({ ...prev, page: value }))
  }, [])

  const setPageSize = useCallback((value: number) => {
    setPagination((prev) => ({ ...prev, pageSize: value, page: 1 }))
  }, [])

  const isUpdateDisabled = selectedIds.size === 0

  return {
    filters,
    setSearch,
    setStatus,
    setSubscriptionStatus,
    resetFilters,

    sorting,
    setSortBy,
    setUserSortDirection,
    setSorting,

    pagination,
    setPage,
    setPageSize,
    setPagination,

    query,

    selection: {
      selectedUserIds,
      isSelected,
      selectRow,
      unselectRow,
      toggleRow,
      selectAllOnCurrentPage,
      clearSelection,
    },

    bulkAction: {
      selectedAction,
      setSelectedAction,
      isUpdateDisabled,
    },
  }
}

