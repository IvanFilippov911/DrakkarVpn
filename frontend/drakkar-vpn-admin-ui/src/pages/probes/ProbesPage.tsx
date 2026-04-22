import { useMemo, useState } from 'react'
import { PageHeader } from '../../shared/ui'
import { AddProbeNodeShell } from '../../features/probes/register-probe/ui/AddProbeNodeShell'
import { EditProbeNodeShell } from '../../features/probes/edit-probe/ui/EditProbeNodeShell'
import { ConfirmDeleteProbeShell } from '../../features/probes/delete-probe/ui/ConfirmDeleteProbeShell'
import { useProbeNodesQuery } from '../../entities/probes'
import type { ProbeRow, UiState } from './ui/types'
import { mapProbesToRows } from './ui/probesUiMappers'
import { ProbesTableSection } from './ui/ProbesTableSection'

export function ProbesPage() {
  const probesQuery = useProbeNodesQuery()
  const [isAddOpen, setIsAddOpen] = useState(false)
  const [editProbe, setEditProbe] = useState<ProbeRow | null>(null)
  const [deleteConfirmForId, setDeleteConfirmForId] = useState<string | null>(null)

  const tableState: UiState = probesQuery.isLoading
    ? 'loading'
    : probesQuery.isError
      ? 'error'
      : probesQuery.data && probesQuery.data.length === 0
        ? 'empty'
        : 'success'

  const rows = useMemo<ProbeRow[]>(() => {
    if (!probesQuery.data) return []
    return mapProbesToRows(probesQuery.data)
  }, [probesQuery.data])

  const deleteProbe = useMemo(() => {
    if (!deleteConfirmForId) return null
    return rows.find((r) => r.id === deleteConfirmForId) ?? null
  }, [deleteConfirmForId, rows])

  return (
    <div>
      <PageHeader
        title="Probes"
        description="Network monitoring probe nodes."
        action={
          <button
            type="button"
            onClick={() => setIsAddOpen(true)}
            className="h-9 px-4 rounded-md text-sm font-medium bg-primary text-primary-foreground hover:bg-primary/90"
          >
            Register probe
          </button>
        }
      />

      <ProbesTableSection
        state={tableState}
        rows={rows}
        onEdit={(row) => setEditProbe(row)}
        onDelete={(id) => setDeleteConfirmForId(id)}
      />

      <AddProbeNodeShell open={isAddOpen} onClose={() => setIsAddOpen(false)} />

      <EditProbeNodeShell open={editProbe != null} probe={editProbe} onClose={() => setEditProbe(null)} />

      <ConfirmDeleteProbeShell
        open={deleteConfirmForId != null}
        probeId={deleteConfirmForId ?? ''}
        probeName={deleteProbe?.name ?? ''}
        title="Delete probe node?"
        description="This action is destructive and cannot be undone."
        confirmLabel="Delete"
        onCancel={() => setDeleteConfirmForId(null)}
      />
    </div>
  )
}

