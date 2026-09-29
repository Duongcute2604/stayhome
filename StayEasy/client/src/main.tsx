import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'
import './index.css'
import App from './App'

// ============================================================================
// ENTRY POINT - Điểm vào của ứng dụng
// ============================================================================
// BrowserRouter: Cho phép React Router hoạt động
// StrictMode: Bật các cảnh báo để phát hiện lỗi sớm
// ============================================================================

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <BrowserRouter>
      <App />
    </BrowserRouter>
  </StrictMode>,
)
