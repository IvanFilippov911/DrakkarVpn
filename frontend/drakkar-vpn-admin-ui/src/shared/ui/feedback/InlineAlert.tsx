export function InlineAlert({
  title = 'Error',
  message,
}: {
  title?: string
  message: string
}) {
  return (
    <div className="rounded-lg border border-destructive/30 bg-destructive/5 p-4">
      <div className="text-sm font-medium">{title}</div>
      <div className="text-sm text-muted-foreground mt-1">{message}</div>
    </div>
  )
}

