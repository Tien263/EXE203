# BÁO CÁO CHI TIẾT WEBSITE MỘC VỊ STORE

## TỔNG QUAN DỰ ÁN

Mộc Vị Store là một hệ thống thương mại điện tử hoàn chỉnh được phát triển chuyên biệt cho việc kinh doanh hoa quả sấy cao cấp từ vùng Mộc Châu. Dự án này được xây dựng với tầm nhìn tạo ra một nền tảng bán hàng trực tuyến hiện đại, tích hợp công nghệ trí tuệ nhân tạo và các công cụ marketing tiên tiến nhằm nâng cao trải nghiệm khách hàng và tối ưu hóa hiệu quả kinh doanh.

### Thông Tin Cơ Bản Về Dự Án

Dự án Mộc Vị Store được định vị là một nền tảng thương mại điện tử chuyên ngành, tập trung vào phân khúc thực phẩm sạch và đặc sản vùng miền. Hệ thống được thiết kế theo mô hình Business to Consumer (B2C), phục vụ khách hàng cá nhân trên toàn quốc Việt Nam. Ngôn ngữ chính của hệ thống là tiếng Việt, được tối ưu hóa cho thị trường nội địa với khả năng mở rộng ra thị trường quốc tế trong tương lai.

---

## KIẾN TRÚC HỆ THỐNG

### Công Nghệ Backend

Hệ thống Mộc Vị Store được xây dựng trên nền tảng ASP.NET Core 8.0 MVC, một framework hiện đại và mạnh mẽ của Microsoft. Framework này được lựa chọn do tính ổn định cao, khả năng mở rộng tốt và hỗ trợ đầy đủ các tính năng cần thiết cho một ứng dụng thương mại điện tử. Entity Framework Core được sử dụng làm Object-Relational Mapping (ORM) để quản lý việc truy cập và thao tác với cơ sở dữ liệu một cách hiệu quả và an toàn.

Về hệ quản trị cơ sở dữ liệu, dự án sử dụng SQL Server làm hệ thống chính trong môi trường phát triển, đảm bảo hiệu suất cao và khả năng xử lý giao dịch phức tạp. Đồng thời, SQLite được tích hợp như một giải pháp sao lưu cho môi trường production, mang lại tính linh hoạt trong việc triển khai và bảo trì hệ thống.

Kiến trúc phần mềm của hệ thống tuân thủ các nguyên tắc thiết kế SOLID và áp dụng các design pattern tiên tiến. Repository Pattern được triển khai để tách biệt logic truy cập dữ liệu khỏi business logic, giúp code dễ bảo trì và kiểm thử. Unit of Work Pattern được sử dụng để quản lý các giao dịch cơ sở dữ liệu một cách nhất quán. Service Layer được thiết kế để xử lý toàn bộ business logic, đảm bảo tính tách biệt và độc lập giữa các thành phần.

Hệ thống bảo mật được xây dựng trên ASP.NET Core Identity, cung cấp một framework hoàn chỉnh cho việc quản lý người dùng và phân quyền. Cookie Authentication được sử dụng để duy trì phiên đăng nhập của người dùng một cách an toàn. Google OAuth 2.0 được tích hợp để cung cấp tùy chọn đăng nhập nhanh chóng và tiện lợi cho người dùng. Hệ thống xác thực hai lớp thông qua OTP qua email được triển khai để tăng cường bảo mật tài khoản. Mật khẩu được mã hóa bằng thuật toán SHA-256 kết hợp với salt để đảm bảo an toàn tuyệt đối.

### 2. CÔNG NGHỆ FRONTEND

#### **UI Framework**
- **HTML5, CSS3, JavaScript**: Công nghệ web cơ bản
- **Bootstrap 4**: Responsive framework
- **jQuery**: JavaScript library
- **Font Awesome**: Icon library
- **Owl Carousel**: Slider component
- **Magnific Popup**: Lightbox plugin

#### **Responsive Design**
- **Mobile-First Approach**: Thiết kế ưu tiên mobile
- **Breakpoints**: Desktop (1200px+), Tablet (768px-1199px), Mobile (<768px)
- **Flexible Grid System**: Layout linh hoạt
- **Touch-Friendly Interface**: Giao diện thân thiện với cảm ứng

### 3. HỆ THỐNG AI CHATBOT

#### **AI Architecture**
- **FastAPI (Python)**: Backend framework cho AI service
- **Google Gemini 2.0 Flash**: Large Language Model
- **RAG (Retrieval-Augmented Generation)**: Kiến trúc AI
- **Vector Database**: ChromaDB/SimpleVectorStore
- **Sentence Transformers**: Embedding model

#### **AI Features**
- **Product Consultation**: Tư vấn sản phẩm thông minh
- **Natural Language Processing**: Xử lý ngôn ngữ tự nhiên
- **Context Awareness**: Hiểu ngữ cảnh hội thoại
- **Purchase Intent Detection**: Phát hiện ý định mua hàng
- **24/7 Availability**: Hoạt động liên tục

---

## 💼 TÍNH NĂNG BUSINESS

### 1. HỆ THỐNG QUẢN LÝ SẢN PHẨM

#### **Catalog Management**
- **4 Danh mục chính**:
  - Sấy dẻo (200g): 5 sản phẩm
  - Sấy giòn (200g): 2 sản phẩm  
  - Sấy thăng hoa (100g): 2 sản phẩm
  - Mini mix (50g): 9 variants
- **18 Sản phẩm tổng cộng**
- **Product Attributes**: Tên, mô tả, giá, hình ảnh, tồn kho
- **SEO Optimization**: URL thân thiện, meta tags

#### **Inventory Management**
- **Stock Tracking**: Theo dõi tồn kho real-time
- **Low Stock Alerts**: Cảnh báo hết hàng
- **Stock Updates**: Cập nhật tự động khi bán
- **Min Stock Level**: Ngưỡng tồn kho tối thiểu

### 2. HỆ THỐNG MARKETING

#### **Voucher & Promotion System**
- **2 Loại voucher**:
  - Giảm theo phần trăm (%) với giới hạn tối đa
  - Giảm số tiền cố định (VND)
- **Điều kiện áp dụng**:
  - Đơn hàng tối thiểu
  - Thời gian hiệu lực
  - Số lần sử dụng giới hạn
- **Validation Logic**: Kiểm tra tính hợp lệ tự động

#### **Loyalty Points System**
- **Tích điểm**: 10,000 VND = 1 điểm
- **Quy đổi**: 100 điểm = 10,000 VND giảm giá
- **Point History**: Lịch sử tích/tiêu điểm
- **Automatic Calculation**: Tính toán tự động

#### **Email Marketing**
- **SMTP Integration**: Gửi email qua Gmail SMTP
- **Order Confirmation**: Email xác nhận đơn hàng
- **OTP Verification**: Email xác thực tài khoản
- **Password Reset**: Email đặt lại mật khẩu
- **Promotional Emails**: Email khuyến mãi (future)

### 3. HỆ THỐNG THANH TOÁN

#### **Payment Methods**
- **COD (Cash on Delivery)**: Thanh toán khi nhận hàng
- **Bank Transfer**: Chuyển khoản ngân hàng
- **VietQR Integration**: QR code thanh toán tự động
- **Payment Tracking**: Theo dõi trạng thái thanh toán

#### **Order Management**
- **Order Status Workflow**:
  1. Chờ xác nhận
  2. Đang xử lý
  3. Đang giao hàng
  4. Hoàn thành
  5. Hủy đơn
- **Order Tracking**: Theo dõi đơn hàng
- **Order History**: Lịch sử mua hàng

---

## 👥 HỆ THỐNG NGƯỜI DÙNG

### 1. CUSTOMER PORTAL

#### **Registration & Authentication**
- **Email Registration**: Đăng ký bằng email
- **OTP Verification**: Xác thực qua mã OTP
- **Google OAuth**: Đăng nhập nhanh bằng Google
- **Profile Management**: Quản lý thông tin cá nhân
- **Avatar Upload**: Tải lên ảnh đại diện

#### **Shopping Experience**
- **Product Browsing**: Duyệt sản phẩm theo danh mục
- **Search & Filter**: Tìm kiếm và lọc sản phẩm
- **Product Details**: Xem chi tiết sản phẩm
- **Shopping Cart**: Giỏ hàng với session persistence
- **Wishlist**: Danh sách yêu thích (future)

### 2. STAFF/ADMIN PORTAL

#### **Dashboard Analytics**
- **Revenue Metrics**: Doanh thu ngày/tháng
- **Order Statistics**: Thống kê đơn hàng
- **Product Performance**: Sản phẩm bán chạy
- **Customer Insights**: Thông tin khách hàng

#### **Management Functions**
- **Product Management**: CRUD sản phẩm
- **Order Management**: Quản lý đơn hàng
- **Voucher Management**: Quản lý mã giảm giá
- **Customer Management**: Quản lý khách hàng
- **Blog Management**: Quản lý nội dung blog

#### **Reporting & Export**
- **Excel Export**: Xuất báo cáo Excel
- **Order Reports**: Báo cáo đơn hàng chi tiết
- **Sales Analytics**: Phân tích bán hàng
- **Inventory Reports**: Báo cáo tồn kho

---

## 📊 CƠ SỞ DỮ LIỆU

### 1. DATABASE SCHEMA

#### **Core Tables**
- **Users**: Thông tin người dùng (15 fields)
- **Customers**: Thông tin khách hàng (12 fields)
- **Products**: Sản phẩm (18 fields)
- **Categories**: Danh mục (8 fields)
- **Orders**: Đơn hàng (20 fields)
- **OrderDetails**: Chi tiết đơn hàng (7 fields)

#### **Marketing Tables**
- **Vouchers**: Mã giảm giá (12 fields)
- **LoyaltyPointsHistory**: Lịch sử điểm (8 fields)
- **Carts**: Giỏ hàng (6 fields)

#### **Content Tables**
- **Blogs**: Bài viết (12 fields)
- **BlogComments**: Bình luận (8 fields)
- **Reviews**: Đánh giá sản phẩm (8 fields)

#### **System Tables**
- **OtpVerifications**: Xác thực OTP (7 fields)
- **ChatHistories**: Lịch sử chat AI (6 fields)
- **Settings**: Cấu hình hệ thống (5 fields)

### 2. DATA RELATIONSHIPS

#### **Primary Relationships**
- Users → Customers (1:1)
- Users → Employees (1:1)
- Categories → Products (1:N)
- Customers → Orders (1:N)
- Orders → OrderDetails (1:N)
- Products → OrderDetails (1:N)

#### **Marketing Relationships**
- Customers → LoyaltyPointsHistory (1:N)
- Customers → Carts (1:N)
- Orders → Vouchers (N:1)
- Products → Reviews (1:N)

---

## 🎨 UX/UI DESIGN

### 1. DESIGN PRINCIPLES

#### **Visual Identity**
- **Color Scheme**: Xanh lá cây chủ đạo (#28a745)
- **Typography**: Roboto, Open Sans (web-safe fonts)
- **Logo**: Mộc Vị branding
- **Imagery**: High-quality product photos

#### **User Experience**
- **Intuitive Navigation**: Menu rõ ràng, breadcrumb
- **Fast Loading**: Optimized images, caching
- **Mobile Responsive**: Hoạt động tốt trên mọi thiết bị
- **Accessibility**: Tuân thủ WCAG guidelines

### 2. PAGE LAYOUTS

#### **Homepage**
- **Hero Section**: Banner chính với CTA
- **Featured Products**: Sản phẩm nổi bật
- **Categories**: Danh mục sản phẩm
- **Testimonials**: Đánh giá khách hàng
- **Newsletter**: Đăng ký nhận tin

#### **Product Pages**
- **Product Grid**: Lưới sản phẩm responsive
- **Filter Sidebar**: Bộ lọc sản phẩm
- **Pagination**: Phân trang
- **Sort Options**: Tùy chọn sắp xếp

#### **Product Detail**
- **Image Gallery**: Thư viện ảnh sản phẩm
- **Product Info**: Thông tin chi tiết
- **Add to Cart**: Nút thêm giỏ hàng
- **Related Products**: Sản phẩm liên quan
- **Reviews Section**: Phần đánh giá

---

## 🚀 PERFORMANCE & OPTIMIZATION

### 1. CACHING STRATEGY

#### **Application Caching**
- **Memory Caching**: Cache dữ liệu trong RAM
- **Response Caching**: Cache HTTP responses
- **Database Query Caching**: Cache kết quả truy vấn
- **Static File Caching**: Cache files tĩnh

#### **Cache Policies**
- **Product Data**: 30 phút
- **Category Data**: 1 giờ
- **User Sessions**: 2 giờ
- **Static Content**: 24 giờ

### 2. DATABASE OPTIMIZATION

#### **Query Optimization**
- **Entity Framework**: Lazy loading, eager loading
- **Indexing**: Index trên các trường quan trọng
- **NoTracking Queries**: Queries chỉ đọc
- **Pagination**: Phân trang hiệu quả

#### **Connection Management**
- **Connection Pooling**: Tái sử dụng kết nối
- **Async Operations**: Xử lý bất đồng bộ
- **Transaction Management**: Quản lý giao dịch

---

## 🔒 BẢO MẬT & SECURITY

### 1. AUTHENTICATION SECURITY

#### **Password Security**
- **SHA-256 Hashing**: Mã hóa mật khẩu
- **Salt**: Thêm salt để tăng bảo mật
- **Password Policy**: Yêu cầu mật khẩu mạnh
- **Account Lockout**: Khóa tài khoản sau nhiều lần sai

#### **Session Security**
- **Secure Cookies**: HttpOnly, Secure flags
- **Session Timeout**: Hết hạn session tự động
- **CSRF Protection**: Chống tấn công CSRF
- **XSS Prevention**: Chống tấn công XSS

### 2. DATA PROTECTION

#### **Input Validation**
- **Model Validation**: Validation ở model level
- **SQL Injection Prevention**: Parameterized queries
- **File Upload Security**: Kiểm tra file upload
- **Rate Limiting**: Giới hạn request rate

#### **Privacy Compliance**
- **GDPR Compliance**: Tuân thủ GDPR (future)
- **Data Encryption**: Mã hóa dữ liệu nhạy cảm
- **Audit Logging**: Log hoạt động hệ thống
- **Backup Strategy**: Sao lưu dữ liệu định kỳ

---

## 📈 MARKETING & SEO

### 1. SEO OPTIMIZATION

#### **Technical SEO**
- **URL Structure**: URLs thân thiện SEO
- **Meta Tags**: Title, description, keywords
- **Schema Markup**: Structured data
- **Sitemap**: XML sitemap tự động
- **Robots.txt**: Hướng dẫn crawler

#### **Content SEO**
- **Product Descriptions**: Mô tả sản phẩm chi tiết
- **Blog Content**: Nội dung blog chất lượng
- **Image Alt Tags**: Alt text cho hình ảnh
- **Internal Linking**: Liên kết nội bộ

### 2. CONVERSION OPTIMIZATION

#### **CRO Elements**
- **Clear CTAs**: Call-to-action rõ ràng
- **Trust Signals**: Chứng chỉ, đánh giá
- **Social Proof**: Testimonials, reviews
- **Urgency**: Tạo cảm giác cấp bách
- **Simplified Checkout**: Quy trình thanh toán đơn giản

#### **A/B Testing Ready**
- **Modular Components**: Thành phần có thể test
- **Analytics Integration**: Tích hợp Google Analytics
- **Conversion Tracking**: Theo dõi chuyển đổi
- **Heat Mapping**: Bản đồ nhiệt (future)

---

## 🔮 TƯƠNG LAI & MỞ RỘNG

### 1. TÍNH NĂNG SẮP TỚI

#### **Phase 2 - Enhanced Features**
- **VNPay/Momo Integration**: Thanh toán online
- **Real-time Shipping**: Theo dõi vận chuyển
- **Product Reviews**: Hệ thống đánh giá
- **Wishlist**: Danh sách yêu thích
- **Push Notifications**: Thông báo đẩy

#### **Phase 3 - Advanced Features**
- **Mobile App**: Ứng dụng di động
- **Voice Chat AI**: Chat bằng giọng nói
- **Multi-language**: Đa ngôn ngữ
- **Advanced Analytics**: Phân tích nâng cao
- **Inventory Management**: Quản lý kho nâng cao

### 2. SCALABILITY PLANNING

#### **Technical Scaling**
- **Microservices**: Chuyển sang kiến trúc microservices
- **Cloud Deployment**: Deploy lên cloud (Azure/AWS)
- **CDN Integration**: Content Delivery Network
- **Load Balancing**: Cân bằng tải
- **Database Sharding**: Phân mảnh database

#### **Business Scaling**
- **Multi-vendor**: Đa nhà cung cấp
- **B2B Portal**: Cổng thông tin B2B
- **Franchise System**: Hệ thống nhượng quyền
- **International**: Mở rộng quốc tế
- **Wholesale**: Bán buôn

---

## 💡 KẾT LUẬN & ĐÁNH GIÁ

### 1. ĐIỂM MẠNH

#### **Technical Strengths**
- ✅ **Kiến trúc hiện đại**: ASP.NET Core 8.0, clean architecture
- ✅ **AI Integration**: Chatbot thông minh với Gemini AI
- ✅ **Responsive Design**: Hoạt động tốt trên mọi thiết bị
- ✅ **Security**: Bảo mật tốt với multiple layers
- ✅ **Performance**: Tối ưu hóa tốc độ với caching

#### **Business Strengths**
- ✅ **Complete E-commerce**: Đầy đủ tính năng bán hàng
- ✅ **Marketing Tools**: Voucher, loyalty points, email
- ✅ **Admin Dashboard**: Quản lý toàn diện
- ✅ **User Experience**: Giao diện thân thiện, dễ sử dụng
- ✅ **Scalable**: Có thể mở rộng dễ dàng

### 2. CƠ HỘI PHÁT TRIỂN

#### **Market Opportunities**
- 🎯 **Thị trường thực phẩm sạch**: Xu hướng tăng trưởng mạnh
- 🎯 **E-commerce boom**: Thương mại điện tử phát triển
- 🎯 **AI Adoption**: Ứng dụng AI trong bán hàng
- 🎯 **Mobile Commerce**: Mua sắm trên di động
- 🎯 **Social Commerce**: Bán hàng qua mạng xã hội

#### **Technology Trends**
- 🚀 **Voice Commerce**: Mua sắm bằng giọng nói
- 🚀 **AR/VR**: Thực tế ảo trong shopping
- 🚀 **Blockchain**: Truy xuất nguồn gốc
- 🚀 **IoT Integration**: Kết nối thiết bị thông minh
- 🚀 **Edge Computing**: Xử lý tại edge

---

**📝 Báo cáo này cung cấp cái nhìn toàn diện về hệ thống Mộc Vị Store, phù hợp cho sinh viên học công nghệ và marketing hiểu rõ về kiến trúc, tính năng và tiềm năng phát triển của một website thương mại điện tử hiện đại.**
