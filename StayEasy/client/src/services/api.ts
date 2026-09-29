import axios from 'axios'

// ============================================================================
// API INSTANCE - Cấu hình Axios cho toàn bộ API calls
// ============================================================================
// Tại sao dùng Axios: Tự động parse JSON, hỗ trợ interceptor, dễ quản lý token
// ============================================================================

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || '/api',
  headers: {
    'Content-Type': 'application/json',
  },
})

// ============================================================================
// REQUEST INTERCEPTOR - Thêm JWT token vào mỗi request
// ============================================================================
// Tại sao cần: Tự động thêm token vào header, không cần thủ công ở mỗi call
// ============================================================================
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// ============================================================================
// RESPONSE INTERCEPTOR - Xử lý lỗi response
// ============================================================================
// Tại sao cần: Xử lý lỗi 401 (token hết hạn) tự động logout
// ============================================================================
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token hết hạn hoặc không hợp lệ
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export default api
