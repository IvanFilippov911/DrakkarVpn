export type LoginRequestDto = {
  email: string
  password: string
}

export type AdminDto = {
  adminId: string
  email: string
  roles: string[]
  permissions: string[]
}

export type AuthResponseDto = {
  accessToken: string
  accessTokenExpiresAtUtc: string
  accessTokenId: string
  admin: AdminDto
}

export type CurrentAdminDto = AdminDto

