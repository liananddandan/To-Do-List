<template>
  <nav class="navbar">
    <div class="navbar-inner">
      <router-link to="/" class="brand">
        <span class="brand-mark">✓</span>
        <span class="brand-copy">
          <span class="brand-title">TaskFlow</span>
          <span class="brand-subtitle">Personal workspace</span>
        </span>
      </router-link>

      <div class="navbar-links">
        <router-link to="/" class="nav-link">Tasks</router-link>
        <router-link to="/categories" class="nav-link">Categories</router-link>
        <router-link to="/profile" class="nav-link">Profile</router-link>
      </div>

      <div class="navbar-right">
        <router-link to="/profile" class="user-link">
          <span class="user-avatar">{{ userInitial }}</span>
          <span class="user-meta">
            <span class="welcome-text">Signed in as</span>
            <span class="user-name">{{ userName }}</span>
          </span>
        </router-link>

        <button class="logout-btn" @click="handleLogout">Logout</button>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { computed } from 'vue'
import { logout, userInfo } from '../tools/auth'
import { useRouter } from 'vue-router'

const router = useRouter()

const userName = computed(() => userInfo.value?.userName || 'User')

const userInitial = computed(() => {
  const name = userName.value?.trim()
  return name ? name.charAt(0).toUpperCase() : 'U'
})

const handleLogout = () => {
  logout()
  router.push('/login')
}
</script>

<style scoped>
.navbar {
  position: fixed;
  inset: 0 0 auto 0;
  z-index: 50;
  background: rgba(20, 33, 61, 0.9);
  backdrop-filter: blur(14px);
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
}

.navbar-inner {
  max-width: 1180px;
  margin: 0 auto;
  padding: 14px 20px;
  display: grid;
  grid-template-columns: auto 1fr auto;
  align-items: center;
  gap: 16px;
}

.brand {
  display: inline-flex;
  align-items: center;
  gap: 12px;
  color: white;
}

.brand-mark {
  width: 38px;
  height: 38px;
  border-radius: 12px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #48bb78 0%, #2f855a 100%);
  box-shadow: 0 10px 24px rgba(72, 187, 120, 0.24);
  font-weight: 800;
}

.brand-copy {
  display: flex;
  flex-direction: column;
  line-height: 1.1;
}

.brand-title {
  font-size: 18px;
  font-weight: 800;
}

.brand-subtitle {
  font-size: 11px;
  color: rgba(255, 255, 255, 0.65);
  text-transform: uppercase;
  letter-spacing: 0.08em;
}

.navbar-links {
  display: flex;
  justify-content: center;
  gap: 8px;
}

.nav-link {
  padding: 10px 14px;
  border-radius: 12px;
  color: rgba(255, 255, 255, 0.82);
  font-weight: 700;
}

.nav-link.router-link-exact-active {
  background: rgba(255, 255, 255, 0.1);
  color: white;
}

.navbar-right {
  display: flex;
  align-items: center;
  gap: 12px;
}

.user-link {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  padding: 8px 12px;
  border-radius: 14px;
  color: white;
}

.user-link:hover {
  background: rgba(255, 255, 255, 0.06);
}

.user-avatar {
  width: 38px;
  height: 38px;
  border-radius: 999px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  background: rgba(72, 187, 120, 0.18);
  color: #ccfbdf;
  border: 1px solid rgba(204, 251, 223, 0.18);
  font-weight: 800;
}

.user-meta {
  display: flex;
  flex-direction: column;
  line-height: 1.15;
}

.welcome-text {
  font-size: 12px;
  color: rgba(255, 255, 255, 0.62);
}

.user-name {
  font-size: 14px;
  font-weight: 700;
  color: white;
}

.logout-btn {
  border: none;
  border-radius: 12px;
  padding: 10px 14px;
  background: rgba(249, 115, 22, 0.14);
  color: #fed7aa;
  font-weight: 700;
}

.logout-btn:hover {
  background: rgba(249, 115, 22, 0.22);
}

@media (max-width: 920px) {
  .navbar-inner {
    grid-template-columns: 1fr;
    justify-items: stretch;
  }

  .navbar-links,
  .navbar-right {
    justify-content: space-between;
  }
}

@media (max-width: 640px) {
  .navbar-inner {
    padding: 12px 14px;
  }

  .navbar-links {
    justify-content: flex-start;
    overflow-x: auto;
  }

  .welcome-text {
    display: none;
  }

  .user-meta {
    display: none;
  }
}
</style>
