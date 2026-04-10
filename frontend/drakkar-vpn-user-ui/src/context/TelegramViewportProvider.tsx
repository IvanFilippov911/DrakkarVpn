import { useEffect, type ReactNode } from 'react'
import { subscribeAppViewport, syncAppViewportHeight } from '../shared/lib/telegramViewport'

/**
 * Re-syncs height after mount and subscribes to Telegram viewportChanged (+ browser resize).
 * expand() runs in main.tsx via primeTelegramMiniAppViewport() before the root renders.
 */
export function TelegramViewportProvider({ children }: { children: ReactNode }) {
  useEffect(() => {
    syncAppViewportHeight()
    return subscribeAppViewport(syncAppViewportHeight)
  }, [])

  return children
}
