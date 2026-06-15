<template>
  <div class="listing-details-view">
    <div v-if="listingStore.selectedListing" class="listing-details">
      <router-link to="-1" class="back-link">
        <i class="pi pi-arrow-left"></i> Back
      </router-link>

      <div class="details-container">
        <div v-if="listingStore.selectedListing.imageUrl" class="details-image">
          <img :src="listingStore.selectedListing.imageUrl" :alt="listingStore.selectedListing.title" />
        </div>

        <Card class="details-card">
          <template #title>
            {{ listingStore.selectedListing.title }}
          </template>
          <template #content>
            <div class="listing-info">
              <div class="info-row">
                <span class="label">Price:</span>
                <span class="value">{{ listingStore.selectedListing.price }} {{ listingStore.selectedListing.currency }}</span>
              </div>

              <div v-if="listingStore.selectedListing.mileageKm" class="info-row">
                <span class="label">Mileage:</span>
                <span class="value">{{ listingStore.selectedListing.mileageKm }} km</span>
              </div>

              <div v-if="listingStore.selectedListing.year" class="info-row">
                <span class="label">Year:</span>
                <span class="value">{{ listingStore.selectedListing.year }}</span>
              </div>

              <div class="info-row">
                <span class="label">Fuel Type:</span>
                <span class="value">{{ listingStore.selectedListing.fuelType }}</span>
              </div>

              <div class="info-row">
                <span class="label">Transmission:</span>
                <span class="value">{{ listingStore.selectedListing.transmission }}</span>
              </div>

              <div v-if="listingStore.selectedListing.powerHp" class="info-row">
                <span class="label">Power:</span>
                <span class="value">{{ listingStore.selectedListing.powerHp }} HP</span>
              </div>

              <div class="info-row">
                <span class="label">Location:</span>
                <span class="value">{{ listingStore.selectedListing.city }}, {{ listingStore.selectedListing.countryCode }}</span>
              </div>

              <div class="info-row">
                <span class="label">Source:</span>
                <span class="value">{{ listingStore.selectedListing.source }}</span>
              </div>

              <div class="actions">
                <Button
                  v-if="!listingStore.selectedListing.isSeen"
                  label="Mark as Seen"
                  icon="pi pi-check-circle"
                  @click="markSeen"
                />
                <Button
                  :label="listingStore.selectedListing.isFavorite ? 'Remove from Favorites' : 'Add to Favorites'"
                  :icon="listingStore.selectedListing.isFavorite ? 'pi pi-heart-fill' : 'pi pi-heart'"
                  @click="toggleFavorite"
                />
                <Button
                  label="View on Source"
                  icon="pi pi-external-link"
                  severity="secondary"
                  @click="openUrl"
                />
              </div>
            </div>
          </template>
        </Card>
      </div>

      <PriceHistoryTable
        v-if="listingStore.selectedListing.priceHistory.length > 0"
        :history="listingStore.selectedListing.priceHistory"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useListingStore } from '@/stores/listingStore'
import Card from 'primevue/card'
import Button from 'primevue/button'
import PriceHistoryTable from '@/components/listings/PriceHistoryTable.vue'

const route = useRoute()
const listingStore = useListingStore()

const markSeen = async () => {
  const listingId = route.params.id as string
  await listingStore.markSeen(listingId)
}

const toggleFavorite = async () => {
  const listingId = route.params.id as string
  await listingStore.toggleFavorite(listingId)
}

const openUrl = () => {
  if (listingStore.selectedListing?.url) {
    window.open(listingStore.selectedListing.url, '_blank')
  }
}

onMounted(async () => {
  const listingId = route.params.id as string
  if (listingId) {
    await listingStore.loadListingDetails(listingId)
  }
})
</script>

<style scoped>
.listing-details-view {
  max-width: 900px;
}

.back-link {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  color: #667eea;
  text-decoration: none;
  margin-bottom: 1.5rem;
  font-size: 0.9rem;
}

.back-link:hover {
  text-decoration: underline;
}

.details-container {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
  margin-bottom: 2rem;
}

.details-image {
  border-radius: 0.5rem;
  overflow: hidden;
  background: #f0f0f0;
}

.details-image img {
  width: 100%;
  height: auto;
  object-fit: cover;
}

.details-card :deep(.p-card-title) {
  font-size: 1.25rem;
}

.listing-info {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.info-row {
  display: flex;
  justify-content: space-between;
  padding-bottom: 0.5rem;
  border-bottom: 1px solid #eee;
}

.label {
  font-weight: 600;
  color: #555;
}

.value {
  color: #333;
}

.actions {
  display: flex;
  gap: 0.5rem;
  margin-top: 1rem;
  flex-wrap: wrap;
}

@media (max-width: 768px) {
  .details-container {
    grid-template-columns: 1fr;
  }

  .actions {
    flex-direction: column;
  }

  .actions :deep(.p-button) {
    width: 100%;
  }
}
</style>
