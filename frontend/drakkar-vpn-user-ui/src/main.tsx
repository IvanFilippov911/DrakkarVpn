import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { TelegramViewportProvider } from './context/TelegramViewportProvider'
import { primeTelegramMiniAppViewport } from './shared/lib/telegramViewport'
import './index.css'
import App from './App.tsx'

primeTelegramMiniAppViewport()

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
      <TelegramViewportProvider>
        <div className="relative flex h-full min-h-0 flex-1 flex-col overflow-hidden">
          <div className="app-atmosphere" aria-hidden>
            <span className="app-atmosphere-glow app-atmosphere-glow--top" />
            <span className="app-atmosphere-glow app-atmosphere-glow--bottom" />
          </div>
          <div className="relative z-10 flex min-h-0 flex-1 flex-col overflow-hidden">
            <App />
          </div>
        </div>
      </TelegramViewportProvider>
    </QueryClientProvider>
  </StrictMode>,
)
