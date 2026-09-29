# StayEasy — Bộ quy tắc làm việc cho OpenChamber/OpenCode

## 0. Vai trò của agent

Agent có nhiệm vụ:

1. Đọc và hiểu code hiện tại.
2. Làm đúng yêu cầu đã được chốt.
3. Sửa code có kiểm soát.
4. Kiểm tra build/test sau thay đổi.
5. Báo cáo rõ nguyên nhân nếu có lỗi.
6. Không tự ý thay đổi kiến trúc hoặc nghiệp vụ.

Agent **không được tự ý mở rộng phạm vi dự án**.

---

# 1. Nguồn sự thật của dự án

Ưu tiên theo thứ tự:

```text
Yêu cầu người dùng hiện tại
        ↓
Tài liệu nghiệp vụ đã chốt
        ↓
Database/schema đã chốt
        ↓
API contract đã chốt
        ↓
Code hiện tại
```

Nếu phát hiện mâu thuẫn:

- Dừng thay đổi lớn.
- Nêu rõ mâu thuẫn.
- Đề xuất phương án.
- Chỉ thực hiện phương án sau khi được xác nhận.

Không tự đoán nghiệp vụ.

---

# 2. Hai actor duy nhất

Hệ thống chỉ có:

```text
1. Khách hàng
2. Admin
```

Không tự tạo:

```text
Employee
Staff
Host
Manager
Receptionist
Cashier
```

trừ khi người dùng yêu cầu thay đổi.

Admin chịu trách nhiệm các chức năng quản trị và vận hành.

---

# 3. Không thay đổi tech stack

Stack đã chốt:

```text
Frontend:
React + TypeScript + Vite

Backend:
ASP.NET Core Web API

ORM:
Entity Framework Core

Database:
MySQL

Environment:
Docker / Docker Compose
```

Không tự chuyển sang:

```text
NestJS
Node backend
Prisma
SQL Server
MongoDB
Firebase
```

nếu chưa được yêu cầu.

---

# 4. Không làm thay đổi kiến trúc vì một lỗi nhỏ

Nếu một chức năng lỗi:

```text
Không được:
- đổi framework
- đổi database
- viết lại toàn bộ project
- cài hàng loạt package
- tạo kiến trúc mới
```

Trước tiên:

```text
Đọc lỗi
 ↓
Xác định file
 ↓
Xác định nguyên nhân
 ↓
Sửa nhỏ nhất có thể
 ↓
Build/test
```

---

# 5. Quy tắc xử lý lỗi

Khi gặp lỗi, xử lý theo 7 bước:

### Bước 1 — Ghi nhận lỗi

Lấy:

- command
- error message
- file
- line
- stack trace nếu có

Không đoán nguyên nhân khi chưa đọc log.

### Bước 2 — Phân loại

```text
Syntax
Dependency
Build
Runtime
Database
API
Network
Docker
Environment
Logic nghiệp vụ
```

### Bước 3 — Kiểm tra nguyên nhân gần nhất

Ví dụ:

```text
API không chạy
→ kiểm tra backend build trước
```

Không lập tức sửa frontend.

### Bước 4 — Sửa nhỏ nhất

Chỉ thay đổi phần liên quan.

### Bước 5 — Kiểm tra lại

Backend:

```powershell
dotnet build
```

Frontend:

```powershell
npm run build
```

### Bước 6 — Nếu vẫn lỗi

Thu thập log mới.

Không lặp lại cùng một lệnh vô hạn.

### Bước 7 — Báo cáo

Phải nói:

```text
Nguyên nhân:
...

Đã sửa:
...

Kiểm tra:
...

Kết quả:
...
```

---

# 6. Quy tắc command chạy lâu

Đây là quy tắc quan trọng vì OpenChamber từng bị:

```text
The running turn was stopped before OpenCode could send the next message.
```

Không chạy process vô hạn trong một turn nếu không cần.

Không dùng agent để giữ:

```text
npm run dev
dotnet run
dotnet watch run
docker compose logs -f
```

chạy liên tục.

Các process dài nên chạy ở terminal riêng hoặc Docker.

Để kiểm tra code, ưu tiên command kết thúc:

```text
npm run build
dotnet build
dotnet test
```

---

# 7. Không tự cài package khi chưa kiểm tra

Trước khi:

```text
npm install
npm install package
dotnet add package
dotnet tool install
```

phải kiểm tra:

1. Package đã tồn tại chưa?
2. Có thật sự cần không?
3. Version hiện tại có tương thích không?
4. Có thể giải quyết bằng code hiện tại không?

Không chạy `npm install` hoặc package installation lặp đi lặp lại.

Nếu installation bị treo:

```text
Dừng
 ↓
Kiểm tra network/process/cache
 ↓
Kiểm tra package manager
 ↓
Chạy lại một lần có kiểm soát
```

---

# 8. Không dùng PM2 cho ASP.NET Core

Backend là:

```text
ASP.NET Core Web API
```

Không dùng PM2 để quản lý backend.

Development:

```text
dotnet watch run
```

Production/container:

```text
Docker container
```

---

# 9. Docker rules

Docker dùng để chạy môi trường, không phải để agent liên tục restart container.

Ưu tiên:

```powershell
docker compose up -d
```

Kiểm tra:

```powershell
docker compose ps
```

Log:

```powershell
docker compose logs --tail=100
```

Build lại khi cần:

```powershell
docker compose up -d --build
```

Không dùng:

```text
docker compose down -v
```

một cách tùy tiện.

Lệnh này có thể xóa volume database.

Trước khi xóa volume phải cảnh báo người dùng.

---

# 10. Database rules

Database:

```text
MySQL
StayEasy
```

Không tự reset database.

Không tự:

```text
DROP DATABASE
DROP TABLE
docker compose down -v
```

trừ khi người dùng xác nhận rõ.

Migration phải được kiểm tra trước khi áp dụng.

Không sửa database thủ công để che giấu lỗi migration nếu chưa xác định nguyên nhân.

---

# 11. Quy tắc booking

### Theo giờ

- Tối thiểu 3 giờ.
- Đặt trước ít nhất 2 giờ.
- Không được trùng booking.
- Tính tiền theo PricePerHour.

### Theo ngày

- Check-in: 14:00.
- Check-out: 12:00 ngày hôm sau.
- Đặt trước ít nhất 2 giờ.
- Không được trùng booking.
- Tính tiền theo PricePerDay.

### Vệ sinh

Sau check-out:

```text
OCCUPIED
→ CLEANING
→ 2 giờ
→ AVAILABLE
```

Không cho booking mới trong thời gian CLEANING.

---

# 12. Quy tắc giá lịch sử

Booking phải lưu giá tại thời điểm đặt.

Ví dụ:

```text
Room Price:
200.000 → 250.000

Booking cũ:
200.000
```

Không được để booking cũ tự đổi thành 250.000.

---

# 13. Quy tắc sửa code

Trước khi sửa:

```text
Read
 ↓
Understand
 ↓
Plan
 ↓
Edit
 ↓
Build/Test
```

Không sửa file khi chưa đọc phần code liên quan.

Không rewrite toàn bộ file nếu chỉ cần sửa một hàm.

Không tạo duplicate:

```text
BookingController2
BookingServiceNew
RoomServiceFinal
RoomServiceFinal2
```

Nếu đã có service/controller thì sửa cái hiện tại.

---

# 14. Quy tắc frontend/backend

Frontend không tự chứa nghiệp vụ quan trọng.

Ví dụ kiểm tra:

```text
Đặt trước 2 giờ
Tối thiểu 3 giờ
Không trùng phòng
```

Backend phải kiểm tra lại.

Frontend chỉ hỗ trợ trải nghiệm người dùng.

Backend là nơi xác thực nghiệp vụ cuối cùng.

---

# 15. Quy tắc API

API phải:

- Có validation.
- Trả HTTP status phù hợp.
- Không trả dữ liệu nhạy cảm.
- Không expose password.
- Không tự ý thay đổi format response đang được frontend sử dụng.

Khi thay đổi API:

```text
Backend
 ↓
Kiểm tra response
 ↓
Frontend
 ↓
Test lại flow
```

---

# 16. Quy tắc Authentication

Không lưu password dạng plain text.

Password phải được hash bằng cơ chế phù hợp.

Không đưa:

```text
password
secret
JWT secret
database password
```

vào Git.

---

# 17. Quy tắc file

Trước khi tạo file mới:

```text
Tìm file tương tự
 ↓
Nếu đã có → sửa file cũ
Nếu chưa có → tạo file mới
```

Không tạo file trùng chức năng.

Không tự đổi cấu trúc thư mục nếu chưa cần.

---

# 18. Quy tắc khi không hiểu yêu cầu

Nếu thiếu thông tin quan trọng:

```text
Không đoán.
Không tự triển khai một nghiệp vụ mới.
Hỏi hoặc ghi nhận điểm cần xác nhận.
```

Nếu có thể tiếp tục bằng một phần độc lập, chỉ làm phần chắc chắn.

---

# 19. Quy tắc khi OpenChamber bị dừng

Nếu xuất hiện:

```text
The running turn was stopped before OpenCode could send the next message.
```

Không lập tức chạy lại toàn bộ.

Làm:

```text
1. Kiểm tra process đang chạy.
2. Kiểm tra Docker/container.
3. Kiểm tra backend log.
4. Kiểm tra frontend log.
5. Kiểm tra command cuối cùng.
6. Xác định command bị treo.
7. Chạy lại command ngắn để xác nhận.
```

Không cài thêm PM2 chỉ vì lỗi này.

---

# 20. Quy tắc chống vòng lặp lỗi

Nếu cùng một lỗi xuất hiện 2 lần:

```text
DỪNG
```

Sau đó:

```text
Phân tích nguyên nhân mới
→ kiểm tra giả thuyết
→ mới sửa tiếp
```

Không:

```text
retry
retry
retry
retry
```

vô hạn.

---

# 21. Quy tắc trước khi commit

Trước commit:

```text
git status
git diff
```

Sau đó kiểm tra:

```text
npm run build
dotnet build
```

và test chức năng liên quan.

Commit phải mô tả đúng thay đổi.

---

# 22. Quy tắc hoàn thành một feature

Không coi feature hoàn thành chỉ vì code đã viết.

Feature chỉ READY khi:

```text
Code
 ↓
Build
 ↓
API test
 ↓
UI test
 ↓
Database test nếu có
 ↓
Kiểm tra nghiệp vụ
 ↓
Không có lỗi console nghiêm trọng
```

---

# 23. Quy tắc báo cáo sau mỗi task

Sau khi làm xong, agent phải trả lời ngắn:

```text
Đã làm:
- ...

Đã sửa:
- ...

Đã kiểm tra:
- ...

Kết quả:
- PASS / FAIL

Còn vấn đề:
- ...
```

Không nói “đã hoàn thành” nếu chưa build/test.

---

# 24. Nguyên tắc quan trọng nhất

```text
Không đoán nghiệp vụ.
Không tự đổi kiến trúc.
Không sửa quá phạm vi.
Không chạy process vô hạn trong turn.
Không reset database tùy tiện.
Không retry lỗi vô hạn.
Luôn đọc code trước khi sửa.
Luôn build/test sau thay đổi quan trọng.
```


# 25. Quy trình bắt buộc: Một chức năng → Test → GitHub → Mới sang chức năng tiếp theo

Đây là quy trình phát triển chính của StayEasy.

Không làm kiểu:

```text
Code 10 chức năng
→ cuối cùng mới test
```

Mà bắt buộc:

```text
Chức năng 01
    ↓
Code
    ↓
Build
    ↓
Test lần 1
    ↓
Sửa lỗi
    ↓
Test lần 2
    ↓
Sửa lỗi nếu có
    ↓
Test lần 3 nếu cần
    ↓
PASS
    ↓
Git commit
    ↓
GitHub push
    ↓
Ghi nhật ký thay đổi
    ↓
Chuyển chức năng tiếp theo
```

Chỉ chuyển sang chức năng mới khi chức năng hiện tại đã PASS.

---

# 26. Test tối thiểu 1–3 lần cho mỗi chức năng

Mỗi chức năng phải có ít nhất một vòng kiểm thử hoàn chỉnh.

Khuyến nghị:

### Lần 1 — Happy Path

Kiểm tra trường hợp hợp lệ bình thường.

Ví dụ đăng nhập:

```text
Email đúng
Password đúng
→ đăng nhập thành công
```

### Lần 2 — Invalid / Boundary

Kiểm tra dữ liệu sai và giới hạn.

Ví dụ:

```text
Sai password
Email rỗng
Email sai định dạng
```

### Lần 3 — Regression / Edge Case

Kiểm tra trường hợp đặc biệt và bảo đảm sửa lỗi không phá chức năng cũ.

Ví dụ đặt phòng:

```text
Đặt dưới 3 giờ
Đặt đúng 3 giờ
Đặt trước chưa đủ 2 giờ
Đặt trùng phòng
Phòng đang CLEANING
```

Không nhất thiết mọi chức năng luôn cần đúng 3 lần nếu test suite đã bao phủ tương đương, nhưng agent phải thực hiện đủ kiểm tra cần thiết trước khi đánh dấu PASS.

---

# 27. Viết test cho chức năng trước khi đánh dấu hoàn thành

Tùy tầng:

### Backend

Ưu tiên:

```text
Unit Test
Integration Test
API Test
```

### Frontend

Kiểm tra:

```text
UI flow
Validation
API response
Error state
Loading state
```

### Database

Kiểm tra:

```text
CRUD
FK
Unique
Constraint
Migration
Dữ liệu sau transaction
```

Không chỉ nhìn giao diện rồi kết luận chức năng đúng.

---

# 28. Definition of Done

Một chức năng chỉ được đánh dấu:

```text
DONE / PASS
```

khi:

- Code hoàn tất.
- Build thành công.
- Test hợp lệ thành công.
- Test dữ liệu sai/biên đã thực hiện.
- Không còn lỗi blocking.
- Không làm hỏng chức năng đã hoàn thành trước đó.
- Git diff đã được kiểm tra.
- Commit đã tạo.
- Push GitHub thành công.
- Có ghi nhật ký thay đổi.

---

# 29. Quy tắc GitHub sau MỖI chức năng

Repository:

```text
https://github.com/Duongcute2604/stayhome
```

Sau mỗi chức năng PASS:

```text
git status
git diff
git add .
git commit
git push
```

Không gom nhiều chức năng chưa test vào một commit.

---

# 30. Commit message

Commit phải nói rõ chức năng.

Ví dụ:

```text
feat(auth): implement login and registration
feat(room): add room search and availability
feat(booking): implement hourly booking
feat(booking): implement daily booking
fix(booking): validate 2-hour advance rule
feat(admin): manage rooms
```

Không dùng:

```text
update
fix
test
abc
done
final
final2
```

---

# 31. Nhật ký thay đổi sau mỗi lần push

Sau mỗi chức năng phải ghi rõ:

```text
CHỨC NĂNG:
UC-01 Đăng nhập / Đăng ký

ĐÃ LÀM:
- ...
- ...
- ...

ĐÃ TEST:
- Test 1: ...
- Test 2: ...
- Test 3: ...

KẾT QUẢ:
PASS

COMMIT:
feat(auth): implement login and registration

GITHUB:
Đã push

TRẠNG THÁI:
UC-01 hoàn thành

TIẾP THEO:
UC-02 Tìm kiếm và xem phòng
```

Nên lưu nhật ký trong project:

```text
docs/
└── CHANGELOG.md
```

Mỗi chức năng thêm một entry mới.

---

# 32. Không chuyển bước khi test còn lỗi

Nếu:

```text
Test → FAIL
```

thì:

```text
Không commit DONE
Không chuyển chức năng
Không bắt đầu chức năng tiếp theo
```

Phải:

```text
Xác định lỗi
→ sửa
→ test lại
→ PASS
```

Nếu lỗi không thể xử lý ngay, ghi rõ:

```text
BLOCKED
```

và báo cáo nguyên nhân thay vì giả vờ hoàn thành.

---

# 33. Git checkpoint

Mỗi chức năng hoàn thành là một checkpoint.

Ví dụ:

```text
Checkpoint 01
UC-01 PASS
        ↓
Checkpoint 02
UC-02 PASS
        ↓
Checkpoint 03
UC-03 PASS
```

Nếu chức năng sau làm hỏng chức năng trước:

```text
git diff
→ xác định commit gây lỗi
→ sửa hoặc revert có kiểm soát
```

Không reset toàn bộ project tùy tiện.

---

# 34. Chuyển model khi gần hết context/token

Khi OpenChamber báo context/token sắp hết hoặc agent nhận thấy turn không còn đủ dư địa an toàn:

```text
DỪNG triển khai feature mới
        ↓
Hoàn thành trạng thái hiện tại ở mức an toàn
        ↓
Build/test nếu có thể
        ↓
Ghi HANDOFF
        ↓
Commit/push nếu thay đổi đã ở trạng thái an toàn
        ↓
Chuyển sang model khác
        ↓
Model mới đọc HANDOFF
        ↓
Tiếp tục đúng từ checkpoint
```

Không bắt đầu một thay đổi lớn ngay trước khi hết context.

---

# 35. Handoff bắt buộc khi chuyển model

Tạo file:

```text
docs/HANDOFF.md
```

Mẫu:

```markdown
# StayEasy — Handoff

## Thời điểm
YYYY-MM-DD HH:mm

## Chức năng đang làm
UC-XX ...

## Trạng thái
IN_PROGRESS / BLOCKED / PASS

## Đã hoàn thành
- ...

## Đang làm dở
- ...

## File đã thay đổi
- ...
- ...

## Database đã thay đổi
- ...

## API đã thay đổi
- ...

## Test đã chạy
- Test 1: PASS/FAIL
- Test 2: PASS/FAIL
- Test 3: PASS/FAIL

## Lỗi hiện tại
- ...

## Commit gần nhất
...

## GitHub
- Đã push / Chưa push

## Việc tiếp theo
1. ...
2. ...
3. ...

## Không được làm
- ...
- ...

## Quy tắc nghiệp vụ liên quan
- ...
```

Model mới **phải đọc `docs/HANDOFF.md` trước khi sửa code**.

---

# 36. Cập nhật HANDOFF liên tục

Không chỉ tạo khi hết token.

Sau mỗi checkpoint quan trọng có thể cập nhật:

```text
Đang làm UC-03
→ đã xong API
→ đang test overlap
→ test 1 PASS
→ test 2 FAIL
→ đang sửa
```

Như vậy nếu OpenChamber bị crash/timeout thì model khác có thể tiếp tục mà không cần đoán.

---

# 37. Quy tắc tự chuyển model

Nếu OpenChamber phiên bản/provider hiện tại có cơ chế fallback hoặc model switching:

```text
Context/token warning
→ tạo HANDOFF
→ chuyển model
→ đọc HANDOFF
→ tiếp tục
```

Nếu phiên bản đang dùng **không hỗ trợ auto-switch**, không được giả vờ rằng đã chuyển model.

Khi đó phải:

```text
Ghi HANDOFF
→ dừng an toàn
→ người dùng chọn model khác
→ model mới đọc HANDOFF
```

---

# 38. Không mất trạng thái khi đổi model

Model mới không được:

```text
đọc project sơ sài
→ viết lại
→ tạo file trùng
→ thay đổi kiến trúc
```

Mà phải:

```text
1. Đọc HANDOFF.md
2. Đọc CHANGELOG.md
3. Đọc rules
4. Kiểm tra git status
5. Kiểm tra commit gần nhất
6. Kiểm tra build/test
7. Sau đó mới tiếp tục
```

---

# 39. Quy trình hoàn chỉnh từ đầu đến cuối

```text
Chốt thiết kế
      ↓
UC-01
      ↓
Code
      ↓
Test 1
      ↓
Test 2
      ↓
Test 3 nếu cần
      ↓
PASS
      ↓
Commit
      ↓
Push GitHub
      ↓
CHANGELOG
      ↓
HANDOFF
      ↓
UC-02
      ↓
Code
      ↓
Test
      ↓
PASS
      ↓
GitHub
      ↓
...
```

Đây là quy trình mặc định cho toàn bộ StayEasy.
