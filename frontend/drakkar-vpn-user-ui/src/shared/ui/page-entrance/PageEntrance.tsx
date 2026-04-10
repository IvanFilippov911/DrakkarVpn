import { useLayoutEffect, useState, type ReactNode } from 'react'

/**
 * Unified first paint + route transition: fade + subtle lift after layout.
 * Parent must be a flex column; this node participates as flex-1 so child pages keep full height.
 */
export function PageEntrance({ children }: { children: ReactNode }) {
  const [on, setOn] = useState(false)

  useLayoutEffect(() => {
    let raf1 = 0
    let raf2 = 0
    raf1 = requestAnimationFrame(() => {
      raf2 = requestAnimationFrame(() => setOn(true))
    })
    return () => {
      cancelAnimationFrame(raf1)
      cancelAnimationFrame(raf2)
    }
  }, [])

  return (
    <div
      className={[
        'flex min-h-0 min-w-0 flex-1 flex-col',
        on ? 'page-entrance page-entrance--on' : 'page-entrance',
      ].join(' ')}
    >
      {children}
    </div>
  )
}
