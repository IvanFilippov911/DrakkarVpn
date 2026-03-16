import { PageHeader } from '@/components/admin/page-header'
import { MetricCard, MetricRow } from '@/components/admin/metric-card'
import { StatusBadge, ReachableBadge } from '@/components/admin/status-badge'
import { Button } from '@/components/ui/button'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from '@/components/ui/dropdown-menu'
import { mockServers, mockDashboardOverview } from '@/lib/mock-data'
import { formatBytes, formatDecimal, formatNumber } from '@/lib/format'
import { Plus, MoreHorizontal, Trash2 } from 'lucide-react'

export default function ServersPage() {
  const servers = mockServers
  const overview = mockDashboardOverview

  return (
    <div>
      <PageHeader 
        title="Servers" 
        description="VPN server fleet management and monitoring"
        action={
          <Button size="sm">
            <Plus className="size-4 mr-1.5" />
            Add Server
          </Button>
        }
      />

      {/* Summary Cards */}
      <div className="grid grid-cols-3 gap-4 mb-6">
        <MetricCard title="Infrastructure">
          <MetricRow 
            label="Total Servers" 
            value={overview.totalServers} 
          />
          <MetricRow 
            label="Servers Online" 
            value={overview.serversOnline} 
            highlight
          />
          <MetricRow 
            label="Peers Online" 
            value={formatNumber(overview.peersOnline)} 
          />
          <MetricRow 
            label="Active Peers" 
            value={formatNumber(overview.totalActivePeers)} 
          />
        </MetricCard>

        <MetricCard title="Performance">
          <MetricRow 
            label="Avg VPN Speed" 
            value={formatDecimal(overview.avgVpnSpeedMbps)} 
            secondary="Mbps" 
          />
          <MetricRow 
            label="Avg Latency" 
            value={formatDecimal(overview.avgInfraLatencyMs)} 
            secondary="ms" 
          />
        </MetricCard>

        <MetricCard title="Traffic">
          <MetricRow 
            label="Today" 
            value={formatBytes(overview.trafficTodayBytes)} 
          />
          <MetricRow 
            label="Last 24h" 
            value={formatBytes(overview.trafficLast24hBytes)} 
          />
        </MetricCard>
      </div>

      {/* Servers Table */}
      <div className="border rounded-lg">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Server</TableHead>
              <TableHead>Region</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Reachable</TableHead>
              <TableHead className="text-right">Online</TableHead>
              <TableHead className="text-right">Active</TableHead>
              <TableHead className="text-right">Max</TableHead>
              <TableHead className="text-right">Speed</TableHead>
              <TableHead className="text-right">Latency</TableHead>
              <TableHead className="text-right">1h Traffic</TableHead>
              <TableHead className="text-right">24h Traffic</TableHead>
              <TableHead className="w-10"></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {servers.map((server) => (
              <TableRow key={server.id}>
                <TableCell className="font-medium">{server.name}</TableCell>
                <TableCell className="text-muted-foreground">{server.region}</TableCell>
                <TableCell>
                  <StatusBadge status={server.status} />
                </TableCell>
                <TableCell>
                  <ReachableBadge reachable={server.reachable} />
                </TableCell>
                <TableCell className="text-right tabular-nums">
                  {server.onlinePeers !== null ? server.onlinePeers : '—'}
                </TableCell>
                <TableCell className="text-right tabular-nums">{server.peersActive}</TableCell>
                <TableCell className="text-right tabular-nums text-muted-foreground">
                  {server.maxPeers !== null ? server.maxPeers : '—'}
                </TableCell>
                <TableCell className="text-right tabular-nums">
                  {server.vpnSpeedMbps > 0 ? formatDecimal(server.vpnSpeedMbps) : '—'}
                </TableCell>
                <TableCell className="text-right tabular-nums">
                  {server.infraLatencyMs > 0 ? formatDecimal(server.infraLatencyMs) : '—'}
                </TableCell>
                <TableCell className="text-right tabular-nums text-muted-foreground">
                  {formatBytes(server.trafficLast1hBytes)}
                </TableCell>
                <TableCell className="text-right tabular-nums text-muted-foreground">
                  {formatBytes(server.trafficLast24hBytes)}
                </TableCell>
                <TableCell>
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button variant="ghost" size="icon-sm">
                        <MoreHorizontal className="size-4" />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                      <DropdownMenuItem variant="destructive">
                        <Trash2 className="size-4 mr-2" />
                        Delete Server
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
