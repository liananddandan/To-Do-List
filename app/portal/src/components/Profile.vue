<template>
  <div class="profile-page">
    <div class="profile-shell">
      <div class="profile-header">
        <div class="avatar">
          {{ userInitial }}
        </div>

        <div class="header-text">
          <h1>{{ userName }}</h1>
          <p>Manage your account settings and personal information.</p>
        </div>
      </div>

      <div class="profile-grid">
        <section class="profile-card">
          <h2>Account</h2>

          <div class="info-list">
            <div class="info-row">
              <span class="label">Username</span>
              <span class="value">{{ userName }}</span>
            </div>

            <div class="info-row">
              <span class="label">Email</span>
              <span class="value">{{ userEmail }}</span>
            </div>
          </div>
        </section>

        <section class="profile-card">
          <h2>Security</h2>
          <p class="card-text">
            Keep your account secure by updating your password regularly.
          </p>

          <button class="primary-btn" @click="goToChangePassword">
            Change Password
          </button>
        </section>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { userInfo } from '../tools/auth'

const router = useRouter()

const userName = computed(() => userInfo.value?.userName || 'User')
const userEmail = computed(() => userInfo.value?.email || 'No email available')

const userInitial = computed(() => {
  const name = userName.value?.trim()
  return name ? name.charAt(0).toUpperCase() : 'U'
})

const goToChangePassword = () => {
  router.push('/change-password')
}
</script>

<style scoped>
.profile-page {
  min-height: calc(100vh - 70px);
  padding: 32px 20px 48px;
  background:
    radial-gradient(circle at top left, rgba(76, 175, 80, 0.08), transparent 25%),
    radial-gradient(circle at bottom right, rgba(33, 150, 243, 0.08), transparent 22%),
    linear-gradient(135deg, #f5f7fb 0%, #eef3f8 100%);
}

.profile-shell {
  max-width: 1000px;
  margin: 0 auto;
}

.profile-header {
  display: flex;
  align-items: center;
  gap: 18px;
  margin-bottom: 28px;
  padding: 28px;
  border-radius: 22px;
  background: rgba(255, 255, 255, 0.94);
  border: 1px solid #e5e7eb;
  box-shadow: 0 16px 36px rgba(15, 23, 42, 0.08);
}

.avatar {
  width: 74px;
  height: 74px;
  border-radius: 999px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #4CAF50 0%, #43a047 100%);
  color: white;
  font-size: 28px;
  font-weight: 800;
  box-shadow: 0 12px 24px rgba(76, 175, 80, 0.24);
}

.header-text h1 {
  margin: 0;
  font-size: 30px;
  color: #111827;
  letter-spacing: -0.02em;
}

.header-text p {
  margin: 8px 0 0;
  color: #6b7280;
  font-size: 15px;
  line-height: 1.6;
}

.profile-grid {
  display: grid;
  grid-template-columns: 1.2fr 0.8fr;
  gap: 22px;
}

.profile-card {
  padding: 24px;
  border-radius: 20px;
  background: rgba(255, 255, 255, 0.94);
  border: 1px solid #e5e7eb;
  box-shadow: 0 14px 30px rgba(15, 23, 42, 0.07);
}

.profile-card h2 {
  margin: 0 0 18px;
  font-size: 20px;
  color: #111827;
}

.info-list {
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.info-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 16px;
  padding: 14px 16px;
  border-radius: 14px;
  background: #f8fafc;
  border: 1px solid #edf2f7;
}

.label {
  color: #6b7280;
  font-size: 14px;
  font-weight: 600;
}

.value {
  color: #111827;
  font-size: 14px;
  font-weight: 700;
  text-align: right;
  word-break: break-word;
}

.card-text {
  color: #6b7280;
  font-size: 14px;
  line-height: 1.7;
  margin: 0 0 18px;
}

.primary-btn {
  border: none;
  border-radius: 12px;
  padding: 12px 16px;
  background: linear-gradient(135deg, #4CAF50 0%, #43a047 100%);
  color: #fff;
  font-size: 14px;
  font-weight: 700;
  cursor: pointer;
  box-shadow: 0 10px 20px rgba(76, 175, 80, 0.2);
  transition: all 0.2s ease;
}

.primary-btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 14px 24px rgba(76, 175, 80, 0.26);
}

@media (max-width: 860px) {
  .profile-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 640px) {
  .profile-page {
    padding: 20px 14px 32px;
  }

  .profile-header {
    padding: 20px;
    align-items: flex-start;
  }

  .avatar {
    width: 60px;
    height: 60px;
    font-size: 22px;
  }

  .header-text h1 {
    font-size: 24px;
  }

  .info-row {
    flex-direction: column;
    align-items: flex-start;
  }

  .value {
    text-align: left;
  }
}
</style>