import type { ReactNode } from 'react'

export function Overlay({
  open,
  children,
  onClose,
}: {
  open: boolean
  children: ReactNode
  onClose: () => void
}) {
  if (!open) return null

  return (
    <div className="fixed inset-0 z-50">
      <button
        type="button"
        aria-label="Close overlay"
        className="absolute inset-0 bg-black/40"
        onClick={onClose}
      />
      <div className="relative z-10 mx-auto mt-24 w-full max-w-lg px-4">
        <div className="rounded-lg border bg-background shadow-sm">{children}</div>
      </div>
    </div>
  )
}

