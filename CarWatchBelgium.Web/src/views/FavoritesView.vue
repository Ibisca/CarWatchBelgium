<template>
  <div class="favorites-view">
    <h1>Favorite Listings</h1>

    <div v-if="listingStore.favorites.length === 0" class="empty-state">
      <i class="pi pi-heart"></i>
      <p>No favorite listings yet.</p>
    </div>

    <div v-else class="listings-grid">
      <ListingCard
        v-for="listing in listingStore.favorites"
        :key="listing.listingId"
        :listing="listing"
        @view="viewListing"
        @mark-seen="markSeen"
        @toggle-favorite="toggleFavorite"
        @ignore="ignoreListing"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useListingStore } from '@/stores/listingStore'
import ListingCard from '@/components/listings/ListingCard.vue'

const router = useRouter()
const listingStore = useListingStore()

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
  await listingStore.loadFavorites()
})
</script>

<style scoped>
.favorites-view {
  max-width: 1200px;
}

.favorites-view h1 {
  margin-bottom: 2rem;
  color: #333;
}

.empty-state {
  text-align: center;
  padding: 3rem 1rem;
  color: #999;
}

.empty-state i {
  font-size: 3rem;
  display: block;
  margin-bottom: 1rem;
  color: #ddd;
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
