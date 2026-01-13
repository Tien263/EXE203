# THÔNG TIN TÀI KHOẢN STAFF VÀ ADMIN - MOCVISTORE

## 🔐 TÀI KHOẢN MẪU

### Staff Account
- **Email:** `staff@mocvistore.com`
- **Password:** `Staff@123`
- **Họ tên:** Nguyễn Văn A
- **Vai trò:** Staff
- **Mã nhân viên:** NV001
- **Chức vụ:** Nhân viên bán hàng
- **Phòng ban:** Bán hàng
- **Lương:** 8,000,000 VNĐ

### Admin Account
- **Email:** `admin@mocvistore.com`
- **Password:** `Admin@123`
- **Họ tên:** Quản Trị Viên
- **Vai trò:** Admin
- **Mã nhân viên:** ADMIN001
- **Chức vụ:** Quản lý
- **Phòng ban:** Quản lý
- **Lương:** 15,000,000 VNĐ

## 🌐 ĐƯỜNG DẪN TRUY CẬP

- **Trang chủ:** http://localhost:8080
- **Trang đăng nhập:** http://localhost:8080/Auth/Login
- **Dashboard quản lý:** http://localhost:8080/Staff/Dashboard
- **AI API:** http://localhost:8000
- **AI API Docs:** http://localhost:8000/docs

## 🛠️ CÁCH TẠO TÀI KHOẢN MỚI

### Phương pháp 1: Sử dụng Helper Class
```csharp
// Trong Controller hoặc Service
var helper = new StaffAccountHelper(_context);
var result = await helper.CreateSampleStaffAccountsAsync();
```

### Phương pháp 2: Chạy SQL Script
Chạy file: `SQL_Scripts/CreateStaffAccount.sql` hoặc `SQL_Scripts/QuickCreateStaff.sql`

### Phương pháp 3: Đăng ký + Nâng cấp quyền
1. Đăng ký tài khoản bình thường tại `/Auth/Register`
2. Chạy script SQL để nâng cấp quyền

## ⚠️ LƯU Ý QUAN TRỌNG

1. **Password Hash:** Các script SQL cần được cập nhật với password đã hash bằng BCrypt
2. **Bảo mật:** Đổi password mặc định sau khi đăng nhập lần đầu
3. **Database:** Hệ thống sử dụng SQL Server trong Development và SQLite trong Production
4. **OAuth:** Google OAuth đã được cấu hình sẵn

## 📊 QUYỀN HẠN

### Staff
- Xem và quản lý đơn hàng
- Xem danh sách sản phẩm
- Xuất báo cáo Excel
- Truy cập Staff Dashboard

### Admin
- Tất cả quyền của Staff
- Quản lý nhân viên
- Quản lý hệ thống
- Cấu hình website

## 🔧 TRẠNG THÁI HỆ THỐNG

✅ **Web Application:** Đang chạy trên port 8080
✅ **AI Service:** Đang chạy trên port 8000
✅ **Database:** Đã khởi tạo thành công
✅ **Google OAuth:** Đã cấu hình

## 📞 HỖ TRỢ

Nếu gặp vấn đề với tài khoản:
1. Kiểm tra database connection
2. Chạy lại migration nếu cần
3. Xem log trong console để debug
4. Kiểm tra file `appsettings.json` cho cấu hình

---
*Cập nhật lần cuối: 13/11/2025*
