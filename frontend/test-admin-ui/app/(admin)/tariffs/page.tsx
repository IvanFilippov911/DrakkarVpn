import { PageHeader } from '@/components/admin/page-header'
import { StatusBadge } from '@/components/admin/status-badge'
import { Button } from '@/components/ui/button'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuSeparator, DropdownMenuTrigger } from '@/components/ui/dropdown-menu'
import { mockTariffs } from '@/lib/mock-data'
import { formatDate, formatCurrency } from '@/lib/format'
import { MoreHorizontal, Plus, Edit, Power, PowerOff } from 'lucide-react'

export default function TariffsPage() {
  const tariffs = mockTariffs

  return (
    <div>
      <PageHeader 
        title="Tariffs" 
        description="VPN subscription plans and pricing"
        action={
          <Button size="sm">
            <Plus className="size-4 mr-1.5" />
            Create Tariff
          </Button>
        }
      />

      {/* Tariffs Table */}
      <div className="border rounded-lg">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Name</TableHead>
              <TableHead className="text-right">Price</TableHead>
              <TableHead className="text-right">Duration</TableHead>
              <TableHead className="text-right">Devices</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Created</TableHead>
              <TableHead className="w-10"></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {tariffs.map((tariff) => (
              <TableRow key={tariff.id}>
                <TableCell className="font-medium">{tariff.name}</TableCell>
                <TableCell className="text-right tabular-nums">
                  {formatCurrency(tariff.price)}
                </TableCell>
                <TableCell className="text-right tabular-nums">
                  {tariff.durationDays} days
                </TableCell>
                <TableCell className="text-right tabular-nums">
                  {tariff.defaultMaxDevices}
                </TableCell>
                <TableCell>
                  <StatusBadge status={tariff.status} />
                </TableCell>
                <TableCell className="text-muted-foreground">
                  {formatDate(tariff.createdAt)}
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
                        <Edit className="size-4 mr-2" />
                        Edit Tariff
                      </DropdownMenuItem>
                      <DropdownMenuSeparator />
                      {tariff.status === 'active' ? (
                        <DropdownMenuItem variant="destructive">
                          <PowerOff className="size-4 mr-2" />
                          Disable Tariff
                        </DropdownMenuItem>
                      ) : (
                        <DropdownMenuItem>
                          <Power className="size-4 mr-2" />
                          Enable Tariff
                        </DropdownMenuItem>
                      )}
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
