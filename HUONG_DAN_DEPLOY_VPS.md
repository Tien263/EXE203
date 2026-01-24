# ☁️ HƯỚNG DẪN DEPLOY LÊN CLOUD SERVER (VPS)

Tài liệu này hướng dẫn bạn deploy sau khi đã mua **Cloud Server (Linux)** và **Tên miền**.

---

## 🚀 PHẦN 1: CẤU HÌNH TÊN MIỀN (DOMAIN)

Sau khi mua tên miền (ví dụ `mocvi.shop`) và có IP của Server (ví dụ `103.1.2.3`), bạn cần trỏ tên miền về Server.

1.  Đăng nhập trang quản lý tên miền (nơi bạn mua).
2.  Tìm mục **DNS Management** (Cấu hình DNS).
3.  Tạo 2 bản ghi (Record) sau:

| Loại (Type) | Tên (Host/Name) | Giá trị (Value/IP) | Ý nghĩa |
| :--- | :--- | :--- | :--- |
| **A** | **@** | `150.95.112.79` | Trỏ `mocvi.shop` về Server |
| **A** | **www** | `150.95.112.79` | Trỏ `www.mocvi.shop` về Server |

*(Đây là IP thật của bạn: `150.95.112.79`)*.
> 💡 Lưu ý: DNS có thể mất 5-30 phút để cập nhật.

---

## 🛠️ PHẦN 2: TRUY CẬP SERVER (SSH)

Nhà cung cấp sẽ gửi cho bạn thông tin qua Email, gồm:
- **IP:** (Ví dụ 103.1.2.3)
- **User:** `root`
- **Password:** (Một chuỗi ngẫu nhiên)

### Cách 1: Dùng CMD/Terminal (Windows/Mac)
Mở CMD trên máy tính của bạn và gõ:
```bash
ssh root@150.95.112.79
```
*(Thay IP bằng IP thật)*.
Khi nó hỏi Password, hãy gõ mật khẩu vào (lưu ý: khi gõ password sẽ **không hiện ký tự gì cả**, cứ gõ đúng rồi Enter).

---

## 🏗️ PHẦN 3: CÀI ĐẶT DOCKER

Sau khi đã vào được màn hình đen của Server, bạn copy-paste từng dòng lệnh sau để cài môi trường:

```bash
# 1. Cập nhật hệ điều hành
apt-get update

# 2. Cài đặt Docker
apt-get install -y docker.io

# 3. Cài đặt Docker Compose
apt-get install -y docker-compose

# 4. Kiểm tra cài đặt thành công chưa
docker --version
docker-compose --version
```

---

## 📦 PHẦN 4: DEPLOY DỰ ÁN

Chúng ta sẽ dùng cách đơn giản nhất: Copy code lên và chạy.

### 1. Kéo code về Server
```bash
# Clone code từ GitHub (Thay link bằng link repo của bạn)
git clone https://github.com/Tien263/EXE203.git

# Vào thư mục chứa Dockerfile
# Vào thư mục chứa code
cd EXE203
```

### 2. Tạo file cấu hình
Tạo file `docker-compose.yml` để chạy cả Web và AI:

```bash
nano docker-compose.yml
```

**Copy nội dung sau dán vào:**
```yaml
version: '3.8'
services:
  backend:
    build: .
    container_name: mocvi_backend
    restart: always
    ports:
      - "80:8080"  # Mở cổng 80 cho web
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - AI__ApiUrl=http://ai-service:8000
    env_file:
      - .env
    volumes:
      - ./docker_data:/app/DbStorage
    depends_on:
      - ai-service

  ai-service:
    build: ./Trainning_AI
    container_name: mocvi_ai
    restart: always
    environment:
      - PORT=8000
    env_file:
      - .env
```
*(Bấm `Ctrl+O` -> `Enter` để lưu, `Ctrl+X` để thoát)*

### 3. Tạo file biến môi trường (.env)
Bạn mở file `DEPLOY_SECRETS_TEMPLATE.txt` trên máy tính, điền thông tin thật vào, rồi copy nội dung.
Sau đó trên server:

```bash
nano .env
```
Paste nội dung đã điền vào đây.
*(Bấm `Ctrl+O` -> `Enter` để lưu, `Ctrl+X` để thoát)*

### 4. Chạy Server
**Lưu ý quan trọng**: Nếu gặp lỗi build, hãy chạy lệnh fix dưới đây trước:
```bash
bash fix_build.sh
```

Sau đó chạy build:
```bash
docker-compose up -d --build
```

---

## 🌐 PHẦN 5: KIỂM TRA

Mở trình duyệt truy cập: `http://mocvi.shop` (hoặc IP server).
Nếu thấy web hiện lên là THÀNH CÔNG! 🎉

---

## 🔒 BONUS: CÀI SSL (HTTPS - Ổ KHÓA XANH)

Để web có `https://`, bạn cần cài Nginx và Certbot.
*(Phần này hơi nâng cao, nếu bạn chạy được HTTP ổn rồi thì bảo tôi, tôi sẽ hướng dẫn tiếp phần SSL này sau cho đỡ rối nhé!)*
