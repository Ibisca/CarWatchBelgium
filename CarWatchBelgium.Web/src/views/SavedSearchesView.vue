<template>
  <div class="saved-searches-view">
    <div class="view-header">
      <h1>Saved Searches</h1>
      <Button
        label="Create New Search"
        icon="pi pi-plus"
        severity="success"
        @click="showCreateForm = true"
      />
    </div>

    <div v-if="savedSearchStore.savedSearches.length === 0" class="empty-state">
      <i class="pi pi-inbox"></i>
      <p>No saved searches yet. Create one to get started!</p>
    </div>

    <div v-else class="searches-grid">
      <SavedSearchCard
        v-for="search in savedSearchStore.savedSearches"
        :key="search.id"
        :search="search"
        @edit="editSearch"
        @delete="deleteSearch"
        @view="viewSearch"
      />
    </div>

    <Dialog
      v-model:visible="showCreateForm"
      header="Create New Search"
      modal
      :style="{ width: '90vw', maxWidth: '600px' }"
      @hide="resetForm"
    >
      <SavedSearchForm
        :search="currentSearch"
        @submit="submitForm"
        @cancel="showCreateForm = false"
      />
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useSavedSearchStore } from '@/stores/savedSearchStore'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import SavedSearchCard from '@/components/saved-searches/SavedSearchCard.vue'
import SavedSearchForm from '@/components/saved-searches/SavedSearchForm.vue'
import type { SavedSearchDto } from '@/types/savedSearch'

const router = useRouter()
const savedSearchStore = useSavedSearchStore()

const showCreateForm = ref(false)
const currentSearch = ref<SavedSearchDto | null>(null)

const editSearch = (search: SavedSearchDto) => {
  currentSearch.value = search
  showCreateForm.value = true
}

const deleteSearch = async (id: string) => {
  await savedSearchStore.deleteSavedSearch(id)
}

const viewSearch = (id: string) => {
  router.push(`/saved-searches/${id}`)
}

const submitForm = () => {
  showCreateForm.value = false
  resetForm()
}

const resetForm = () => {
  currentSearch.value = null
}

onMounted(async () => {
  await savedSearchStore.loadSavedSearches()
})
</script>

<style scoped>
.saved-searches-view {
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
}

.searches-grid {
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

  .searches-grid {
    grid-template-columns: 1fr;
  }
}
</style>
