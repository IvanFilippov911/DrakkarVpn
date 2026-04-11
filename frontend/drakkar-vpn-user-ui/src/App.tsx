import { BrowserRouter, Route, Routes } from 'react-router-dom'
import { AppBootLayout } from './app/AppBootLayout'
import { HomePage } from './pages/home'
import { ProvisionWaitingPage } from './pages/provision-waiting'
import { SupportPage } from './pages/support'
import { TariffsPage } from './pages/tariffs'

function App() {
  return (
    <div className="flex h-full min-h-0 flex-1 flex-col overflow-hidden">
      <BrowserRouter>
        <Routes>
          <Route element={<AppBootLayout />}>
            <Route path="/" element={<HomePage />} />
            <Route path="/provision" element={<ProvisionWaitingPage />} />
            <Route path="/tariffs" element={<TariffsPage />} />
            <Route path="/support" element={<SupportPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </div>
  )
}

export default App
