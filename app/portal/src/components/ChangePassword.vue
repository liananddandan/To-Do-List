<template>
  <div class="page">
    <div class="shell">
      <section class="card">
        <div class="header">
          <p class="eyebrow">Security</p>
          <h1>Change password</h1>
          <p>Updating your password signs you out and requires a new login with the new credentials.</p>
        </div>

        <form class="form" @submit.prevent="submitForm">
          <div class="form-group">
            <label for="new-password">New password</label>
            <input id="new-password" type="password" v-model="newPassword" required />
          </div>

          <div class="form-group">
            <label for="confirm-password">Confirm new password</label>
            <input id="confirm-password" type="password" v-model="confirmPassword" required />
          </div>

          <p v-if="message" class="message" :class="messageType">{{ message }}</p>

          <div class="actions">
            <button class="secondary-btn" type="button" @click="router.push('/profile')">Back</button>
            <button class="primary-btn" type="submit" :disabled="isSubmitting">
              {{ isSubmitting ? 'Updating...' : 'Update password' }}
            </button>
          </div>
        </form>
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../tools/api-request.js'
import { logout } from '../tools/auth.js'

const router = useRouter()
const newPassword = ref('')
const confirmPassword = ref('')
const isSubmitting = ref(false)
const message = ref('')
const messageType = ref('error')

const submitForm = async () => {
  message.value = ''

  if (newPassword.value !== confirmPassword.value) {
    message.value = 'Passwords do not match.'
    messageType.value = 'error'
    return
  }

  try {
    isSubmitting.value = true
    const response = await api.put('/api/auth/password', {
      password: newPassword.value,
      confirmPassword: confirmPassword.value
    })

    if (response.data?.data?.code === 200002) {
      message.value = 'Password updated. Redirecting to login...'
      messageType.value = 'success'

      setTimeout(() => {
        logout()
        router.push('/login')
      }, 700)
    } else {
      message.value = 'Password update failed.'
      messageType.value = 'error'
    }
  } catch (err) {
    message.value = 'Password update failed.'
    messageType.value = 'error'
  } finally {
    isSubmitting.value = false
  }
}
</script>

<style scoped>
.page {
  min-height: calc(100vh - 70px);
  padding: 32px 20px 48px;
  background: var(--page-background);
}

.shell {
  max-width: 760px;
  margin: 0 auto;
}

.card {
  padding: 28px;
  border-radius: 28px;
  background: var(--surface);
  border: 1px solid var(--border);
  box-shadow: var(--shadow-soft);
}

.eyebrow {
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  color: var(--accent);
  font-weight: 700;
}

.header h1 {
  margin-top: 10px;
  font-size: 34px;
  font-weight: 800;
}

.header p {
  margin-top: 10px;
  color: var(--text-secondary);
}

.form {
  margin-top: 24px;
  display: flex;
  flex-direction: column;
  gap: 18px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-size: 14px;
  font-weight: 700;
}

.form-group input {
  border: 1px solid var(--border);
  border-radius: 14px;
  padding: 12px 14px;
  background: var(--surface-muted);
  outline: none;
}

.form-group input:focus {
  border-color: rgba(47, 133, 90, 0.5);
  box-shadow: 0 0 0 4px rgba(47, 133, 90, 0.12);
}

.message {
  border-radius: 14px;
  padding: 12px 14px;
  font-weight: 600;
}

.message.error {
  color: var(--danger);
  background: var(--danger-soft);
}

.message.success {
  color: var(--brand-strong);
  background: var(--brand-soft);
}

.actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.primary-btn,
.secondary-btn {
  border: none;
  border-radius: 14px;
  padding: 12px 16px;
  font-weight: 700;
}

.primary-btn {
  background: linear-gradient(135deg, #48bb78 0%, #2f855a 100%);
  color: white;
}

.secondary-btn {
  background: var(--surface-muted);
  color: var(--text-primary);
}

@media (max-width: 640px) {
  .page {
    padding: 20px 14px 32px;
  }

  .header h1 {
    font-size: 28px;
  }

  .actions {
    flex-direction: column-reverse;
  }

  .primary-btn,
  .secondary-btn {
    width: 100%;
  }
}
</style>
