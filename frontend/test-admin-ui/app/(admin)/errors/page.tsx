import { PageHeader } from '@/components/admin/page-header'
import { StatusBadge } from '@/components/admin/status-badge'
import { Button } from '@/components/ui/button'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from '@/components/ui/dropdown-menu'
import { mockErrors, mockCoreHealth } from '@/lib/mock-data'
import { formatRelativeTime, formatNumber, formatPercent } from '@/lib/format'
import { MoreHorizontal, Eye, CheckCircle, RefreshCw } from 'lucide-react'
import { MetricCard, MetricRow } from '@/components/admin/metric-card'

export default function ErrorsPage() {
  const errors = mockErrors
  const coreHealth = mockCoreHealth

  return (
    <div>
      <PageHeader 
        title="Errors" 
        description="System errors and diagnostic information"
        action={
          <Button variant="outline" size="sm">
            <RefreshCw className="size-4 mr-1.5" />
            Refresh
          </Button>
        }
      />

      {/* Summary Cards */}
      <div className="grid grid-cols-4 gap-4 mb-6">
        <MetricCard title="Overview">
          <MetricRow label="Total Errors" value={formatNumber(coreHealth.totalErrors)} />
          <MetricRow label="Error Rate" value={formatPercent(coreHealth.errorRatePct)} />
        </MetricCard>
        <MetricCard title="System Health">
          <MetricRow label="RPS" value={coreHealth.rps.toFixed(1)} />
          <MetricRow label="Avg Latency" value={`${coreHealth.avgLatencyMs.toFixed(1)} ms`} />
        </MetricCard>
        <MetricCard title="Requests">
          <MetricRow label="Total" value={formatNumber(coreHealth.totalRequests)} />
          <MetricRow label="Success Rate" value={`${(100 - coreHealth.errorRatePct).toFixed(2)}%`} highlight />
        </MetricCard>
        <MetricCard title="Top Area">
          {Object.entries(coreHealth.errorsByArea)
            .sort(([, a], [, b]) => b - a)
            .slice(0, 2)
            .map(([area, count]) => (
              <MetricRow key={area} label={area} value={formatNumber(count)} />
            ))}
        </MetricCard>
      </div>

      {/* Errors Table */}
      <div className="border rounded-lg">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Level</TableHead>
              <TableHead>Area</TableHead>
              <TableHead className="w-[40%]">Message</TableHead>
              <TableHead className="text-right">Count</TableHead>
              <TableHead>Last Occurred</TableHead>
              <TableHead className="w-10"></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {errors.map((error) => (
              <TableRow key={error.id}>
                <TableCell>
                  <StatusBadge status={error.level} />
                </TableCell>
                <TableCell className="font-medium capitalize">
                  {error.area.replace('-', ' ')}
                </TableCell>
                <TableCell className="font-mono text-xs text-muted-foreground">
                  {error.message}
                </TableCell>
                <TableCell className="text-right tabular-nums font-medium">
                  {formatNumber(error.count)}
                </TableCell>
                <TableCell className="text-muted-foreground">
                  {formatRelativeTime(error.occurredAt)}
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
                        <CheckCircle className="size-4 mr-2" />
                        Mark Resolved
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
