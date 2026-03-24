export function EmptyState({
  title,
  description,
}: {
  title: string
  description: string
}) {
  return (
    <div className="rounded-lg border-dashed border p-6 text-center">
      <div className="text-lg font-medium">{title}</div>
      <div className="text-sm text-muted-foreground mt-1">{description}</div>
    </div>
  )
}

