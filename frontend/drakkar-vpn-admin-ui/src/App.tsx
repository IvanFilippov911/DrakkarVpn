import { AppRouter } from './app/router'
import { QueryClientProvider } from './app/providers/QueryClientProvider'

function App() {
  return (
    <QueryClientProvider>
      <AppRouter />
    </QueryClientProvider>
  )
}

export default App
