import axios from 'axios'
import router from '@/router'
import { getAccessToken, getRefreshToken, logout, saveTokens } from './auth'

const baseURL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5166'

const api = axios.create({
    baseURL
})

// add token in request
api.interceptors.request.use(config => {
    if (!config.skipAuth) {
        const accessToken = getAccessToken()
        if (accessToken) {
            config.headers.Authorization = `Bearer ${accessToken}`
        }
    }

    return config
})

let isRefreshing = false
let refreshSubscribers = []

function subscribeTokenRefresh(cb) {
    refreshSubscribers.push(cb)
}

function onRefreshed(newToken) {
    refreshSubscribers.forEach(cb => cb(newToken))
    refreshSubscribers = []
}

api.interceptors.response.use(
    res => res,
    async error => {
        const originalRequest = error.config
        const status = error.response?.status
        const code = error.response?.data?.code ?? error.response?.data?.data?.code

        if (status === 401) {
            if (code === 3 && !originalRequest._retry) {
                if (isRefreshing) {
                    return new Promise(resolve => {
                        subscribeTokenRefresh(token => {
                            originalRequest.headers.Authorization = 'Bearer ' + token
                            resolve(api(originalRequest))
                        })
                    })
                }

                originalRequest._retry = true
                isRefreshing = true

                try {
                    const refreshResponse = await axios.post(
                        `${baseURL}/api/auth/refresh`,
                        {
                            refreshToken: getRefreshToken(),
                        },
                        {
                            headers: {
                                'Content-Type': 'application/json'
                            }
                        }
                    )
                    const { accessToken, refreshToken } = refreshResponse.data.data || {}

                    if (!accessToken || !refreshToken) {
                        throw new Error('Refresh response does not contain tokens.')
                    }

                    saveTokens(accessToken, refreshToken)
                    onRefreshed(accessToken)

                    originalRequest.headers.Authorization = 'Bearer ' + accessToken
                    return api(originalRequest)
                } catch (refreshErr) {
                    console.error('refresh token failed:', refreshErr)

                    logout()
                    router.push('/login')
                    return Promise.reject(refreshErr)
                } finally {
                    isRefreshing = false
                }
            }

            logout()
            router.push('/login')
        }

        return Promise.reject(error)
    }
)

export default api
