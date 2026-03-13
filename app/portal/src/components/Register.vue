<template>
  <div class="page">
    <div class="register-card">
      <div class="header">
        <h1>Create account</h1>
        <p>Start organizing your tasks with a clean and simple workflow.</p>
      </div>

      <form @submit.prevent="register" class="form">
        <div class="form-group">
          <label for="username">Username</label>
          <input
            v-model.trim="username"
            type="text"
            id="username"
            placeholder="Enter your username"
            required
          />
        </div>

        <div class="form-group">
          <label for="email">Email</label>
          <input
            v-model.trim="email"
            type="email"
            id="email"
            placeholder="Enter your email"
            required
          />
        </div>

        <div class="form-group">
          <label for="password">Password</label>
          <input
            v-model="password"
            type="password"
            id="password"
            placeholder="Enter your password"
            required
          />
        </div>

        <div class="form-group">
          <label for="confirmPassword">Confirm password</label>
          <input
            v-model="confirmPassword"
            type="password"
            id="confirmPassword"
            placeholder="Confirm your password"
            required
          />
        </div>

        <button type="submit" :disabled="isLoading" class="submit-btn">
          {{ isLoading ? 'Creating account...' : 'Register' }}
        </button>

        <p v-if="successMessage" class="success-message">
          {{ successMessage }}
        </p>

        <p v-if="error" class="error-message">
          {{ error }}
        </p>
      </form>

      <div class="footer-text">
        Already have an account?
        <router-link to="/login">Login</router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import api from '../tools/api-request.js'
import { useRouter } from 'vue-router'

const username = ref('')
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const isLoading = ref(false)
const error = ref('')
const successMessage = ref('')
const router = useRouter()

const register = async () => {
  error.value = ''
  successMessage.value = ''

  if (!username.value || !email.value || !password.value || !confirmPassword.value) {
    error.value = 'Please complete all fields.'
    return
  }

  if (password.value !== confirmPassword.value) {
    error.value = 'Passwords do not match.'
    return
  }

  isLoading.value = true

  try {
    const response = await api.post(
      '/api/auth/register',
      {
        userName: username.value,
        email: email.value,
        password: password.value
      },
      { skipAuth: true }
    )

    console.log('register success:', response.data)

    successMessage.value = 'Registration successful. Redirecting to login...'

    setTimeout(() => {
      router.push('/login')
    }, 800)
  } catch (err) {
    console.log('register failed:', err)

    const status = err.response?.status
    const data = err.response?.data

    if (status === 400) {
      if (Array.isArray(data?.data)) {
        error.value = data.data.map(x => x.description || x.code || 'Validation failed').join(' ; ')
      } else if (typeof data?.data === 'string') {
        error.value = data.data
      } else if (typeof data?.message === 'string') {
        error.value = data.message
      } else {
        error.value = 'Registration failed. Please check your input.'
      }
    } else if (status === 500) {
      error.value = 'Server error occurred during registration.'
    } else {
      error.value = 'Registration failed. Please try again.'
    }
  } finally {
    isLoading.value = false
  }
}
</script>

<style scoped>
.page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 32px 16px;
  background:
    radial-gradient(circle at top left, rgba(76, 175, 80, 0.12), transparent 28%),
    radial-gradient(circle at bottom right, rgba(33, 150, 243, 0.1), transparent 26%),
    linear-gradient(135deg, #f4f7fb 0%, #eef3f8 100%);
}

.register-card {
  width: 100%;
  max-width: 460px;
  background: rgba(255, 255, 255, 0.94);
  border: 1px solid rgba(255, 255, 255, 0.7);
  border-radius: 20px;
  box-shadow: 0 16px 40px rgba(15, 23, 42, 0.12);
  padding: 32px 28px;
  backdrop-filter: blur(8px);
}

.header {
  margin-bottom: 24px;
}

.header h1 {
  margin: 0 0 8px;
  font-size: 28px;
  font-weight: 700;
  color: #1f2937;
}

.header p {
  margin: 0;
  color: #6b7280;
  line-height: 1.5;
  font-size: 14px;
}

.form {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group label {
  margin-bottom: 8px;
  font-size: 14px;
  font-weight: 600;
  color: #374151;
}

.form-group input {
  width: 100%;
  box-sizing: border-box;
  padding: 12px 14px;
  font-size: 14px;
  border: 1px solid #d1d5db;
  border-radius: 12px;
  background: #fff;
  color: #111827;
  transition: all 0.2s ease;
  outline: none;
}

.form-group input::placeholder {
  color: #9ca3af;
}

.form-group input:focus {
  border-color: #4CAF50;
  box-shadow: 0 0 0 4px rgba(76, 175, 80, 0.14);
}

.submit-btn {
  margin-top: 4px;
  width: 100%;
  padding: 13px 16px;
  border: none;
  border-radius: 12px;
  background: linear-gradient(135deg, #4CAF50 0%, #43a047 100%);
  color: #fff;
  font-size: 15px;
  font-weight: 700;
  cursor: pointer;
  transition: transform 0.15s ease, box-shadow 0.2s ease, opacity 0.2s ease;
  box-shadow: 0 10px 20px rgba(76, 175, 80, 0.22);
}

.submit-btn:hover:enabled {
  transform: translateY(-1px);
  box-shadow: 0 14px 26px rgba(76, 175, 80, 0.28);
}

.submit-btn:disabled {
  cursor: not-allowed;
  opacity: 0.7;
  box-shadow: none;
}

.error-message,
.success-message {
  margin: 2px 0 0;
  padding: 12px 14px;
  border-radius: 12px;
  font-size: 14px;
  line-height: 1.5;
}

.error-message {
  color: #b42318;
  background: #fef3f2;
  border: 1px solid #fecdca;
}

.success-message {
  color: #067647;
  background: #ecfdf3;
  border: 1px solid #abefc6;
}

.footer-text {
  margin-top: 22px;
  text-align: center;
  color: #6b7280;
  font-size: 14px;
}

.footer-text a {
  color: #2e7d32;
  font-weight: 600;
  text-decoration: none;
}

.footer-text a:hover {
  text-decoration: underline;
}

@media (max-width: 520px) {
  .register-card {
    padding: 24px 18px;
    border-radius: 16px;
  }

  .header h1 {
    font-size: 24px;
  }
}
</style>