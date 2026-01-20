# 📋 HƯỚNG DẪN DEPLOY DỰ ÁN MỘC VỊ STORE

## 🎯 Tổng quan

Dự án hiện tại đang chạy trên **localhost** với:
- **Database**: SQL Server (localhost)
- **Backend**: ASP.NET Core (.NET 8)
- **AI Service**: Python FastAPI (localhost:8000)

Để deploy cho bộ phận kiểm tra, bạn có **3 phương án**:

---

## 📊 So sánh các phương án deploy

| Phương án | Độ khó | Chi phí | Thời gian setup | Phù hợp |
|-----------|--------|---------|-----------------|---------|
| **1. IIS trên Windows Server** | Trung bình | Cao (cần server) | 2-3 giờ | Doanh nghiệp lớn |
| **2. Azure App Service** | Dễ | Trung bình ($10-50/tháng) | 1-2 giờ | Startup, SME |
| **3. Render.com (FREE)** | Dễ nhất | **MIỄN PHÍ** | 30 phút | **Kiểm tra, demo** |

**👉 KHUYẾN NGHỊ: Dùng Render.com (FREE) cho mục đích kiểm tra**

---

## 🚀 PHƯƠNG ÁN 1: Deploy lên Render.com (MIỄN PHÍ)

### ✅ Ưu điểm:
- ✅ **Hoàn toàn miễn phí** (tier Free)
- ✅ Deploy tự động từ GitHub
- ✅ Có SSL certificate miễn phí
- ✅ Database SQLite tích hợp sẵn
- ✅ Không cần cấu hình phức tạp
- ✅ URL công khai: `https://mocvistore.onrender.com`

### 📝 Cách hoạt động của Database:

#### **Hiện tại (Localhost):**
```
User đăng nhập → ASP.NET Core → SQL Server (localhost) → Lưu vào database local
```

#### **Sau khi deploy lên Render:**
```
User đăng nhập → ASP.NET Core (Render) → SQLite (mocvistore.db trên server Render) → Lưu vào database cloud
```

### 🔧 Cấu hình tự động:

Dự án của bạn **ĐÃ SẴN SÀNG** deploy lên Render vì:

1. **Database tự động chuyển đổi** (dòng 47-52 trong `Program.cs`):
   ```csharp
   if (builder.Environment.IsProduction())
   {
       // Tự động dùng SQLite khi deploy
       var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "mocvistore.db");
       options.UseSqlite($"Data Source={dbPath}");
   }
   ```

2. **Dữ liệu được lưu vào file SQLite** trên server Render
3. **Mỗi user đăng nhập** → dữ liệu lưu vào `mocvistore.db` trên cloud

---

## 📋 HƯỚNG DẪN DEPLOY CHI TIẾT

### Bước 1: Chuẩn bị dự án

#### 1.1. Tạo file `.gitignore` (nếu chưa có)
```gitignore
# Secrets - KHÔNG push lên GitHub
appsettings.json
appsettings.Development.json
*.db
*.db-shm
*.db-wal

# Build files
bin/
obj/
.vs/
```

#### 1.2. Tạo file `appsettings.Production.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": "587",
    "SenderEmail": "YOUR_EMAIL",
    "SenderPassword": "YOUR_APP_PASSWORD",
    "SenderName": "Mộc Vị Store"
  },
  "Authentication": {
    "Google": {
      "ClientId": "YOUR_GOOGLE_CLIENT_ID",
      "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
    }
  },
  "AI": {
    "ApiUrl": "https://your-ai-service.onrender.com"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Bước 2: Push code lên GitHub

```bash
# Khởi tạo Git (nếu chưa có)
git init

# Add tất cả file
git add .

# Commit
git commit -m "Initial commit for deployment"

# Tạo repository trên GitHub (https://github.com/new)
# Sau đó link với local repo
git remote add origin https://github.com/YOUR_USERNAME/MocViStore.git
git branch -M main
git push -u origin main
```

### Bước 3: Deploy lên Render.com

#### 3.1. Đăng ký tài khoản Render
1. Truy cập: https://render.com
2. Đăng ký bằng GitHub account
3. Authorize Render truy cập GitHub repos

#### 3.2. Tạo Web Service mới
1. Click **"New +"** → **"Web Service"**
2. Connect GitHub repository: `MocViStore`
3. Cấu hình:
   - **Name**: `mocvistore`
   - **Region**: `Singapore` (gần VN nhất)
   - **Branch**: `main`
   - **Runtime**: `Docker` hoặc `.NET`
   - **Build Command**: `dotnet publish -c Release -o out`
   - **Start Command**: `dotnet out/Exe_Demo.dll`
   - **Plan**: **Free**

#### 3.3. Thêm Environment Variables
Trong phần **Environment**, thêm:

```
ASPNETCORE_ENVIRONMENT=Production
EmailSettings__SenderEmail=xuantien50d@gmail.com
EmailSettings__SenderPassword=waqk yhhx eije nona
Authentication__Google__ClientId=295048594899-qajmf9hdnhd3v94ip0ovi1gioopip89h.apps.googleusercontent.com
Authentication__Google__ClientSecret=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
```

⚠️ **LƯU Ý BẢO MẬT**: Nên tạo credentials mới cho production, không dùng chung với development!

#### 3.4. Deploy
1. Click **"Create Web Service"**
2. Render sẽ tự động:
   - Build code
   - Tạo SQLite database
   - Deploy lên server
   - Tạo URL: `https://mocvistore.onrender.com`

### Bước 4: Deploy AI Service (Python)

#### 4.1. Tạo Web Service cho AI
1. Click **"New +"** → **"Web Service"**
2. Connect cùng repo GitHub
3. Cấu hình:
   - **Name**: `mocvistore-ai`
   - **Root Directory**: `Trainning_AI`
   - **Runtime**: `Python 3`
   - **Build Command**: `pip install -r requirements.txt`
   - **Start Command**: `uvicorn app.main:app --host 0.0.0.0 --port $PORT`
   - **Plan**: **Free**

#### 4.2. Thêm Environment Variables
```
GEMINI_API_KEY=your_gemini_api_key
```

#### 4.3. Cập nhật URL AI trong Web Service chính
Sau khi AI service deploy xong, cập nhật environment variable:
```
AI__ApiUrl=https://mocvistore-ai.onrender.com
```

---

## 🔄 Luồng dữ liệu sau khi deploy

### 1. User đăng ký tài khoản mới:
```
User điền form đăng ký
    ↓
ASP.NET Core nhận request
    ↓
Tạo User + Customer trong code
    ↓
Entity Framework Core
    ↓
SQLite Database (mocvistore.db trên Render server)
    ↓
Dữ liệu được lưu vĩnh viễn
```

### 2. User đăng nhập:
```
User nhập email/password
    ↓
ASP.NET Core hash password và kiểm tra
    ↓
Query database SQLite
    ↓
Nếu đúng: Tạo Cookie authentication
    ↓
User được đăng nhập
```

### 3. User đặt hàng:
```
User thêm sản phẩm vào giỏ
    ↓
Lưu vào bảng Cart (SQLite)
    ↓
User checkout
    ↓
Tạo Order + OrderDetails (SQLite)
    ↓
Gửi email xác nhận
```

---

## 🗄️ Database trên Production

### Cấu trúc file:
```
Render Server
├── /opt/render/project/src/
│   ├── Exe_Demo.dll
│   ├── appsettings.Production.json
│   └── mocvistore.db  ← Database file
```

### Đặc điểm SQLite trên Render:
- ✅ File database được lưu cùng với code
- ✅ Dữ liệu persistent (không mất khi restart)
- ⚠️ **LƯU Ý**: Render Free tier có thể sleep sau 15 phút không hoạt động
- ⚠️ Nếu service sleep, lần truy cập đầu tiên sẽ mất 30-60s để wake up

### Backup database:
Render không tự động backup database trên Free tier. Bạn cần:
1. Tạo endpoint để export database
2. Hoặc upgrade lên Paid tier ($7/tháng) có auto-backup

---

## 🔐 Bảo mật khi deploy

### 1. Secrets cần bảo vệ:
- ❌ **KHÔNG** push `appsettings.json` lên GitHub
- ❌ **KHÔNG** hardcode password, API keys
- ✅ Dùng Environment Variables trên Render
- ✅ Tạo credentials riêng cho production

### 2. Google OAuth Redirect URI:
Sau khi deploy, cập nhật Google Console:
```
Authorized redirect URIs:
- https://mocvistore.onrender.com/signin-google
```

### 3. CORS cho AI Service:
File `app/main.py` đã có CORS config, chỉ cần cập nhật:
```python
origins = [
    "https://mocvistore.onrender.com",
    "http://localhost:8080"  # Giữ lại cho development
]
```

---

## 📊 Giám sát và Logs

### Xem logs trên Render:
1. Vào Dashboard → Service
2. Click tab **"Logs"**
3. Xem real-time logs:
   - User đăng nhập
   - Database queries
   - Errors

### Metrics:
- CPU usage
- Memory usage
- Request count
- Response time

---

## 🎯 Kịch bản kiểm tra

### Test case 1: Đăng ký tài khoản mới
```
1. Truy cập: https://mocvistore.onrender.com/Auth/Register
2. Điền thông tin đăng ký
3. Nhận OTP qua email
4. Xác thực OTP
5. ✅ Tài khoản được tạo trong database SQLite trên Render
```

### Test case 2: Đăng nhập Google
```
1. Click "Đăng nhập bằng Google"
2. Chọn tài khoản Google
3. ✅ Tự động tạo User + Customer trong database
4. ✅ Redirect về trang chủ đã đăng nhập
```

### Test case 3: Đặt hàng
```
1. Thêm sản phẩm vào giỏ
2. ✅ Lưu vào bảng Cart (database)
3. Checkout
4. ✅ Tạo Order trong database
5. ✅ Nhận email xác nhận
```

---

## 💡 Các vấn đề thường gặp

### 1. Service sleep sau 15 phút
**Triệu chứng**: Lần truy cập đầu mất 30-60s
**Giải pháp**: 
- Chấp nhận (Free tier)
- Hoặc upgrade Paid tier ($7/tháng)
- Hoặc dùng cron job ping service mỗi 10 phút

### 2. Database bị reset
**Nguyên nhân**: Deploy code mới
**Giải pháp**: 
- Dùng Render Disk để persistent storage
- Hoặc migrate sang PostgreSQL (free tier có sẵn)

### 3. AI service chậm
**Nguyên nhân**: Cold start
**Giải pháp**: 
- Tối ưu code Python
- Hoặc deploy AI riêng trên Railway.app (có free tier tốt hơn)

---

## 🎓 Tóm tắt

### Localhost (Hiện tại):
```
User → ASP.NET Core (localhost:8080) → SQL Server (localhost) → Database local
```

### Production (Sau deploy):
```
User → ASP.NET Core (Render cloud) → SQLite (mocvistore.db trên Render) → Database cloud
```

### Dữ liệu user:
- ✅ Mỗi user đăng ký/đăng nhập → Tự động lưu vào database cloud
- ✅ Dữ liệu persistent (không mất)
- ✅ Bộ phận kiểm tra có thể truy cập từ bất kỳ đâu
- ✅ URL công khai: `https://mocvistore.onrender.com`

---

## 📞 Hỗ trợ

Nếu gặp vấn đề khi deploy, check:
1. Logs trên Render Dashboard
2. Database connection string
3. Environment variables
4. Google OAuth redirect URI

**Thời gian deploy ước tính**: 30-60 phút cho lần đầu

---

## 🚀 Bước tiếp theo

1. ✅ Push code lên GitHub
2. ✅ Đăng ký Render.com
3. ✅ Deploy Web Service
4. ✅ Deploy AI Service
5. ✅ Test đầy đủ các tính năng
6. ✅ Gửi URL cho bộ phận kiểm tra

**URL sau khi deploy**: `https://mocvistore.onrender.com`
