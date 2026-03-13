// src/stores/auth.js
import { ref } from 'vue'

export const isLoggedIn = ref(!!getAccessToken())
export const userInfo = ref(JSON.parse(localStorage.getItem('user_info')) || null)

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
    localStorage.setItem('user_info', JSON.stringify(info))
    userInfo.value = info
}

export function getAccessToken(){
  return localStorage.getItem('access_token')
}

export function getRefreshToken() {
  return localStorage.getItem('refresh_token')
}