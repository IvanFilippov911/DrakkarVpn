import { useLayoutEffect, useRef, useState, type ReactNode } from 'react'

const firstMountDone = { current: false }

/**
 * Route transition: fade + subtle lift after layout.
 * On the very first mount the app-level AppReadyGate handles the entrance,
 * so we skip animation here to avoid a double-fade.
 */
export function PageEntrance({ children }: { children: ReactNode }) {
  const isFirstMount = useRef(!firstMountDone.current)
  const [on, setOn] = useState(isFirstMount.current)

  useLayoutEffect(() => {
    if (isFirstMount.current) {
      firstMountDone.current = true
      return
    }
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
