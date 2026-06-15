export const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL as string | undefined) || 'http://localhost:5003'

export const API_ENDPOINTS = {
  LOGIN: `${API_BASE_URL}/api/auth/login`,
  SAVED_SEARCHES: `${API_BASE_URL}/api/saved-searches`,
  INGEST_LISTINGS: `${API_BASE_URL}/api/listings/ingest`,
}

export const STORAGE_KEYS = {
  AUTH_TOKEN: 'carwatch_auth_token',
  USER_INFO: 'carwatch_user_info',
  SELECTED_SEARCH_ID: 'carwatch_selected_search_id',
  SELECTED_SEARCH_NAME: 'carwatch_selected_search_name',
}

export const DEBOUNCE_DELAY = 1500

export const SCANNER_SELECTORS = {
  AUTOSCOUT24: {
    ARTICLE: 'article',
    LIST_ITEM: '[data-testid*="list-item"]',
    OFFER_LINK: 'a[href*="/offers/"]',
  },
  FACEBOOK: {
    MARKETPLACE_LINK: 'a[href*="/marketplace/item/"]',
  },
}
