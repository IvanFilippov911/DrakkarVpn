import { NavLink, useNavigate } from 'react-router-dom'
import { useLogoutMutation } from '../../features/auth/useLogoutMutation'
import './layout.css'

export function Layout({ children }: { children: React.ReactNode }) {
  const navigate = useNavigate()
  const logoutMutation = useLogoutMutation()

  const handleLogout = () => {
    logoutMutation.mutate(undefined, {
      onSuccess: () => {
        navigate('/login', { replace: true })
      },
    })
  }

  return (
    <div className="app-root">
      <header className="app-header">
        <div className="app-header-left">
          <span className="app-title">Drakkar Admin Panel</span>
          <nav className="app-nav">
            <NavLink to="/dashboard">Dashboard</NavLink>
            <NavLink to="/servers">Servers</NavLink>
            <NavLink to="/peers">Peers</NavLink>
            <NavLink to="/users">Users</NavLink>
            <NavLink to="/tariffs">Tariffs</NavLink>
            <NavLink to="/errors">Errors</NavLink>
          </nav>
        </div>
        <button
          type="button"
          className="logout-button"
          onClick={handleLogout}
          disabled={logoutMutation.isPending}
        >
          {logoutMutation.isPending ? 'Logout…' : 'Logout'}
        </button>
      </header>

      <main className="app-content">{children}</main>
    </div>
  )
}

