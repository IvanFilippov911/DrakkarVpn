import type { ReactElement } from 'react'
import { useEffect } from 'react'
import { BrowserRouter, Navigate, Outlet, Route, Routes } from 'react-router-dom'
import { Layout } from '../layout/Layout'
import { LoginPage } from '../../pages/auth/LoginPage'
import { DashboardPage } from '../../pages/dashboard/DashboardPage'
import { ServersPage } from '../../pages/servers/ServersPage'
import { PeersPage } from '../../pages/peers/PeersPage'
import { UserDetailPage } from '../../pages/users/UserDetailPage'
import { UsersPage } from '../../pages/users/UsersPage'
import { TariffsPage } from '../../pages/tariffs/TariffsPage'
import { ErrorsPage } from '../../pages/errors/ErrorsPage'
import { getStoredAccessToken } from '../../shared/lib/authTokenStorage'
import { clearSession } from '../../entities/auth/session'
import { useCurrentAdminQuery } from '../../entities/auth'

type AuthGuardProps = {
  children: ReactElement
}

function AuthGuard({ children }: AuthGuardProps) {
  const token = getStoredAccessToken()
  const { isPending, isError } = useCurrentAdminQuery({
    enabled: !!token,
  })

  useEffect(() => {
    if (isError) {
      clearSession()
    }
  }, [isError])

  if (!token) {
    return <Navigate to="/login" replace />
  }

  if (isPending) {
    return (
      <div style={{ padding: '2rem', textAlign: 'center' }}>
        Loading…
      </div>
    )
  }

  if (isError) {
    return <Navigate to="/login" replace />
  }

  return children
}

function ProtectedLayout() {
  return (
    <AuthGuard>
      <Layout>
        <Outlet />
      </Layout>
    </AuthGuard>
  )
}

export function AppRouter() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />

        <Route element={<ProtectedLayout />}>
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="/servers" element={<ServersPage />} />
          <Route path="/servers/:serverId/peers" element={<PeersPage />} />
          <Route path="/users" element={<UsersPage />} />
          <Route path="/users/:userId" element={<UserDetailPage />} />
          <Route path="/tariffs" element={<TariffsPage />} />
          <Route path="/errors" element={<ErrorsPage />} />
        </Route>

        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </BrowserRouter>
  )
}

