-- Script thêm Blog posts về Mộc Vị Store
USE MocViStoreDB;
GO

-- Blog 1
INSERT INTO Blogs (Title, Slug, Content, ShortDescription, ImageUrl, AuthorName, ViewCount, IsPublished, PublishedDate, CreatedDate)
VALUES (
    N'Hoa Quả Sấy Mộc Châu - Tinh Hoa Từ Cao Nguyên',
    'hoa-qua-say-moc-chau',
    N'<p>Mộc Châu - vùng đất của những mùa quả ngọt. Với khí hậu ôn hòa, đất đai màu mỡ, đây là nơi lý tưởng để các loại hoa quả phát triển.</p><h3>Quy Trình Sấy Hiện Đại</h3><p>Chúng tôi áp dụng công nghệ sấy lạnh, giữ trọn dinh dưỡng và hương vị tự nhiên.</p>',
    N'Khám phá hương vị đặc trưng của hoa quả sấy Mộc Châu - nơi thiên nhiên hòa quyện cùng công nghệ hiện đại.',
    '/images/bg_1.jpg',
    N'Admin Mộc Vị',
    150,
    1,
    GETDATE(),
    GETDATE()
);

-- Blog 2
INSERT INTO Blogs (Title, Slug, Content, ShortDescription, ImageUrl, AuthorName, ViewCount, IsPublished, PublishedDate, CreatedDate)
VALUES (
    N'Dâu Tây Sấy - Vị Ngọt Thanh Khó Quên',
    'dau-tay-say-moc-chau',
    N'<p>Dâu tây Mộc Châu nổi tiếng với màu đỏ tươi, vị ngọt thanh và hương thơm đặc trưng.</p><h3>Lợi Ích</h3><ul><li>Giàu Vitamin C</li><li>Chất chống oxy hóa cao</li><li>Hỗ trợ giảm cân</li></ul>',
    N'Dâu tây sấy Mộc Châu - sản phẩm tiên phong, giữ trọn vị ngọt thanh và màu sắc tự nhiên.',
    '/images/bg_2.jpg',
    N'Admin Mộc Vị',
    230,
    1,
    GETDATE(),
    GETDATE()
);

-- Blog 3
INSERT INTO Blogs (Title, Slug, Content, ShortDescription, ImageUrl, AuthorName, ViewCount, IsPublished, PublishedDate, CreatedDate)
VALUES (
    N'5 Lý Do Nên Ăn Hoa Quả Sấy Mỗi Ngày',
    '5-ly-do-nen-an-hoa-qua-say',
    N'<h3>1. Giàu Dinh Dưỡng</h3><p>Giữ lại vitamin, khoáng chất và chất xơ.</p><h3>2. Tăng Miễn Dịch</h3><p>Vitamin C và chất chống oxy hóa.</p><h3>3. Tiện Lợi</h3><p>Dễ bảo quản, mang theo mọi nơi.</p>',
    N'Khám phá 5 lý do tuyệt vời khiến hoa quả sấy trở thành lựa chọn hoàn hảo cho sức khỏe.',
    '/images/bg_1.jpg',
    N'Chuyên gia dinh dưỡng',
    180,
    1,
    GETDATE(),
    GETDATE()
);

-- Blog 4
INSERT INTO Blogs (Title, Slug, Content, ShortDescription, ImageUrl, AuthorName, ViewCount, IsPublished, PublishedDate, CreatedDate)
VALUES (
    N'Phân Biệt Hoa Quả Sấy Dẻo, Giòn Và Thăng Hoa',
    'phan-biet-cac-loai-hoa-qua-say',
    N'<h3>Sấy Dẻo</h3><p>Mềm, dẻo dai, độ ẩm 15-20%</p><h3>Sấy Giòn</h3><p>Giòn rụm, độ ẩm dưới 5%</p><h3>Sấy Thăng Hoa</h3><p>Giòn xốp, giữ nguyên dinh dưỡng tốt nhất</p>',
    N'Tìm hiểu sự khác biệt giữa các loại hoa quả sấy. Mỗi loại có đặc điểm riêng về kết cấu và dinh dưỡng.',
    '/images/bg_2.jpg',
    N'Admin Mộc Vị',
    195,
    1,
    GETDATE(),
    GETDATE()
);

-- Blog 5
INSERT INTO Blogs (Title, Slug, Content, ShortDescription, ImageUrl, AuthorName, ViewCount, IsPublished, PublishedDate, CreatedDate)
VALUES (
    N'Công Thức Món Ngon Từ Hoa Quả Sấy',
    'cong-thuc-mon-ngon-tu-hoa-qua-say',
    N'<h3>1. Yến Mạch Hoa Quả</h3><p>Bữa sáng healthy với yến mạch, sữa và hoa quả sấy.</p><h3>2. Trà Hoa Quả</h3><p>Thức uống detox từ dâu sấy và chanh sấy.</p><h3>3. Sữa Chua</h3><p>Món tráng miệng lành mạnh.</p>',
    N'Khám phá 5 công thức đơn giản nhưng cực kỳ ngon miệng và bổ dưỡng từ hoa quả sấy.',
    '/images/bg_1.jpg',
    N'Đầu bếp Mộc Vị',
    210,
    1,
    GETDATE(),
    GETDATE()
);

PRINT N'Đã thêm 5 bài viết blog thành công!';
GO
