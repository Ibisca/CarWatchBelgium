<template>
  <div class="unseen-view">
    <div class="view-header">
      <h1>Unseen Listings</h1>
      <Button
        label="Mark All as Seen"
        icon="pi pi-check-circle"
        @click="markAllSeen"
        :loading="loading"
      />
    </div>

    <div v-if="listingStore.unseen.length === 0" class="empty-state">
      <i class="pi pi-eye"></i>
      <p>All listings have been seen!</p>
    </div>

    <div v-else class="listings-grid">
      <ListingCard
        v-for="listing in listingStore.unseen"
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
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useListingStore } from '@/stores/listingStore'
import Button from 'primevue/button'
import ListingCard from '@/components/listings/ListingCard.vue'

const router = useRouter()
const listingStore = useListingStore()
const loading = ref(false)

const markAllSeen = async () => {
  loading.value = true
  try {
    const listingIds = listingStore.unseen.map(l => l.listingId)
    await listingStore.markManySeen(listingIds)
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
  await listingStore.loadUnseen()
})
</script>

<style scoped>
.unseen-view {
  max-width: 1200px;
}

.view-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.view-header h1 {
  margin: 0;
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
  .view-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }

  .listings-grid {
    grid-template-columns: 1fr;
  }
}
</style>
