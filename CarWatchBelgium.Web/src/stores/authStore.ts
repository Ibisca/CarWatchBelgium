import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi } from '@/services/authApi'
import type { AuthResponse, CurrentUserDto } from '@/types/auth'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('authToken'))
  const user = ref<CurrentUserDto | null>(null)
  const loading = ref(false)

  const isAuthenticated = computed(() => !!token.value)

  async function register(email: string, password: string, displayName?: string): Promise<void> {
    loading.value = true
    try {
      const response: AuthResponse = await authApi.register({ email, password, displayName })
      token.value = response.token
      user.value = {
        userId: response.userId,
        email: response.email,
        displayName: response.displayName,
        createdAtUtc: new Date().toISOString(),
      }
      localStorage.setItem('authToken', response.token)
      localStorage.setItem('userInfo', JSON.stringify(user.value))
    } finally {
      loading.value = false
    }
  }

  async function login(email: string, password: string): Promise<void> {
    loading.value = true
    try {
      const response: AuthResponse = await authApi.login({ email, password })
      token.value = response.token
      user.value = {
        userId: response.userId,
        email: response.email,
        displayName: response.displayName,
        createdAtUtc: new Date().toISOString(),
      }
      localStorage.setItem('authToken', response.token)
      localStorage.setItem('userInfo', JSON.stringify(user.value))
    } finally {
      loading.value = false
    }
  }

  async function loadCurrentUser(): Promise<void> {
    if (!isAuthenticated.value) return
    loading.value = true
    try {
      user.value = await authApi.getCurrentUser()
    } finally {
      loading.value = false
    }
  }

  function logout(): void {
    token.value = null
    user.value = null
    localStorage.removeItem('authToken')
    localStorage.removeItem('userInfo')
  }

  function restoreFromStorage(): void {
    const savedToken = localStorage.getItem('authToken')
    const savedUser = localStorage.getItem('userInfo')
    if (savedToken) {
      token.value = savedToken
    }
    if (savedUser) {
      try {
        user.value = JSON.parse(savedUser)
      } catch {
        // ignore
      }
    }
  }

  return {
    token,
    user,
    loading,
    isAuthenticated,
    register,
    login,
    loadCurrentUser,
    logout,
    restoreFromStorage,
  }
})
