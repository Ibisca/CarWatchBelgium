export interface MatchedListingDto {
  listingId: string
  savedSearchId: string
  userListingStateId?: string
  source: string
  externalId?: string
  url: string
  imageUrl?: string
  title: string
  make?: string
  model?: string
  price?: number
  currency: string
  previousPrice?: number
  priceChanged: boolean
  priceDropped: boolean
  priceDropAmount?: number
  mileageKm?: number
  year?: number
  fuelType: string
  transmission: string
  powerHp?: number
  sellerType: string
  city?: string
  countryCode: string
  isAvailable: boolean
  isSeen: boolean
  isFavorite: boolean
  isIgnored: boolean
  isNew: boolean
  firstSeenAtUtc: string
  lastSeenAtUtc: string
  lastPriceChangeAtUtc?: string
  matchedAtUtc: string
}

export interface ListingDetailsDto {
  listingId: string
  source: string
  externalId?: string
  url: string
  imageUrl?: string
  title: string
  make?: string
  model?: string
  price?: number
  currency: string
  mileageKm?: number
  year?: number
  fuelType: string
  transmission: string
  powerHp?: number
  sellerType: string
  city?: string
  countryCode: string
  isAvailable: boolean
  isSeen: boolean
  isFavorite: boolean
  isIgnored: boolean
  firstSeenAtUtc: string
  lastSeenAtUtc: string
  lastPriceChangeAtUtc?: string
  priceHistory: PriceHistoryDto[]
}

export interface PriceHistoryDto {
  id: string
  price: number
  currency: string
  capturedAtUtc: string
}

export interface UpdateListingStateRequest {
  isSeen?: boolean
  isFavorite?: boolean
  isIgnored?: boolean
}
