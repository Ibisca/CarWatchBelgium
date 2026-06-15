import apiClient from './apiClient'
import type { SavedSearchDto, CreateSavedSearchRequest, UpdateSavedSearchRequest } from '@/types/savedSearch'

export const savedSearchApi = {
  getAll: async (): Promise<SavedSearchDto[]> => {
    const response = await apiClient.get('/api/saved-searches')
    return response.data
  },

  getById: async (id: string): Promise<SavedSearchDto> => {
    const response = await apiClient.get(`/api/saved-searches/${id}`)
    return response.data
  },

  create: async (payload: CreateSavedSearchRequest): Promise<SavedSearchDto> => {
    const response = await apiClient.post('/api/saved-searches', payload)
    return response.data
  },

  update: async (id: string, payload: UpdateSavedSearchRequest): Promise<SavedSearchDto> => {
    const response = await apiClient.put(`/api/saved-searches/${id}`, payload)
    return response.data
  },

  deactivate: async (id: string): Promise<void> => {
    await apiClient.patch(`/api/saved-searches/${id}/deactivate`)
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/api/saved-searches/${id}`)
  },
}
