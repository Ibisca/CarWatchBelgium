<template>
  <div class="dashboard-view">
    <h1>Dashboard</h1>

    <div class="dashboard-grid">
      <Card class="stat-card">
        <template #title>
          <i class="pi pi-search"></i> Saved Searches
        </template>
        <template #content>
          <div class="stat-value">{{ savedSearchStore.savedSearches.length }}</div>
          <Button
            label="View All"
            icon="pi pi-arrow-right"
            text
            size="small"
            to="/saved-searches"
            as-router-link
          />
        </template>
      </Card>

      <Card class="stat-card">
        <template #title>
          <i class="pi pi-eye"></i> Unseen Listings
        </template>
        <template #content>
          <div class="stat-value">{{ listingStore.unseen.length }}</div>
          <Button
            label="View All"
            icon="pi pi-arrow-right"
            text
            size="small"
            to="/unseen"
            as-router-link
          />
        </template>
      </Card>

      <Card class="stat-card">
        <template #title>
          <i class="pi pi-heart"></i> Favorites
        </template>
        <template #content>
          <div class="stat-value">{{ listingStore.favorites.length }}</div>
          <Button
            label="View All"
            icon="pi pi-arrow-right"
            text
            size="small"
            to="/favorites"
            as-router-link
          />
        </template>
      </Card>
    </div>

    <Card class="quick-actions-card">
      <template #title>Quick Actions</template>
      <template #content>
        <div class="quick-actions">
          <Button
            label="Create New Search"
            icon="pi pi-plus"
            severity="success"
            @click="navigateToCreateSearch"
          />
          <Button
            label="Refresh All"
            icon="pi pi-refresh"
            @click="refreshData"
            :loading="loading"
          />
        </div>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useSavedSearchStore } from '@/stores/savedSearchStore'
import { useListingStore } from '@/stores/listingStore'
import Card from 'primevue/card'
import Button from 'primevue/button'

const router = useRouter()
const savedSearchStore = useSavedSearchStore()
const listingStore = useListingStore()
const loading = ref(false)

const navigateToCreateSearch = () => {
  router.push('/saved-searches')
}

const refreshData = async () => {
  loading.value = true
  try {
    await savedSearchStore.loadSavedSearches()
    await listingStore.loadFavorites()
    await listingStore.loadUnseen()
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  await refreshData()
})
</script>

<style scoped>
.dashboard-view {
  max-width: 1200px;
}

.dashboard-view h1 {
  margin-bottom: 2rem;
  color: #333;
}

.dashboard-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.stat-card {
  text-align: center;
}

.stat-card :deep(.p-card-title) {
  font-size: 1rem;
  color: #667eea;
}

.stat-value {
  font-size: 2.5rem;
  font-weight: bold;
  color: #667eea;
  margin: 1rem 0;
}

.quick-actions-card {
  margin-top: 2rem;
}

.quick-actions {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
}

@media (max-width: 768px) {
  .dashboard-grid {
    grid-template-columns: 1fr;
  }
}
</style>
