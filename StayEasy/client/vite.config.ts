import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// ============================================================================
// VITE CONFIGURATION
// ============================================================================
// Mục đích: Cấu hình Vite cho ứng dụng React + TypeScript
// ============================================================================

export default defineConfig({
  plugins: [react()],
  server: {
    port: 3000,
    proxy: {
      // Proxy API requests để tránh CORS khi development
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
      },
    },
  },
})
