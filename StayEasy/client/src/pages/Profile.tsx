import { useAuth } from '../hooks/useAuth'

// ============================================================================
// PROFILE PAGE - Trang hồ sơ người dùng
// ============================================================================

export default function Profile() {
  const { user, logout } = useAuth()

  if (!user) {
    return <div className="container py-8">Vui lòng đăng nhập</div>
  }

  return (
    <div className="container py-8 max-w-2xl">
      <h1 className="text-2xl font-bold mb-6">Hồ sơ của tôi</h1>

      <div className="card space-y-4">
        <div>
          <label className="block text-sm font-medium text-gray-600">Họ và tên</label>
          <p className="text-lg">{user.fullName}</p>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-600">Email</label>
          <p className="text-lg">{user.email}</p>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-600">Số điện thoại</label>
          <p className="text-lg">{user.phone || 'Chưa cập nhật'}</p>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-600">Vai trò</label>
          <p className="text-lg">{user.role}</p>
        </div>

        <div className="pt-4">
          <button onClick={logout} className="btn btn-secondary">
            Đăng xuất
          </button>
        </div>
      </div>
    </div>
  )
}
