<template>
  <div class="register-form-wrapper">
    <form @submit.prevent="handleRegister" class="register-form">
      <h2>Create Account</h2>

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
        <label for="displayName">Display Name (Optional)</label>
        <InputText
          id="displayName"
          v-model="displayName"
          placeholder="Enter your display name"
          :disabled="loading"
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

      <div class="form-group">
        <label for="confirmPassword">Confirm Password</label>
        <Password
          id="confirmPassword"
          v-model="confirmPassword"
          :feedback="false"
          :disabled="loading"
          placeholder="Confirm your password"
          required
        />
      </div>

      <Button
        type="submit"
        label="Register"
        :loading="loading"
        class="w-full"
        @click="handleRegister"
      />

      <div class="register-footer">
        <p>Already have an account? <router-link to="/login">Login here</router-link></p>
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
const displayName = ref('')
const password = ref('')
const confirmPassword = ref('')
const loading = ref(false)

const handleRegister = async () => {
  if (!email.value || !password.value || !confirmPassword.value) return
  if (password.value !== confirmPassword.value) {
    console.error('Passwords do not match')
    return
  }

  loading.value = true
  try {
    await authStore.register(email.value, password.value, displayName.value || undefined)
    router.push('/')
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Registration failed'
    console.error(message)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.register-form-wrapper {
  padding: 2rem 1rem;
}

.register-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.register-form h2 {
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

.register-footer {
  text-align: center;
  font-size: 0.85rem;
  color: #666;
}

.register-footer a {
  color: #667eea;
  text-decoration: none;
  font-weight: 600;
}

.register-footer a:hover {
  text-decoration: underline;
}
</style>
