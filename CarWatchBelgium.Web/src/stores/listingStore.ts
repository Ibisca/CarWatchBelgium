import { defineStore } from 'pinia'
import { ref } from 'vue'
import { listingApi } from '@/services/listingApi'
import type { MatchedListingDto, ListingDetailsDto } from '@/types/listing'

export const useListingStore = defineStore('listing', () => {
  const listings = ref<MatchedListingDto[]>([])
  const favorites = ref<MatchedListingDto[]>([])
  const unseen = ref<MatchedListingDto[]>([])
  const selectedListing = ref<ListingDetailsDto | null>(null)
  const loading = ref(false)

  async function loadListingsBySavedSearch(
    savedSearchId: string,
    includeUnavailable: boolean = false,
    includeIgnored: boolean = false
  ): Promise<void> {
    loading.value = true
    try {
      listings.value = await listingApi.getBySearch(savedSearchId, includeUnavailable, includeIgnored)
    } finally {
      loading.value = false
    }
  }

  async function loadFavorites(): Promise<void> {
    loading.value = true
    try {
      favorites.value = await listingApi.getFavorites()
    } finally {
      loading.value = false
    }
  }

  async function loadUnseen(): Promise<void> {
    loading.value = true
    try {
      unseen.value = await listingApi.getUnseen()
    } finally {
      loading.value = false
    }
  }

  async function loadListingDetails(listingId: string): Promise<void> {
    loading.value = true
    try {
      selectedListing.value = await listingApi.getDetails(listingId)
    } finally {
      loading.value = false
    }
  }

  async function markSeen(listingId: string): Promise<void> {
    await listingApi.markSeen(listingId)
    updateListingInArrays(listingId, { isSeen: true })
  }

  async function toggleFavorite(listingId: string): Promise<void> {
    await listingApi.toggleFavorite(listingId)
    const listing = listings.value.find((l: MatchedListingDto) => l.listingId === listingId)
    if (listing) {
      listing.isFavorite = !listing.isFavorite
    }
  }

  async function ignoreListing(listingId: string): Promise<void> {
    await listingApi.ignore(listingId)
    updateListingInArrays(listingId, { isIgnored: true })
  }

  async function markManySeen(listingIds: string[]): Promise<void> {
    await listingApi.markManySeen(listingIds)
    listings.value.forEach((l: MatchedListingDto) => {
      if (listingIds.includes(l.listingId)) {
        l.isSeen = true
      }
    })
  }

  function updateListingInArrays(listingId: string, updates: Partial<MatchedListingDto>): void {
    const updateArray = (arr: MatchedListingDto[]): void => {
      const item = arr.find(l => l.listingId === listingId)
      if (item) {
        Object.assign(item, updates)
      }
    }
    updateArray(listings.value)
    updateArray(favorites.value)
    updateArray(unseen.value)
  }

  return {
    listings,
    favorites,
    unseen,
    selectedListing,
    loading,
    loadListingsBySavedSearch,
    loadFavorites,
    loadUnseen,
    loadListingDetails,
    markSeen,
    toggleFavorite,
    ignoreListing,
    markManySeen,
  }
})
