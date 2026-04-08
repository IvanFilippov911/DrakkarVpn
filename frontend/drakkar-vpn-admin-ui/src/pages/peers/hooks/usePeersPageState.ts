import { useMemo, useState } from 'react'
import type { GetServerPeersQuery, ServerPeerSortBy, SortDirection } from '../../../entities/peers'

export type PeersPageUiState = {
  peerId: string
  onlyOnline: 'all' | 'online'
  sortBy: ServerPeerSortBy
  direction: SortDirection
  page: number
  pageSize: number
}

const DEFAULT_STATE: PeersPageUiState = {
  peerId: '',
  onlyOnline: 'all',
  sortBy: 'LastActivity',
  direction: 'Desc',
  page: 1,
  pageSize: 25,
}

export function usePeersPageState() {
  const [state, setState] = useState<PeersPageUiState>(DEFAULT_STATE)

  const query = useMemo<GetServerPeersQuery>(() => {
    const trimmedPeerId = state.peerId.trim()

    return {
      page: state.page,
      pageSize: state.pageSize,
      onlyOnline: state.onlyOnline === 'online' ? true : undefined,
      peerId: trimmedPeerId.length > 0 ? trimmedPeerId : undefined,
      sortBy: state.sortBy,
      peerSortDirection: state.direction,
    }
  }, [state])

  return {
    state,
    query,
    setPeerId: (value: string) => setState((s) => ({ ...s, peerId: value, page: 1 })),
    setOnlyOnline: (value: 'all' | 'online') => setState((s) => ({ ...s, onlyOnline: value, page: 1 })),
    setSortBy: (value: ServerPeerSortBy) => setState((s) => ({ ...s, sortBy: value, page: 1 })),
    setDirection: (value: SortDirection) => setState((s) => ({ ...s, direction: value, page: 1 })),
    setPage: (page: number) => setState((s) => ({ ...s, page })),
    setPageSize: (pageSize: number) => setState((s) => ({ ...s, pageSize, page: 1 })),
  }
}

