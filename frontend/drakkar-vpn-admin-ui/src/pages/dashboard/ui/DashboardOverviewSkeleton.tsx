import { Skeleton } from '../../../shared/ui'

export function DashboardOverviewSkeleton() {
  return (
    <div>
      <div className="flex border rounded-lg bg-card mb-6 overflow-hidden">
        {Array.from({ length: 4 }).map((_, idx) => (
          <div
            key={idx}
            className={['px-4 py-3 w-1/4', idx === 3 ? '' : 'border-r'].join(' ')}
          >
            <Skeleton className="h-3 w-24" />
            <Skeleton className="h-6 w-20 mt-2" />
          </div>
        ))}
      </div>

      <div className="grid grid-cols-4 gap-4 mb-6">
        {Array.from({ length: 4 }).map((_, idx) => (
          <div key={idx} className="rounded-lg border bg-card p-4">
            <Skeleton className="h-3 w-28 mb-4" />
            <div className="space-y-2">
              <Skeleton className="h-4 w-full" />
              <Skeleton className="h-4 w-5/6" />
              <Skeleton className="h-4 w-4/6" />
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

