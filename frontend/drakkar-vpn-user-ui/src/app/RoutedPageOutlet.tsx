import { Outlet, useLocation } from 'react-router-dom'
import { PageEntrance } from '../shared/ui/page-entrance/PageEntrance'

/** One entrance animation per route; stable flex shell for all tab pages. */
export function RoutedPageOutlet() {
  const { pathname } = useLocation()
  return (
    <PageEntrance key={pathname}>
      <Outlet />
    </PageEntrance>
  )
}
