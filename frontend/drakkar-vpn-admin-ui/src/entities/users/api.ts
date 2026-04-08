import { apiClient } from '../../shared/api/client'
import type { PagedResponseDto } from '../../shared/api/types'
import type {
  AdminUserCardDto,
  AdminUserDetailsDto,
  UsersListQuery,
  UsersOverviewDto,
} from './types'

export async function getUsersOverview(): Promise<UsersOverviewDto> {
  const { data } = await apiClient.get<UsersOverviewDto>('/api/admin/overview/users', {
    withCredentials: true,
  })

  return data
}

export async function getUsers(params: UsersListQuery): Promise<PagedResponseDto<AdminUserCardDto>> {
  const { data } = await apiClient.get<PagedResponseDto<AdminUserCardDto>>('/api/admin/users', {
    withCredentials: true,
    params,
  })

  return data
}

export async function getUserDetails(userId: string): Promise<AdminUserDetailsDto> {
  const { data } = await apiClient.get<AdminUserDetailsDto>(`/api/admin/users/${userId}/details`, {
    withCredentials: true,
  })

  return data
}

