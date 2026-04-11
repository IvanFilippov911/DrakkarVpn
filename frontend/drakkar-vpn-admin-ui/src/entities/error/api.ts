import { apiClient } from '../../shared/api/client'
import type { PagedResponseDto } from '../../shared/api/types'
import type { CoreErrorEventListItemDto, CoreErrorEventsListQuery } from './types'

export async function getCoreErrorEvents(
  params: CoreErrorEventsListQuery,
): Promise<PagedResponseDto<CoreErrorEventListItemDto>> {
  const { data } = await apiClient.get<PagedResponseDto<CoreErrorEventListItemDto>>(
    '/api/admin/core/errors',
    {
      withCredentials: true,
      params,
    },
  )

  return data
}

