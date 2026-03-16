import type { ReactNode } from 'react'
import { QueryClient, QueryClientProvider as TanStackQueryClientProvider } from '@tanstack/react-query'

const queryClient = new QueryClient()

type Props = {
  children: ReactNode
}

export function QueryClientProvider({ children }: Props) {
  return <TanStackQueryClientProvider client={queryClient}>{children}</TanStackQueryClientProvider>
}

