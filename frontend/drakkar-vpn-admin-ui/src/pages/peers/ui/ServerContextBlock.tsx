export function ServerContextBlock({ serverId }: { serverId: string }) {
  return (
    <div className="border rounded-lg bg-card p-4 mb-4">
      <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
        Server
      </div>
      <div className="text-sm">
        <span className="text-muted-foreground">serverId:</span>{' '}
        <span className="font-mono text-xs tabular-nums">{serverId}</span>
      </div>
    </div>
  )
}

