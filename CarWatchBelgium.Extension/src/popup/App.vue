<template>
  <div class="app">
    <div class="header">
      <h1>CarWatch Belgium</h1>
      <p v-if="isAuthenticated">{{ userInfo?.email }}</p>
      <p v-else>Not authenticated</p>
    </div>

    <div class="container">
      <!-- Login Section -->
      <div v-if="!isAuthenticated" class="section">
        <div class="section-title">Authentication</div>
        
        <div v-if="error" class="alert alert-error">{{ error }}</div>
        
        <div class="form-group">
          <label>Email</label>
          <input
            v-model="loginForm.email"
            type="email"
            placeholder="test@example.com"
            :disabled="isLoading"
          />
        </div>

        <div class="form-group">
          <label>Password</label>
          <input
            v-model="loginForm.password"
            type="password"
            placeholder="••••••••"
            :disabled="isLoading"
          />
        </div>

        <button class="btn btn-primary" @click="handleLogin" :disabled="isLoading">
          <span v-if="isLoading"><span class="spinner"></span> Logging in...</span>
          <span v-else>Login</span>
        </button>
      </div>

      <!-- Authenticated Section -->
      <div v-else>
        <!-- Saved Searches -->
        <div class="section">
          <div class="section-title">Saved Searches</div>
          
          <div v-if="loadingSearches" class="alert alert-info">
            <span class="spinner"></span> Loading searches...
          </div>

          <div v-else-if="savedSearches.length === 0" class="alert alert-warning">
            No saved searches found. Create one in the API first.
          </div>

          <div v-else>
            <div class="form-group">
              <label>Select Search</label>
              <select
                v-model="selectedSearchId"
                @change="saveSelectedSearch"
                :disabled="isLoading"
              >
                <option value="">-- Choose a search --</option>
                <option v-for="search in savedSearches" :key="search.id" :value="search.id">
                  {{ search.name }} ({{ search.matchCount }} matches)
                </option>
              </select>
            </div>

            <div class="button-group">
              <button class="btn btn-primary" @click="loadSavedSearches" :disabled="isLoading">
                Refresh
              </button>
            </div>
          </div>
        </div>

        <!-- Scan Section -->
        <div class="section">
          <div class="section-title">Scan Page</div>
          
          <div v-if="!selectedSearchId" class="alert alert-warning">
            Select a saved search above to enable scanning.
          </div>

          <div v-else>
            <button
              class="btn btn-primary"
              @click="handleScanPage"
              :disabled="isLoading || !selectedSearchId"
              style="width: 100%; margin-bottom: 12px"
            >
              <span v-if="isScanning"><span class="spinner"></span> Scanning...</span>
              <span v-else>Scan Current Page</span>
            </button>
          </div>
        </div>

        <!-- Last Scan Result -->
        <div v-if="lastScanResult" class="section">
          <div class="section-title">Last Scan Result</div>
          
          <div v-if="lastScanResult.success" class="alert alert-success">
            Scan completed successfully!
          </div>
          <div v-else class="alert alert-error">
            Scan failed: {{ lastScanResult.error }}
          </div>

          <div v-if="lastScanResult.success" class="result-grid">
            <div class="result-item">
              <strong>{{ lastScanResult.receivedCount }}</strong>
              <span>Received</span>
            </div>
            <div class="result-item">
              <strong>{{ lastScanResult.insertedCount }}</strong>
              <span>Inserted</span>
            </div>
            <div class="result-item">
              <strong>{{ lastScanResult.updatedCount }}</strong>
              <span>Updated</span>
            </div>
            <div class="result-item">
              <strong>{{ lastScanResult.priceDroppedCount }}</strong>
              <span>Price Drops</span>
            </div>
          </div>
        </div>

        <!-- Status -->
        <div class="section">
          <div class="section-title">Status</div>
          <div class="status-box">
            <div class="status-item">
              <span class="status-label">Authentication:</span>
              <span class="status-value">✓ Connected</span>
            </div>
            <div class="status-item">
              <span class="status-label">Selected Search:</span>
              <span class="status-value">{{ selectedSearchName || 'None' }}</span>
            </div>
            <div class="status-item">
              <span class="status-label">API Base:</span>
              <span class="status-value" style="font-size: 10px; word-break: break-all">
                {{ apiBase }}
              </span>
            </div>
          </div>
        </div>

        <!-- Logout -->
        <div class="section">
          <button class="btn btn-danger" @click="handleLogout" style="width: 100%">
            Logout
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { login, getSavedSearches } from '@/shared/apiClient'
import { getAuth, setAuth, clearAuth } from '@/shared/authStorage'
import { getSelectedSavedSearchId, setSelectedSavedSearchId, getSelectedSavedSearchName, setSelectedSavedSearchName } from '@/shared/extensionStorage'
import { API_BASE_URL } from '@/shared/constants'
import type { SavedSearchDto, AuthResponse, IngestListingsResult } from '@/shared/types'

const isAuthenticated = ref<boolean>(false)
const userInfo = ref<AuthResponse | null>(null)
const isLoading = ref<boolean>(false)
const isScanning = ref<boolean>(false)
const error = ref<string>('')
const loadingSearches = ref<boolean>(false)

const loginForm = ref<{ email: string; password: string }>({ email: '', password: '' })
const savedSearches = ref<SavedSearchDto[]>([])
const selectedSearchId = ref<string>('')
const selectedSearchName = ref<string>('')

const lastScanResult = ref<IngestListingsResult | null>(null)
const apiBase = ref<string>(API_BASE_URL)

// Check if already logged in on mount
onMounted(async () => {
  try {
    const auth = await getAuth()
    if (auth) {
      isAuthenticated.value = true
      userInfo.value = auth

      // Load saved searches
      await loadSavedSearches()

      // Load selected search
      const searchId = await getSelectedSavedSearchId()
      if (searchId) {
        selectedSearchId.value = searchId
      }
      const searchName = await getSelectedSavedSearchName()
      if (searchName) {
        selectedSearchName.value = searchName
      }
    }
  } catch (err) {
    console.error('Failed to load auth:', err)
  }
})

async function handleLogin(): Promise<void> {
  if (!loginForm.value.email || !loginForm.value.password) {
    error.value = 'Please enter email and password'
    return
  }

  isLoading.value = true
  error.value = ''

  try {
    const response: AuthResponse = await login(loginForm.value.email, loginForm.value.password)
    await setAuth(response)

    isAuthenticated.value = true
    userInfo.value = response
    loginForm.value = { email: '', password: '' }

    // Load saved searches
    await loadSavedSearches()
  } catch (err) {
    error.value = err instanceof Error ? err.message : String(err)
  } finally {
    isLoading.value = false
  }
}

async function loadSavedSearches(): Promise<void> {
  loadingSearches.value = true
  try {
    const auth = await getAuth()
    if (auth) {
      savedSearches.value = await getSavedSearches(auth.token)
    }
  } catch (err) {
    error.value = err instanceof Error ? err.message : String(err)
  } finally {
    loadingSearches.value = false
  }
}

async function saveSelectedSearch(): Promise<void> {
  if (selectedSearchId.value) {
    await setSelectedSavedSearchId(selectedSearchId.value)
    const search = savedSearches.value.find((s: SavedSearchDto) => s.id === selectedSearchId.value)
    if (search) {
      selectedSearchName.value = search.name
      await setSelectedSavedSearchName(search.name)
    }
  }
}

async function handleScanPage(): Promise<void> {
  if (!selectedSearchId.value) {
    error.value = 'Please select a saved search first'
    return
  }

  isScanning.value = true
  error.value = ''

  try {
    // Send message to content script
    const tabs = await chrome.tabs.query({ active: true, currentWindow: true })
    const tab = tabs[0]
    
    if (!tab.id) {
      throw new Error('No active tab found')
    }

    chrome.tabs.sendMessage(tab.id, { type: 'SCAN_VISIBLE_LISTINGS' }, (response: IngestListingsResult | undefined) => {
      if (response) {
        lastScanResult.value = response
      }
      isScanning.value = false
    })
  } catch (err) {
    error.value = err instanceof Error ? err.message : String(err)
    isScanning.value = false
  }
}

function handleLogout(): void {
  clearAuth()
  isAuthenticated.value = false
  userInfo.value = null
  selectedSearchId.value = ''
  selectedSearchName.value = ''
  savedSearches.value = []
  lastScanResult.value = null
}
</script>

<style scoped>
@import './popup.css';
</style>
