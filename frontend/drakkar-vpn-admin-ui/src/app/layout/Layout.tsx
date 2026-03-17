import { NavLink, useNavigate } from 'react-router-dom'
import { useLogoutMutation } from '../../features/auth/useLogoutMutation'

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
    <div className="min-h-screen bg-background text-foreground">
      <header className="sticky top-0 z-50 border-b bg-background">
        <div className="flex h-12 items-center px-6 gap-6">
          <div className="flex items-center gap-6">
            <span className="text-sm font-semibold tracking-wide">
              Drakkar Admin Panel
            </span>

            <nav className="flex items-center gap-1">
              <NavLink
                to="/dashboard"
                className={({ isActive }) =>
                  [
                    'flex items-center gap-1.5 px-3 py-1.5 text-sm rounded-md',
                    isActive
                      ? 'bg-secondary text-foreground font-medium'
                      : 'text-muted-foreground hover:bg-secondary/50 hover:text-foreground',
                  ].join(' ')
                }
              >
                Dashboard
              </NavLink>
              <NavLink
                to="/servers"
                className={({ isActive }) =>
                  [
                    'flex items-center gap-1.5 px-3 py-1.5 text-sm rounded-md',
                    isActive
                      ? 'bg-secondary text-foreground font-medium'
                      : 'text-muted-foreground hover:bg-secondary/50 hover:text-foreground',
                  ].join(' ')
                }
              >
                Servers
              </NavLink>
              <NavLink
                to="/peers"
                className={({ isActive }) =>
                  [
                    'flex items-center gap-1.5 px-3 py-1.5 text-sm rounded-md',
                    isActive
                      ? 'bg-secondary text-foreground font-medium'
                      : 'text-muted-foreground hover:bg-secondary/50 hover:text-foreground',
                  ].join(' ')
                }
              >
                Peers
              </NavLink>
              <NavLink
                to="/users"
                className={({ isActive }) =>
                  [
                    'flex items-center gap-1.5 px-3 py-1.5 text-sm rounded-md',
                    isActive
                      ? 'bg-secondary text-foreground font-medium'
                      : 'text-muted-foreground hover:bg-secondary/50 hover:text-foreground',
                  ].join(' ')
                }
              >
                Users
              </NavLink>
              <NavLink
                to="/tariffs"
                className={({ isActive }) =>
                  [
                    'flex items-center gap-1.5 px-3 py-1.5 text-sm rounded-md',
                    isActive
                      ? 'bg-secondary text-foreground font-medium'
                      : 'text-muted-foreground hover:bg-secondary/50 hover:text-foreground',
                  ].join(' ')
                }
              >
                Tariffs
              </NavLink>
              <NavLink
                to="/errors"
                className={({ isActive }) =>
                  [
                    'flex items-center gap-1.5 px-3 py-1.5 text-sm rounded-md',
                    isActive
                      ? 'bg-secondary text-foreground font-medium'
                      : 'text-muted-foreground hover:bg-secondary/50 hover:text-foreground',
                  ].join(' ')
                }
              >
                Errors
              </NavLink>
            </nav>
          </div>

          <div className="ml-auto">
            <button
              type="button"
              className="h-8 px-3 rounded-md text-sm font-medium text-muted-foreground hover:bg-accent hover:text-foreground disabled:opacity-60"
              onClick={handleLogout}
              disabled={logoutMutation.isPending}
            >
              {logoutMutation.isPending ? 'Logout…' : 'Logout'}
            </button>
          </div>
        </div>
      </header>

      <main className="p-6">
        <div className="max-w-7xl mx-auto">{children}</div>
      </main>
    </div>
  )
}

