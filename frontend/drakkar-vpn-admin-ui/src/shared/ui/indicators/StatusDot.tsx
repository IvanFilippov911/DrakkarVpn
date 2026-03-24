export function StatusDot({ ok, label }: { ok: boolean; label: string }) {
  return (
    <span className="inline-flex items-center gap-2">
      <span
        className={['size-2 rounded-full', ok ? 'bg-emerald-500' : 'bg-zinc-400'].join(' ')}
        aria-hidden
      />
      <span className="text-sm text-muted-foreground">{label}</span>
    </span>
  )
}

