import { useMemo, useState } from 'react'
import { PageHeader } from '../../shared/ui'
import { useUsersPageState } from './useUsersPageState'
import { UsersBulkActionsBar } from './ui/UsersBulkActionsBar'
import { UsersFiltersRow } from './ui/UsersFiltersRow'
import { UsersPagination } from './ui/UsersPagination'
import { UsersSummaryRow } from './ui/UsersSummaryRow'
import { UsersTableSection } from './ui/UsersTableSection'
import type { UiState, UserRow } from './ui/types'
import { useUsersListQuery, useUsersOverviewQuery } from '../../entities/users'
import {
  formatMaybeBytes,
  formatMaybeDateTimeUtc,
  formatMaybeNumber,
  formatMaybeSubscriptionStatus,
  formatMaybeTelegram,
  formatMaybeUsername,
} from './ui/formatters'
import { BanUsersDialogShell } from './ui/BanUsersDialogShell'
import { UnbanUsersDialogShell } from './ui/UnbanUsersDialogShell'
import { GrantSubscriptionDialogShell } from './ui/GrantSubscriptionDialogShell'
import { MarkInternalDialogShell } from './ui/MarkInternalDialogShell'
import type { UsersBulkAction } from './types'

export function UsersPage() {
  const pageState = useUsersPageState()
  const [dialog, setDialog] = useState<
    | { type: 'ban'; userIds: string[] }
    | { type: 'unban'; userIds: string[] }
    | { type: 'grantSubscription'; userIds: string[] }
    | { type: 'markInternal'; userIds: string[] }
    | null
  >(null)

  const overviewQuery = useUsersOverviewQuery()
  const usersQuery = useUsersListQuery(pageState.query)

  const summaryState: UiState =
    overviewQuery.isLoading && !overviewQuery.data
      ? 'loading'
      : overviewQuery.isError && !overviewQuery.data
        ? 'error'
        : 'success'

  const tableState: UiState =
    usersQuery.isLoading && !usersQuery.data
      ? 'loading'
      : usersQuery.isError && !usersQuery.data
        ? 'error'
        : usersQuery.data && usersQuery.data.items.length === 0
          ? 'empty'
          : 'success'

  const summary = useMemo(() => {
    const d = overviewQuery.data
    if (!d) return undefined

    return {
      totalUsers: formatMaybeNumber(d.totalUsers),
      activeSubscriptions: formatMaybeNumber(d.activeSubscriptions),
      expiringSoonDays3: formatMaybeNumber(d.expiringSoonDays3),
      onlineUsersNow: formatMaybeNumber(d.onlineUsersNow),
    }
  }, [overviewQuery.data])

  const rows = useMemo<UserRow[]>(() => {
    const items = usersQuery.data?.items ?? []

    return items.map((dto) => ({
      id: dto.user.id,
      telegram: formatMaybeTelegram(dto.user.telegram),
      username: formatMaybeUsername(dto.user.username),
      createdAt: formatMaybeDateTimeUtc(dto.user.createdAtUtc),
      status: dto.user.status,
      online: dto.user.isOnline,
      devices: formatMaybeNumber(dto.user.deviceCount),
      lastSeen: formatMaybeDateTimeUtc(dto.user.lastSeenUtc),
      subscriptionStatus: formatMaybeSubscriptionStatus(dto.subscription.lastSubscriptionStatus),
      subscriptionEnd: formatMaybeDateTimeUtc(dto.subscription.endAtUtc),
      maxDevices: formatMaybeNumber(dto.subscription.maxDevices),
      traffic24h: formatMaybeBytes(dto.trafficLast24hBytes),
    }))
  }, [usersQuery.data])

  function closeDialog() {
    setDialog(null)
    pageState.selection.clearSelection()
    pageState.bulkAction.setSelectedAction(null)
  }

  return (
    <div>
      <PageHeader title="Users" />

      <UsersSummaryRow state={summaryState} data={summary} />
      <UsersFiltersRow
        filters={pageState.filters}
        sorting={pageState.sorting}
        onSearchChange={pageState.setSearch}
        onStatusChange={pageState.setStatus}
        onSubscriptionStatusChange={pageState.setSubscriptionStatus}
        onSortByChange={pageState.setSortBy}
        onDirectionChange={pageState.setUserSortDirection}
      />
      <UsersBulkActionsBar
        selectedCount={pageState.selection.selectedUserIds.length}
        selectedAction={pageState.bulkAction.selectedAction ?? ''}
        onActionChange={(value) => pageState.bulkAction.setSelectedAction(value ? (value as UsersBulkAction) : null)}
        updateDisabled={pageState.bulkAction.isUpdateDisabled || !pageState.bulkAction.selectedAction}
        onUpdate={() => {
          const action = pageState.bulkAction.selectedAction
          if (!action) return
          const ids = pageState.selection.selectedUserIds
          if (ids.length === 0) return
          setDialog({ type: action, userIds: ids })
        }}
      />

      <UsersTableSection
        state={tableState}
        rows={rows}
        selectedUserIds={pageState.selection.selectedUserIds}
        onToggleRow={(id) => pageState.selection.toggleRow(id)}
        onSelectAllCurrentPage={() => pageState.selection.selectAllOnCurrentPage(rows.map((r) => r.id))}
        onClearSelection={() => pageState.selection.clearSelection()}
        onRowAction={(userId, action) => {
          setDialog({ type: action, userIds: [userId] })
        }}
      />

      <UsersPagination
        page={usersQuery.data?.page ?? pageState.pagination.page}
        pageSize={usersQuery.data?.pageSize ?? pageState.pagination.pageSize}
        total={usersQuery.data?.total ?? 0}
        onPrev={() => pageState.setPage(pageState.pagination.page - 1)}
        onNext={() => pageState.setPage(pageState.pagination.page + 1)}
      />

      <BanUsersDialogShell
        open={dialog?.type === 'ban'}
        userIds={dialog?.type === 'ban' ? dialog.userIds : []}
        onClose={closeDialog}
      />
      <UnbanUsersDialogShell
        open={dialog?.type === 'unban'}
        userIds={dialog?.type === 'unban' ? dialog.userIds : []}
        onClose={closeDialog}
      />
      <GrantSubscriptionDialogShell
        open={dialog?.type === 'grantSubscription'}
        userIds={dialog?.type === 'grantSubscription' ? dialog.userIds : []}
        onClose={closeDialog}
      />
      <MarkInternalDialogShell
        open={dialog?.type === 'markInternal'}
        userIds={dialog?.type === 'markInternal' ? dialog.userIds : []}
        onClose={closeDialog}
      />
    </div>
  )
}

