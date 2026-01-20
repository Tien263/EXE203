# 🔧 HƯỚNG DẪN SỬA LỖI DEPLOY VPS

## Vấn đề hiện tại
1. ❌ AI Service offline
2. ❌ Không hiển thị sản phẩm
3. ❌ Google Login bị loop về trang login

## Nguyên nhân
- **Cookie Security Policy**: Code đang bắt buộc HTTPS nhưng VPS đang chạy HTTP
- **Database**: Chưa được seed dữ liệu hoặc không có quyền ghi
- **AI Service**: Có thể chưa start hoặc backend không kết nối được

## Cách sửa (Chạy trên VPS)

### Bước 1: Lấy code mới nhất
```bash
cd /EXE203
git pull origin main
```

### Bước 2: Xóa database cũ và tạo lại
```bash
# Xóa database cũ
rm -f mocvistore.db

# Tạo file mới và cấp quyền
touch mocvistore.db
chmod 666 mocvistore.db
```

### Bước 3: Cấp quyền cho thư mục
```bash
# Cấp quyền cho thư mục images
chmod -R 777 images

# Cấp quyền cho thư mục AI
chmod -R 777 Trainning_AI
```

### Bước 4: Kiểm tra file .env
```bash
nano .env
```

Đảm bảo có đủ các dòng sau:
```env
ASPNETCORE_ENVIRONMENT=Production
AI__ApiUrl=http://ai-service:8000

# Google Login (nếu dùng)
Authentication__Google__ClientId=YOUR_CLIENT_ID
Authentication__Google__ClientSecret=YOUR_CLIENT_SECRET

# Email (nếu dùng)
EmailSettings__SenderEmail=your-email@gmail.com
EmailSettings__SenderPassword="your-app-password"

# AI Service
PORT=8000
HOST=0.0.0.0
GEMINI_API_KEY=YOUR_GEMINI_KEY
```

### Bước 5: Rebuild và khởi động lại
```bash
# Dừng tất cả container
docker compose down

# Xóa volumes cũ (nếu cần)
docker compose down -v

# Build lại và chạy
docker compose up -d --build
```

### Bước 6: Kiểm tra logs
```bash
# Xem log backend
docker logs mocvi_backend

# Xem log AI
docker logs mocvi_ai
```

## Kiểm tra kết quả

1. **Kiểm tra AI Service**:
   ```bash
   curl http://localhost:8000/api/health
   ```
   Phải trả về: `{"status":"healthy","documents_count":...}`

2. **Kiểm tra Backend**:
   ```bash
   curl -I http://localhost:80
   ```
   Phải trả về: `HTTP/1.1 200 OK`

3. **Kiểm tra Database**:
   ```bash
   ls -lh mocvistore.db
   ```
   File phải có dung lượng > 0 bytes

## Nếu vẫn lỗi

### AI Service không start
```bash
# Vào trong container AI để debug
docker exec -it mocvi_ai /bin/bash

# Chạy thử manual
python -m app.main
```

### Database không có dữ liệu
```bash
# Vào container backend
docker exec -it mocvi_backend /bin/bash

# Chạy migration
dotnet ef database update
```

### Google Login vẫn lỗi
- Kiểm tra lại Google Cloud Console
- Đảm bảo Redirect URI là: `http://mocvi.shop/signin-google`
- Không dùng IP, chỉ dùng domain name
