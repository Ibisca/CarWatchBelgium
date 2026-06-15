<template>
  <Card class="saved-search-card">
    <template #header>
      <div class="card-header">
        <Badge v-if="!search.isActive" value="Inactive" severity="warning" />
      </div>
    </template>
    <template #title>{{ search.name }}</template>
    <template #content>
      <div class="search-info">
        <div class="info-item">
          <span class="label">Make:</span>
          <span class="value">{{ search.make || 'Any' }}</span>
        </div>
        <div class="info-item">
          <span class="label">Model:</span>
          <span class="value">{{ search.model || 'Any' }}</span>
        </div>
        <div class="info-item">
          <span class="label">Price Range:</span>
          <span class="value">{{ search.priceMin || '0' }} - {{ search.priceMax || '∞' }}</span>
        </div>
        <div class="info-item">
          <span class="label">Matches:</span>
          <span class="value">{{ search.matchCount }}</span>
        </div>
        <div class="info-item">
          <span class="label">Last Matched:</span>
          <span class="value">{{ formatDate(search.lastMatchedAtUtc) }}</span>
        </div>
      </div>
    </template>
    <template #footer>
      <div class="card-actions">
        <Button
          icon="pi pi-eye"
          label="View"
          text
          severity="secondary"
          size="small"
          @click="$emit('view', search.id)"
        />
        <Button
          icon="pi pi-pencil"
          label="Edit"
          text
          size="small"
          @click="$emit('edit', search)"
        />
        <Button
          icon="pi pi-trash"
          label="Delete"
          text
          severity="danger"
          size="small"
          @click="$emit('delete', search.id)"
        />
      </div>
    </template>
  </Card>
</template>

<script setup lang="ts">
import Card from 'primevue/card'
import Badge from 'primevue/badge'
import Button from 'primevue/button'
import type { SavedSearchDto } from '@/types/savedSearch'

defineProps<{
  search: SavedSearchDto
}>()

defineEmits<{
  view: [id: string]
  edit: [search: SavedSearchDto]
  delete: [id: string]
}>()

const formatDate = (date?: string) => {
  if (!date) return 'Never'
  return new Date(date).toLocaleDateString()
}
</script>

<style scoped>
.saved-search-card {
  height: 100%;
}

.card-header {
  padding: 0.5rem;
  text-align: right;
}

.search-info {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.info-item {
  display: flex;
  justify-content: space-between;
  font-size: 0.9rem;
}

.label {
  font-weight: 600;
  color: #666;
}

.value {
  color: #333;
}

.card-actions {
  display: flex;
  gap: 0.5rem;
  justify-content: flex-end;
}
</style>
