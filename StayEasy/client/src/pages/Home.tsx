import { Link } from 'react-router-dom'
import { useAuth } from '../hooks/useAuth'

// ============================================================================
// HOME PAGE - Trang chủ
// ============================================================================

export default function Home() {
  const { isAuthenticated } = useAuth()

  return (
    <div className="min-h-screen">
      {/* Hero Section */}
      <section className="bg-blue-600 text-white py-20">
        <div className="container text-center">
          <h1 className="text-4xl font-bold mb-4">StayEasy</h1>
          <p className="text-xl mb-8">Hệ thống đặt phòng và quản lý homestay</p>
          <div className="flex gap-4 justify-center">
            {isAuthenticated ? (
              <Link to="/rooms" className="btn btn-primary bg-white text-blue-600">
                Xem phòng
              </Link>
            ) : (
              <>
                <Link to="/login" className="btn btn-primary bg-white text-blue-600">
                  Đăng nhập
                </Link>
                <Link to="/register" className="btn btn-secondary">
                  Đăng ký
                </Link>
              </>
            )}
          </div>
        </div>
      </section>

      {/* Features */}
      <section className="py-16">
        <div className="container">
          <h2 className="text-2xl font-bold text-center mb-12">Tính năng chính</h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            <div className="card text-center">
              <h3 className="text-lg font-semibold mb-2">Đặt phòng trực tuyến</h3>
              <p className="text-gray-600">Tìm kiếm và đặt phòng homestay dễ dàng</p>
            </div>
            <div className="card text-center">
              <h3 className="text-lg font-semibold mb-2">Quản lý đặt phòng</h3>
              <p className="text-gray-600">Theo dõi trạng thái đặt phòng realtime</p>
            </div>
            <div className="card text-center">
              <h3 className="text-lg font-semibold mb-2">Đánh giá & nhận xét</h3>
              <p className="text-gray-600">Chia sẻ trải nghiệm của bạn</p>
            </div>
          </div>
        </div>
      </section>
    </div>
  )
}
