import apiClient from './apiClient'
import type { AuthResponse, CurrentUserDto, LoginRequest, RegisterRequest } from '@/types/auth'

export const authApi = {
  register: async (payload: RegisterRequest): Promise<AuthResponse> => {
    const response = await apiClient.post('/api/auth/register', payload)
    return response.data
  },

  login: async (payload: LoginRequest): Promise<AuthResponse> => {
    const response = await apiClient.post('/api/auth/login', payload)
    return response.data
  },

  getCurrentUser: async (): Promise<CurrentUserDto> => {
    const response = await apiClient.get('/api/auth/me')
    return response.data
  },
}
