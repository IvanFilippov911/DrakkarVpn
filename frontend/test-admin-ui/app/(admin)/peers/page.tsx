import { PageHeader } from '@/components/admin/page-header'
import { StatusBadge } from '@/components/admin/status-badge'
import { Button } from '@/components/ui/button'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from '@/components/ui/dropdown-menu'
import { mockPeers, mockDashboardOverview } from '@/lib/mock-data'
import { formatBytes, formatRelativeTime, formatDate, formatNumber } from '@/lib/format'
import { MoreHorizontal, Trash2, RefreshCw } from 'lucide-react'
import { MetricCard, MetricRow } from '@/components/admin/metric-card'

export default function PeersPage() {
  const peers = mockPeers
  const overview = mockDashboardOverview

  return (
    <div>
      <PageHeader 
        title="Peers" 
        description="VPN connection peers and their status"
      />

      {/* Summary Cards */}
      <div className="grid grid-cols-4 gap-4 mb-6">
        <MetricCard title="Peers">
          <MetricRow label="Total Active" value={formatNumber(overview.totalActivePeers)} />
          <MetricRow label="Online Now" value={formatNumber(overview.peersOnline)} highlight />
        </MetricCard>
        <MetricCard title="Infrastructure">
          <MetricRow label="Total Servers" value={overview.totalServers} />
          <MetricRow label="Servers Online" value={overview.serversOnline} highlight />
        </MetricCard>
        <MetricCard title="Performance">
          <MetricRow label="Avg Speed" value={`${overview.avgVpnSpeedMbps?.toFixed(1) ?? '—'} Mbps`} />
          <MetricRow label="Avg Latency" value={`${overview.avgInfraLatencyMs?.toFixed(1) ?? '—'} ms`} />
        </MetricCard>
        <MetricCard title="Traffic">
          <MetricRow label="Today" value={formatBytes(overview.trafficTodayBytes)} />
          <MetricRow label="Last 24h" value={formatBytes(overview.trafficLast24hBytes)} />
        </MetricCard>
      </div>

      {/* Peers Table */}
      <div className="border rounded-lg">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Peer ID</TableHead>
              <TableHead>User</TableHead>
              <TableHead>Server</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Last Handshake</TableHead>
              <TableHead className="text-right">Upload</TableHead>
              <TableHead className="text-right">Download</TableHead>
              <TableHead>Created</TableHead>
              <TableHead className="w-10"></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {peers.map((peer) => (
              <TableRow key={peer.id}>
                <TableCell className="font-mono text-xs">{peer.id}</TableCell>
                <TableCell>{peer.userEmail}</TableCell>
                <TableCell className="text-muted-foreground">{peer.serverName}</TableCell>
                <TableCell>
                  <StatusBadge status={peer.status} />
                </TableCell>
                <TableCell className="text-muted-foreground">
                  {formatRelativeTime(peer.lastHandshakeAt)}
                </TableCell>
                <TableCell className="text-right tabular-nums text-muted-foreground">
                  {formatBytes(peer.trafficUpBytes)}
                </TableCell>
                <TableCell className="text-right tabular-nums text-muted-foreground">
                  {formatBytes(peer.trafficDownBytes)}
                </TableCell>
                <TableCell className="text-muted-foreground">
                  {formatDate(peer.createdAt)}
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
                        <RefreshCw className="size-4 mr-2" />
                        Reset Connection
                      </DropdownMenuItem>
                      <DropdownMenuItem variant="destructive">
                        <Trash2 className="size-4 mr-2" />
                        Delete Peer
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
