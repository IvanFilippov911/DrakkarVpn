/**
 * Telegram Mini App viewport → CSS variable --app-viewport-height.
 * Falls back to visualViewport / innerHeight when WebApp is absent (local dev).
 */

function readTelegramViewportHeightPx(): number {
  const tw = window.Telegram?.WebApp
  const raw = tw?.viewportHeight
  if (typeof raw === 'number' && Number.isFinite(raw) && raw > 0) {
    return Math.round(raw)
  }
  return 0
}

function readBrowserViewportHeightPx(): number {
  const vv = window.visualViewport?.height
  if (typeof vv === 'number' && Number.isFinite(vv) && vv > 0) {
    return Math.round(vv)
  }
  const ih = window.innerHeight
  if (typeof ih === 'number' && Number.isFinite(ih) && ih > 0) {
    return Math.round(ih)
  }
  return 0
}

export function syncAppViewportHeight(): void {
  const fromTg = readTelegramViewportHeightPx()
  const h = fromTg > 0 ? fromTg : readBrowserViewportHeightPx()
  if (h > 0) {
    document.documentElement.style.setProperty('--app-viewport-height', `${h}px`)
  }
}

/** Call once at startup (before React root): expand + initial height. */
export function primeTelegramMiniAppViewport(): void {
  window.Telegram?.WebApp?.expand()
  syncAppViewportHeight()
}

export function subscribeAppViewport(onChange: () => void): () => void {
  const tw = window.Telegram?.WebApp
  const handler = () => onChange()

  tw?.onEvent?.('viewportChanged', handler)
  window.visualViewport?.addEventListener('resize', handler)
  window.addEventListener('resize', handler)

  return () => {
    tw?.offEvent?.('viewportChanged', handler)
    window.visualViewport?.removeEventListener('resize', handler)
    window.removeEventListener('resize', handler)
  }
}
