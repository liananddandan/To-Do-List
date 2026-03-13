// src/stores/auth.js
import { ref } from 'vue'

function normalizeUserInfo(info) {
  if (!info) return null

  if (info.info && typeof info.info === 'object') {
    return info.info
  }

  return info
}

export const isLoggedIn = ref(!!getAccessToken())
export const userInfo = ref(normalizeUserInfo(JSON.parse(localStorage.getItem('user_info')) || null))

export function login(accessToken, refreshToken) {
  saveTokens(accessToken, refreshToken)
  isLoggedIn.value = true
}

export function saveTokens(accessToken, refreshToken){
  localStorage.setItem('access_token', accessToken)
  localStorage.setItem('refresh_token', refreshToken)
}

export function logout() {
  localStorage.removeItem('access_token')
  localStorage.removeItem('refresh_token')
  localStorage.removeItem('user_info')
  isLoggedIn.value = false
  userInfo.value = null

}

export function setUserInfo(info) {
    const normalizedInfo = normalizeUserInfo(info)
    localStorage.setItem('user_info', JSON.stringify(normalizedInfo))
    userInfo.value = normalizedInfo
}

export function getAccessToken(){
  return localStorage.getItem('access_token')
}

export function getRefreshToken() {
  return localStorage.getItem('refresh_token')
}
