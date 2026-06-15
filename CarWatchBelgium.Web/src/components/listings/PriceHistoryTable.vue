<template>
  <Card class="price-history-card">
    <template #title>Price History</template>
    <template #content>
      <DataTable :value="history" striped-rows responsive-layout="scroll">
        <Column field="capturedAtUtc" header="Date">
          <template #body="{ data }">
            {{ formatDate(data.capturedAtUtc) }}
          </template>
        </Column>
        <Column field="price" header="Price">
          <template #body="{ data }">
            {{ data.price }} {{ data.currency }}
          </template>
        </Column>
      </DataTable>
    </template>
  </Card>
</template>

<script setup lang="ts">
import Card from 'primevue/card'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import type { PriceHistoryDto } from '@/types/listing'

defineProps<{
  history: PriceHistoryDto[]
}>()

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString()
}
</script>

<style scoped>
.price-history-card {
  margin-top: 2rem;
}
</style>
