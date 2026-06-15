<template>
  <div class="login-form-wrapper">
    <form @submit.prevent="handleLogin" class="login-form">
      <h2>Login</h2>

      <div class="form-group">
        <label for="email">Email</label>
        <InputText
          id="email"
          v-model="email"
          type="email"
          placeholder="Enter your email"
          :disabled="loading"
          required
        />
      </div>

      <div class="form-group">
        <label for="password">Password</label>
        <Password
          id="password"
          v-model="password"
          :feedback="false"
          :disabled="loading"
          placeholder="Enter your password"
          required
        />
      </div>

      <Button
        type="submit"
        label="Login"
        :loading="loading"
        class="w-full"
        @click="handleLogin"
      />

      <div class="login-footer">
        <p>Don't have an account? <router-link to="/register">Register here</router-link></p>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import InputText from 'primevue/inputtext'
import Password from 'primevue/password'
import Button from 'primevue/button'

const router = useRouter()
const authStore = useAuthStore()

const email = ref('')
const password = ref('')
const loading = ref(false)

const handleLogin = async () => {
  if (!email.value || !password.value) return

  loading.value = true
  try {
    await authStore.login(email.value, password.value)
    router.push('/')
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Login failed'
    console.error(message)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.login-form-wrapper {
  padding: 2rem 1rem;
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.login-form h2 {
  margin: 0 0 1rem 0;
  text-align: center;
  color: #333;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-group label {
  font-weight: 500;
  color: #555;
  font-size: 0.9rem;
}

.form-group :deep(.p-inputtext) {
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 0.375rem;
}

.form-group :deep(.p-password) {
  width: 100%;
}

.w-full {
  width: 100% !important;
}

.login-footer {
  text-align: center;
  font-size: 0.85rem;
  color: #666;
}

.login-footer a {
  color: #667eea;
  text-decoration: none;
  font-weight: 600;
}

.login-footer a:hover {
  text-decoration: underline;
}
</style>
