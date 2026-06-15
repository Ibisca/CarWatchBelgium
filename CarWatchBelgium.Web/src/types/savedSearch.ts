export interface SavedSearchDto {
  id: string
  userId: string
  name: string
  make?: string
  model?: string
  countryCode: string
  priceMin?: number
  priceMax?: number
  yearMin?: number
  yearMax?: number
  maxMileageKm?: number
  fuelType: string
  transmission: string
  minPowerHp?: number
  sellerType: string
  city?: string
  radiusKm?: number
  requiredKeywords: string[]
  excludedKeywords: string[]
  isActive: boolean
  createdAtUtc: string
  lastMatchedAtUtc?: string
  matchCount: number
}

export interface CreateSavedSearchRequest {
  name: string
  make?: string
  model?: string
  countryCode: string
  priceMin?: number
  priceMax?: number
  yearMin?: number
  yearMax?: number
  maxMileageKm?: number
  fuelType: string
  transmission: string
  minPowerHp?: number
  sellerType: string
  city?: string
  radiusKm?: number
  requiredKeywords: string[]
  excludedKeywords: string[]
}

export interface UpdateSavedSearchRequest extends CreateSavedSearchRequest {
  isActive: boolean
}
