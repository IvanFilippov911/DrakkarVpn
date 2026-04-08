import { useMemo, useState } from 'react'
import { EmptyState, InlineAlert, PageHeader } from '../../shared/ui'
import { useCoreErrorEventsQuery } from '../../entities/error'
import { useDeleteCoreErrorEventMutation } from '../../features/error/delete-core-error-event/useDeleteCoreErrorEventMutation'
import { ErrorsFiltersRow } from './ui/ErrorsFiltersRow'
import { ErrorsPagination } from './ui/ErrorsPagination'
import { ErrorsTableSection } from './ui/ErrorsTableSection'
import { DeleteErrorEventDialog } from './ui/DeleteErrorEventDialog'
import type { ErrorsUiState, ErrorEventRow } from './ui/types'

type FiltersState = {
  search: string
  area: string
  errorType: string
  command: string
  domainCode: string
  userId: string
  telegramId: string
  fromUtc: string
  toUtc: string
}

function toOptionalString(v: string) {
  const t = v.trim()
  return t.length > 0 ? t : undefined
}

function maybeToIsoString(v: string) {
  const t = v.trim()
  if (!t) return undefined

  const d = new Date(t)
  if (Number.isNaN(d.getTime())) return undefined
  return d.toISOString()
}

function formatTimestampUtc(ts: string) {
  const d = new Date(ts)
  if (Number.isNaN(d.getTime())) return ts

  return new Intl.DateTimeFormat(undefined, {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
  }).format(d)
}

export function ErrorsPage() {
  const [page, setPage] = useState(1)
  const [pageSize] = useState(50)
  const [filters, setFilters] = useState<FiltersState>({
    search: '',
    area: '',
    errorType: '',
    command: '',
    domainCode: '',
    userId: '',
    telegramId: '',
    fromUtc: '',
    toUtc: '',
  })

  const queryParams = useMemo(
    () => ({
      page,
      pageSize,
      search: toOptionalString(filters.search),
      area: toOptionalString(filters.area),
      errorType: toOptionalString(filters.errorType),
      command: toOptionalString(filters.command),
      domainCode: toOptionalString(filters.domainCode),
      userId: toOptionalString(filters.userId),
      telegramId: toOptionalString(filters.telegramId),
      fromUtc: maybeToIsoString(filters.fromUtc),
      toUtc: maybeToIsoString(filters.toUtc),
    }),
    [filters, page, pageSize],
  )

  const errorsQuery = useCoreErrorEventsQuery(queryParams)
  const deleteMutation = useDeleteCoreErrorEventMutation()
  const [deleteDialogId, setDeleteDialogId] = useState<string | null>(null)

  const uiState: ErrorsUiState =
    errorsQuery.isLoading && !errorsQuery.data
      ? 'loading'
      : errorsQuery.isError && !errorsQuery.data
        ? 'error'
        : errorsQuery.data && errorsQuery.data.items.length === 0
          ? 'empty'
          : 'success'

  const rows = useMemo<ErrorEventRow[]>(() => {
    const items = errorsQuery.data?.items ?? []

    return items.map((dto) => ({
      id: dto.id,
      timestamp: formatTimestampUtc(dto.timestampUtc),
      command: dto.command,
      area: dto.area,
      errorType: dto.errorType,
      domainCode: dto.domainCode,
      message: dto.message,
      traceId: dto.traceId,
      userId: dto.userId,
      telegramId: dto.telegramId,
    }))
  }, [errorsQuery.data])

  return (
    <div>
      <PageHeader
        title="Errors"
        description="System error events and failure records."
      />

      <ErrorsFiltersRow
        filters={filters}
        onChange={(next) => {
          setPage(1)
          setFilters(next)
        }}
      />

      {uiState === 'error' ? (
        <InlineAlert message="Failed to load error events." />
      ) : uiState === 'empty' ? (
        <EmptyState title="No errors" description="There are no error events for the selected filters." />
      ) : (
        <>
          <ErrorsTableSection
            state={uiState}
            rows={rows}
            onDeleteClick={(id) => setDeleteDialogId(id)}
            deleteDisabled={deleteMutation.isPending}
          />
          <ErrorsPagination
            page={errorsQuery.data?.page ?? page}
            pageSize={errorsQuery.data?.pageSize ?? pageSize}
            total={errorsQuery.data?.total ?? 0}
            totalPages={errorsQuery.data?.totalPages ?? 0}
            onPrev={() => setPage((p) => Math.max(1, p - 1))}
            onNext={() => setPage((p) => p + 1)}
          />
        </>
      )}

      <DeleteErrorEventDialog
        open={deleteDialogId !== null}
        id={deleteDialogId}
        pending={deleteMutation.isPending}
        errorMessage={deleteMutation.isError ? 'Request failed.' : null}
        onClose={() => {
          if (deleteMutation.isPending) return
          setDeleteDialogId(null)
        }}
        onConfirm={(id) => {
          deleteMutation.mutate(id, {
            onSuccess: () => setDeleteDialogId(null),
          })
        }}
      />
    </div>
  )
}

