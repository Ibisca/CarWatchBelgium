import { API_ENDPOINTS } from './constants'
import type { AuthResponse, SavedSearchDto, IngestListingsRequest, IngestListingsResult } from './types'

export async function login(email: string, password: string): Promise<AuthResponse> {
  const response = await fetch(API_ENDPOINTS.LOGIN, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password }),
  })

  if (!response.ok) {
    const error = await response.json().catch(() => ({ error: 'Login failed' }))
    throw new Error(error.error || 'Login failed')
  }

  return response.json()
}

export async function getSavedSearches(token: string): Promise<SavedSearchDto[]> {
  const response = await fetch(API_ENDPOINTS.SAVED_SEARCHES, {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
  })

  if (!response.ok) {
    throw new Error('Failed to fetch saved searches')
  }

  return response.json()
}

export async function ingestListings(token: string, request: IngestListingsRequest): Promise<IngestListingsResult> {
  const response = await fetch(API_ENDPOINTS.INGEST_LISTINGS, {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(request),
  })

  if (!response.ok) {
    throw new Error('Failed to ingest listings')
  }

  return response.json()
}
