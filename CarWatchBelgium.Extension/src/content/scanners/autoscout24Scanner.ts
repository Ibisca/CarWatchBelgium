import type { ExtractedListing } from '@/shared/types'

export function scanAutoScout24(): ExtractedListing[] {
  const listings: ExtractedListing[] = []
  const seenUrls = new Set<string>()

  // Try multiple selector strategies
  const candidates = [
    ...document.querySelectorAll('article'),
    ...document.querySelectorAll('[data-testid*="list-item"]'),
    ...document.querySelectorAll('a[href*="/offers/"]'),
  ]

  for (const candidate of candidates) {
    // Skip duplicates by URL
    const linkEl = candidate.querySelector('a[href*="/offers/"]')
    const link = (linkEl as HTMLAnchorElement | null) || 
                 (candidate.tagName === 'A' ? (candidate as HTMLAnchorElement) : null)
    
    if (!link || !link.href) continue
    
    const url = link.href
    if (seenUrls.has(url)) continue
    seenUrls.add(url)

    // Extract external ID from URL
    const offerMatch = url.match(/\/offers\/(\d+)/)
    const externalId = offerMatch ? offerMatch[1] : null

    // Get the card container
    const card: Element | null = link.closest('article') || link.closest('[data-testid*="list-item"]') || link.parentElement
    if (!card) continue

    // Extract title
    const titleEl = card.querySelector('h1, h2, h3, [class*="title"]') as HTMLElement | null
    const title: string = titleEl?.textContent?.trim() || link.textContent?.trim() || 'Unknown'

    // Extract image
    const imgEl = card.querySelector('img') as HTMLImageElement | null
    const imageUrl: string | null = imgEl?.src || imgEl?.dataset['src'] || null

    // Extract price (look for € symbol)
    const textContent = card.textContent || ''
    const priceMatch: RegExpMatchArray | null = textContent.match(/€\s*([\d\.,]+)/)
    const price: number | undefined = priceMatch ? parseFloat(priceMatch[1].replace(/[.,]/g, '.')) : undefined

    // Extract mileage (km)
    const mileageMatch = card.textContent?.match(/([\d\.,]+)\s*km/)
    const mileageKm = mileageMatch ? parseInt(mileageMatch[1].replace(/[.,]/g, '')) : undefined

    // Extract year (19xx or 20xx)
    const yearMatch = card.textContent?.match(/(19|20)\d{2}/)
    const year = yearMatch ? parseInt(yearMatch[0]) : undefined

    // Detect fuel type
    let fuelType = 'Unknown'
    const textContentLower: string = textContent.toLowerCase()
    if (textContentLower.includes('diesel')) fuelType = 'Diesel'
    else if (textContentLower.includes('petrol') || textContentLower.includes('benzine') || textContentLower.includes('essence')) fuelType = 'Petrol'
    else if (textContentLower.includes('hybrid')) fuelType = 'Hybrid'
    else if (textContentLower.includes('electric') || textContentLower.includes('électrique')) fuelType = 'Electric'
    else if (textContentLower.includes('lpg')) fuelType = 'LPG'

    // Detect transmission
    let transmission = 'Unknown'
    if (textContentLower.includes('automatic') || textContentLower.includes('automaat')) transmission = 'Automatic'
    else if (textContentLower.includes('manual') || textContentLower.includes('manueel')) transmission = 'Manual'

    listings.push({
      source: 'AutoScout24',
      externalId: externalId ? externalId : undefined,
      url,
      imageUrl,
      title,
      price,
      currency: 'EUR',
      mileageKm,
      year,
      fuelType,
      transmission,
      sellerType: 'Unknown',
      countryCode: 'BE',
    })
  }

  return listings
}
