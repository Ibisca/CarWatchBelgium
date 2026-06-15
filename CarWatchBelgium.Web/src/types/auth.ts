export interface AuthResponse {
  userId: string
  email: string
  displayName?: string
  token: string
  expiresAtUtc: string
}

export interface RegisterRequest {
  email: string
  password: string
  displayName?: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface CurrentUserDto {
  userId: string
  email: string
  displayName?: string
  createdAtUtc: string
  lastLoginAtUtc?: string
}
