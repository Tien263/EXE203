# 🚀 HƯỚNG DẪN CHẠY TOÀN BỘ HỆ THỐNG MỘC VỊ STORE

## 📋 Tổng Quan Hệ Thống

Hệ thống **Mộc Vị Store** bao gồm 3 thành phần chính:

```
┌─────────────────────────────────────────────────────────┐
│                  MỘC VỊ STORE SYSTEM                    │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐ │
│  │   FRONTEND   │  │   BACKEND    │  │   AI SERVICE │ │
│  │              │  │              │  │              │ │
│  │  HTML/CSS/JS │  │  ASP.NET     │  │  Python      │ │
│  │  Bootstrap   │◄─┤  Core MVC    │◄─┤  FastAPI     │ │
│  │  jQuery      │  │  .NET 8      │  │  Gemini AI   │ │
│  │              │  │              │  │              │ │
│  │ Port: 5241   │  │ Port: 5241   │  │ Port: 8000   │ │
│  └──────────────┘  └──────────────┘  └──────────────┘ │
│         │                  │                  │         │
│         └──────────────────┼──────────────────┘         │
│                            │                            │
│                   ┌────────▼────────┐                   │
│                   │   SQL SERVER    │                   │
│                   │   Database      │                   │
│                   │ MocViStoreDB    │                   │
│                   └─────────────────┘                   │
└─────────────────────────────────────────────────────────┘
```

### 🔍 Chi Tiết Các Thành Phần:

1. **Frontend (Giao diện người dùng)**
   - Công nghệ: HTML5, CSS3, JavaScript, Bootstrap 4, jQuery
   - Chạy tích hợp trong ASP.NET Core MVC
   - Port: `5241` (cùng với Backend)
   - Đường dẫn: `Views/` và `wwwroot/`

2. **Backend (Máy chủ xử lý)**
   - Công nghệ: ASP.NET Core 8.0 MVC
   - Database: SQL Server (Development) / SQLite (Production)
   - Port: `5241`
   - API: RESTful Controllers

3. **AI Service (Trợ lý thông minh)**
   - Công nghệ: Python FastAPI + Google Gemini AI
   - Port: `8000`
   - Đường dẫn: `Trainning_AI/`
   - Chức năng: Chatbot tư vấn sản phẩm 24/7

---

## 📦 Yêu Cầu Hệ Thống

### Phần Mềm Cần Thiết:

| Phần mềm | Phiên bản | Mục đích | Link tải |
|----------|-----------|----------|----------|
| **.NET SDK** | 8.0+ | Chạy Backend | [Download](https://dotnet.microsoft.com/download) |
| **SQL Server** | 2019+ | Database | [Download](https://www.microsoft.com/sql-server/sql-server-downloads) |
| **Python** | 3.8+ | Chạy AI Service | [Download](https://www.python.org/downloads/) |
| **Visual Studio** | 2022 | IDE (Optional) | [Download](https://visualstudio.microsoft.com/) |
| **VS Code** | Latest | Code Editor (Optional) | [Download](https://code.visualstudio.com/) |

### Kiểm Tra Cài Đặt:

Mở **PowerShell** hoặc **Command Prompt** và chạy:

```powershell
# Kiểm tra .NET
dotnet --version
# Kết quả mong đợi: 8.0.x

# Kiểm tra Python
python --version
# Kết quả mong đợi: Python 3.8.x trở lên

# Kiểm tra SQL Server
sqlcmd -?
# Nếu có lỗi, cài đặt SQL Server Command Line Tools
```

---

## 🎯 CÁCH 1: CHẠY NHANH (Recommended)

### Bước 1: Chuẩn Bị Database

#### 1.1. Mở SQL Server Management Studio (SSMS)

1. Kết nối đến SQL Server: `localhost` hoặc `(localdb)\MSSQLLocalDB`
2. Tạo database mới:

```sql
CREATE DATABASE MocViStoreDB;
GO
```

#### 1.2. Chạy Migration

Mở **PowerShell** tại thư mục dự án:

```powershell
cd c:\Users\ADMIN\Desktop\Exe_Demo_1\Exe_Demo

# Restore packages
dotnet restore

# Chạy migration để tạo bảng
dotnet ef database update
```

#### 1.3. Insert Dữ Liệu Mẫu

```powershell
# Insert sản phẩm
a

# Insert vouchers (optional)
sqlcmd -S localhost -d MocViStoreDB -i SQL_Scripts/InsertVouchers.sql -f 65001
```

### Bước 2: Cấu Hình AI Service

#### 2.1. Cài Đặt Python Dependencies

```powershell
cd Trainning_AI

# Tạo virtual environment (recommended)
python -m venv venv

# Kích hoạt virtual environment
.\venv\Scripts\activate

# Cài đặt packages
pip install -r requirements.txt
```

#### 2.2. Cấu Hình API Key

Tạo file `.env` trong thư mục `Trainning_AI`:

```env
GEMINI_API_KEY=your_gemini_api_key_here
```

**Lấy API Key miễn phí:**
1. Truy cập: https://makersuite.google.com/app/apikey
2. Đăng nhập bằng Google
3. Click "Create API Key"
4. Copy và paste vào file `.env`

### Bước 3: Chạy Hệ Thống

#### Option A: Chạy Từng Thành Phần (Khuyên dùng để debug)

**Terminal 1 - AI Service:**
```powershell
cd c:\Users\ADMIN\Desktop\Exe_Demo_1\Exe_Demo\Trainning_AI
.\venv\Scripts\activate
python -m app.main
```

Chờ đến khi thấy:
```
INFO:     Uvicorn running on http://0.0.0.0:8000 (Press CTRL+C to quit)
```

**Terminal 2 - Backend + Frontend:**
```powershell
cd c:\Users\ADMIN\Desktop\Exe_Demo_1\Exe_Demo
dotnet run
```

Chờ đến khi thấy:
```
Now listening on: http://localhost:5241
```

#### Option B: Chạy Tất Cả Cùng Lúc (Nhanh hơn)

Tạo file `start-all.bat` trong thư mục gốc:

```batch
@echo off
echo ========================================
echo   KHOI DONG MOC VI STORE - FULL STACK
echo ========================================
echo.

echo [1/2] Khoi dong AI Service...
start "AI Service" cmd /k "cd Trainning_AI && .\venv\Scripts\activate && python -m app.main"

timeout /t 5 /nobreak > nul

echo [2/2] Khoi dong Web Application...
start "Web App" cmd /k "dotnet run"

echo.
echo ========================================
echo   TAT CA DICH VU DA KHOI DONG
echo ========================================
echo.
echo - AI Service: http://localhost:8000
echo - Web App:    http://localhost:5241
echo.
echo Nhan phim bat ky de dong cua so nay...
pause > nul
```

Chạy file:
```powershell
.\start-all.bat
```

### Bước 4: Truy Cập Hệ Thống

Mở trình duyệt và truy cập:

| Thành phần | URL | Mô tả |
|------------|-----|-------|
| **Website chính** | http://localhost:5241 | Trang chủ Mộc Vị Store |
| **AI API Docs** | http://localhost:8000/docs | Swagger UI - Test API |
| **AI Chat Demo** | http://localhost:5241/ai-chat-demo.html | Demo chatbot |
| **Admin Dashboard** | http://localhost:5241/Staff/Dashboard | Quản trị hệ thống |

### Bước 5: Tạo Tài Khoản Test

#### Tạo Tài Khoản Admin/Staff:

Chạy SQL script:

```sql
-- File: SQL_Scripts/QuickCreateStaff.sql
USE MocViStoreDB;
GO

-- Tạo User với role Staff
INSERT INTO Users (Username, Email, PasswordHash, PhoneNumber, Role, IsActive, CreatedAt)
VALUES 
('admin', 'admin@mocvi.vn', 'AQAAAAIAAYagAAAAEKxK...', '0912345678', 'Staff', 1, GETDATE());

-- Lấy UserId vừa tạo
DECLARE @UserId INT = SCOPE_IDENTITY();

-- Tạo Customer tương ứng
INSERT INTO Customers (UserId, FullName, Email, PhoneNumber, LoyaltyPoints, CreatedAt)
VALUES 
(@UserId, 'Admin User', 'admin@mocvi.vn', '0912345678', 0, GETDATE());
```

**Hoặc đăng ký tài khoản mới:**
1. Truy cập: http://localhost:5241/Auth/Register
2. Điền thông tin đăng ký
3. Nhận OTP qua email
4. Xác thực và đăng nhập

---

## 🔧 CÁCH 2: CHẠY CHI TIẾT (Từng Bước)

### Phần 1: Backend (ASP.NET Core)

#### Bước 1: Restore Dependencies

```powershell
cd c:\Users\ADMIN\Desktop\Exe_Demo_1\Exe_Demo
dotnet restore
```

#### Bước 2: Build Project

```powershell
dotnet build
```

Nếu có lỗi, kiểm tra:
- .NET SDK version
- Connection string trong `appsettings.json`
- SQL Server đang chạy

#### Bước 3: Chạy Migration

```powershell
# Xem danh sách migrations
dotnet ef migrations list

# Apply migrations
dotnet ef database update
```

#### Bước 4: Run Application

```powershell
dotnet run
```

Hoặc với hot reload:

```powershell
dotnet watch run
```

### Phần 2: AI Service (Python FastAPI)

#### Bước 1: Setup Virtual Environment

```powershell
cd Trainning_AI

# Tạo venv
python -m venv venv

# Activate (Windows)
.\venv\Scripts\activate

# Activate (Linux/Mac)
source venv/bin/activate
```

#### Bước 2: Install Dependencies

```powershell
pip install -r requirements.txt
```

Nếu gặp lỗi, cài từng package:

```powershell
pip install fastapi
pip install uvicorn[standard]
pip install python-dotenv
pip install sentence-transformers
pip install pydantic
pip install python-multipart
pip install requests
```

#### Bước 3: Configure Environment

Tạo file `.env`:

```env
GEMINI_API_KEY=AIzaSyXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
PORT=8000
```

#### Bước 4: Test AI Service

```powershell
# Chạy server
python -m app.main

# Hoặc với uvicorn trực tiếp
uvicorn app.main:app --reload --port 8000
```

#### Bước 5: Test API

Mở trình duyệt: http://localhost:8000/docs

Test endpoint `/chat`:

```json
{
  "message": "Cho tôi biết về sản phẩm mận sấy dẻo",
  "user_id": "test_user"
}
```

### Phần 3: Frontend (Tích hợp sẵn)

Frontend đã được tích hợp trong ASP.NET Core MVC, không cần chạy riêng.

**Cấu trúc:**
```
Views/
├── Shared/
│   ├── _Layout.cshtml          # Layout chính
│   └── _StaffLayout.cshtml     # Layout admin
├── Home/
│   └── Index.cshtml            # Trang chủ
├── Product/
│   ├── Index.cshtml            # Danh sách sản phẩm
│   └── Details.cshtml          # Chi tiết sản phẩm
└── ...

wwwroot/
├── css/
│   ├── style.css               # Main styles
│   └── ai-chat.css             # AI chat styles
├── js/
│   ├── main.js                 # Main JavaScript
│   └── ai-chat.js              # AI chat logic
└── images/                     # Hình ảnh
```

---

## 🧪 Kiểm Tra Hệ Thống

### Test 1: Backend API

```powershell
# Test health check
curl http://localhost:5241

# Test product API
curl http://localhost:5241/Product
```

### Test 2: AI Service

```powershell
# Test health check
curl http://localhost:8000/health

# Test chat
curl -X POST http://localhost:8000/chat ^
  -H "Content-Type: application/json" ^
  -d "{\"message\":\"Xin chào\",\"user_id\":\"test\"}"
```

### Test 3: Database Connection

```sql
USE MocViStoreDB;

-- Kiểm tra số lượng sản phẩm
SELECT COUNT(*) FROM Products;

-- Kiểm tra danh mục
SELECT * FROM Categories;

-- Kiểm tra users
SELECT * FROM Users;
```

### Test 4: Full Flow

1. **Đăng ký tài khoản mới**
   - Truy cập: http://localhost:5241/Auth/Register
   - Điền thông tin
   - Nhận OTP qua email
   - Xác thực

2. **Đăng nhập**
   - Email + Password
   - Hoặc Google OAuth

3. **Xem sản phẩm**
   - Danh sách sản phẩm
   - Chi tiết sản phẩm
   - Lọc theo danh mục

4. **Thêm vào giỏ hàng**
   - Chọn số lượng
   - Thêm vào giỏ
   - Xem giỏ hàng

5. **Checkout**
   - Điền thông tin giao hàng
   - Áp dụng voucher
   - Chọn phương thức thanh toán
   - Đặt hàng

6. **Chat với AI**
   - Click icon chat góc phải
   - Hỏi về sản phẩm
   - Nhận tư vấn

---

## 🐛 Xử Lý Lỗi Thường Gặp

### Lỗi 1: "Connection string not found"

**Nguyên nhân:** Chưa cấu hình database

**Giải pháp:**
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MocViStoreDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Lỗi 2: "Port 5241 already in use"

**Nguyên nhân:** Port đang được sử dụng

**Giải pháp:**
```powershell
# Tìm process đang dùng port
netstat -ano | findstr :5241

# Kill process (thay PID)
taskkill /PID <PID> /F

# Hoặc đổi port trong launchSettings.json
```

### Lỗi 3: "Python module not found"

**Nguyên nhân:** Chưa cài đặt dependencies

**Giải pháp:**
```powershell
cd Trainning_AI
pip install -r requirements.txt
```

### Lỗi 4: "GEMINI_API_KEY not found"

**Nguyên nhân:** Chưa cấu hình API key

**Giải pháp:**
```powershell
# Tạo file .env trong Trainning_AI/
echo GEMINI_API_KEY=your_key_here > .env
```

### Lỗi 5: "Database migration failed"

**Nguyên nhân:** SQL Server không chạy hoặc connection string sai

**Giải pháp:**
```powershell
# Kiểm tra SQL Server
services.msc
# Tìm "SQL Server" và start

# Hoặc dùng SQLite (không cần SQL Server)
# Trong Program.cs đã có logic tự động chuyển sang SQLite
```

### Lỗi 6: "Email sending failed"

**Nguyên nhân:** Chưa cấu hình email hoặc App Password sai

**Giải pháp:**
```json
// appsettings.json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": "587",
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-app-password"
  }
}
```

**Lấy App Password:**
1. Truy cập: https://myaccount.google.com/apppasswords
2. Tạo App Password mới
3. Copy và paste vào config

---

## 📊 Giám Sát Hệ Thống

### Xem Logs

**Backend Logs:**
```powershell
# Console output
dotnet run

# Hoặc xem file log (nếu có cấu hình)
type logs/app.log
```

**AI Service Logs:**
```powershell
# Console output
python -m app.main

# Hoặc redirect to file
python -m app.main > logs/ai.log 2>&1
```

### Performance Monitoring

**Backend:**
- Memory usage: Task Manager
- Response time: Browser DevTools (Network tab)
- Database queries: SQL Server Profiler

**AI Service:**
- Request count: http://localhost:8000/docs
- Response time: Swagger UI
- Model performance: Console logs

---

## 🔒 Bảo Mật

### Checklist Bảo Mật:

- [ ] ✅ Không commit `appsettings.json` lên Git
- [ ] ✅ Sử dụng `.gitignore` để loại trừ secrets
- [ ] ✅ Sử dụng Environment Variables cho production
- [ ] ✅ Bật HTTPS cho production
- [ ] ✅ Validate input từ user
- [ ] ✅ Sử dụng parameterized queries (Entity Framework)
- [ ] ✅ Hash password (ASP.NET Identity)
- [ ] ✅ Implement rate limiting cho API
- [ ] ✅ CORS configuration đúng

### File `.gitignore`:

```gitignore
# Secrets
appsettings.json
appsettings.Development.json
appsettings.Production.json
.env

# Database
*.db
*.db-shm
*.db-wal

# Build
bin/
obj/
.vs/

# Python
venv/
__pycache__/
*.pyc
```

---

## 📈 Tối Ưu Performance

### Backend Optimization:

1. **Enable Response Caching:**
```csharp
// Program.cs
builder.Services.AddResponseCaching();
app.UseResponseCaching();
```

2. **Use Memory Cache:**
```csharp
builder.Services.AddMemoryCache();
```

3. **Database Indexing:**
```sql
CREATE INDEX IX_Products_CategoryId ON Products(CategoryId);
CREATE INDEX IX_Orders_CustomerId ON Orders(CustomerId);
```

### AI Service Optimization:

1. **Use Async/Await:**
```python
@app.post("/chat")
async def chat_endpoint(request: ChatRequest):
    response = await llm_service.chat(request.message)
    return response
```

2. **Cache Embeddings:**
```python
# simple_vector_store.py
# Đã implement caching với pickle
```

3. **Batch Processing:**
```python
# Process multiple requests together
```

---

## 🎓 Tài Liệu Tham Khảo

### Documentation Files:

- `README.md` - Tổng quan dự án
- `HUONG_DAN_DEPLOY.md` - Hướng dẫn deploy lên cloud
- `QUICK_START.md` - Hướng dẫn nhanh
- `DATABASE_STRUCTURE.md` - Cấu trúc database
- `AI_CHAT_WIDGET_GUIDE.md` - Hướng dẫn AI chatbot

### API Documentation:

- Backend API: http://localhost:5241/swagger (nếu enable)
- AI API: http://localhost:8000/docs

### Video Tutorials:

1. Setup môi trường
2. Chạy dự án lần đầu
3. Tạo tài khoản và đăng nhập
4. Quản lý sản phẩm
5. Xử lý đơn hàng
6. Cấu hình AI chatbot

---

## 🆘 Hỗ Trợ

### Khi Gặp Vấn Đề:

1. **Kiểm tra logs** (Console output)
2. **Xem documentation** (README.md)
3. **Google error message**
4. **Check GitHub Issues**
5. **Liên hệ team**

### Contact:

- **Email:** support@mocvi.vn
- **Phone:** +84 912 345 678
- **GitHub:** https://github.com/Tien263/MocViStore

---

## ✅ Checklist Hoàn Thành

### Lần Đầu Setup:

- [ ] Cài đặt .NET 8.0 SDK
- [ ] Cài đặt SQL Server
- [ ] Cài đặt Python 3.8+
- [ ] Clone repository
- [ ] Restore packages (.NET)
- [ ] Install dependencies (Python)
- [ ] Cấu hình database
- [ ] Chạy migrations
- [ ] Insert dữ liệu mẫu
- [ ] Cấu hình email
- [ ] Cấu hình Google OAuth
- [ ] Cấu hình Gemini API key
- [ ] Test backend
- [ ] Test AI service
- [ ] Test full flow

### Mỗi Lần Chạy:

- [ ] Start SQL Server
- [ ] Start AI Service (Terminal 1)
- [ ] Start Web App (Terminal 2)
- [ ] Verify http://localhost:8000/docs
- [ ] Verify http://localhost:5241
- [ ] Test chat widget

---

## 🎉 Kết Luận

Bây giờ bạn đã có:

✅ **Backend** chạy trên `http://localhost:5241`  
✅ **Frontend** tích hợp trong Backend  
✅ **AI Service** chạy trên `http://localhost:8000`  
✅ **Database** SQL Server với dữ liệu mẫu  
✅ **Full-stack** e-commerce website hoàn chỉnh  

**Chúc bạn code vui vẻ! 🚀**

---

**Made with ❤️ by Mộc Vị Team**
