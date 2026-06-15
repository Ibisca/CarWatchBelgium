import type { ExtractedListing } from '@/shared/types'

export function scanFacebookMarketplace(): ExtractedListing[] {
  const listings: ExtractedListing[] = []
  const seenUrls = new Set<string>()

  // Check if we're on Facebook Marketplace
  if (!window.location.href.includes('facebook.com/marketplace')) {
    return listings
  }

  // Find all marketplace item links
  const itemLinks = document.querySelectorAll('a[href*="/marketplace/item/"]') as NodeListOf<HTMLAnchorElement>

  for (const link of itemLinks) {
    const url: string = link.href
    if (!url || seenUrls.has(url)) continue
    seenUrls.add(url)

    // Extract external ID from URL
    const idMatch: RegExpMatchArray | null = url.match(/\/marketplace\/item\/(\d+)/)
    const externalId: string | null = idMatch ? idMatch[1] : null

    // Get the card container
    const card: Element | null = link.closest('[role="article"]') || link.closest('div[class*="card"]') || link.parentElement
    if (!card) continue

    // Extract title
    const title: string = link.textContent?.trim() || 'Unknown'

    // Extract image
    const imgEl = card.querySelector('img') as HTMLImageElement | null
    const imageUrl: string | null = imgEl?.src || imgEl?.dataset['src'] || null

    // Extract price (look for currency symbol)
    const textContent: string = card.textContent || ''
    const priceMatch: RegExpMatchArray | null = textContent.match(/€\s*([\d\.,]+)|\$\s*([\d\.,]+)/)
    let price: number | undefined
    if (priceMatch) {
      const priceStr: string = priceMatch[1] || priceMatch[2]
      price = parseFloat(priceStr.replace(/[.,]/g, '.'))
    }

    // Extract mileage, year, fuel, transmission if visible
    const cardText: string = textContent.toLowerCase()
    
    const mileageMatch: RegExpMatchArray | null = textContent.match(/([\d\.,]+)\s*km/)
    const mileageKm: number | undefined = mileageMatch ? parseInt(mileageMatch[1].replace(/[.,]/g, '')) : undefined

    const yearMatch: RegExpMatchArray | null = textContent.match(/(19|20)\d{2}/)
    const year: number | undefined = yearMatch ? parseInt(yearMatch[0]) : undefined

    let fuelType: string = 'Unknown'
    if (cardText.includes('diesel')) fuelType = 'Diesel'
    else if (cardText.includes('petrol') || cardText.includes('benzine') || cardText.includes('essence')) fuelType = 'Petrol'
    else if (cardText.includes('hybrid')) fuelType = 'Hybrid'
    else if (cardText.includes('electric') || cardText.includes('électrique')) fuelType = 'Electric'
    else if (cardText.includes('lpg')) fuelType = 'LPG'

    let transmission: string = 'Unknown'
    if (cardText.includes('automatic') || cardText.includes('automaat')) transmission = 'Automatic'
    else if (cardText.includes('manual') || cardText.includes('manueel')) transmission = 'Manual'

    listings.push({
      source: 'FacebookMarketplace',
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
