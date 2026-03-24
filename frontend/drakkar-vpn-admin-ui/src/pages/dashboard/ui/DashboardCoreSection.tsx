import { Skeleton } from '../../../shared/ui'
import { formatDecimal, formatNumber, formatPercent } from '../../../shared/lib/formatters'

function CoreMetric({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide">
        {label}
      </div>
      <div className="text-lg font-semibold tabular-nums mt-1">{value}</div>
    </div>
  )
}

export function DashboardCoreSection({
  core,
}: {
  core: {
    rps: number
    avgLatencyMs: number
    errorRatePct: number
    totalRequests: number
    totalErrors: number
    windowStartUtc: string
    windowEndUtc: string
    errorsByArea: Record<string, number>
  }
}) {
  return (
    <div>
      <div className="grid grid-cols-5 gap-6">
        <CoreMetric label="RPS" value={formatDecimal(core.rps)} />
        <CoreMetric label="Avg Latency" value={`${formatDecimal(core.avgLatencyMs)} ms`} />
        <CoreMetric label="Error Rate" value={`${formatDecimal(core.errorRatePct)}%`} />
        <CoreMetric label="Total Requests" value={formatNumber(core.totalRequests)} />
        <CoreMetric label="Total Errors" value={formatNumber(core.totalErrors)} />
      </div>

      <div className="mt-6">
        <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-3">
          Errors by area
        </div>

        <div className="space-y-2">
          {Object.entries(core.errorsByArea)
            .sort((a, b) => b[1] - a[1])
            .slice(0, 5)
            .map(([area, count]) => {
              const percent = core.totalErrors === 0 ? 0 : (count / core.totalErrors) * 100

              return (
                <div key={area} className="flex items-center justify-between text-sm">
                  <div className="text-muted-foreground">{area}</div>
                  <div className="tabular-nums">
                    {formatNumber(count)} ({formatPercent(percent)})
                  </div>
                </div>
              )
            })}
        </div>
      </div>
    </div>
  )
}

export function DashboardCoreSkeleton() {
  return (
    <div>
      <div className="grid grid-cols-5 gap-6">
        {Array.from({ length: 5 }).map((_, idx) => (
          <div key={idx}>
            <Skeleton className="h-3 w-20" />
            <Skeleton className="h-6 w-16 mt-2" />
          </div>
        ))}
      </div>

      <div className="mt-6">
        <Skeleton className="h-3 w-32 mb-4" />
        <div className="space-y-2">
          {Array.from({ length: 5 }).map((_, idx) => (
            <div key={idx} className="flex items-center justify-between">
              <Skeleton className="h-4 w-32" />
              <Skeleton className="h-4 w-28" />
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}

