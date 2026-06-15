<template>
  <nav class="navbar">
    <div class="navbar-content">
      <div class="navbar-brand">
        <h1>CarWatch Belgium</h1>
      </div>
      <div class="navbar-user">
        <span v-if="authStore.user" class="user-name">{{ authStore.user.displayName || authStore.user.email }}</span>
        <Button
          icon="pi pi-sign-out"
          severity="secondary"
          text
          rounded
          @click="handleLogout"
        />
      </div>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import Button from 'primevue/button'

const router = useRouter()
const authStore = useAuthStore()

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}
</script>

<style scoped>
.navbar {
  background: linear-gradient(90deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 0;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.navbar-content {
  max-width: 1400px;
  margin: 0 auto;
  padding: 1rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.navbar-brand h1 {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
}

.navbar-user {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.user-name {
  font-size: 0.9rem;
}

:deep(.p-button-secondary) {
  color: white !important;
}

:deep(.p-button-secondary:hover) {
  background-color: rgba(255, 255, 255, 0.1) !important;
}
</style>
