# 🍓 Mộc Vị Store - Hoa Quả Sấy Mộc Châu

## 📖 Giới Thiệu

**Mộc Vị Store** là website thương mại điện tử chuyên bán hoa quả sấy cao cấp từ Mộc Châu. Website được xây dựng bằng **ASP.NET Core MVC** với giao diện hiện đại, thân thiện, tích hợp AI tư vấn sản phẩm và đầy đủ tính năng quản lý bán hàng chuyên nghiệp.

## ✨ Tính Năng Chính

### 🔐 Hệ Thống Xác Thực & Bảo Mật
- ✅ Đăng ký tài khoản với xác thực OTP qua email
- ✅ Đăng nhập bằng tài khoản hoặc Google OAuth
- ✅ Quên mật khẩu với OTP verification
- ✅ Bảo mật session và cookie
- ✅ Authorization cho các trang yêu cầu đăng nhập
- ✅ **[MỚI]** Hỗ trợ HTTPS với Nginx và SSL tự động

### 👤 Quản Lý Profile
- ✅ Xem và chỉnh sửa thông tin cá nhân
- ✅ Upload và thay đổi ảnh đại diện
- ✅ Hiển thị thông tin khách hàng (mã KH, điểm thưởng)
- ✅ Lịch sử đăng nhập
- ✅ **[MỚI]** Xác thực địa chỉ Việt Nam (Tỉnh/Thành, Quận/Huyện, Phường/Xã) với Select2

### 🛍️ Hệ Thống Sản Phẩm
- ✅ Hiển thị danh sách sản phẩm với 4 danh mục chính:
  - Sản phẩm sấy dẻo (200g)
  - **Sản phẩm sấy giòn (200g)** (Đã cập nhật tên danh mục)
  - Sản phẩm sấy thăng hoa (100g)
  - Mini size mix (50g)
- ✅ Lọc sản phẩm theo danh mục
- ✅ Tìm kiếm sản phẩm thông minh
- ✅ Sắp xếp theo giá, tên, mới nhất
- ✅ Chi tiết sản phẩm với đầy đủ thông tin dinh dưỡng
- ✅ Sản phẩm liên quan
- ✅ Giao diện card sản phẩm Premium

### 🛒 Giỏ Hàng & Thanh Toán
- ✅ Thêm sản phẩm vào giỏ hàng
- ✅ Cập nhật số lượng, xóa sản phẩm
- ✅ Tính tổng tiền tự động
- ✅ Lưu giỏ hàng vào database
- ✅ Checkout với thông tin đầy đủ
- ✅ Thanh toán COD hoặc chuyển khoản ngân hàng
- ✅ QR Code thanh toán tự động (VietQR)
- ✅ Email xác nhận đơn hàng tự động

### 🎫 Hệ Thống Voucher
- ✅ Tạo và quản lý voucher giảm giá (%, số tiền)
- ✅ Thiết lập điều kiện: đơn tối thiểu, giảm tối đa, thời gian, số lượng
- ✅ Áp dụng voucher tại trang checkout
- ✅ Hiển thị chi tiết giảm giá minh bạch

### ⭐ Hệ Thống Điểm Tích Lũy
- ✅ Tích điểm khi mua hàng (10,000đ = 1 điểm)
- ✅ Sử dụng điểm để giảm giá (100 điểm = 10,000đ)
- ✅ Lịch sử tích điểm chi tiết

### 📊 Quản Lý Staff/Admin
- ✅ Dashboard thống kê tổng quan (Doanh thu, Đơn hàng)
- ✅ Quản lý sản phẩm (CRUD)
- ✅ Quản lý đơn hàng (Cập nhật trạng thái: Chờ xử lý -> Hoàn thành/Hủy)
- ✅ **[MỚI]** Tự động trừ tồn kho khi đơn hàng thành công
- ✅ Quản lý voucher & KH
- ✅ Báo cáo doanh số
- ✅ Export/Import đơn hàng Excel chuyên nghiệp
- ✅ Tài khoản Staff với quyền hạn phù hợp

### 🎨 Giao Diện & UX
- ✅ Responsive design (Desktop, Tablet, Mobile)
- ✅ Giao diện hiện đại, màu xanh thương hiệu (#4f6a4c)
- ✅ Animation mượt mà, hiệu ứng glassmorphism
- ✅ SEO optimizations (Title, Meta, Structure)

### 🤖 AI Chat Widget (Mộc Vị AI)
- ✅ Trợ lý AI thông minh tư vấn sản phẩm 24/7
- ✅ Tích hợp Google Gemini 2.0 Flash
- ✅ RAG (Retrieval-Augmented Generation) với dữ liệu sản phẩm thực tế
- ✅ Trả lời câu hỏi về giá cả, dinh dưỡng, bảo quản

## 🛠️ Công Nghệ Sử Dụng

### Backend
- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: ASP.NET Core Identity, Google OAuth 2.0
- **Server**: Kestrel, Nginx (Reverse Proxy)
- **Containerization**: Docker, Docker Compose

### Frontend
- **HTML5, CSS3, JavaScript**
- **Bootstrap 4**
- **jQuery** & Plugins (Select2, Owl Carousel)
- **Font Awesome**

### AI System
- **Framework**: Python FastAPI
- **LLM**: Google Gemini 2.0 Flash
- **Vector DB**: ChromaDB
- **Embeddings**: Sentence Transformers

## 🚀 Cài Đặt & Chạy Dự Án

### Yêu Cầu
- Docker & Docker Compose (Khuyên dùng)
- Hoặc: .NET 8 SDK, SQL Server, Python 3.8+

### Chạy với Docker (Khuyên dùng)
Dự án đã được cấu hình sẵn Docker Compose để chạy toàn bộ hệ thống (Web + Database + AI + Nginx).

1. **Clone repository**
   ```bash
   git clone https://github.com/Tien263/MocViStore.git
   cd MocViStore
   ```

2. **Cấu hình Environment**
   - Đảm bảo file `.env` (nếu có) hoặc biến môi trường đã được thiết lập cho AI Service.

3. **Khởi chạy**
   ```bash
   docker-compose up -d --build
   ```

4. **Truy cập**
   - Website: `http://localhost` (hoặc domain cấu hình)
   - AI Service: Internal network

### Chạy Thủ Công (Cục bộ)
Tham khảo file `QUICK_START.md` để biết chi tiết cách chạy từng service (Web, AI) thủ công trên Windows.

## 📦 Danh Sách Sản Phẩm Nổi Bật

### Sản Phẩm Sấy Dẻo (200g)
- Mận Sấy Dẻo - 65,000đ
- Xoài Sấy Dẻo - 70,000đ
- Đào Sấy Dẻo - 65,000đ
- Dâu Sấy Dẻo - 90,000đ
- Hồng Sấy Dẻo - 95,000đ

### Sản Phẩm Sấy Giòn (200g)
- Mít Sấy Giòn - 80,000đ
- Chuối Sấy Giòn - 80,000đ

### Sản Phẩm Sấy Thăng Hoa & Mini
(Xem chi tiết trên website)

## 📞 Liên Hệ

- **Website**: [mocvi.vn](https://mocvi.vn)
- **Email**: [mocvi.vn@gmail.com](mailto:mocvi.vn@gmail.com)
- **Hotline**: 0929 161 999
- **Fanpage**: [Facebook Mộc Vị Store](https://www.facebook.com/profile.php?id=61586750100786)
- **Địa chỉ**: Số 123, Mộc Châu, Sơn La, Việt Nam

## 📝 License
Dự án được phát hành dưới giấy phép MIT.

---
**Made with ❤️ by Mộc Vị Team**
