<script setup>
import Navbar from './components/Navbar.vue'
import { isLoggedIn } from './tools/auth'
</script>

<template>
  <div class="app-container">
    <Navbar v-if="isLoggedIn" />
    <div class="content">
      <router-view></router-view>
    </div>
  </div>
</template>

<style scoped>
.app-container {
  min-height: 100vh;
  flex: 1;
  background: var(--page-background);
}

.content {
  min-height: calc(100vh - 70px);
  padding-top: 70px;
  flex: 1;
}
</style>
