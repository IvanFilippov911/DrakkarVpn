'use client'

import { useState } from 'react'
import { PageHeader } from '@/components/admin/page-header'
import { Button } from '@/components/ui/button'
import { MetricCard, MetricRow, KpiCard } from '@/components/admin/metric-card'
import { StatusBadge, ReachableBadge } from '@/components/admin/status-badge'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { mockServers, mockDashboardOverview } from '@/lib/mock-data'
import { formatBytes, formatDecimal, formatNumber } from '@/lib/format'
import { Plus, LayoutGrid, Rows3, TableIcon } from 'lucide-react'

type LayoutOption = 'cards-above' | 'inline-kpi' | 'minimal'

export default function LayoutOptionsPage() {
  const [layout, setLayout] = useState<LayoutOption>('cards-above')
  const servers = mockServers
  const overview = mockDashboardOverview

  return (
    <div>
      <PageHeader 
        title="Layout Options Demo" 
        description="Compare different layout approaches for admin tables"
        action={
          <div className="flex items-center gap-2">
            <div className="flex border rounded-md">
              <Button 
                variant={layout === 'cards-above' ? 'secondary' : 'ghost'} 
                size="sm"
                onClick={() => setLayout('cards-above')}
              >
                <LayoutGrid className="size-4 mr-1.5" />
                Cards Above
              </Button>
              <Button 
                variant={layout === 'inline-kpi' ? 'secondary' : 'ghost'} 
                size="sm"
                onClick={() => setLayout('inline-kpi')}
              >
                <Rows3 className="size-4 mr-1.5" />
                Inline KPI
              </Button>
              <Button 
                variant={layout === 'minimal' ? 'secondary' : 'ghost'} 
                size="sm"
                onClick={() => setLayout('minimal')}
              >
                <TableIcon className="size-4 mr-1.5" />
                Minimal
              </Button>
            </div>
            <Button size="sm">
              <Plus className="size-4 mr-1.5" />
              Add Server
            </Button>
          </div>
        }
      />

      {/* Layout Option 1: Cards Above Table */}
      {layout === 'cards-above' && (
        <>
          <div className="grid grid-cols-3 gap-4 mb-6">
            <MetricCard title="Infrastructure">
              <MetricRow label="Total Servers" value={overview.totalServers} />
              <MetricRow label="Servers Online" value={overview.serversOnline} highlight />
              <MetricRow label="Peers Online" value={formatNumber(overview.peersOnline)} />
              <MetricRow label="Active Peers" value={formatNumber(overview.totalActivePeers)} />
            </MetricCard>
            <MetricCard title="Performance">
              <MetricRow label="Avg VPN Speed" value={formatDecimal(overview.avgVpnSpeedMbps)} secondary="Mbps" />
              <MetricRow label="Avg Latency" value={formatDecimal(overview.avgInfraLatencyMs)} secondary="ms" />
            </MetricCard>
            <MetricCard title="Traffic">
              <MetricRow label="Today" value={formatBytes(overview.trafficTodayBytes)} />
              <MetricRow label="Last 24h" value={formatBytes(overview.trafficLast24hBytes)} />
            </MetricCard>
          </div>
        </>
      )}

      {/* Layout Option 2: Inline KPI Bar */}
      {layout === 'inline-kpi' && (
        <div className="flex border rounded-lg mb-6 bg-card">
          <KpiCard label="Servers Online" value={overview.serversOnline} subValue={`/ ${overview.totalServers}`} />
          <KpiCard label="Peers Online" value={formatNumber(overview.peersOnline)} />
          <KpiCard label="Avg Speed" value={formatDecimal(overview.avgVpnSpeedMbps)} subValue="Mbps" />
          <KpiCard label="Avg Latency" value={formatDecimal(overview.avgInfraLatencyMs)} subValue="ms" />
          <KpiCard label="Traffic Today" value={formatBytes(overview.trafficTodayBytes)} />
        </div>
      )}

      {/* Layout Option 3: Minimal - Just Table */}
      {layout === 'minimal' && (
        <div className="mb-4 flex items-center gap-4 text-sm">
          <div className="flex items-center gap-2">
            <span className="text-muted-foreground">Servers:</span>
            <span className="font-medium">{overview.serversOnline} / {overview.totalServers}</span>
          </div>
          <div className="h-4 w-px bg-border" />
          <div className="flex items-center gap-2">
            <span className="text-muted-foreground">Peers:</span>
            <span className="font-medium">{formatNumber(overview.peersOnline)}</span>
          </div>
          <div className="h-4 w-px bg-border" />
          <div className="flex items-center gap-2">
            <span className="text-muted-foreground">Speed:</span>
            <span className="font-medium">{formatDecimal(overview.avgVpnSpeedMbps)} Mbps</span>
          </div>
        </div>
      )}

      {/* Servers Table - Same across all layouts */}
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
            </TableRow>
          </TableHeader>
          <TableBody>
            {servers.map((server) => (
              <TableRow key={server.id}>
                <TableCell className="font-medium">{server.name}</TableCell>
                <TableCell className="text-muted-foreground">{server.region}</TableCell>
                <TableCell><StatusBadge status={server.status} /></TableCell>
                <TableCell><ReachableBadge reachable={server.reachable} /></TableCell>
                <TableCell className="text-right tabular-nums">{server.onlinePeers ?? '—'}</TableCell>
                <TableCell className="text-right tabular-nums">{server.peersActive}</TableCell>
                <TableCell className="text-right tabular-nums text-muted-foreground">{server.maxPeers ?? '—'}</TableCell>
                <TableCell className="text-right tabular-nums">{server.vpnSpeedMbps > 0 ? formatDecimal(server.vpnSpeedMbps) : '—'}</TableCell>
                <TableCell className="text-right tabular-nums">{server.infraLatencyMs > 0 ? formatDecimal(server.infraLatencyMs) : '—'}</TableCell>
                <TableCell className="text-right tabular-nums text-muted-foreground">{formatBytes(server.trafficLast1hBytes)}</TableCell>
                <TableCell className="text-right tabular-nums text-muted-foreground">{formatBytes(server.trafficLast24hBytes)}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>
    </div>
  )
}
