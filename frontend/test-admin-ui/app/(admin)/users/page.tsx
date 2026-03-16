import { PageHeader } from '@/components/admin/page-header'
import { StatusBadge } from '@/components/admin/status-badge'
import { Button } from '@/components/ui/button'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuSeparator, DropdownMenuTrigger } from '@/components/ui/dropdown-menu'
import { mockUsers, mockDashboardOverview } from '@/lib/mock-data'
import { formatDate, formatRelativeTime, formatNumber } from '@/lib/format'
import { MoreHorizontal, UserPlus, Eye, Edit, Ban, RefreshCw } from 'lucide-react'
import { MetricCard, MetricRow } from '@/components/admin/metric-card'

export default function UsersPage() {
  const users = mockUsers
  const overview = mockDashboardOverview

  return (
    <div>
      <PageHeader 
        title="Users" 
        description="User accounts and subscription management"
        action={
          <Button size="sm">
            <UserPlus className="size-4 mr-1.5" />
            Add User
          </Button>
        }
      />

      {/* Summary Cards */}
      <div className="grid grid-cols-4 gap-4 mb-6">
        <MetricCard title="Users">
          <MetricRow label="Total Users" value={formatNumber(overview.totalUsers)} />
          <MetricRow label="Online Now" value={overview.onlineUsersNow} highlight />
        </MetricCard>
        <MetricCard title="Subscriptions">
          <MetricRow label="Active" value={formatNumber(overview.activeSubscriptions)} highlight />
          <MetricRow label="Expiring ≤3d" value={overview.expiringSoonDays3} />
        </MetricCard>
        <MetricCard title="Peers">
          <MetricRow label="Total Active" value={formatNumber(overview.totalActivePeers)} />
          <MetricRow label="Online" value={formatNumber(overview.peersOnline)} highlight />
        </MetricCard>
        <MetricCard title="Traffic">
          <MetricRow label="Today" value={`${Math.round(overview.trafficTodayBytes / (1024 ** 3))} GB`} />
          <MetricRow label="Last 24h" value={`${Math.round(overview.trafficLast24hBytes / (1024 ** 3))} GB`} />
        </MetricCard>
      </div>

      {/* Users Table */}
      <div className="border rounded-lg">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Email</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Subscription</TableHead>
              <TableHead>Expires</TableHead>
              <TableHead className="text-right">Devices</TableHead>
              <TableHead>Created</TableHead>
              <TableHead>Last Active</TableHead>
              <TableHead className="w-10"></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {users.map((user) => (
              <TableRow key={user.id}>
                <TableCell className="font-medium">{user.email}</TableCell>
                <TableCell>
                  <StatusBadge status={user.status} />
                </TableCell>
                <TableCell>
                  {user.subscription ? (
                    <div className="flex items-center gap-2">
                      <span>{user.subscription.tariffName}</span>
                      <StatusBadge status={user.subscription.status} />
                    </div>
                  ) : (
                    <span className="text-muted-foreground">—</span>
                  )}
                </TableCell>
                <TableCell className="text-muted-foreground">
                  {user.subscription ? formatDate(user.subscription.expiresAt) : '—'}
                </TableCell>
                <TableCell className="text-right tabular-nums">
                  {user.devices} / {user.maxDevices}
                </TableCell>
                <TableCell className="text-muted-foreground">
                  {formatDate(user.createdAt)}
                </TableCell>
                <TableCell className="text-muted-foreground">
                  {formatRelativeTime(user.lastActiveAt)}
                </TableCell>
                <TableCell>
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button variant="ghost" size="icon-sm">
                        <MoreHorizontal className="size-4" />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                      <DropdownMenuItem>
                        <Eye className="size-4 mr-2" />
                        View Details
                      </DropdownMenuItem>
                      <DropdownMenuItem>
                        <Edit className="size-4 mr-2" />
                        Edit User
                      </DropdownMenuItem>
                      <DropdownMenuItem>
                        <RefreshCw className="size-4 mr-2" />
                        Reset Password
                      </DropdownMenuItem>
                      <DropdownMenuSeparator />
                      <DropdownMenuItem variant="destructive">
                        <Ban className="size-4 mr-2" />
                        Suspend User
                      </DropdownMenuItem>
                    </DropdownMenuContent>
                  </DropdownMenu>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>
    </div>
  )
}
