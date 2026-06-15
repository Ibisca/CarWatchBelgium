<template>
  <div class="saved-search-details-view">
    <div v-if="savedSearchStore.selectedSavedSearch" class="search-details">
      <div class="details-header">
        <router-link to="/saved-searches" class="back-link">
          <i class="pi pi-arrow-left"></i> Back
        </router-link>
        <h1>{{ savedSearchStore.selectedSavedSearch.name }}</h1>
      </div>

      <div class="listings-controls">
        <Button
          label="Mark All as Seen"
          icon="pi pi-check-circle"
          @click="markAllSeen"
          :loading="loading"
        />
        <Dropdown
          v-model="filterType"
          :options="filterOptions"
          option-label="label"
          option-value="value"
          placeholder="Filter listings"
        />
      </div>

      <div v-if="filteredListings.length === 0" class="empty-state">
        <p>No listings found for this search.</p>
      </div>

      <div v-else class="listings-grid">
        <ListingCard
          v-for="listing in filteredListings"
          :key="listing.listingId"
          :listing="listing"
          @view="viewListing"
          @mark-seen="markSeen"
          @toggle-favorite="toggleFavorite"
          @ignore="ignoreListing"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useSavedSearchStore } from '@/stores/savedSearchStore'
import { useListingStore } from '@/stores/listingStore'
import Button from 'primevue/button'
import Dropdown from 'primevue/dropdown'
import ListingCard from '@/components/listings/ListingCard.vue'

const router = useRouter()
const route = useRoute()
const savedSearchStore = useSavedSearchStore()
const listingStore = useListingStore()

const loading = ref(false)
const filterType = ref('all')

const filterOptions = [
  { label: 'All', value: 'all' },
  { label: 'New Only', value: 'new' },
  { label: 'Price Dropped', value: 'priceDropped' },
  { label: 'Available Only', value: 'available' },
]

const filteredListings = computed(() => {
  let results = listingStore.listings
  if (filterType.value === 'new') {
    results = results.filter(l => l.isNew)
  } else if (filterType.value === 'priceDropped') {
    results = results.filter(l => l.priceDropped)
  } else if (filterType.value === 'available') {
    results = results.filter(l => l.isAvailable)
  }
  return results
})

const markAllSeen = async () => {
  loading.value = true
  try {
    const unseenIds = listingStore.listings
      .filter(l => !l.isSeen)
      .map(l => l.listingId)
    if (unseenIds.length > 0) {
      await listingStore.markManySeen(unseenIds)
    }
  } finally {
    loading.value = false
  }
}

const markSeen = async (listingId: string) => {
  await listingStore.markSeen(listingId)
}

const toggleFavorite = async (listingId: string) => {
  await listingStore.toggleFavorite(listingId)
}

const ignoreListing = async (listingId: string) => {
  await listingStore.ignoreListing(listingId)
}

const viewListing = (listingId: string) => {
  router.push(`/listings/${listingId}`)
}

onMounted(async () => {
  const searchId = route.params.id as string
  if (searchId) {
    await savedSearchStore.getSavedSearchById(searchId)
    await listingStore.loadListingsBySavedSearch(searchId)
  }
})
</script>

<style scoped>
.saved-search-details-view {
  max-width: 1200px;
}

.search-details {
  width: 100%;
}

.details-header {
  margin-bottom: 2rem;
}

.back-link {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  color: #667eea;
  text-decoration: none;
  margin-bottom: 1rem;
  font-size: 0.9rem;
}

.back-link:hover {
  text-decoration: underline;
}

.details-header h1 {
  margin: 0;
  color: #333;
}

.listings-controls {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  flex-wrap: wrap;
}

.empty-state {
  text-align: center;
  padding: 2rem;
  color: #999;
}

.listings-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}

@media (max-width: 768px) {
  .listings-grid {
    grid-template-columns: 1fr;
  }
}
</style>
