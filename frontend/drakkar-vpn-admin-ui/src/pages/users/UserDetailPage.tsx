import { useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { useUserDetailsQuery } from '../../entities/users'
import { InlineAlert, PageHeader, Skeleton } from '../../shared/ui'
import { BanUsersDialogShell } from './ui/BanUsersDialogShell'
import { GrantSubscriptionDialogShell } from './ui/GrantSubscriptionDialogShell'
import { MarkInternalDialogShell } from './ui/MarkInternalDialogShell'
import { UnbanUsersDialogShell } from './ui/UnbanUsersDialogShell'
import { UserAlertsSection } from './ui/UserAlertsSection'
import { UserDetailActionsBar } from './ui/UserDetailActionsBar'
import { UserDevicesSection } from './ui/UserDevicesSection'
import { UserRealtimeCard } from './ui/UserRealtimeCard'
import { UserSummaryCard } from './ui/UserSummaryCard'

export function UserDetailPage() {
  const { userId } = useParams<{ userId: string }>()
  const detailsQuery = useUserDetailsQuery(userId)
  const [dialog, setDialog] = useState<
    | { type: 'ban'; userIds: string[] }
    | { type: 'unban'; userIds: string[] }
    | { type: 'grantSubscription'; userIds: string[] }
    | { type: 'markInternal'; userIds: string[] }
    | null
  >(null)

  const detailUser = detailsQuery.data?.user

  function closeDialog() {
    setDialog(null)
  }

  return (
    <div>
      <PageHeader
        title="User Detail"
        description="User diagnostics, realtime state, devices, and alerts."
        action={
          <div className="flex flex-col items-end gap-2 sm:flex-row sm:items-center sm:gap-3">
            {userId && detailUser ? (
              <UserDetailActionsBar
                user={detailUser}
                onBan={() => setDialog({ type: 'ban', userIds: [userId] })}
                onUnban={() => setDialog({ type: 'unban', userIds: [userId] })}
                onMarkInternal={() => setDialog({ type: 'markInternal', userIds: [userId] })}
                onGrantSubscription={() => setDialog({ type: 'grantSubscription', userIds: [userId] })}
              />
            ) : null}
            <Link
              to="/users"
              className="text-sm text-muted-foreground hover:text-foreground underline-offset-2 hover:underline shrink-0"
            >
              Back to users
            </Link>
          </div>
        }
      />

      {!userId ? (
        <InlineAlert message="Missing user id in URL." />
      ) : detailsQuery.isLoading && !detailsQuery.data ? (
        <UserDetailLoadingSkeleton />
      ) : detailsQuery.isError && !detailsQuery.data ? (
        <InlineAlert message="Failed to load user details." />
      ) : detailsQuery.data ? (
        <>
          <div className="grid gap-6 lg:grid-cols-2">
            <UserSummaryCard user={detailsQuery.data.user} />
            <UserRealtimeCard realtime={detailsQuery.data.realtime} />
          </div>
          <UserDevicesSection userId={userId} devices={detailsQuery.data.devices} />
          <UserAlertsSection alerts={detailsQuery.data.alerts} />
        </>
      ) : (
        <InlineAlert message="User details are unavailable." />
      )}

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

function UserDetailLoadingSkeleton() {
  return (
    <div className="space-y-3 max-w-xl">
      <Skeleton className="h-4 w-2/3" />
      <Skeleton className="h-4 w-1/2" />
      <Skeleton className="h-24 w-full" />
    </div>
  )
}
