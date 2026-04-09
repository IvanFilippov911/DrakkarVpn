import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: true,
    },
    mutations: {
      retry: false,
    },
  },
})

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <div className="relative flex h-full min-h-0 flex-1 flex-col overflow-hidden">
        <div className="app-atmosphere" aria-hidden />
        <div className="relative z-10 flex min-h-0 flex-1 flex-col overflow-hidden">
          <App />
        </div>
      </div>
    </QueryClientProvider>
  </StrictMode>,
)
