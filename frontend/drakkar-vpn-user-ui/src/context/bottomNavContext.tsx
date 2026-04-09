/* eslint-disable react-refresh/only-export-components */
import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import { BottomNav } from '../shared/ui/bottom-nav/BottomNav'

type BottomNavContextValue = {
  setBottomNavHidden: (hidden: boolean) => void
}

const BottomNavContext = createContext<BottomNavContextValue | null>(null)

export function BottomNavProvider({ children }: { children: ReactNode }) {
  const [hidden, setHidden] = useState(false)
  const setBottomNavHidden = useCallback((h: boolean) => {
    setHidden(h)
  }, [])
  const value = useMemo(() => ({ setBottomNavHidden }), [setBottomNavHidden])

  return (
    <BottomNavContext.Provider value={value}>
      <div className="flex h-[100dvh] max-h-[100dvh] flex-col overflow-hidden">
        <div
          className={
            hidden
              ? 'box-border flex min-h-0 flex-1 flex-col overflow-hidden'
              : 'box-border flex min-h-0 flex-1 flex-col overflow-hidden pb-[calc(4.25rem+1rem+max(0.5rem,env(safe-area-inset-bottom)))]'
          }
        >
          {children}
        </div>
      </div>
      {hidden ? null : <BottomNav />}
    </BottomNavContext.Provider>
  )
}

export function useBottomNavControl() {
  const ctx = useContext(BottomNavContext)
  if (!ctx) {
    throw new Error('useBottomNavControl must be used within BottomNavProvider')
  }
  return ctx
}
