<template>
  <form @submit.prevent="handleSubmit" class="saved-search-form">
    <div class="form-row">
      <div class="form-group">
        <label for="name">Search Name *</label>
        <InputText
          id="name"
          v-model="formData.name"
          placeholder="My search name"
          required
        />
      </div>
    </div>

    <div class="form-row">
      <div class="form-group">
        <label for="make">Make</label>
        <InputText id="make" v-model="formData.make" placeholder="e.g., BMW" />
      </div>
      <div class="form-group">
        <label for="model">Model</label>
        <InputText id="model" v-model="formData.model" placeholder="e.g., 3 Series" />
      </div>
    </div>

    <div class="form-row">
      <div class="form-group">
        <label for="priceMin">Min Price</label>
        <InputNumber
          id="priceMin"
          v-model="formData.priceMin"
          placeholder="0"
          :use-grouping="false"
        />
      </div>
      <div class="form-group">
        <label for="priceMax">Max Price</label>
        <InputNumber
          id="priceMax"
          v-model="formData.priceMax"
          placeholder="999999"
          :use-grouping="false"
        />
      </div>
    </div>

    <div class="form-row">
      <div class="form-group">
        <label for="yearMin">Min Year</label>
        <InputNumber id="yearMin" v-model="formData.yearMin" placeholder="2000" />
      </div>
      <div class="form-group">
        <label for="yearMax">Max Year</label>
        <InputNumber id="yearMax" v-model="formData.yearMax" placeholder="2025" />
      </div>
    </div>

    <div class="form-row">
      <div class="form-group">
        <label for="maxMileage">Max Mileage (km)</label>
        <InputNumber id="maxMileage" v-model="formData.maxMileageKm" :use-grouping="false" />
      </div>
      <div class="form-group">
        <label for="minPower">Min Power (HP)</label>
        <InputNumber id="minPower" v-model="formData.minPowerHp" />
      </div>
    </div>

    <div class="form-row">
      <div class="form-group">
        <label for="fuelType">Fuel Type</label>
        <Dropdown
          id="fuelType"
          v-model="formData.fuelType"
          :options="fuelTypes"
          placeholder="Select fuel type"
        />
      </div>
      <div class="form-group">
        <label for="transmission">Transmission</label>
        <Dropdown
          id="transmission"
          v-model="formData.transmission"
          :options="transmissions"
          placeholder="Select transmission"
        />
      </div>
    </div>

    <div class="form-row">
      <div class="form-group">
        <label for="sellerType">Seller Type</label>
        <Dropdown
          id="sellerType"
          v-model="formData.sellerType"
          :options="sellerTypes"
          placeholder="Select seller type"
        />
      </div>
      <div class="form-group">
        <label for="countryCode">Country Code *</label>
        <InputText id="countryCode" v-model="formData.countryCode" placeholder="BE" required />
      </div>
    </div>

    <div class="form-row">
      <div class="form-group full-width">
        <label for="city">City</label>
        <InputText id="city" v-model="formData.city" placeholder="e.g., Brussels" />
      </div>
    </div>

    <div class="form-row">
      <div class="form-group full-width">
        <label for="radiusKm">Search Radius (km)</label>
        <InputNumber id="radiusKm" v-model="formData.radiusKm" />
      </div>
    </div>

    <div class="form-actions">
      <Button type="submit" label="Save" icon="pi pi-check" />
      <Button type="button" label="Cancel" icon="pi pi-times" severity="secondary" @click="$emit('cancel')" />
    </div>
  </form>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import InputText from 'primevue/inputtext'
import InputNumber from 'primevue/inputnumber'
import Dropdown from 'primevue/dropdown'
import Button from 'primevue/button'
import { useSavedSearchStore } from '@/stores/savedSearchStore'
import type { SavedSearchDto } from '@/types/savedSearch'

const props = defineProps<{
  search?: SavedSearchDto | null
}>()

const emit = defineEmits<{
  submit: []
  cancel: []
}>()

const savedSearchStore = useSavedSearchStore()

const fuelTypes = ['Petrol', 'Diesel', 'Electric', 'Hybrid', 'LPG', 'CNG', 'Any']
const transmissions = ['Manual', 'Automatic', 'CVT', 'Any']
const sellerTypes = ['Private', 'Dealer', 'Any']

const formData = ref({
  name: '',
  make: '',
  model: '',
  countryCode: 'BE',
  priceMin: undefined as number | undefined,
  priceMax: undefined as number | undefined,
  yearMin: undefined as number | undefined,
  yearMax: undefined as number | undefined,
  maxMileageKm: undefined as number | undefined,
  fuelType: 'Any',
  transmission: 'Any',
  minPowerHp: undefined as number | undefined,
  sellerType: 'Any',
  city: '',
  radiusKm: undefined as number | undefined,
  requiredKeywords: [] as string[],
  excludedKeywords: [] as string[],
  isActive: true,
})

watch(
  () => props.search,
  (newSearch) => {
    if (newSearch) {
      formData.value = {
        name: newSearch.name,
        make: newSearch.make || '',
        model: newSearch.model || '',
        countryCode: newSearch.countryCode,
        priceMin: newSearch.priceMin,
        priceMax: newSearch.priceMax,
        yearMin: newSearch.yearMin,
        yearMax: newSearch.yearMax,
        maxMileageKm: newSearch.maxMileageKm,
        fuelType: newSearch.fuelType,
        transmission: newSearch.transmission,
        minPowerHp: newSearch.minPowerHp,
        sellerType: newSearch.sellerType,
        city: newSearch.city || '',
        radiusKm: newSearch.radiusKm,
        requiredKeywords: newSearch.requiredKeywords || [],
        excludedKeywords: newSearch.excludedKeywords || [],
        isActive: newSearch.isActive,
      }
    }
  },
  { immediate: true }
)

const handleSubmit = async () => {
  try {
    if (props.search) {
      await savedSearchStore.updateSavedSearch(props.search.id, {
        ...formData.value,
      })
    } else {
      await savedSearchStore.createSavedSearch({
        name: formData.value.name,
        make: formData.value.make,
        model: formData.value.model,
        countryCode: formData.value.countryCode,
        priceMin: formData.value.priceMin,
        priceMax: formData.value.priceMax,
        yearMin: formData.value.yearMin,
        yearMax: formData.value.yearMax,
        maxMileageKm: formData.value.maxMileageKm,
        fuelType: formData.value.fuelType,
        transmission: formData.value.transmission,
        minPowerHp: formData.value.minPowerHp,
        sellerType: formData.value.sellerType,
        city: formData.value.city,
        radiusKm: formData.value.radiusKm,
        requiredKeywords: formData.value.requiredKeywords,
        excludedKeywords: formData.value.excludedKeywords,
      })
    }
    emit('submit')
  } catch (error) {
    console.error('Error saving search:', error)
  }
}
</script>

<style scoped>
.saved-search-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.form-row.full-width {
  grid-template-columns: 1fr;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-group.full-width {
  grid-column: 1 / -1;
}

.form-group label {
  font-weight: 500;
  font-size: 0.9rem;
  color: #555;
}

.form-group :deep(.p-inputtext),
.form-group :deep(.p-inputnumber),
.form-group :deep(.p-dropdown) {
  width: 100%;
}

.form-actions {
  display: flex;
  gap: 0.5rem;
  margin-top: 1rem;
  justify-content: flex-end;
}

@media (max-width: 640px) {
  .form-row {
    grid-template-columns: 1fr;
  }
}
</style>
