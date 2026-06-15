import apiClient from './apiClient'
import type { MatchedListingDto, ListingDetailsDto, UpdateListingStateRequest } from '@/types/listing'

export const listingApi = {
  getBySearch: async (
    savedSearchId: string,
    includeUnavailable: boolean = false,
    includeIgnored: boolean = false
  ): Promise<MatchedListingDto[]> => {
    const response = await apiClient.get(
      `/api/listing-queries/saved-search/${savedSearchId}`,
      { params: { includeUnavailable, includeIgnored } }
    )
    return response.data
  },

  getFavorites: async (): Promise<MatchedListingDto[]> => {
    const response = await apiClient.get('/api/listing-queries/favorites')
    return response.data
  },

  getUnseen: async (): Promise<MatchedListingDto[]> => {
    const response = await apiClient.get('/api/listing-queries/unseen')
    return response.data
  },

  getDetails: async (listingId: string): Promise<ListingDetailsDto> => {
    const response = await apiClient.get(`/api/listing-queries/${listingId}`)
    return response.data
  },

  markSeen: async (listingId: string): Promise<void> => {
    await apiClient.patch(`/api/listing-states/${listingId}/seen`)
  },

  toggleFavorite: async (listingId: string): Promise<void> => {
    await apiClient.patch(`/api/listing-states/${listingId}/favorite/toggle`)
  },

  ignore: async (listingId: string): Promise<void> => {
    await apiClient.patch(`/api/listing-states/${listingId}/ignore`)
  },

  update: async (listingId: string, payload: UpdateListingStateRequest): Promise<void> => {
    await apiClient.patch(`/api/listing-states/${listingId}`, payload)
  },

  markManySeen: async (listingIds: string[]): Promise<void> => {
    await apiClient.post('/api/listing-states/mark-seen', { listingIds })
  },
}
