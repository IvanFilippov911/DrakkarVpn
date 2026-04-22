import { useMemo, useState } from 'react'
import { isAxiosError } from 'axios'
import { PageHeader, InlineAlert } from '../../shared/ui'
import { useTransportProfilesListQuery } from '../../entities/transport-profiles'
import type { TransportProfile } from '../../entities/transport-profiles'
import {
  useDeleteTransportProfileMutation,
  useDisableTransportProfileMutation,
  useEnableTransportProfileMutation,
} from '../../features/transport-profiles'
import { AddTransportProfileShell } from '../../features/transport-profiles/create-transport-profile/ui/AddTransportProfileShell'
import { ConfirmDeleteTransportProfileShell } from '../../features/transport-profiles/delete-transport-profile/ui/ConfirmDeleteTransportProfileShell'
import { EditTransportProfileShell } from '../../features/transport-profiles/edit-transport-profile/ui/EditTransportProfileShell'
import { useTransportProfilesQueryState } from './hooks/useTransportProfilesQueryState'
import { PaginationSection } from './ui/PaginationSection'
import { TransportProfileDetailsShell } from './ui/TransportProfileDetailsShell'
import { TransportProfilesFiltersRow } from './ui/TransportProfilesFiltersRow'
import { TransportProfilesTableSection } from './ui/TransportProfilesTableSection'
import { mapTransportProfilesToRows } from './ui/transportProfilesUiMappers'
import type { TransportProfileRow, UiState } from './ui/types'

export function TransportProfilesPage() {
  const listState = useTransportProfilesQueryState()
  const query = useTransportProfilesListQuery(listState.query)

  const enableMutation = useEnableTransportProfileMutation()
  const disableMutation = useDisableTransportProfileMutation()
  const deleteMutation = useDeleteTransportProfileMutation()

  const [isAddOpen, setIsAddOpen] = useState(false)
  const [editProfileId, setEditProfileId] = useState<string | null>(null)
  const [detailsProfileId, setDetailsProfileId] = useState<string | null>(null)
  const [deleteProfileId, setDeleteProfileId] = useState<string | null>(null)

  const uiState: UiState =
    query.isLoading && !query.data
      ? 'loading'
      : query.isError && !query.data
        ? 'error'
        : query.data && query.data.items.length === 0
          ? 'empty'
          : 'success'

  const profiles = query.data?.items ?? []

  const rows = useMemo<TransportProfileRow[]>(() => mapTransportProfilesToRows(profiles), [profiles])

  const byId = useMemo(() => {
    const map = new Map<string, TransportProfile>()
    for (const profile of profiles) {
      map.set(profile.id, profile)
    }
    return map
  }, [profiles])

  const editProfile = editProfileId ? byId.get(editProfileId) ?? null : null
  const detailsProfile = detailsProfileId ? byId.get(detailsProfileId) ?? null : null
  const deleteProfile = deleteProfileId ? byId.get(deleteProfileId) ?? null : null

  const page = query.data?.page ?? listState.state.page
  const totalPages = query.data?.totalPages ?? 1

  const actionErrorMessage = useMemo(() => {
    const err = enableMutation.error ?? disableMutation.error ?? deleteMutation.error
    if (!err) return null

    if (isAxiosError(err)) {
      const maybeMessage = getMessageFromUnknown(err.response?.data)
      if (typeof maybeMessage === 'string' && maybeMessage.trim().length > 0) {
        return maybeMessage
      }

      return err.response?.status ? `Request failed (${err.response.status}).` : 'Request failed.'
    }

    return err instanceof Error ? err.message : 'Action failed.'
  }, [deleteMutation.error, disableMutation.error, enableMutation.error])

  async function handleEnable(profileId: string) {
    try {
      await enableMutation.mutateAsync(profileId)
    } catch {
      // Error is shown by actionErrorMessage.
    }
  }

  async function handleDisable(profileId: string) {
    try {
      await disableMutation.mutateAsync(profileId)
    } catch {
      // Error is shown by actionErrorMessage.
    }
  }

  return (
    <div>
      <PageHeader
        title="Transport Profiles"
        description="Global transport templates used by server activations."
        action={
          <button
            type="button"
            onClick={() => setIsAddOpen(true)}
            className="h-9 px-4 rounded-md text-sm font-medium bg-primary text-primary-foreground hover:bg-primary/90"
          >
            Create Profile
          </button>
        }
      />

      {actionErrorMessage ? (
        <div className="mb-4">
          <InlineAlert title="Profile action failed" message={actionErrorMessage} />
        </div>
      ) : null}

      <TransportProfilesFiltersRow
        search={listState.state.search}
        isEnabled={listState.state.isEnabled}
        transportType={listState.state.transportType}
        sortBy={listState.state.sortBy}
        sortDirection={listState.state.sortDirection}
        pageSize={listState.state.pageSize}
        onSearchChange={listState.setSearch}
        onIsEnabledChange={listState.setIsEnabled}
        onTransportTypeChange={listState.setTransportType}
        onSortByChange={listState.setSortBy}
        onSortDirectionChange={listState.setSortDirection}
        onPageSizeChange={listState.setPageSize}
      />

      <TransportProfilesTableSection
        state={uiState}
        rows={rows}
        onView={setDetailsProfileId}
        onEdit={setEditProfileId}
        onEnable={(id) => {
          void handleEnable(id)
        }}
        onDisable={(id) => {
          void handleDisable(id)
        }}
        onDelete={setDeleteProfileId}
      />

      <PaginationSection
        page={page}
        totalPages={totalPages}
        onPrev={() => listState.setPage(Math.max(1, page - 1))}
        onNext={() => listState.setPage(Math.min(totalPages, page + 1))}
      />

      <AddTransportProfileShell open={isAddOpen} onClose={() => setIsAddOpen(false)} />

      <EditTransportProfileShell
        open={editProfileId != null}
        profile={editProfile}
        onClose={() => setEditProfileId(null)}
      />

      <TransportProfileDetailsShell
        open={detailsProfileId != null}
        profile={detailsProfile}
        onClose={() => setDetailsProfileId(null)}
      />

      <ConfirmDeleteTransportProfileShell
        open={deleteProfileId != null}
        profileId={deleteProfileId ?? ''}
        profileName={deleteProfile?.name ?? ''}
        onCancel={() => setDeleteProfileId(null)}
      />
    </div>
  )
}

function getMessageFromUnknown(value: unknown): string | undefined {
  if (!value || typeof value !== 'object') return undefined
  if (!('message' in value)) return undefined
  const msg = (value as { message?: unknown }).message
  return typeof msg === 'string' ? msg : undefined
}
