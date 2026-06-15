import { STORAGE_KEYS } from './constants'
import type { AuthResponse } from './types'

export async function getToken(): Promise<string | null> {
  const data = (await chrome.storage.local.get(STORAGE_KEYS.AUTH_TOKEN)) as Record<string, unknown>
  const token = data[STORAGE_KEYS.AUTH_TOKEN]
  return typeof token === 'string' ? token : null
}

export async function setAuth(authResponse: AuthResponse): Promise<void> {
  await chrome.storage.local.set({
    [STORAGE_KEYS.AUTH_TOKEN]: authResponse.token,
    [STORAGE_KEYS.USER_INFO]: {
      userId: authResponse.userId,
      email: authResponse.email,
      displayName: authResponse.displayName,
      expiresAtUtc: authResponse.expiresAtUtc,
    },
  })
}

export async function clearAuth(): Promise<void> {
  await chrome.storage.local.remove([STORAGE_KEYS.AUTH_TOKEN, STORAGE_KEYS.USER_INFO])
}

export async function getAuth(): Promise<{ token: string; userInfo: AuthResponse } | null> {
  const data = (await chrome.storage.local.get([STORAGE_KEYS.AUTH_TOKEN, STORAGE_KEYS.USER_INFO])) as Record<string, unknown>
  const token = data[STORAGE_KEYS.AUTH_TOKEN]
  if (!token || typeof token !== 'string') {
    return null
  }
  return {
    token,
    userInfo: data[STORAGE_KEYS.USER_INFO] as AuthResponse,
  }
}

export async function isAuthenticated(): Promise<boolean> {
  const auth = await getAuth()
  return auth !== null
}
