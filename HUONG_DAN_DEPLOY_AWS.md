# ☁️ HƯỚNG DẪN DEPLOY DỰ ÁN MỘC VỊ STORE LÊN AWS

Tài liệu này hướng dẫn cách deploy hệ thống Mộc Vị Store (ASP.NET Core Backend + Python AI) lên **Amazon Web Services (AWS)**.

Chúng ta sẽ sử dụng phương pháp **Containerization (Docker)** vì đây là cách chuẩn và dễ quản lý nhất cho hệ thống đa ngôn ngữ (.NET + Python).

---

## 🏗️ Kiến trúc Deploy

1.  **Backend (.NET 8)**: Đóng gói thành Docker Image → Chạy trên AWS
2.  **AI Service (Python)**: Đóng gói thành Docker Image → Chạy trên AWS
3.  **Database**: Sử dụng SQLite (nhúng trong container - đơn giản nhất) hoặc RDS SQL Server (nâng cao). *Trong hướng dẫn này dùng SQLite để giống cấu hình Render.*

---

## 🛠️ Chuẩn bị

Đảm bảo bạn đã cài đặt:
1.  [Docker Desktop](https://www.docker.com/products/docker-desktop/)
2.  [AWS CLI](https://aws.amazon.com/cli/) (cấu hình `aws configure` với Key của bạn)
3.  Tài khoản AWS

---

## 🚀 CÁCH 1: AWS APP RUNNER (Khuyên dùng - Dễ nhất)

AWS App Runner tương tự như Render, tự động build và run container, không cần quản lý server.

### Bước 1: Chuẩn bị Source Code
Hai file `Dockerfile` đã được tạo sẵn trong dự án:
- Backend: `Exe_Demo/Dockerfile`
- AI: `Exe_Demo/Trainning_AI/Dockerfile`

### Bước 2: Push code lên GitHub
Đẩy toàn bộ code lên repository GitHub của bạn (như hướng dẫn trong `HUONG_DAN_DEPLOY.md`).

### Bước 3: Tạo service cho Backend
1.  Vào [AWS App Runner Console](https://console.aws.amazon.com/apprunner).
2.  Chọn **Create service**.
3.  **Source**: Chọn **Source code repository**.
4.  Kết nối GitHub và chọn repo `MocViStore`.
5.  **Deployment settings**: chọn **Automatic**.
6.  **Build configuration**: chọn **Configure all settings here**.
    - **Runtime**: `Corretto 11` hoặc `Dotnet` (Tuy nhiên App Runner hỗ trợ Docker tốt hơn, nên chọn **Flow dùng ECR** hoặc **App Runner build from Code**).
    
    *> 💡 Mẹo: App Runner build trực tiếp từ code .NET đôi khi phức tạp. Cách ổn định nhất là **Push Docker Image lên Amazon ECR** trước.*
    
    **PHƯƠNG ÁN ĐƠN GIẢN HƠN VỚI APP RUNNER (Build from Source):**
    Nếu App Runner chưa hỗ trợ build trực tiếp Dockerfile từ sub-folder tốt, ta dùng **CÁCH 2 (EC2)** bên dưới sẽ rẻ và linh hoạt hơn cho sinh viên.
    
    *Tuy nhiên, nếu muốn tiếp tục App Runner:*
    1. Cần đẩy Docker Image lên ECR (Elastic Container Registry).
    2. App Runner sẽ pull image từ ECR về chạy.

---

## 💻 CÁCH 2: AWS EC2 (Truyền thống - Tiết kiệm - Full quyền)

Chúng ta sẽ thuê 1 server EC2 (Ubuntu), cài Docker và chạy 2 container (Backend + AI) trên đó bằng `docker-compose`.

### Bước 1: Tạo EC2 Instance
1.  Vào AWS EC2 Console → **Launch Instances**.
2.  **Name**: `MocViServer`.
3.  **OS**: Ubuntu Server 22.04 LTS (Free tier eligible).
4.  **Instance type**: `t2.micro` (Free tier) hoặc `t3.small` (tốt hơn).
5.  **Key pair**: Tạo mới `mocvi-key.pem` (Lưu file này kỹ!).
6.  **Network settings**:
    - Allow SSH traffic from Anywhere (0.0.0.0/0).
    - Allow HTTP traffic from the internet.
    - Allow HTTPS traffic from the internet.
7.  Click **Launch instance**.

### Bước 2: Cấu hình Security Group (Mở port)
1.  Vào Instance vừa tạo → Tab **Security**.
2.  Click vào Security Group.
3.  **Edit inbound rules** → Add rule:
    - Custom TCP - Port **8080** (Backend) - Source: Anywhere.
    - Custom TCP - Port **8000** (AI Service) - Source: Anywhere.
    - Custom TCP - Port **80** (HTTP) - Source: Anywhere.
4.  Save rules.

### Bước 3: SSH vào Server
Mở terminal (trên máy bạn) tại nơi chứa file key `.pem`:
```bash
ssh -i "mocvi-key.pem" ubuntu@<PUBLIC_IP_CUA_DUNG_EC2>
```

### Bước 4: Cài đặt Docker trên EC2
Chạy lần lượt các lệnh sau trên Server EC2:

```bash
# Cập nhật
sudo apt-get update

# Cài Docker
sudo apt-get install -y docker.io

# Cài Docker Compose
sudo apt-get install -y docker-compose

# Cho phép user ubuntu dùng docker không cần sudo
sudo usermod -aG docker ubuntu
```
*Sau đó gõ `exit` để thoát ra, rồi SSH lại để cập nhật quyền.*

### Bước 5: Deploy Code
Có 2 cách để đưa code lên:
1.  **Git Clone (Dễ nhất)**:
    ```bash
    git clone https://github.com/YOUR_USERNAME/MocViStore.git
    cd MocViStore/Exe_Demo_1/Exe_Demo
    ```

2.  **Tạo file docker-compose.yml**:
    Tại thư mục `Exe_Demo` trên server, tạo file `docker-compose.yml`:
    ```bash
    nano docker-compose.yml
    ```
    Dán nội dung sau:
    ```yaml
    version: '3.8'
    services:
      backend:
        build: .
        ports:
          - "8080:8080"
        environment:
          - ASPNETCORE_ENVIRONMENT=Production
          - AI__ApiUrl=http://ai-service:8000  # Gọi nội bộ trong mạng Docker
          # Thêm các biến môi trường khác (Email, Google Auth)...
        depends_on:
          - ai-service

      ai-service:
        build: ./Trainning_AI
        ports:
          - "8000:8000"
        environment:
          - PORT=8000
    ```
    (Nhấn `Ctrl+O` → Enter để lưu, `Ctrl+X` để thoát).

### Bước 6: Chạy Server
```bash
# Build và chạy ngầm (Detached mode)
docker-compose up -d --build
```

### Bước 7: Kiểm tra
Truy cập trình duyệt:
- Backend: `http://<PUBLIC_IP_EC2>:8080`
- AI Test: `http://<PUBLIC_IP_EC2>:8000/docs`

---

## 🔒 Cấp phát SSL (HTTPS) Miễn phí (Opsional)
Nếu dùng EC2, mặc định chỉ có HTTP. Để có HTTPS (ổ khóa xanh):
1.  Mua domain (hoặc dùng domain free).
2.  Trỏ domain về IP của EC2.
3.  Sử dụng **Nginx** làm Reverse Proxy và **Certbot** để lấy SSL free.

---

## 🔑 QUẢN LÝ BIẾN MÔI TRƯỜNG (ENVIRONMENT VARIABLES)

Tôi đã tạo sẵn file **`DEPLOY_SECRETS_TEMPLATE.txt`** trong thư mục dự án.

1.  Mở file này ra trên máy tính của bạn.
2.  Điền các thông tin thật (Email, Passsword, Google Client ID...) vào đó.
3.  Khi deploy (ở bước chạy Docker hoặc App Runner), bạn chỉ cần copy nội dung file này vào là xong.

---

## 🌐 CẤU HÌNH GOOGLE LOGIN CHO DOMAIN THẬT (BẮT BUỘC)

Khi chạy Localhost, Google cho phép thoải mái. Nhưng khi có IP hoặc Domain thật, bạn phải khai báo với Google, nếu không sẽ bị lỗi `403 Access Denied`.

### Bước 1: Lấy Public IP hoặc Domain
Sau khi deploy xong bước trên, bạn sẽ có một địa chỉ, ví dụ:
- IP của EC2: `http://54.123.45.67:8080`
- Domain App Runner: `https://mocvistore.awsapprunner.com`

### Bước 2: Vào Google Console
1.  Truy cập: [Google Cloud Console](https://console.cloud.google.com/apis/credentials)
2.  Chọn Project của bạn.
3.  Tìm mục **"OAuth 2.0 Client IDs"** -> Click vào cái bạn đang dùng.

### Bước 3: Thêm Redirect URI
Tìm mục **"Authorized redirect URIs"**, bấm **ADD URI** và thêm 2 dòng sau (ví dụ với IP EC2):

1.  `http://54.123.45.67:8080/signin-google`
2.  `http://54.123.45.67:8080`

*(Lưu ý: Thay `54.123.45.67:8080` bằng IP hoặc Domain thật của bạn. Đuôi `/signin-google` là BẮT BUỘC).*

### Bước 4: Lưu lại
Bấm **SAVE**. Đợi khoảng 5 phút để Google cập nhật.

---

Chúc bạn deploy thành công lên AWS! 🚀


