<template>
  <div class="page">
    <div class="login-card">
      <div class="header">
        <h1>Welcome back</h1>
        <p>Sign in to manage your tasks.</p>
      </div>

      <form @submit.prevent="loginEvent" class="form">
        <div class="form-group">
          <label>Email</label>
          <input
            v-model="email"
            type="email"
            placeholder="Enter your email"
            required
          />
        </div>

        <div class="form-group">
          <label>Password</label>
          <input
            v-model="password"
            type="password"
            placeholder="Enter your password"
            required
          />
        </div>

        <button type="submit" :disabled="isLoading" class="submit-btn">
          {{ isLoading ? "Signing in..." : "Login" }}
        </button>

        <p v-if="error" class="error-message">
          {{ error }}
        </p>
      </form>

      <div class="footer-text">
        Don't have an account?
        <router-link to="/register">Sign up</router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import api from '../tools/api-request.js'
import { useRouter } from 'vue-router'
import { login, setUserInfo } from '../tools/auth.js'

const email = ref('')
const password = ref('')
const isLoading = ref(false)
const error = ref('')
const router = useRouter()

const loginEvent = async () => {
  error.value = ''
  isLoading.value = true

  try {
    const response = await api.post(
      '/api/auth/login',
      {
        email: email.value,
        password: password.value
      },
      { skipAuth: true }
    )

    const { accessToken, refreshToken } = response.data.data || {}

    if (!accessToken || !refreshToken) {
      error.value = 'Login succeeded but token data is missing.'
      return
    }

    login(accessToken, refreshToken)

    const userInfoResponse = await api.get('/api/users/me')
    const currentUser = userInfoResponse.data?.data?.info
    setUserInfo(currentUser)

    router.push('/')
  } catch (err) {
    const status = err.response?.status

    if (status === 401) {
      error.value = 'Email or password is incorrect.'
    } else if (status === 400) {
      error.value = 'Login request is invalid.'
    } else if (status === 500) {
      error.value = 'Server error occurred during login.'
    } else {
      error.value = 'Login failed. Please try again.'
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
  padding: 32px;
  background:
    radial-gradient(circle at top left, rgba(76,175,80,0.12), transparent 30%),
    radial-gradient(circle at bottom right, rgba(33,150,243,0.12), transparent 30%),
    linear-gradient(135deg,#f4f7fb 0%,#eef3f8 100%);
}

.login-card {
  width: 100%;
  max-width: 420px;
  background: rgba(255,255,255,0.95);
  border-radius: 18px;
  padding: 32px 28px;
  box-shadow: 0 18px 40px rgba(0,0,0,0.12);
}

.header {
  text-align: center;
  margin-bottom: 24px;
}

.header h1 {
  font-size: 26px;
  margin-bottom: 6px;
}

.header p {
  color: #6b7280;
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
  font-size: 14px;
  margin-bottom: 6px;
}

.form-group input {
  padding: 12px 14px;
  border-radius: 10px;
  border: 1px solid #d1d5db;
  font-size: 14px;
  outline: none;
  transition: 0.2s;
}

.form-group input:focus {
  border-color: #4CAF50;
  box-shadow: 0 0 0 3px rgba(76,175,80,0.15);
}

.submit-btn {
  margin-top: 6px;
  padding: 13px;
  border-radius: 10px;
  border: none;
  font-weight: 600;
  font-size: 15px;
  background: linear-gradient(135deg,#4CAF50,#43a047);
  color: white;
  cursor: pointer;
  transition: 0.2s;
}

.submit-btn:hover:enabled {
  transform: translateY(-1px);
  box-shadow: 0 6px 16px rgba(76,175,80,0.35);
}

.submit-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.error-message {
  margin-top: 8px;
  padding: 10px;
  border-radius: 8px;
  background: #fdecea;
  color: #b42318;
  font-size: 14px;
  text-align: center;
}

.footer-text {
  margin-top: 20px;
  text-align: center;
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

</style>
