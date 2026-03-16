'use client'

import Link from 'next/link'
import { usePathname } from 'next/navigation'
import { cn } from '@/lib/utils'
import { Button } from '@/components/ui/button'
import { LogOut, Server, Users, Activity, CreditCard, AlertTriangle, LayoutDashboard, Layers } from 'lucide-react'

const navItems = [
  { href: '/dashboard', label: 'Dashboard', icon: LayoutDashboard },
  { href: '/servers', label: 'Servers', icon: Server },
  { href: '/users', label: 'Users', icon: Users },
  { href: '/peers', label: 'Peers', icon: Activity },
  { href: '/tariffs', label: 'Tariffs', icon: CreditCard },
  { href: '/errors', label: 'Errors', icon: AlertTriangle },
  { href: '/layout-options', label: 'Layout Options', icon: Layers },
]

export function TopNav() {
  const pathname = usePathname()

  return (
    <header className="sticky top-0 z-50 border-b bg-background">
      <div className="flex h-12 items-center px-4 gap-6">
        <Link href="/dashboard" className="flex items-center gap-2 font-semibold text-sm">
          <div className="size-6 rounded bg-foreground flex items-center justify-center">
            <span className="text-background text-xs font-bold">D</span>
          </div>
          <span>Drakkar Admin</span>
        </Link>

        <nav className="flex items-center gap-1">
          {navItems.map((item) => {
            const isActive = pathname === item.href || pathname.startsWith(item.href + '/')
            const Icon = item.icon
            return (
              <Link
                key={item.href}
                href={item.href}
                className={cn(
                  'flex items-center gap-1.5 px-3 py-1.5 text-sm rounded-md transition-colors',
                  isActive
                    ? 'bg-secondary text-foreground font-medium'
                    : 'text-muted-foreground hover:text-foreground hover:bg-secondary/50'
                )}
              >
                <Icon className="size-4" />
                {item.label}
              </Link>
            )
          })}
        </nav>

        <div className="ml-auto">
          <Button variant="ghost" size="sm" className="text-muted-foreground hover:text-foreground">
            <LogOut className="size-4 mr-1.5" />
            Logout
          </Button>
        </div>
      </div>
    </header>
  )
}
