import { useAdminOverviewQuery, useCoreHealthQuery } from '../../entities/dashboard'
import { formatUtcWindow } from '../../shared/lib/formatters'
import { InlineAlert, PageHeader } from '../../shared/ui'
import { DashboardCoreSection, DashboardCoreSkeleton } from './ui/DashboardCoreSection'
import { DashboardOverviewSection } from './ui/DashboardOverviewSection'
import { DashboardOverviewSkeleton } from './ui/DashboardOverviewSkeleton'

export function DashboardPage() {
  const overviewQuery = useAdminOverviewQuery()
  const coreQuery = useCoreHealthQuery()

  return (
    <div>
      <PageHeader
        title="Dashboard"
        description="System health overview and key metrics."
      />

      {overviewQuery.isLoading && !overviewQuery.data ? (
        <DashboardOverviewSkeleton />
      ) : overviewQuery.isError && !overviewQuery.data ? (
        <OverviewErrorCard message="Failed to load overview metrics." />
      ) : overviewQuery.data ? (
        <DashboardOverviewSection overview={overviewQuery.data} />
      ) : (
        <OverviewErrorCard message="Overview metrics are unavailable." />
      )}

      <div className="border rounded-lg bg-card">
        <div className="p-4 border-b">
          <div className="text-lg font-semibold">Core Metrics</div>
          <div className="text-sm text-muted-foreground mt-0.5">
            Window:{' '}
            {coreQuery.data
              ? formatUtcWindow(coreQuery.data.windowStartUtc, coreQuery.data.windowEndUtc)
              : '—'}
          </div>
        </div>

        <div className="p-4">
          {coreQuery.isLoading && !coreQuery.data ? (
            <DashboardCoreSkeleton />
          ) : coreQuery.isError && !coreQuery.data ? (
            <InlineAlert message="Failed to load core metrics." />
          ) : coreQuery.data ? (
            <DashboardCoreSection core={coreQuery.data} />
          ) : (
            <InlineAlert message="Core metrics are unavailable." />
          )}
        </div>
      </div>
    </div>
  )
}

function OverviewErrorCard({ message }: { message: string }) {
  return (
    <div className="mb-6">
      <InlineAlert message={message} />
    </div>
  )
}
