import type { ReactNode } from 'react'

type AppContainerProps = {
  children: ReactNode
}

export function AppContainer({ children }: AppContainerProps) {
  return (
    <div className="ui-screen bg-[var(--background)] font-sans text-[var(--foreground)] antialiased">
      <div className="ui-gradient-layer" aria-hidden />
      <div className="ui-radial-layer" aria-hidden />
      <div className="ui-content-layer">{children}</div>
    </div>
  )
}
