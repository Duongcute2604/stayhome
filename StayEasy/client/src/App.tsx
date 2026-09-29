import { Routes, Route } from 'react-router-dom'
import { AuthProvider } from './context/AuthContext'
import Home from './pages/Home'
import Login from './pages/Login'
import Register from './pages/Register'
import Rooms from './pages/Rooms'
import RoomDetail from './pages/RoomDetail'
import Booking from './pages/Booking'
import MyBookings from './pages/MyBookings'
import Admin from './pages/Admin'
import Profile from './pages/Profile'
import LocationManager from './pages/LocationManager'
import AmenityManager from './pages/AmenityManager'

// ============================================================================
// APP COMPONENT - Root component định nghĩa routes
// ============================================================================
// AuthProvider: Cung cấp thông tin xác thực cho toàn bộ app
// Routes: Định nghĩa các route chính
// ============================================================================

function App() {
  return (
    <AuthProvider>
      <div className="min-h-screen bg-gray-50">
        <Routes>
          {/* Public routes */}
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/rooms" element={<Rooms />} />
          <Route path="/rooms/:id" element={<RoomDetail />} />

          {/* Protected routes - cần đăng nhập */}
          <Route path="/booking/:roomId" element={<Booking />} />
          <Route path="/my-bookings" element={<MyBookings />} />
          <Route path="/profile" element={<Profile />} />

          {/* Admin routes */}
          <Route path="/admin/*" element={<Admin />} />
          <Route path="/admin/locations" element={<LocationManager />} />
          <Route path="/admin/amenities" element={<AmenityManager />} />
        </Routes>
      </div>
    </AuthProvider>
  )
}

export default App
