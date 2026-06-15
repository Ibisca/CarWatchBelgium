import type { IngestListingItemResultDto } from '@/shared/types'

export function markListingsOnPage(results: IngestListingItemResultDto[]): void {
  // Create a map for quick lookup by URL
  const resultMap = new Map<string, IngestListingItemResultDto>()
  for (const result of results) {
    if (result.url) {
      resultMap.set(result.url, result)
    }
  }

  // Find all links that might be listing URLs
  const links = document.querySelectorAll('a[href]') as NodeListOf<HTMLAnchorElement>

  for (const link of links) {
    const result = resultMap.get(link.href)
    if (!result) continue

    // Find the card container
    const card = link.closest('article') || 
                 link.closest('[data-testid*="list-item"]') || 
                 link.closest('[role="article"]') ||
                 link.parentElement

    if (!card || card.getAttribute('data-carwatch-marked') === 'true') {
      continue
    }

    // Mark as processed
    card.setAttribute('data-carwatch-marked', 'true')

    // Create badge
    const badge = createBadge(result)
    if (badge) {
      card.appendChild(badge)
    }
  }
}

function createBadge(result: IngestListingItemResultDto): HTMLElement | null {
  const badge = document.createElement('div')
  badge.className = 'carwatch-badge'
  badge.setAttribute('data-carwatch-badge', 'true')

  const statuses: string[] = []

  if (result.wasInserted || result.wasNewMatch) {
    statuses.push('NEW')
  }
  if (result.priceDropped) {
    statuses.push('PRICE DROP')
  }
  if (result.wasUpdated && !result.wasInserted) {
    statuses.push('UPDATED')
  }

  if (statuses.length === 0) {
    statuses.push('SCANNED')
  }

  badge.textContent = `CarWatch: ${statuses.join(' • ')}`
  badge.style.cssText = `
    position: absolute;
    top: 8px;
    right: 8px;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    padding: 6px 12px;
    border-radius: 4px;
    font-size: 11px;
    font-weight: bold;
    z-index: 1000;
    box-shadow: 0 2px 8px rgba(0,0,0,0.15);
    border: 1px solid rgba(255,255,255,0.2);
  `

  // Color based on status
  if (result.priceDropped) {
    badge.style.background = 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)'
  } else if (result.wasInserted || result.wasNewMatch) {
    badge.style.background = 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)'
  }

  // Ensure parent has position for absolute badge
  const parent = badge.parentElement
  if (parent && window.getComputedStyle(parent).position === 'static') {
    parent.style.position = 'relative'
  }

  return badge
}

export function injectStyles(): void {
  if (document.getElementById('carwatch-styles')) {
    return
  }

  const style = document.createElement('style')
  style.id = 'carwatch-styles'
  style.textContent = `
    [data-carwatch-marked="true"] {
      transition: box-shadow 0.2s ease;
    }
    [data-carwatch-marked="true"]:hover {
      box-shadow: 0 0 12px rgba(102, 126, 234, 0.3) !important;
    }
    .carwatch-badge {
      animation: slideIn 0.3s ease-out;
    }
    @keyframes slideIn {
      from {
        opacity: 0;
        transform: translateX(10px);
      }
      to {
        opacity: 1;
        transform: translateX(0);
      }
    }
  `
  document.head.appendChild(style)
}
