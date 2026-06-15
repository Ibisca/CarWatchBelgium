export interface AuthResponse {
  userId: string
  email: string
  displayName?: string
  token: string
  expiresAtUtc: string
}

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

export interface ExtractedListing {
  source: 'AutoScout24' | 'FacebookMarketplace'
  externalId?: string | null
  url: string
  imageUrl?: string | null
  title: string
  make?: string | null
  model?: string | null
  price?: number | null
  currency: string
  mileageKm?: number | null
  year?: number | null
  fuelType: string
  transmission: string
  powerHp?: number | null
  sellerType: string
  city?: string | null
  countryCode: string
}

export interface IngestListingsRequest {
  savedSearchId: string
  scannedAtUtc: string
  listings: ExtractedListing[]
}

export interface IngestListingItemResultDto {
  listingId?: string
  title: string
  url: string
  wasInserted: boolean
  wasUpdated: boolean
  wasMatched: boolean
  wasNewMatch: boolean
  priceChanged: boolean
  priceDropped: boolean
  oldPrice?: number
  newPrice?: number
  skipReason?: string
}

export interface IngestListingsResult {
  receivedCount: number
  insertedCount: number
  updatedCount: number
  matchedCount: number
  newMatchCount: number
  priceChangedCount: number
  priceDroppedCount: number
  skippedCount: number
  items: IngestListingItemResultDto[]
}

export interface ExtensionMessage<T = any> {
  type: string
  payload?: T
}

export type MessageType =
  | 'SCAN_VISIBLE_LISTINGS'
  | 'INGEST_LISTINGS'
  | 'MARK_LISTINGS'
  | 'SHOW_NOTIFICATION'
