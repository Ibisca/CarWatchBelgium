import { defineStore } from 'pinia'
import { ref } from 'vue'
import { savedSearchApi } from '@/services/savedSearchApi'
import type { SavedSearchDto, CreateSavedSearchRequest, UpdateSavedSearchRequest } from '@/types/savedSearch'

export const useSavedSearchStore = defineStore('savedSearch', () => {
  const savedSearches = ref<SavedSearchDto[]>([])
  const selectedSavedSearch = ref<SavedSearchDto | null>(null)
  const loading = ref(false)

  async function loadSavedSearches(): Promise<void> {
    loading.value = true
    try {
      savedSearches.value = await savedSearchApi.getAll()
    } finally {
      loading.value = false
    }
  }

  async function getSavedSearchById(id: string): Promise<SavedSearchDto> {
    const search = savedSearches.value.find(s => s.id === id)
    if (search) {
      return search
    }
    const fetched = await savedSearchApi.getById(id)
    selectedSavedSearch.value = fetched
    return fetched
  }

  async function createSavedSearch(payload: CreateSavedSearchRequest): Promise<SavedSearchDto> {
    loading.value = true
    try {
      const created = await savedSearchApi.create(payload)
      savedSearches.value.push(created)
      return created
    } finally {
      loading.value = false
    }
  }

  async function updateSavedSearch(id: string, payload: UpdateSavedSearchRequest): Promise<void> {
    loading.value = true
    try {
      const updated = await savedSearchApi.update(id, payload)
      const index = savedSearches.value.findIndex((s: SavedSearchDto) => s.id === id)
      if (index >= 0) {
        savedSearches.value[index] = updated
      }
      if (selectedSavedSearch.value?.id === id) {
        selectedSavedSearch.value = updated
      }
    } finally {
      loading.value = false
    }
  }

  async function deactivateSavedSearch(id: string): Promise<void> {
    loading.value = true
    try {
      await savedSearchApi.deactivate(id)
      const search = savedSearches.value.find((s: SavedSearchDto) => s.id === id)
      if (search) {
        search.isActive = false
      }
    } finally {
      loading.value = false
    }
  }

  async function deleteSavedSearch(id: string): Promise<void> {
    loading.value = true
    try {
      await savedSearchApi.delete(id)
      savedSearches.value = savedSearches.value.filter((s: SavedSearchDto) => s.id !== id)
    } finally {
      loading.value = false
    }
  }

  return {
    savedSearches,
    selectedSavedSearch,
    loading,
    loadSavedSearches,
    getSavedSearchById,
    createSavedSearch,
    updateSavedSearch,
    deactivateSavedSearch,
    deleteSavedSearch,
  }
})
