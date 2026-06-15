import { STORAGE_KEYS } from './constants'

export async function getSelectedSavedSearchId(): Promise<string | null> {
  const data = (await chrome.storage.local.get(STORAGE_KEYS.SELECTED_SEARCH_ID)) as Record<string, unknown>
  const id = data[STORAGE_KEYS.SELECTED_SEARCH_ID]
  return typeof id === 'string' ? id : null
}

export async function setSelectedSavedSearchId(id: string): Promise<void> {
  await chrome.storage.local.set({
    [STORAGE_KEYS.SELECTED_SEARCH_ID]: id,
  })
}

export async function getSelectedSavedSearchName(): Promise<string | null> {
  const data = (await chrome.storage.local.get(STORAGE_KEYS.SELECTED_SEARCH_NAME)) as Record<string, unknown>
  const name = data[STORAGE_KEYS.SELECTED_SEARCH_NAME]
  return typeof name === 'string' ? name : null
}

export async function setSelectedSavedSearchName(name: string): Promise<void> {
  await chrome.storage.local.set({
    [STORAGE_KEYS.SELECTED_SEARCH_NAME]: name,
  })
}
