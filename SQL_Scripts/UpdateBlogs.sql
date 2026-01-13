-- Script cập nhật nội dung blog chi tiết hơn
USE MocViStoreDB;
GO

-- Xóa blog cũ
DELETE FROM Blogs;
GO

-- Blog 1: Hoa quả sấy Mộc Châu - Bài viết dài
INSERT INTO Blogs (Title, Slug, Content, ShortDescription, ImageUrl, AuthorName, ViewCount, IsPublished, PublishedDate, CreatedDate)
VALUES (
    N'Hoa Quả Sấy Mộc Châu - Tinh Hoa Từ Cao Nguyên',
    'hoa-qua-say-moc-chau',
    N'<div class="blog-content">
        <h2>Mộc Châu - Vùng Đất Của Những Mùa Quả Ngọt</h2>
        <p>Nằm ở độ cao trên 1.000m so với mặt nước biển, Mộc Châu được thiên nhiên ưu đãi với khí hậu ôn hòa quanh năm, đất đai màu mỡ và nguồn nước trong lành. Đây chính là điều kiện lý tưởng để các loại hoa quả phát triển, mang trong mình hương vị đặc trưng không thể lẫn vào đâu được.</p>
        
        <p>Với nhiệt độ trung bình từ 18-25°C, lượng mưa phân bố đều và ánh nắng vừa phải, Mộc Châu là "thiên đường" cho việc trồng trọt các loại hoa quả chất lượng cao. Từ dâu tây, mận hậu, đào, xoài cho đến chuối, mỗi loại đều mang trong mình hương vị ngọt ngào, thơm ngon đặc trưng của vùng cao.</p>
        
        <h3>Quy Trình Sấy Hiện Đại, Giữ Trọn Dinh Dưỡng</h3>
        <p>Tại Mộc Vị, chúng tôi áp dụng công nghệ sấy lạnh hiện đại, giúp bảo toàn tối đa vitamin, khoáng chất và hương vị tự nhiên của trái cây. Không giống như phương pháp sấy truyền thống với nhiệt độ cao có thể phá hủy dưỡng chất, công nghệ sấy lạnh của chúng tôi duy trì nhiệt độ ổn định, giúp hoa quả giữ được màu sắc tươi sáng và giá trị dinh dưỡng.</p>
        
        <h4>Các Bước Trong Quy Trình Sản Xuất:</h4>
        <ol>
            <li><strong>Chọn lọc nguyên liệu:</strong> Chỉ sử dụng trái cây chín tự nhiên, không dùng hóa chất kích thích. Mỗi trái cây đều được kiểm tra kỹ lưỡng về độ chín, màu sắc và hương vị trước khi đưa vào sản xuất.</li>
            <li><strong>Sơ chế sạch sẽ:</strong> Rửa sạch bằng nước tinh khiết, loại bỏ phần không đạt chuẩn. Quá trình này được thực hiện trong môi trường vệ sinh, đảm bảo an toàn thực phẩm tuyệt đối.</li>
            <li><strong>Cắt lát đều:</strong> Sử dụng máy cắt chuyên dụng để đảm bảo độ dày đồng đều, giúp quá trình sấy đạt hiệu quả tốt nhất.</li>
            <li><strong>Sấy ở nhiệt độ thấp:</strong> Nhiệt độ được kiểm soát chặt chẽ từ 50-60°C, giữ nguyên màu sắc và dưỡng chất. Thời gian sấy tùy thuộc vào từng loại trái cây, thường từ 8-12 giờ.</li>
            <li><strong>Kiểm tra chất lượng:</strong> Sau khi sấy, sản phẩm được kiểm tra độ ẩm, màu sắc, hương vị và độ giòn/dẻo theo tiêu chuẩn.</li>
            <li><strong>Đóng gói kín:</strong> Bảo quản trong môi trường vệ sinh, không chất bảo quản. Bao bì được thiết kế kín khí, giúp sản phẩm giữ được chất lượng lâu dài.</li>
        </ol>
        
        <h3>Lợi Ích Của Hoa Quả Sấy</h3>
        <p>Hoa quả sấy không chỉ là món ăn vặt ngon miệng mà còn mang lại nhiều lợi ích cho sức khỏe:</p>
        
        <h4>1. Giàu Chất Xơ</h4>
        <p>Chất xơ trong hoa quả sấy giúp hệ tiêu hóa hoạt động tốt hơn, giảm táo bón và hỗ trợ kiểm soát cân nặng. Một khẩu phần 50g hoa quả sấy có thể cung cấp đến 3-5g chất xơ, tương đương 10-15% nhu cầu hàng ngày.</p>
        
        <h4>2. Nhiều Vitamin</h4>
        <p>Vitamin C, E, A và các vitamin nhóm B được giữ lại trong quá trình sấy, giúp tăng cường sức đề kháng, làm đẹp da và bảo vệ thị lực. Đặc biệt, vitamin C trong dâu tây sấy có thể đáp ứng đến 50% nhu cầu hàng ngày.</p>
        
        <h4>3. Khoáng Chất Dồi Dào</h4>
        <p>Kali, magie, sắt, canxi... đều có trong hoa quả sấy, tốt cho xương khớp, tim mạch và hệ thần kinh. Chuối sấy đặc biệt giàu kali, giúp điều hòa huyết áp và hỗ trợ hoạt động của cơ tim.</p>
        
        <h4>4. Chất Chống Oxy Hóa</h4>
        <p>Anthocyanin, flavonoid và polyphenol trong hoa quả sấy giúp chống lão hóa, bảo vệ tế bào và giảm nguy cơ mắc các bệnh mãn tính như tim mạch, tiểu đường và ung thư.</p>
        
        <h4>5. Tiện Lợi</h4>
        <p>Dễ bảo quản, mang theo bất cứ đâu - văn phòng, du lịch, picnic. Không cần tủ lạnh, có thể bảo quản ở nhiệt độ phòng trong 6-12 tháng.</p>
        
        <h4>6. An Toàn</h4>
        <p>Không chất bảo quản, không hương liệu tổng hợp, không phẩm màu. Mộc Vị cam kết 100% tự nhiên, an toàn cho sức khỏe.</p>
        
        <h3>Cách Sử Dụng Hoa Quả Sấy</h3>
        <p>Hoa quả sấy có thể được sử dụng theo nhiều cách khác nhau:</p>
        <ul>
            <li><strong>Ăn trực tiếp:</strong> Như một món snack healthy thay thế cho kẹo, bánh ngọt</li>
            <li><strong>Pha trà:</strong> Thêm vào trà nóng để tạo hương vị thơm ngon và bổ dưỡng</li>
            <li><strong>Làm topping:</strong> Cho sữa chua, kem, bánh ngọt, salad</li>
            <li><strong>Nấu chè:</strong> Kết hợp với các loại hạt, tạo món chè bổ dưỡng</li>
            <li><strong>Làm sinh tố:</strong> Xay cùng sữa tươi hoặc sữa hạt</li>
            <li><strong>Nướng bánh:</strong> Thêm vào bánh muffin, bánh quy, bánh mì</li>
        </ul>
        
        <h3>Cam Kết Chất Lượng</h3>
        <p>Mộc Vị luôn đặt chất lượng sản phẩm lên hàng đầu. Chúng tôi cam kết:</p>
        <ul>
            <li>100% nguyên liệu từ Mộc Châu</li>
            <li>Không sử dụng hóa chất, chất bảo quản</li>
            <li>Quy trình sản xuất khép kín, đạt chuẩn VSATTP</li>
            <li>Kiểm tra chất lượng nghiêm ngặt</li>
            <li>Bao bì thân thiện môi trường</li>
        </ul>
        
        <p>Hãy để Mộc Vị đồng hành cùng bạn trên hành trình chăm sóc sức khỏe với những sản phẩm hoa quả sấy chất lượng cao từ Mộc Châu!</p>
    </div>',
    N'Khám phá hương vị đặc trưng của hoa quả sấy Mộc Châu - nơi thiên nhiên hòa quyện cùng công nghệ hiện đại, mang đến sản phẩm chất lượng cao, giàu dinh dưỡng.',
    '/images/bg_1.jpg',
    N'Admin Mộc Vị',
    150,
    1,
    GETDATE(),
    GETDATE()
);

-- Blog 2: Dâu tây sấy
INSERT INTO Blogs (Title, Slug, Content, ShortDescription, ImageUrl, AuthorName, ViewCount, IsPublished, PublishedDate, CreatedDate)
VALUES (
    N'Dâu Tây Sấy Mộc Châu - Vị Ngọt Thanh Khó Quên',
    'dau-tay-say-moc-chau',
    N'<div class="blog-content">
        <h2>Dâu Tây Mộc Châu - Đặc Sản Vùng Cao</h2>
        <p>Dâu tây Mộc Châu nổi tiếng khắp cả nước với màu đỏ tươi, vị ngọt thanh và hương thơm đặc trưng. Được trồng trên cao nguyên với khí hậu mát mẻ quanh năm, dâu tây Mộc Châu có chất lượng vượt trội so với nhiều vùng khác.</p>
        
        <p>Mùa dâu tây Mộc Châu thường bắt đầu từ tháng 11 và kéo dài đến tháng 3 năm sau. Đây là thời điểm cao điểm du lịch Mộc Châu, khi hàng nghìn du khách đổ về để tham quan vườn dâu và thưởng thức trái dâu tươi ngọt lịm.</p>
        
        <h3>Tại Sao Nên Chọn Dâu Tây Sấy?</h3>
        <p>Dâu tây tươi tuy ngon nhưng chỉ có thể bảo quản trong thời gian ngắn, thường chỉ 2-3 ngày trong tủ lạnh. Dâu tây sấy là giải pháp hoàn hảo để bạn có thể thưởng thức hương vị Mộc Châu bất cứ lúc nào, ngay cả khi không phải mùa dâu.</p>
        
        <h4>1. Giữ Nguyên Hương Vị Tự Nhiên</h4>
        <p>Công nghệ sấy lạnh giúp dâu tây giữ được màu sắc đỏ tươi tự nhiên, vị ngọt thanh và hương thơm đặc trưng. Không sử dụng đường, hương liệu hay chất bảo quản. Mỗi miếng dâu sấy đều mang trong mình hương vị nguyên bản của cao nguyên Mộc Châu.</p>
        
        <h4>2. Giàu Vitamin C</h4>
        <p>Dâu tây là nguồn cung cấp vitamin C dồi dào, với hàm lượng cao hơn cả cam. 100g dâu tây sấy có thể cung cấp đến 60mg vitamin C, tương đương 60% nhu cầu hàng ngày. Vitamin C giúp:</p>
        <ul>
            <li>Tăng cường hệ miễn dịch, phòng chống cảm cúm</li>
            <li>Làm đẹp da, chống lão hóa, giảm nếp nhăn</li>
            <li>Hỗ trợ hấp thu sắt, phòng ngừa thiếu máu</li>
            <li>Bảo vệ tim mạch, giảm cholesterol xấu</li>
        </ul>
        
        <h4>3. Chất Chống Oxy Hóa Cao</h4>
        <p>Anthocyanin - chất tạo màu đỏ tự nhiên trong dâu tây - là một chất chống oxy hóa mạnh mẽ. Nó giúp:</p>
        <ul>
            <li>Bảo vệ tế bào khỏi tổn thương do gốc tự do</li>
            <li>Giảm nguy cơ mắc các bệnh mãn tính như tim mạch, tiểu đường</li>
            <li>Cải thiện trí nhớ và chức năng não bộ</li>
            <li>Chống viêm, giảm đau khớp</li>
        </ul>
        
        <h4>4. Hỗ Trợ Giảm Cân</h4>
        <p>Với chỉ khoảng 150 calories trong 100g, dâu tây sấy là lựa chọn lý tưởng cho người đang ăn kiêng. Chất xơ cao giúp no lâu, kiểm soát cảm giác thèm ăn. Đồng thời, dâu tây còn chứa enzyme giúp đốt cháy mỡ thừa hiệu quả.</p>
        
        <h3>Giá Trị Dinh Dưỡng</h3>
        <p>Trong 100g dâu tây sấy có:</p>
        <ul>
            <li>Năng lượng: 150 kcal</li>
            <li>Protein: 2g</li>
            <li>Chất xơ: 5g</li>
            <li>Vitamin C: 60mg</li>
            <li>Vitamin A: 12 IU</li>
            <li>Kali: 220mg</li>
            <li>Canxi: 25mg</li>
            <li>Sắt: 0.8mg</li>
        </ul>
        
        <h3>Cách Thưởng Thức Dâu Tây Sấy</h3>
        <h4>1. Ăn Trực Tiếp</h4>
        <p>Đơn giản nhất là ăn như một món snack healthy. Bạn có thể mang theo văn phòng, đi du lịch hoặc ăn khi xem phim.</p>
        
        <h4>2. Pha Trà Dâu</h4>
        <p>Cho 5-7 miếng dâu sấy vào ấm trà, rót nước sôi vào và ủ 5 phút. Bạn sẽ có ly trà dâu thơm ngon, giàu vitamin C. Có thể thêm mật ong để tăng vị ngọt.</p>
        
        <h4>3. Làm Topping</h4>
        <p>Rắc lên sữa chua, kem, bánh ngọt, salad để tăng hương vị và giá trị dinh dưỡng. Dâu sấy cũng là topping tuyệt vời cho các loại smoothie bowl.</p>
        
        <h4>4. Nấu Chè</h4>
        <p>Kết hợp với hạt sen, long nhãn, hạt chia để tạo món chè bổ dưỡng. Dâu sấy sẽ thấm nước và mềm ra, tạo vị ngọt thanh tự nhiên cho món chè.</p>
        
        <h4>5. Làm Sinh Tố</h4>
        <p>Xay dâu sấy cùng sữa tươi, chuối, yến mạch để tạo món sinh tố bổ dưỡng cho bữa sáng.</p>
        
        <h3>Bảo Quản Dâu Tây Sấy</h3>
        <p>Để giữ được chất lượng tốt nhất:</p>
        <ul>
            <li>Bảo quản ở nơi khô ráo, thoáng mát</li>
            <li>Tránh ánh nắng trực tiếp</li>
            <li>Sau khi mở bao bì, nên bảo quản trong hộp kín</li>
            <li>Sử dụng trong vòng 2-3 tuần sau khi mở</li>
            <li>Có thể bảo quản trong tủ lạnh để kéo dài thời gian sử dụng</li>
        </ul>
        
        <p>Hãy thử ngay dâu tây sấy Mộc Vị để cảm nhận sự khác biệt từ cao nguyên Mộc Châu!</p>
    </div>',
    N'Dâu tây sấy Mộc Châu - sản phẩm tiên phong của Mộc Vị, giữ trọn vị ngọt thanh và màu sắc tự nhiên của cao nguyên. Giàu vitamin C, tốt cho sức khỏe.',
    '/images/bg_2.jpg',
    N'Admin Mộc Vị',
    230,
    1,
    GETDATE(),
    GETDATE()
);

PRINT N'Đã cập nhật blog với nội dung chi tiết!';
GO
