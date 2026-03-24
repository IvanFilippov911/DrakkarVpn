import { useMemo, useState } from 'react'
import { PageHeader } from '../../shared/ui'
import { AddServerShell } from '../../features/servers/register-server/ui/AddServerShell'
import { ConfirmDialogShell } from '../../features/servers/delete-server/ui/ConfirmDialogShell'
import {
  useServersListQuery,
  useServersOverviewQuery,
} from '../../entities/server'
import type { ServerRow, UiState } from './ui/types'
import { PaginationSection } from './ui/PaginationSection'
import { ServersSummaryRow } from './ui/ServersSummaryRow'
import { ServersTableSection } from './ui/ServersTableSection'
import { useServersListQueryState } from './hooks/useServersListQueryState'
import { mapServersListToRows, mapServersOverviewToSummaryData } from './ui/serversUiMappers'

export function ServersPage() {
  const overviewQuery = useServersOverviewQuery()
  const listState = useServersListQueryState()
  const listQuery = useServersListQuery(listState.query)

  const [isAddServerOpen, setIsAddServerOpen] = useState(false)
  const [deleteConfirmForId, setDeleteConfirmForId] = useState<string | null>(null)

  // For partial error UX we prioritize current request state:
  // - if summary fetch is failing -> show summary error cards (not stale data)
  // - if table fetch is failing -> show table error (independently)
  const summaryState: UiState = overviewQuery.isLoading
    ? 'loading'
    : overviewQuery.isError
      ? 'error'
      : 'success'

  const summaryData = useMemo(() => {
    if (!overviewQuery.data) return undefined

    return mapServersOverviewToSummaryData(overviewQuery.data)
  }, [overviewQuery.data])

  const tableState: UiState = listQuery.isLoading
    ? 'loading'
    : listQuery.isError
      ? 'error'
      : listQuery.data && listQuery.data.items.length === 0
        ? 'empty'
        : 'success'

  const rows = useMemo<ServerRow[]>(() => {
    if (!listQuery.data) return []

    return mapServersListToRows(listQuery.data.items)
  }, [listQuery.data])

  const page = listQuery.data?.page ?? listState.state.page
  const totalPages = listQuery.data?.totalPages ?? 1

  return (
    <div>
      <PageHeader
        title="Servers"
        description="VPN server fleet management and monitoring."
        action={
          <button
            type="button"
            onClick={() => setIsAddServerOpen(true)}
            className="h-9 px-4 rounded-md text-sm font-medium bg-primary text-primary-foreground hover:bg-primary/90"
          >
            Add Server
          </button>
        }
      />

      <ServersSummaryRow state={summaryState} data={summaryData} />

      <FiltersRow
        region={listState.state.region}
        status={listState.state.status}
        pageSize={listState.state.pageSize}
        onRegionChange={listState.setRegion}
        onStatusChange={listState.setStatus}
        onPageSizeChange={listState.setPageSize}
      />

      <ServersTableSection
        state={tableState}
        rows={rows}
        onDelete={(id) => setDeleteConfirmForId(id)}
      />

      <PaginationSection
        page={page}
        totalPages={totalPages}
        onPrev={() => listState.setPage(Math.max(1, page - 1))}
        onNext={() => listState.setPage(Math.min(totalPages, page + 1))}
      />

      <AddServerShell open={isAddServerOpen} onClose={() => setIsAddServerOpen(false)} />

      <ConfirmDialogShell
        open={deleteConfirmForId != null}
        serverId={deleteConfirmForId ?? ''}
        title="Delete server?"
        description="This action is destructive and cannot be undone."
        confirmLabel="Delete"
        onCancel={() => setDeleteConfirmForId(null)}
      />
    </div>
  )
}

function FiltersRow({
  region,
  status,
  pageSize,
  onRegionChange,
  onStatusChange,
  onPageSizeChange,
}: {
  region?: string
  status?: string
  pageSize: number
  onRegionChange: (value?: string) => void
  onStatusChange: (value?: string) => void
  onPageSizeChange: (value: number) => void
}) {
  return (
    <div className="flex items-end gap-3 mb-4">
      <div>
        <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
          Region
        </div>
        <input
          value={region ?? ''}
          onChange={(e) => onRegionChange(e.target.value || undefined)}
          placeholder="e.g. eu-central"
          className="h-9 w-56 rounded-md border bg-background px-3 text-sm"
        />
      </div>

      <div>
        <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
          Status
        </div>
        <input
          value={status ?? ''}
          onChange={(e) => onStatusChange(e.target.value || undefined)}
          placeholder="e.g. Online"
          className="h-9 w-56 rounded-md border bg-background px-3 text-sm"
        />
      </div>

      <div className="ml-auto">
        <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
          Page size
        </div>
        <select
          value={pageSize}
          onChange={(e) => onPageSizeChange(Number(e.target.value))}
          className="h-9 rounded-md border bg-background px-3 text-sm"
        >
          {[10, 20, 50].map((v) => (
            <option key={v} value={v}>
              {v}
            </option>
          ))}
        </select>
      </div>
    </div>
  )
}
