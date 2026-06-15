import { getAuth } from '@/shared/authStorage'
import { ingestListings } from '@/shared/apiClient'
import type { ExtensionMessage, ExtractedListing } from '@/shared/types'

console.log('[Background] Service worker initialized')

// Listen for messages from content script and popup
chrome.runtime.onMessage.addListener((message: ExtensionMessage, _sender: chrome.runtime.MessageSender, sendResponse: (response?: Record<string, any>) => void) => {
  console.log('[Background] Received message:', message.type)

  if (message.type === 'INGEST_LISTINGS') {
    handleIngestListings(message.payload as { listings: any[] } | undefined, sendResponse)
    return true // Will respond asynchronously
  }

  if (message.type === 'SHOW_NOTIFICATION') {
    const notifPayload = message.payload as Record<string, any> | undefined
    if (notifPayload) {
      handleShowNotification(notifPayload)
    }
    sendResponse({ success: true })
    return true
  }
})

async function handleIngestListings(
  payload: { listings: ExtractedListing[] } | undefined,
  sendResponse: (response?: Record<string, any>) => void
): Promise<void> {
  if (!payload) {
    sendResponse({ success: false, error: 'Invalid payload' })
    return
  }
  try {
    const auth = await getAuth()
    if (!auth) {
      sendResponse({ success: false, error: 'Not authenticated' })
      return
    }

    // Get selected search ID from storage
    const data = await chrome.storage.local.get('carwatch_selected_search_id')
    const savedSearchId = data['carwatch_selected_search_id']

    if (!savedSearchId) {
      sendResponse({ success: false, error: 'No saved search selected' })
      return
    }

    // Create ingest request
    const request: IngestListingsRequest = {
      savedSearchId,
      scannedAtUtc: new Date().toISOString(),
      listings: payload.listings || [],
    }

    console.log(`[Background] Ingesting ${request.listings.length} listings...`)

    // Call API
    const result = await ingestListings(auth.token, request)

    console.log('[Background] Ingest result:', result)

    // Send notification if needed
    if (result.insertedCount > 0 || result.priceDroppedCount > 0) {
      showNotification(result)
    }

    sendResponse({ success: true, ...result })
  } catch (error) {
    console.error('[Background] Error ingesting listings:', error)
    sendResponse({ success: false, error: String(error) })
  }
}

function handleShowNotification(payload: Record<string, any>): void {
  showNotification(payload)
}

function showNotification(result: Record<string, any>): void {
  const { insertedCount = 0, priceDroppedCount = 0 } = result

  const messages: string[] = []
  if (insertedCount > 0) {
    messages.push(`${insertedCount} new listing${insertedCount > 1 ? 's' : ''}`)
  }
  if (priceDroppedCount > 0) {
    messages.push(`${priceDroppedCount} price drop${priceDroppedCount > 1 ? 's' : ''}`)
  }

  if (messages.length === 0) return

  const message = messages.join(', ')

  chrome.notifications.create('carwatch_ingest_result', {
    type: 'basic',
    iconUrl: '/icon-128.png',
    title: 'CarWatch Belgium',
    message: message,
    priority: 2,
  })

  console.log('[Background] Notification shown:', message)
}

// Handle notification clicks (optional)
chrome.notifications.onClicked.addListener((notificationId) => {
  if (notificationId === 'carwatch_ingest_result') {
    console.log('[Background] Notification clicked')
    // Could open extension popup or saved search page
  }
})
