import { scanCurrentPage, deduplicateListings } from './scanners/scannerTypes'
import { markListingsOnPage, injectStyles } from '@/overlay/listingMarker'
import { DEBOUNCE_DELAY } from '@/shared/constants'
import type { ExtensionMessage } from '@/shared/types'

let detectedListingsCount = 0
let mutationObserverInstance: MutationObserver | null = null

// Inject styles early
injectStyles()

// Setup mutation observer for dynamic content
function setupMutationObserver(): void {
  if (mutationObserverInstance) {
    mutationObserverInstance.disconnect()
  }

  let debounceTimeout: NodeJS.Timeout | null = null

  mutationObserverInstance = new MutationObserver(() => {
    if (debounceTimeout) {
      clearTimeout(debounceTimeout)
    }

    debounceTimeout = setTimeout(() => {
      // Update detected count when DOM changes
      const listings = scanCurrentPage()
      detectedListingsCount = listings.length
      console.log(`[CarWatch] Detected ${detectedListingsCount} listings`)
    }, DEBOUNCE_DELAY)
  })

  mutationObserverInstance.observe(document.body, {
    childList: true,
    subtree: true,
    characterData: false,
    attributes: false,
  })
}

// Listen for messages from popup and background
chrome.runtime.onMessage.addListener((message: ExtensionMessage, _sender: chrome.runtime.MessageSender, sendResponse: (response?: Record<string, any>) => void) => {
  console.log('[ContentScript] Received message:', message.type)

  if (message.type === 'SCAN_VISIBLE_LISTINGS') {
    handleScanRequest(sendResponse)
    return true
  }

  if (message.type === 'MARK_LISTINGS') {
    const payload = message.payload as { results?: any[] } | undefined
    const results = payload?.results
    if (results) {
      markListingsOnPage(results)
    }
    sendResponse({ success: true })
    return true
  }
})

async function handleScanRequest(sendResponse: (response?: Record<string, any>) => void): Promise<void> {
  try {
    // Scan current page
    const listings = scanCurrentPage()
    const deduplicated = deduplicateListings(listings)

    console.log(`[ContentScript] Scanned ${deduplicated.length} unique listings`)

    // Send to background worker for API call
    chrome.runtime.sendMessage(
      {
        type: 'INGEST_LISTINGS',
        payload: {
          listings: deduplicated,
        },
      },
      (response: Record<string, any> | undefined) => {
        if (response) {
          // Mark listings on page with results
          markListingsOnPage(response.items || [])
          sendResponse({ success: true, result: response })
        } else {
          sendResponse({ success: false, error: 'No response from background' })
        }
      }
    )
  } catch (error) {
    console.error('[ContentScript] Error scanning listings:', error)
    sendResponse({ success: false, error: error instanceof Error ? error.message : String(error) })
  }
}

// Initial scan
detectedListingsCount = scanCurrentPage().length
console.log(`[CarWatch] Initial scan found ${detectedListingsCount} listings`)

// Setup mutation observer
setupMutationObserver()

// Periodic re-scan (every 5 seconds)
setInterval(() => {
  const current = scanCurrentPage().length
  if (current !== detectedListingsCount) {
    detectedListingsCount = current
    console.log(`[CarWatch] Detected ${detectedListingsCount} listings (changed)`)
  }
}, 5000)

console.log('[CarWatch] Content script loaded')
