# 🔑 HƯỚNG DẪN CẤU HÌNH GOOGLE LOGIN

Để chức năng "Đăng nhập bằng Google" hoạt động trên website `mocvi.shop`, bạn cần tạo Google Client ID và Client Secret.

## Bước 1: Tạo Project trên Google Cloud
1. Truy cập: [Google Cloud Console](https://console.cloud.google.com/)
2. Đăng nhập Gmail của bạn.
3. Bấm vào menu chọn project ở góc trên bên trái -> chọn **New Project**.
4. Đặt tên (ví dụ: `MocViStore-Login`) -> Bấm **Create**.

## Bước 2: Cấu hình OAuth Consent Screen
1. Ở menu bên trái, chọn **APIs & Services** > **OAuth consent screen**.
2. Chọn **External** -> Bấm **Create**.
3. Điền thông tin:
   - **App name:** Mộc Vị Store
   - **User support email:** (Email của bạn)
   - **Developer contact information:** (Email của bạn)
4. Bấm **Save and Continue** liên tục cho đến khi xong.

## Bước 3: Lấy Client ID và Secret
1. Vào mục **Credentials** (menu bên trái).
2. Bấm **+ CREATE CREDENTIALS** -> chọn **OAuth client ID**.
3. Chọn **Application type**: **Web application**.
4. Phần **Authorized redirect URIs** (QUAN TRỌNG):
   - Bấm **ADD URI**.
   - Điền link này: `https://mocvi.shop/signin-google`
   - (Nếu chạy local thì thêm: `http://localhost:5241/signin-google`)
5. Bấm **Create**.
6. Copy **Client ID** và **Client Secret** hiện ra.

## Bước 4: Nhập vào Server
1. Mở file `.env` trên Server (dùng lệnh `nano .env`).
2. Thêm 2 dòng này vào cuối file:

```env
Authentication__Google__ClientId=CO_DAY_CLIENT_ID_VAO_DAY
Authentication__Google__ClientSecret=CO_DAY_CLIENT_SECRET_VAO_DAY
```

3. Lưu lại và chạy `docker compose up -d --build` để nhận cấu hình mới.
