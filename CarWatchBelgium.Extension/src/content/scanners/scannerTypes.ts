import { scanAutoScout24 } from './autoscout24Scanner'
import { scanFacebookMarketplace } from './facebookMarketplaceScanner'
import type { ExtractedListing } from '@/shared/types'

export function scanCurrentPage(): ExtractedListing[] {
  const url = window.location.href

  if (url.includes('autoscout24.be')) {
    return scanAutoScout24()
  }

  if (url.includes('facebook.com/marketplace')) {
    return scanFacebookMarketplace()
  }

  return []
}

export function deduplicateListings(listings: ExtractedListing[]): ExtractedListing[] {
  const seen = new Map<string, ExtractedListing>()

  for (const listing of listings) {
    const key = listing.externalId
      ? `${listing.source}:${listing.externalId}`
      : `${listing.source}:${listing.url}`

    if (!seen.has(key)) {
      seen.set(key, listing)
    }
  }

  return Array.from(seen.values())
}
