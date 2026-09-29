# stayeasy-react-client

## Skill phát triển Frontend Web (React + TypeScript + Vite)

### Khi nào sử dụng
- Tạo mới Page, Component, Custom Hook
- Viết API service client (Axios)
- Xử lý form validation (React Hook Form + Zod)
- Quản lý trạng thái server với TanStack Query
- Sử dụng TailwindCSS + Headless UI / shadcn/ui

---

## Quy tắc bắt buộc

### 1. Ẩn hoàn toàn ID/UI
```typescript
// ❌ KHÔNG hiển thị ID thô
<div>ID: {room.id}</div>

// ✅ Chỉ hiển thị ID trong detail, không hiển thị trên UI
// Frontend tự tính STT = index + 1 + page * size
const stt = index + 1 + (page - 1) * pageSize
```

### 2. Căn lề (Alignment)
```typescript
// Text → text-left
<div className="text-left">{room.name}</div>
<div className="text-left">{location.address}</div>

// Number → text-right
<div className="text-right">{formatVnd(room.pricePerDay)}</div>
<div className="text-right">{room.capacity}</div>
```

### 3. Format VND
```typescript
export const formatVnd = (value: number) => {
  return new Intl.NumberFormat('vi-VN').format(value) + ' ₫'
}

// Sử dụng
<span className="number-vn">{formatVnd(room.pricePerDay)}</span>
```

### 4. UI Components
```typescript
// Class cho số tiền (căn phải)
<span className="number-vn text-right">{formatVnd(price)}</span>

// Class cho text (căn trái)
<span className="text-left">{name}</span>
```

---

## Cấu trúc thư mục

```
client/src/
├── components/           # React components tái sử dụng
│   ├── common/           # Button, Input, Modal, Spinner
│   ├── layout/           # Header, Footer, Sidebar
│   └── ui/               # Card, Badge, Toast (shadcn/ui)
├── pages/                # Các trang chính
├── services/             # API services (Axios)
├── types/                # TypeScript types (shared với backend)
├── utils/                # Utility functions (formatVnd, formatDate)
├── hooks/                # Custom React hooks
├── context/              # React Context
└── App.tsx               # Root component
```

---

## Page Component Pattern

```typescript
import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useAuth } from '../hooks/useAuth'
import { someService } from '../services/someService'
import type { SomeType } from '../types/someType'

export default function SomePage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { isAuthenticated } = useAuth()
  
  const [data, setData] = useState<SomeType | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    if (id) {
      loadData(parseInt(id))
    }
  }, [id])

  const loadData = async (dataId: number) => {
    try {
      const result = await someService.getById(dataId)
      setData(result)
    } catch (err) {
      setError('Không thể tải dữ liệu')
    } finally {
      setIsLoading(false)
    }
  }

  if (isLoading) return <div>Đang tải...</div>
  if (error) return <div className="text-red-600">{error}</div>
  if (!data) return <div>Không tìm thấy dữ liệu</div>

  return (
    <div className="container py-8">
      <h1 className="text-2xl font-bold mb-4 text-left">{data.name}</h1>
      {/* Content */}
    </div>
  )
}
```

---

## Component Pattern

```typescript
interface SomeComponentProps {
  data: SomeType
  stt: number // Số thứ tự hiển thị (không phải ID)
  onEdit: (item: SomeType) => void
  onDelete: (id: number) => void
}

export default function SomeComponent({ data, stt, onEdit, onDelete }: SomeComponentProps) {
  return (
    <div className="card">
      <div className="flex justify-between items-start">
        <div>
          <h3 className="text-lg font-semibold mb-2 text-left">{data.name}</h3>
          <p className="text-gray-600 text-sm text-left">{data.description}</p>
        </div>
        <div className="text-right">
          <span className="number-vn">{formatVnd(data.price)}</span>
        </div>
      </div>
      <div className="flex gap-2 mt-4">
        <button onClick={() => onEdit(data)} className="btn btn-secondary flex-1">
          Sửa
        </button>
        <button onClick={() => onDelete(data.id)} className="btn btn-danger flex-1">
          Xóa
        </button>
      </div>
    </div>
  )
}
```

---

## Custom Hook Pattern

```typescript
import { useState, useEffect } from 'react'
import { someService } from '../services/someService'
import type { SomeType } from '../types/someType'

export function useSomeData(id: number | null) {
  const [data, setData] = useState<SomeType | null>(null)
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (!id) return

    const loadData = async () => {
      setIsLoading(true)
      setError(null)
      try {
        const result = await someService.getById(id)
        setData(result)
      } catch (err) {
        setError((err as Error).message)
      } finally {
        setIsLoading(false)
      }
    }

    loadData()
  }, [id])

  return { data, isLoading, error, refetch: () => id && loadData() }
}
```

---

## API Service Pattern (Axios)

```typescript
import api from './api'
import type { SomeType, CreateSomeRequest, UpdateSomeRequest } from '../types/some'

export const someService = {
  // Lấy tất cả (không trả id trong list)
  async getAll(): Promise<SomeType[]> {
    const response = await api.get('/somes')
    return response.data
  },

  // Lấy theo ID (detail endpoint)
  async getById(id: number): Promise<SomeType> {
    const response = await api.get(`/somes/${id}`)
    return response.data
  },

  // Tạo mới
  async create(data: CreateSomeRequest): Promise<SomeType> {
    const response = await api.post('/somes', data)
    return response.data
  },

  // Cập nhật
  async update(id: number, data: UpdateSomeRequest): Promise<SomeType> {
    const response = await api.put(`/somes/${id}`, data)
    return response.data
  },

  // Xóa
  async delete(id: number): Promise<void> {
    await api.delete(`/somes/${id}`)
  },
}
```

---

## TanStack Query Pattern

```typescript
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { someService } from '../services/someService'
import type { SomeType, CreateSomeRequest } from '../types/some'

// Query - Lấy danh sách
export function useSomeList() {
  return useQuery({
    queryKey: ['somes'],
    queryFn: () => someService.getAll(),
  })
}

// Query - Lấy chi tiết
export function useSomeDetail(id: number) {
  return useQuery({
    queryKey: ['somes', id],
    queryFn: () => someService.getById(id),
    enabled: !!id,
  })
}

// Mutation - Tạo mới
export function useCreateSome() {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: (data: CreateSomeRequest) => someService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['somes'] })
    },
  })
}

// Mutation - Xóa
export function useDeleteSome() {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: (id: number) => someService.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['somes'] })
    },
  })
}
```

---

## React Hook Form + Zod Validation Pattern

```typescript
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'

const someSchema = z.object({
  name: z.string().min(1, 'Tên là bắt buộc').max(255),
  description: z.string().max(2000).optional(),
  email: z.string().email('Email không hợp lệ'),
  phone: z.string().regex(/^\d{10}$/, 'Số điện thoại không hợp lệ').optional(),
})

type SomeFormData = z.infer<typeof someSchema>

export function SomeForm({ onSubmit, initialData }: Props) {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<SomeFormData>({
    resolver: zodResolver(someSchema),
    defaultValues: initialData,
  })

  const onSubmitForm = async (data: SomeFormData) => {
    await onSubmit(data)
  }

  return (
    <form onSubmit={handleSubmit(onSubmitForm)} className="space-y-4">
      <div>
        <label className="block text-sm font-medium mb-1 text-left">Tên</label>
        <input {...register('name')} className="input" />
        {errors.name && <p className="text-red-600 text-sm text-left">{errors.name.message}</p>}
      </div>

      <div>
        <label className="block text-sm font-medium mb-1 text-left">Email</label>
        <input {...register('email')} className="input" />
        {errors.email && <p className="text-red-600 text-sm text-left">{errors.email.message}</p>}
      </div>

      <button type="submit" disabled={isSubmitting} className="btn btn-primary">
        {isSubmitting ? 'Đang lưu...' : 'Lưu'}
      </button>
    </form>
  )
}
```

---

## TailwindCSS + shadcn/ui Patterns

### Button
```typescript
// Primary button
<button className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded">
  Lưu
</button>

// Secondary button
<button className="bg-gray-200 hover:bg-gray-300 text-gray-800 px-4 py-2 rounded">
  Hủy
</button>

// Danger button
<button className="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded">
  Xóa
</button>

// Loading state
<button disabled={isLoading} className="bg-blue-600 text-white px-4 py-2 rounded disabled:opacity-50">
  {isLoading ? <Spinner /> : 'Lưu'}
</button>
```

### Card (shadcn/ui)
```typescript
<div className="rounded-lg border bg-card text-card-foreground shadow-sm">
  <div className="p-6">
    <h3 className="text-lg font-semibold text-left">{title}</h3>
    <p className="text-sm text-muted-foreground text-left">{description}</p>
  </div>
</div>
```

### Modal (Headless UI)
```typescript
<Dialog open={isOpen} onClose={onClose}>
  <Dialog.Panel className="fixed inset-0 bg-black/50 flex items-center justify-center">
    <div className="bg-white rounded-lg p-6 max-w-md w-full">
      <Dialog.Title className="text-xl font-bold mb-4">{title}</Dialog.Title>
      {/* Content */}
    </div>
  </Dialog.Panel>
</Dialog>
```

### Table (shadcn/ui)
```typescript
<table className="w-full">
  <thead>
    <tr>
      <th className="text-left">STT</th>
      <th className="text-left">Tên</th>
      <th className="text-right">Giá</th>
    </tr>
  </thead>
  <tbody>
    {items.map((item, index) => (
      <tr key={item.id}>
        <td className="text-left">{index + 1}</td>
        <td className="text-left">{item.name}</td>
        <td className="text-right number-vn">{formatVnd(item.price)}</td>
      </tr>
    ))}
  </tbody>
</table>
```

---

## TypeScript Interface Pattern

```typescript
// Entity type (không expose id trong list)
export interface SomeType {
  id: number // Chỉ dùng trong detail
  name: string
  description?: string
  price: number
  createdAt: string
}

// List response (không có id)
export interface SomeListItem {
  name: string
  description?: string
  price: number
}

// Create request
export interface CreateSomeRequest {
  name: string
  description?: string
}

// Update request
export interface UpdateSomeRequest {
  name?: string
  description?: string
}

// API Response
export interface ApiResponse<T> {
  data: T
  message: string
  statusCode: number
}
```

---

## Naming Conventions

- **Components**: PascalCase (e.g., `RoomCard.tsx`, `BookingForm.tsx`)
- **Hooks**: camelCase with `use` prefix (e.g., `useAuth.ts`, `useRooms.ts`)
- **Services**: camelCase with `Service` suffix (e.g., `roomService.ts`)
- **Types**: PascalCase (e.g., `RoomType`, `BookingType`)
- **Constants**: UPPER_SNAKE_CASE (e.g., `API_BASE_URL`)
- **Files**: PascalCase for components, camelCase for others
