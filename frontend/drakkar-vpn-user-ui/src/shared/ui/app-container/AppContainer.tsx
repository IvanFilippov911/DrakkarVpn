import type { ReactNode } from 'react'

type AppContainerProps = {
  children: ReactNode
}

export function AppContainer({ children }: AppContainerProps) {
  return (
    <div className="ui-screen bg-transparent font-sans text-[var(--foreground)] antialiased">
      <div className="ui-content-layer">{children}</div>
    </div>
  )
}
