# BÁO CÁO PHÂN TÍCH HỆ THỐNG WEBSITE MỘC VỊ STORE

## TỔNG QUAN DỰ ÁN

Mộc Vị Store là một hệ thống thương mại điện tử hoàn chỉnh được phát triển chuyên biệt cho việc kinh doanh hoa quả sấy cao cấp từ vùng Mộc Châu. Dự án này được xây dựng với tầm nhìn tạo ra một nền tảng bán hàng trực tuyến hiện đại, tích hợp công nghệ trí tuệ nhân tạo và các công cụ marketing tiên tiến nhằm nâng cao trải nghiệm khách hàng và tối ưu hóa hiệu quả kinh doanh.

Dự án Mộc Vị Store được định vị là một nền tảng thương mại điện tử chuyên ngành, tập trung vào phân khúc thực phẩm sạch và đặc sản vùng miền. Hệ thống được thiết kế theo mô hình Business to Consumer (B2C), phục vụ khách hàng cá nhân trên toàn quốc Việt Nam. Ngôn ngữ chính của hệ thống là tiếng Việt, được tối ưu hóa cho thị trường nội địa với khả năng mở rộng ra thị trường quốc tế trong tương lai.

## KIẾN TRÚC VÀ CÔNG NGHỆ HỆ THỐNG

### Nền Tảng Backend

Hệ thống Mộc Vị Store được xây dựng trên nền tảng ASP.NET Core 8.0 MVC, một framework hiện đại và mạnh mẽ của Microsoft. Framework này được lựa chọn do tính ổn định cao, khả năng mở rộng tốt và hỗ trợ đầy đủ các tính năng cần thiết cho một ứng dụng thương mại điện tử. Entity Framework Core được sử dụng làm Object-Relational Mapping (ORM) để quản lý việc truy cập và thao tác với cơ sở dữ liệu một cách hiệu quả và an toàn.

Về hệ quản trị cơ sở dữ liệu, dự án sử dụng SQL Server làm hệ thống chính trong môi trường phát triển, đảm bảo hiệu suất cao và khả năng xử lý giao dịch phức tạp. Đồng thời, SQLite được tích hợp như một giải pháp sao lưu cho môi trường production, mang lại tính linh hoạt trong việc triển khai và bảo trì hệ thống.

Kiến trúc phần mềm của hệ thống tuân thủ các nguyên tắc thiết kế SOLID và áp dụng các design pattern tiên tiến. Repository Pattern được triển khai để tách biệt logic truy cập dữ liệu khỏi business logic, giúp code dễ bảo trì và kiểm thử. Unit of Work Pattern được sử dụng để quản lý các giao dịch cơ sở dữ liệu một cách nhất quán. Service Layer được thiết kế để xử lý toàn bộ business logic, đảm bảo tính tách biệt và độc lập giữa các thành phần.

### Hệ Thống Bảo Mật

Hệ thống bảo mật được xây dựng trên ASP.NET Core Identity, cung cấp một framework hoàn chỉnh cho việc quản lý người dùng và phân quyền. Cookie Authentication được sử dụng để duy trì phiên đăng nhập của người dùng một cách an toàn. Google OAuth 2.0 được tích hợp để cung cấp tùy chọn đăng nhập nhanh chóng và tiện lợi cho người dùng. Hệ thống xác thực hai lớp thông qua OTP qua email được triển khai để tăng cường bảo mật tài khoản. Mật khẩu được mã hóa bằng thuật toán SHA-256 kết hợp với salt để đảm bảo an toàn tuyệt đối.

### Công Nghệ Frontend

Giao diện người dùng được phát triển dựa trên các công nghệ web tiêu chuẩn bao gồm HTML5, CSS3 và JavaScript. Bootstrap 4 được sử dụng làm framework responsive chính, đảm bảo website hoạt động tối ưu trên mọi thiết bị từ desktop đến mobile. jQuery được tích hợp để xử lý các tương tác động và AJAX calls. Các thư viện bổ sung như Font Awesome cho icons, Owl Carousel cho slider và Magnific Popup cho lightbox được sử dụng để nâng cao trải nghiệm người dùng.

Thiết kế responsive được áp dụng theo phương pháp Mobile-First, với các breakpoint được định nghĩa rõ ràng cho desktop (1200px trở lên), tablet (768px-1199px) và mobile (dưới 768px). Hệ thống grid linh hoạt và giao diện thân thiện với cảm ứng được triển khai để đảm bảo trải nghiệm tối ưu trên mọi thiết bị.

### Hệ Thống Trí Tuệ Nhân Tạo

Một trong những điểm nổi bật của dự án là việc tích hợp hệ thống AI chatbot tiên tiến. Hệ thống AI được xây dựng trên nền tảng FastAPI (Python) và sử dụng Google Gemini 2.0 Flash làm mô hình ngôn ngữ lớn chính. Kiến trúc RAG (Retrieval-Augmented Generation) được áp dụng để kết hợp khả năng tìm kiếm thông tin với khả năng sinh văn bản của AI.

Vector Database sử dụng ChromaDB hoặc SimpleVectorStore để lưu trữ và tìm kiếm thông tin sản phẩm. Sentence Transformers được sử dụng làm mô hình embedding để chuyển đổi văn bản thành vector số. Hệ thống AI có khả năng tư vấn sản phẩm thông minh, xử lý ngôn ngữ tự nhiên, hiểu ngữ cảnh hội thoại, phát hiện ý định mua hàng và hoạt động liên tục 24/7.

## TÍNH NĂNG KINH DOANH VÀ MARKETING

### Quản Lý Sản Phẩm

Hệ thống quản lý sản phẩm được thiết kế để xử lý bốn danh mục chính bao gồm sản phẩm sấy dẻo 200g với năm sản phẩm, sản phẩm sấy giòn 200g với hai sản phẩm, sản phẩm sấy thăng hoa 100g với hai sản phẩm và gói mini mix 50g với chín biến thể khác nhau. Tổng cộng hệ thống quản lý mười tám sản phẩm với đầy đủ các thuộc tính như tên, mô tả, giá cả, hình ảnh và tình trạng tồn kho. Tối ưu hóa SEO được áp dụng với URL thân thiện và meta tags đầy đủ.

Quản lý tồn kho được thực hiện theo thời gian thực với khả năng theo dõi stock, cảnh báo hết hàng, cập nhật tự động khi bán và thiết lập ngưỡng tồn kho tối thiểu. Hệ thống này đảm bảo việc kiểm soát inventory một cách chính xác và hiệu quả.

### Hệ Thống Marketing Tích Hợp

Hệ thống voucher và khuyến mãi được phát triển với hai loại voucher chính là giảm theo phần trăm với giới hạn tối đa và giảm số tiền cố định theo VND. Các điều kiện áp dụng bao gồm đơn hàng tối thiểu, thời gian hiệu lực và số lần sử dụng giới hạn. Logic validation được tích hợp để kiểm tra tính hợp lệ của voucher một cách tự động.

Hệ thống điểm tích lũy hoạt động theo nguyên tắc mười nghìn VND tương đương một điểm, với khả năng quy đổi một trăm điểm thành mười nghìn VND giảm giá. Lịch sử tích và tiêu điểm được lưu trữ đầy đủ với tính toán tự động trong mọi giao dịch.

Email marketing được triển khai thông qua tích hợp SMTP với Gmail, bao gồm các tính năng gửi email xác nhận đơn hàng, xác thực OTP, đặt lại mật khẩu và khả năng mở rộng cho email khuyến mãi trong tương lai.

### Hệ Thống Thanh Toán

Các phương thức thanh toán được hỗ trợ bao gồm COD (thanh toán khi nhận hàng), chuyển khoản ngân hàng và tích hợp VietQR để tạo mã QR thanh toán tự động. Theo dõi trạng thái thanh toán được thực hiện một cách chi tiết và chính xác.

Quản lý đơn hàng được thực hiện theo quy trình năm bước bao gồm chờ xác nhận, đang xử lý, đang giao hàng, hoàn thành và hủy đơn. Khả năng theo dõi đơn hàng và lưu trữ lịch sử mua hàng được tích hợp đầy đủ trong hệ thống.

## HỆ THỐNG QUẢN LY NGƯỜI DÙNG

### Cổng Thông Tin Khách Hàng

Hệ thống đăng ký và xác thực người dùng hỗ trợ đăng ký bằng email với xác thực OTP, đăng nhập nhanh bằng Google OAuth, quản lý thông tin cá nhân và tải lên ảnh đại diện. Trải nghiệm mua sắm được tối ưu hóa với khả năng duyệt sản phẩm theo danh mục, tìm kiếm và lọc sản phẩm, xem chi tiết sản phẩm, quản lý giỏ hàng với session persistence và khả năng mở rộng cho danh sách yêu thích trong tương lai.

### Cổng Thông Tin Quản Trị

Dashboard analytics cung cấp các chỉ số doanh thu theo ngày và tháng, thống kê đơn hàng, hiệu suất sản phẩm và thông tin khách hàng. Các chức năng quản lý bao gồm CRUD sản phẩm, quản lý đơn hàng, quản lý mã giảm giá, quản lý khách hàng và quản lý nội dung blog.

Báo cáo và xuất dữ liệu được hỗ trợ thông qua xuất báo cáo Excel, báo cáo đơn hàng chi tiết, phân tích bán hàng và báo cáo tồn kho. Các tính năng này giúp quản lý có cái nhìn toàn diện về hoạt động kinh doanh.

## CƠ SỞ DỮ LIỆU VÀ KIẾN TRÚC THÔNG TIN

### Cấu Trúc Cơ Sở Dữ Liệu

Cơ sở dữ liệu được thiết kế với các bảng core bao gồm Users với mười lăm trường, Customers với mười hai trường, Products với mười tám trường, Categories với tám trường, Orders với hai mươi trường và OrderDetails với bảy trường. Các bảng marketing bao gồm Vouchers với mười hai trường, LoyaltyPointsHistory với tám trường và Carts với sáu trường. Bảng nội dung gồm Blogs với mười hai trường, BlogComments với tám trường và Reviews với tám trường. Các bảng hệ thống bao gồm OtpVerifications với bảy trường, ChatHistories với sáu trường và Settings với năm trường.

### Mối Quan Hệ Dữ Liệu

Các mối quan hệ chính trong hệ thống bao gồm Users đến Customers theo quan hệ một-một, Users đến Employees theo quan hệ một-một, Categories đến Products theo quan hệ một-nhiều, Customers đến Orders theo quan hệ một-nhiều, Orders đến OrderDetails theo quan hệ một-nhiều và Products đến OrderDetails theo quan hệ một-nhiều.

Các mối quan hệ marketing bao gồm Customers đến LoyaltyPointsHistory theo quan hệ một-nhiều, Customers đến Carts theo quan hệ một-nhiều, Orders đến Vouchers theo quan hệ nhiều-một và Products đến Reviews theo quan hệ một-nhiều. Thiết kế này đảm bảo tính toàn vẹn dữ liệu và hiệu suất truy vấn tối ưu.

## THIẾT KẾ GIAO DIỆN VÀ TRẢI NGHIỆM NGƯỜI DÙNG

### Nguyên Tắc Thiết Kế

Bản sắc thị giác của hệ thống được xây dựng xung quanh tông màu xanh lá cây chủ đạo với mã màu #28a745, typography sử dụng font Roboto và Open Sans, logo thương hiệu Mộc Vị và hình ảnh sản phẩm chất lượng cao. Trải nghiệm người dùng được tối ưu hóa với navigation trực quan, breadcrumb rõ ràng, tốc độ tải nhanh thông qua tối ưu hóa hình ảnh và caching, responsive design hoạt động tốt trên mọi thiết bị và tuân thủ các nguyên tắc accessibility theo WCAG guidelines.

### Bố Cục Trang

Trang chủ được thiết kế với hero section chứa banner chính và call-to-action, phần sản phẩm nổi bật, danh mục sản phẩm, testimonials từ khách hàng và form đăng ký nhận tin. Trang sản phẩm sử dụng lưới sản phẩm responsive, sidebar bộ lọc, phân trang và tùy chọn sắp xếp. Trang chi tiết sản phẩm bao gồm thư viện ảnh sản phẩm, thông tin chi tiết, nút thêm giỏ hàng, sản phẩm liên quan và phần đánh giá.

## HIỆU SUẤT VÀ TỐI ƯU HÓA

### Chiến Lược Caching

Caching ứng dụng được triển khai thông qua memory caching để cache dữ liệu trong RAM, response caching cho HTTP responses, database query caching để cache kết quả truy vấn và static file caching cho files tĩnh. Các chính sách cache được thiết lập với product data trong ba mười phút, category data trong một giờ, user sessions trong hai giờ và static content trong hai mười tư giờ.

### Tối Ưu Hóa Cơ Sở Dữ Liệu

Tối ưu hóa truy vấn được thực hiện thông qua Entity Framework với lazy loading và eager loading, indexing trên các trường quan trọng, NoTracking queries cho các truy vấn chỉ đọc và pagination hiệu quả. Quản lý kết nối bao gồm connection pooling để tái sử dụng kết nối, async operations cho xử lý bất đồng bộ và transaction management để quản lý giao dịch.

## BẢO MẬT VÀ AN TOÀN THÔNG TIN

### Bảo Mật Xác Thực

Bảo mật mật khẩu được thực hiện thông qua mã hóa SHA-256, thêm salt để tăng bảo mật, yêu cầu mật khẩu mạnh và khóa tài khoản sau nhiều lần đăng nhập sai. Bảo mật session bao gồm secure cookies với HttpOnly và Secure flags, session timeout tự động, chống tấn công CSRF và chống tấn công XSS.

### Bảo Vệ Dữ Liệu

Input validation được thực hiện ở model level, chống SQL injection thông qua parameterized queries, kiểm tra bảo mật file upload và rate limiting để giới hạn request rate. Tuân thủ quyền riêng tư bao gồm khả năng tuân thủ GDPR trong tương lai, mã hóa dữ liệu nhạy cảm, audit logging cho hoạt động hệ thống và chiến lược backup định kỳ.

## MARKETING VÀ TỐI ƯU HÓA CÔNG CỤ TÌM KIẾM

### Tối Ưu Hóa SEO

SEO kỹ thuật được triển khai với cấu trúc URL thân thiện SEO, meta tags đầy đủ bao gồm title, description và keywords, schema markup cho structured data, sitemap XML tự động và robots.txt để hướng dẫn crawler. SEO nội dung bao gồm mô tả sản phẩm chi tiết, nội dung blog chất lượng, alt text cho hình ảnh và liên kết nội bộ hiệu quả.

### Tối Ưu Hóa Chuyển Đổi

Các yếu tố CRO bao gồm call-to-action rõ ràng, trust signals như chứng chỉ và đánh giá, social proof thông qua testimonials và reviews, tạo cảm giác cấp bách và quy trình thanh toán đơn giản. Hệ thống sẵn sàng cho A/B testing với các thành phần modular có thể test, tích hợp Google Analytics, theo dõi chuyển đổi và khả năng mở rộng cho heat mapping trong tương lai.

## TƯƠNG LAI VÀ KẾ HOẠCH MỞ RỘNG

### Giai Đoạn Phát Triển Tiếp Theo

Giai đoạn hai sẽ tập trung vào các tính năng nâng cao bao gồm tích hợp VNPay và Momo cho thanh toán online, theo dõi vận chuyển real-time, hệ thống đánh giá sản phẩm, danh sách yêu thích và push notifications. Giai đoạn ba sẽ mở rộng với ứng dụng di động, voice chat AI, hỗ trợ đa ngôn ngữ, phân tích nâng cao và quản lý kho inventory tiên tiến.

### Kế Hoạch Mở Rộng Quy Mô

Mở rộng kỹ thuật bao gồm chuyển sang kiến trúc microservices, triển khai lên cloud như Azure hoặc AWS, tích hợp Content Delivery Network, cân bằng tải và phân mảnh cơ sở dữ liệu. Mở rộng kinh doanh sẽ hướng tới mô hình đa nhà cung cấp, cổng thông tin B2B, hệ thống nhượng quyền, mở rộng quốc tế và phát triển kênh bán buôn.

## KẾT LUẬN VÀ ĐÁNH GIÁ TỔNG THỂ

### Điểm Mạnh Của Hệ Thống

Về mặt kỹ thuật, hệ thống sở hữu kiến trúc hiện đại với ASP.NET Core 8.0 và clean architecture, tích hợp AI tiên tiến với Gemini AI chatbot, thiết kế responsive hoạt động tốt trên mọi thiết bị, bảo mật đa lớp và tối ưu hóa hiệu suất với caching. Về mặt kinh doanh, hệ thống cung cấp đầy đủ tính năng thương mại điện tử, công cụ marketing như voucher và loyalty points, dashboard quản trị toàn diện, trải nghiệm người dùng thân thiện và khả năng mở rộng linh hoạt.

### Cơ Hội Phát Triển

Các cơ hội thị trường bao gồm thị trường thực phẩm sạch với xu hướng tăng trưởng mạnh, sự bùng nổ của thương mại điện tử, việc áp dụng AI trong bán hàng, mobile commerce và social commerce. Xu hướng công nghệ hứa hẹn bao gồm voice commerce, thực tế ảo trong shopping, blockchain cho truy xuất nguồn gốc, tích hợp IoT và edge computing.

Hệ thống Mộc Vị Store thể hiện một ví dụ điển hình về việc áp dụng công nghệ hiện đại vào kinh doanh thương mại điện tử, kết hợp giữa tính năng kỹ thuật tiên tiến và chiến lược marketing hiệu quả. Dự án không chỉ đáp ứng nhu cầu hiện tại mà còn có khả năng thích ứng và phát triển theo xu hướng công nghệ và thị trường trong tương lai.
