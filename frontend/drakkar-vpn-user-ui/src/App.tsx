import { BrowserRouter, Route, Routes } from 'react-router-dom'
import { AppBootLayout } from './app/AppBootLayout'
import { HomePage } from './pages/home'
import { TariffsPage } from './pages/tariffs'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<AppBootLayout />}>
          <Route path="/" element={<HomePage />} />
          <Route path="/tariffs" element={<TariffsPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  )
}

export default App
