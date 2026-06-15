<template>
  <Card class="listing-card">
    <template #header>
      <div v-if="listing.imageUrl" class="card-image">
        <img :src="listing.imageUrl" :alt="listing.title" />
        <div class="badges">
          <Badge v-if="listing.isNew" value="NEW" severity="success" />
          <Badge v-if="listing.priceDropped" value="PRICE DROP" severity="danger" />
        </div>
      </div>
    </template>
    <template #title>
      {{ listing.title }}
    </template>
    <template #content>
      <div class="listing-info">
        <div class="price">{{ listing.price }} {{ listing.currency }}</div>
        <div v-if="listing.previousPrice" class="old-price">
          Was: {{ listing.previousPrice }} {{ listing.currency }}
        </div>

        <div class="details-grid">
          <div v-if="listing.mileageKm" class="detail-item">
            <i class="pi pi-tachometer"></i>
            <span>{{ listing.mileageKm }} km</span>
          </div>
          <div v-if="listing.year" class="detail-item">
            <i class="pi pi-calendar"></i>
            <span>{{ listing.year }}</span>
          </div>
          <div class="detail-item">
            <i class="pi pi-car"></i>
            <span>{{ listing.fuelType }}</span>
          </div>
          <div v-if="listing.powerHp" class="detail-item">
            <i class="pi pi-flash"></i>
            <span>{{ listing.powerHp }} HP</span>
          </div>
        </div>

        <div class="city">{{ listing.city }}, {{ listing.countryCode }}</div>
      </div>
    </template>
    <template #footer>
      <div class="card-actions">
        <Button
          v-if="!listing.isSeen"
          icon="pi pi-check-circle"
          severity="warning"
          text
          size="small"
          @click="$emit('mark-seen', listing.listingId)"
        />
        <Button
          :icon="listing.isFavorite ? 'pi pi-heart-fill' : 'pi pi-heart'"
          :severity="listing.isFavorite ? 'danger' : 'secondary'"
          text
          size="small"
          @click="$emit('toggle-favorite', listing.listingId)"
        />
        <Button
          icon="pi pi-eye"
          text
          size="small"
          @click="$emit('view', listing.listingId)"
        />
      </div>
    </template>
  </Card>
</template>

<script setup lang="ts">
import Card from 'primevue/card'
import Badge from 'primevue/badge'
import Button from 'primevue/button'
import type { MatchedListingDto } from '@/types/listing'

defineProps<{
  listing: MatchedListingDto
}>()

defineEmits<{
  'mark-seen': [listingId: string]
  'toggle-favorite': [listingId: string]
  view: [listingId: string]
  ignore: [listingId: string]
}>()
</script>

<style scoped>
.listing-card {
  height: 100%;
  transition: transform 0.3s, box-shadow 0.3s;
}

.listing-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.15);
}

.card-image {
  position: relative;
  height: 200px;
  background: #f0f0f0;
  overflow: hidden;
}

.card-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.badges {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
  display: flex;
  gap: 0.5rem;
}

.listing-info {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.price {
  font-size: 1.5rem;
  font-weight: bold;
  color: #667eea;
}

.old-price {
  font-size: 0.85rem;
  color: #999;
  text-decoration: line-through;
}

.details-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.5rem;
  font-size: 0.85rem;
  color: #666;
}

.detail-item {
  display: flex;
  align-items: center;
  gap: 0.3rem;
}

.detail-item i {
  font-size: 0.8rem;
}

.city {
  font-size: 0.9rem;
  color: #888;
  margin-top: 0.5rem;
}

.card-actions {
  display: flex;
  gap: 0.5rem;
  justify-content: flex-end;
}

:deep(.p-card-content) {
  padding: 1rem;
}

:deep(.p-card-footer) {
  padding: 0.5rem 1rem;
  background: transparent;
}
</style>
