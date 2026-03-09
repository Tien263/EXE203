using Exe_Demo.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Exe_Demo.Data;

public static class DatabaseSeeder
{
    private static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    public static void SeedData(ApplicationDbContext context)
    {
        Console.WriteLine("--> Aggressively cleaning and seeding database...");

        try
        {
            // Clear existing data to ensure a clean state (handle FKs with CASCADE if supported, or sequence)
            // Use raw SQL for Postgres-compatible truncation
            if (context.Database.IsNpgsql())
            {
                // Clear products, categories and reset IDs
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE \"Products\", \"Categories\" RESTART IDENTITY CASCADE;");
                Console.WriteLine("--> Cleared Products and Categories (Postgres)");
            }
            else if (context.Database.IsSqlServer())
            {
                // SQL Server approach
                context.Database.ExecuteSqlRaw("DELETE FROM Products; DELETE FROM Categories;");
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Products', RESEED, 0); DBCC CHECKIDENT ('Categories', RESEED, 0);");
                Console.WriteLine("--> Cleared Products and Categories (SQL Server)");
            }
            else 
            {
                // SQLite or other
                context.Products.RemoveRange(context.Products);
                context.Categories.RemoveRange(context.Categories);
                context.SaveChanges();
                Console.WriteLine("--> Cleared Products and Categories (Generic)");
            }

            // 1. Seed Categories
            var categoryNames = new[] { "Sản Phẩm Sấy Dẻo", "Sản Phẩm Sấy Giòn", "Sản Phẩm Sấy Thăng Hoa", "Mini Size Mix" };
            foreach (var name in categoryNames)
            {
                context.Categories.Add(new Category 
                { 
                    CategoryName = name, 
                    Description = name switch {
                        "Sản Phẩm Sấy Dẻo" => "Hoa quả sấy dẻo giữ nguyên vị ngọt tự nhiên, mềm mại",
                        "Sản Phẩm Sấy Giòn" => "Hoa quả sấy giòn tan, thơm ngon, giàu chất xơ",
                        "Sản Phẩm Sấy Thăng Hoa" => "Công nghệ sấy thăng hoa hiện đại, giữ nguyên dinh dưỡng",
                        "Mini Size Mix" => "Gói nhỏ tiện lợi để mix nhiều loại (tối thiểu 4 pack)",
                        _ => ""
                    },
                    DisplayOrder = Array.IndexOf(categoryNames, name) + 1, 
                    IsActive = true 
                });
            }
        }
        context.SaveChanges();

        // 2. Seed Products
        Console.WriteLine("--> Seeding products...");
        var categories = context.Categories.ToList();
        var catDeo = categories.First(c => c.CategoryName.Contains("Sấy Dẻo", StringComparison.OrdinalIgnoreCase));
        var catGion = categories.First(c => c.CategoryName.Contains("Sấy Giòn", StringComparison.OrdinalIgnoreCase));
        var catThangHoa = categories.First(c => c.CategoryName.Contains("Thăng Hoa", StringComparison.OrdinalIgnoreCase));
        var catMini = categories.First(c => c.CategoryName.Contains("Mini Size", StringComparison.OrdinalIgnoreCase));

        var productList = new List<Product>
        {
            // SẤY DẺO
            new Product { ProductCode = "SD-MAN-200", ProductName = "Mận Sấy Dẻo", CategoryId = catDeo.CategoryId, Price = 65000, StockQuantity = 100, Description = "Mận sấy dẻo Mộc Châu được chế biến từ những trái mận chín mọng, tươi ngon. Sản phẩm giữ nguyên vị chua ngọt tự nhiên, mềm mại, thơm ngon. Giàu vitamin C, chất xơ tốt cho sức khỏe.", ShortDescription = "Mận sấy dẻo giữ nguyên vị chua ngọt tự nhiên", ImageUrl = "/images/products/man-say.jpg", Unit = "Gói", Weight = "200g", IsActive = true, IsFeatured = true, IsNew = true },
            new Product { ProductCode = "SD-XOAI-200", ProductName = "Xoài Sấy Dẻo", CategoryId = catDeo.CategoryId, Price = 70000, StockQuantity = 100, Description = "Xoài sấy dẻo từ xoài Mộc Châu thơm ngon, ngọt tự nhiên. Sản phẩm giữ nguyên hương vị đặc trưng của xoài tươi, mềm dẻo, không chất bảo quản.", ShortDescription = "Xoài Mộc Châu thơm ngon, ngọt tự nhiên", ImageUrl = "/images/products/xoai-say.jpg", Unit = "Gói", Weight = "200g", IsActive = true, IsFeatured = true, IsNew = true },
            new Product { ProductCode = "SD-DAO-200", ProductName = "Đào Sấy Dẻo", CategoryId = catDeo.CategoryId, Price = 65000, StockQuantity = 100, Description = "Đào sấy dẻo Mộc Châu với vị ngọt thanh, thơm mát. Sản phẩm giữ nguyên màu sắc tự nhiên, mềm dẻo, giàu vitamin và khoáng chất.", ShortDescription = "Đào sấy dẻo vị ngọt thanh, thơm mát", ImageUrl = "/images/products/dao-say.jpg", Unit = "Gói", Weight = "200g", IsActive = true, IsFeatured = true, IsNew = false },
            new Product { ProductCode = "SD-DAU-200", ProductName = "Dâu Sấy Dẻo", CategoryId = catDeo.CategoryId, Price = 90000, StockQuantity = 80, Description = "Dâu sấy dẻo Mộc Châu từ dâu tây tươi ngon, giàu vitamin C. Sản phẩm có vị chua ngọt hài hòa, màu đỏ tự nhiên, mềm dẻo thơm ngon.", ShortDescription = "Dâu tây sấy dẻo giàu vitamin C", ImageUrl = "/images/products/dau-say.jpg", Unit = "Gói", Weight = "200g", IsActive = true, IsFeatured = true, IsNew = true },
            new Product { ProductCode = "SD-HONG-200", ProductName = "Hồng Sấy Dẻo", CategoryId = catDeo.CategoryId, Price = 95000, StockQuantity = 80, Description = "Hồng sấy dẻo Mộc Châu từ hồng giòn cao cấp. Sản phẩm giữ nguyên vị ngọt thanh, thơm mát đặc trưng của hồng tươi, mềm dẻo, bổ dưỡng.", ShortDescription = "Hồng giòn sấy dẻo cao cấp", ImageUrl = "/images/products/hong-say.jpg", Unit = "Gói", Weight = "200g", IsActive = true, IsFeatured = true, IsNew = true },
            
            // SẤY GIÒN
            new Product { ProductCode = "SG-MIT-200", ProductName = "Mít Sấy Giòn", CategoryId = catGion.CategoryId, Price = 80000, StockQuantity = 100, Description = "Mít sấy giòn Mộc Châu từ mít tươi ngon, thơm ngọt. Sản phẩm giòn tan, thơm nức, giữ nguyên hương vị đặc trưng của mít tươi. Giàu chất xơ, vitamin.", ShortDescription = "Mít sấy giòn tan, thơm nức", ImageUrl = "/images/products/mit-say.jpg", Unit = "Gói", Weight = "200g", IsActive = true, IsFeatured = true, IsNew = true },
            new Product { ProductCode = "SG-CHUOI-200", ProductName = "Chuối Sấy Giòn", CategoryId = catGion.CategoryId, Price = 80000, StockQuantity = 100, Description = "Chuối sấy giòn Mộc Châu từ chuối già chín tự nhiên. Sản phẩm giòn rụm, ngọt thanh, giàu kali and năng lượng. Thích hợp làm snack healthy.", ShortDescription = "Chuối sấy giòn rụm, ngọt thanh", ImageUrl = "/images/products/chuoi-say.jpg", Unit = "Gói", Weight = "200g", IsActive = true, IsFeatured = true, IsNew = false },
            
            // SẤY THĂNG HOA
            new Product { ProductCode = "STH-DAU-100", ProductName = "Dâu Sấy Thăng Hoa", CategoryId = catThangHoa.CategoryId, Price = 140000, StockQuantity = 60, Description = "Dâu sấy thăng hoa với công nghệ hiện đại, giữ nguyên 98% dinh dưỡng. Sản phẩm giòn nhẹ, tan trong miệng, hương vị đậm đà. Không chất bảo quản.", ShortDescription = "Công nghệ thăng hoa giữ nguyên dinh dưỡng", ImageUrl = "/images/products/dau-say-thang-hoa.jpg", Unit = "Gói", Weight = "100g", IsActive = true, IsFeatured = true, IsNew = true },
            new Product { ProductCode = "STH-SC-100", ProductName = "Sữa Chua Sấy Thăng Hoa", CategoryId = catThangHoa.CategoryId, Price = 95000, StockQuantity = 60, Description = "Sữa chua sấy thăng hoa độc đáo, mới lạ. Sản phẩm giòn tan, vị chua ngọt hài hòa, giàu men vi sinh có lợi. Thích hợp cho mọi lứa tuổi.", ShortDescription = "Sữa chua sấy giòn tan, giàu men vi sinh", ImageUrl = "/images/products/sua-chua-say-thang-hoa.jpg", Unit = "Gói", Weight = "100g", IsActive = true, IsFeatured = true, IsNew = true },
            
            // MINI SIZE
            new Product { ProductCode = "SD-MAN-50", ProductName = "Mận Sấy Dẻo Mini", CategoryId = catMini.CategoryId, Price = 18000, StockQuantity = 200, Description = "Mận sấy dẻo gói mini 50g tiện lợi. Thích hợp để mix nhiều loại, mang theo du lịch. Tối thiểu đặt 4 pack.", ShortDescription = "Gói mini 50g tiện lợi (tối thiểu 4 pack)", ImageUrl = "/images/products/man-say.jpg", Unit = "Gói", Weight = "50g", IsActive = true, IsFeatured = false, IsNew = false },
            new Product { ProductCode = "SD-XOAI-50", ProductName = "Xoài Sấy Dẻo Mini", CategoryId = catMini.CategoryId, Price = 20000, StockQuantity = 200, Description = "Xoài sấy dẻo gói mini 50g tiện lợi. Thích hợp để mix nhiều loại, mang theo du lịch. Tối thiểu đặt 4 pack.", ShortDescription = "Gói mini 50g tiện lợi (tối thiểu 4 pack)", ImageUrl = "/images/products/xoai-say.jpg", Unit = "Gói", Weight = "50g", IsActive = true, IsFeatured = false, IsNew = false },
            new Product { ProductCode = "SD-DAO-50", ProductName = "Đào Sấy Dẻo Mini", CategoryId = catMini.CategoryId, Price = 18000, StockQuantity = 200, Description = "Đào sấy dẻo gói mini 50g tiện lợi. Thích hợp để mix nhiều loại, mang theo du lịch. Tối thiểu đặt 4 pack.", ShortDescription = "Gói mini 50g tiện lợi (tối thiểu 4 pack)", ImageUrl = "/images/products/dao-say.jpg", Unit = "Gói", Weight = "50g", IsActive = true, IsFeatured = false, IsNew = false },
            new Product { ProductCode = "SD-DAU-50", ProductName = "Dâu Sấy Dẻo Mini", CategoryId = catMini.CategoryId, Price = 25000, StockQuantity = 200, Description = "Dâu sấy dẻo gói mini 50g tiện lợi. Thích hợp để mix nhiều loại, mang theo du lịch. Tối thiểu đặt 4 pack.", ShortDescription = "Gói mini 50g tiện lợi (tối thiểu 4 pack)", ImageUrl = "/images/products/dau-say.jpg", Unit = "Gói", Weight = "50g", IsActive = true, IsFeatured = false, IsNew = false },
            new Product { ProductCode = "SD-HONG-50", ProductName = "Hồng Sấy Dẻo Mini", CategoryId = catMini.CategoryId, Price = 28000, StockQuantity = 200, Description = "Hồng sấy dẻo gói mini 50g tiện lợi. Thích hợp để mix nhiều loại, mang theo du lịch. Tối thiểu đặt 4 pack.", ShortDescription = "Gói mini 50g tiện lợi (tối thiểu 4 pack)", ImageUrl = "/images/products/hong-say.jpg", Unit = "Gói", Weight = "50g", IsActive = true, IsFeatured = false, IsNew = false },
            new Product { ProductCode = "SG-MIT-50", ProductName = "Mít Sấy Giòn Mini", CategoryId = catMini.CategoryId, Price = 22000, StockQuantity = 200, Description = "Mít sấy giòn gói mini 50g tiện lợi. Thích hợp để mix nhiều loại, mang theo du lịch. Tối thiểu đặt 4 pack.", ShortDescription = "Gói mini 50g tiện lợi (tối thiểu 4 pack)", ImageUrl = "/images/products/mit-say.jpg", Unit = "Gói", Weight = "50g", IsActive = true, IsFeatured = false, IsNew = false },
            new Product { ProductCode = "SG-CHUOI-50", ProductName = "Chuối Sấy Giòn Mini", CategoryId = catMini.CategoryId, Price = 22000, StockQuantity = 200, Description = "Chuối sấy giòn gói mini 50g tiện lợi. Thích hợp để mix nhiều loại, mang theo du lịch. Tối thiểu đặt 4 pack.", ShortDescription = "Gói mini 50g tiện lợi (tối thiểu 4 pack)", ImageUrl = "/images/products/chuoi-say.jpg", Unit = "Gói", Weight = "50g", IsActive = true, IsFeatured = false, IsNew = false },
            new Product { ProductCode = "STH-DAU-50", ProductName = "Dâu Sấy Thăng Hoa Mini", CategoryId = catMini.CategoryId, Price = 75000, StockQuantity = 150, Description = "Dâu sấy thăng hoa gói mini 50g tiện lợi. Thích hợp để mix many loại, mang theo du lịch. Tối thiểu đặt 4 pack.", ShortDescription = "Gói mini 50g tiện lợi (tối thiểu 4 pack)", ImageUrl = "/images/products/dau-say-thang-hoa.jpg", Unit = "Gói", Weight = "50g", IsActive = true, IsFeatured = false, IsNew = false },
            new Product { ProductCode = "STH-SC-50", ProductName = "Sữa Chua Sấy Thăng Hoa Mini", CategoryId = catMini.CategoryId, Price = 50000, StockQuantity = 150, Description = "Sữa chua sấy thăng hoa gói mini 50g tiện lợi. Thích hợp để mix many loại, mang theo du lịch. Tối thiểu đặt 4 pack.", ShortDescription = "Gói mini 50g tiện lợi (tối thiểu 4 pack)", ImageUrl = "/images/products/sua-chua-say-thang-hoa.jpg", Unit = "Gói", Weight = "50g", IsActive = true, IsFeatured = false, IsNew = false }
        };

        context.Products.AddRange(productList);
        context.SaveChanges();
        Console.WriteLine($"--> Added {productList.Count} products.");

        var adminEmail = "admin@mocvistore.com";
        var staffEmail = "staff@mocvistore.com";

        // 1. Ensure Staff Employee Exists
        var emp1 = context.Employees.FirstOrDefault(e => e.Email == staffEmail);
        if (emp1 == null)
        {
            emp1 = new Employee
            {
                EmployeeCode = "NV001",
                FullName = "Nguyễn Văn A",
                PhoneNumber = "0901234567",
                Email = staffEmail,
                Position = "Nhân viên bán hàng",
                Department = "Bán hàng",
                Salary = 8000000,
                HireDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-2)),
                IsActive = true,
                CreatedDate = DateTime.Now
            };
            context.Employees.Add(emp1);
            context.SaveChanges();
            Console.WriteLine("Created missing Staff Employee");
        }

        // 2. Ensure Admin Employee Exists
        var emp2 = context.Employees.FirstOrDefault(e => e.Email == adminEmail);
        if (emp2 == null)
        {
            emp2 = new Employee
            {
                EmployeeCode = "ADMIN001",
                FullName = "Quản Trị Viên",
                PhoneNumber = "0912345678",
                Email = adminEmail,
                Position = "Quản lý",
                Department = "Quản lý",
                Salary = 15000000,
                HireDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-3)),
                IsActive = true,
                CreatedDate = DateTime.Now
            };
            context.Employees.Add(emp2);
            context.SaveChanges();
            Console.WriteLine("Created missing Admin Employee");
        }

        if (context.Users.Any(u => u.Email == staffEmail))
        {
             // Update password if exists
             var existingStaff = context.Users.FirstOrDefault(u => u.Email == staffEmail);
             if (existingStaff != null)
             {
                 existingStaff.PasswordHash = HashPassword("Staff@123");
                 existingStaff.EmployeeId = emp1?.EmployeeId; // Ensure link
             }
        }
        else
        {
             // Create if not exists
             if (emp1 != null) {
                var user1 = new User
                {
                    Email = staffEmail,
                    PasswordHash = HashPassword("Staff@123"),
                    FullName = "Nguyễn Văn A",
                    PhoneNumber = "0901234567",
                    Role = "Staff",
                    EmployeeId = emp1.EmployeeId,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };
                context.Users.Add(user1);
             }

        }
        
        // Seed 3 Staff Accounts for First-Login Flow (nv1, nv2, nv3)
        // Moved outside to ensure execution even if main staff exists
        var staffAccounts = new[] { "nv1@gmail.com", "nv2@gmail.com", "nv3@gmail.com" };
        foreach (var email in staffAccounts)
        {
            if (!context.Users.Any(u => u.Email == email))
            {
                context.Users.Add(new User
                {
                    Email = email,
                    PasswordHash = HashPassword("Mocvi@123"),
                    FullName = "Nhân viên mới", // Placeholder
                    Role = "Staff",
                    EmployeeId = null, // IMPORTANT: Null to trigger Update Profile flow
                    IsActive = true,
                    CreatedDate = DateTime.Now
                });
                Console.WriteLine($"Created staff account: {email}");
            }
        }

        if (context.Users.Any(u => u.Email == adminEmail))
        {
             // Update password if exists
             var existingAdmin = context.Users.FirstOrDefault(u => u.Email == adminEmail);
             if (existingAdmin != null)
             {
                 existingAdmin.PasswordHash = HashPassword("Admin@123");
                 existingAdmin.EmployeeId = emp2?.EmployeeId;
             }
        }
        else
        {
             // Create if not exists
             if (emp2 != null) {
                var user2 = new User
                {
                    Email = adminEmail,
                    PasswordHash = HashPassword("Admin@123"),
                    FullName = "Quản Trị Viên",
                    PhoneNumber = "0912345678",
                    Role = "Admin",
                    EmployeeId = emp2.EmployeeId,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };
                context.Users.Add(user2);
             }
        }
        
        context.SaveChanges();

        if (!context.Blogs.Any())
        {
             // Seed Blog logic (User2 is admin)
             var adminUser = context.Users.FirstOrDefault(u => u.Email == adminEmail);
             if (adminUser != null) {
                var blogs = new List<Blog>
                {
                    new Blog
                    {
                        Title = "🍓 Dâu Tây Mộc Châu - Nữ Hoàng Hoa Quả Cao Nguyên",
                        Slug = "dau-tay-moc-chau-nu-hoang-hoa-qua-cao-nguyen",
                        ShortDescription = "Khám phá dâu tây Mộc Châu được mệnh danh là 'Nữ hoàng hoa quả cao nguyên' với vitamin C gấp 3 lần cam!",
                        Content = @"<h2>Nữ Hoàng Hoa Quả Cao Nguyên</h2>
    <p>Dâu tây Mộc Châu được mệnh danh là 'Nữ hoàng hoa quả cao nguyên'! Mỗi trái dâu được chọn lọc kỹ càng từ vườn dâu Mộc Châu 1200m so với mặt nước biển, nơi có khí hậu mát mẻ quanh năm.</p>
    
    <h3>Công Nghệ Sấy Thông Minh</h3>
    <p>Sấy dẻo ở nhiệt độ thấp 50-60°C, giữ trọn 95% vitamin C - gấp 3 lần cam! Màu đỏ tươi rực rỡ 100% tự nhiên, không một giọt màu nhân tạo.</p>
    
    <h3>Dinh Dưỡng Tuyệt Vời</h3>
    <ul>
    <li><strong>Vitamin C:</strong> Siêu cao (180mg/100g) - Gấp 3 lần cam, đáp ứng 200% nhu cầu hàng ngày</li>
    <li><strong>Anthocyanin:</strong> Chất chống oxy hóa mạnh từ màu đỏ tự nhiên - Bảo vệ tim mạch</li>
    <li><strong>Folate (Vitamin B9):</strong> Cao - Tốt cho phụ nữ mang thai và não bộ</li>
    <li><strong>Chất xơ:</strong> 3.5g/100g - Giúp no lâu, hỗ trợ giảm cân hiệu quả</li>
    </ul>
    
    <h3>Lợi Ích Sức Khỏe</h3>
    <p><strong>💪 Tăng Cường Miễn Dịch Vượt Trội</strong> - Vitamin C siêu cao giúp cơ thể chống lại virus, cảm cúm</p>
    <p><strong>✨ Làm Đẹp Da Từ Bên Trong</strong> - Chống oxy hóa mạnh, giảm nám, sạm, da sáng mịn tự nhiên</p>
    <p><strong>❤️ Bảo Vệ Tim Mạch</strong> - Anthocyanin giảm cholesterol xấu, ngăn ngừa đột quỵ</p>
    
    <h3>Cách Dùng Dâu Tây Sấy</h3>
    <ul>
    <li>🍵 Ăn vặt trực tiếp - Thay thế kẹo, bánh không lành mạnh</li>
    <li>🥤 Pha trà dâu detox - Ngâm với nước ấm, thêm mật ong</li>
    <li>🍨 Topping yogurt/kem - Trang trí đẹp mắt, tăng dinh dưỡng</li>
    <li>🎂 Làm bánh, trang trí món ăn - Màu đỏ tự nhiên bắt mắt</li>
    </ul>",
                        AuthorId = adminUser.Id,
                        IsPublished = true,
                        PublishedDate = DateTime.Now.AddDays(-5),
                        CreatedDate = DateTime.Now
                    },
                    new Blog
                    {
                        Title = "🌟 Công Nghệ Freeze-Dried - Dâu Sấy Thăng Hoa Cao Cấp",
                        Slug = "cong-nghe-freeze-dried-dau-say-thang-hoa-cao-cap",
                        ShortDescription = "Khám phá công nghệ freeze-dried hiện đại từ Nhật Bản giữ 98% dinh dưỡng và tạo kết cấu giòn xốp kỳ diệu!",
                        Content = @"<h2>Đỉnh Cao Công Nghệ - Dâu Sấy Thăng Hoa</h2>
    <p>Bạn đã bao giờ thử dâu tây 'tan như tuyết' trong miệng chưa? Đây là sản phẩm CAO CẤP NHẤT của Mộc Vị!</p>
    
    <h3>Công Nghệ Freeze-Dried Nhật Bản</h3>
    <p>Sử dụng công nghệ Freeze-Dried (sấy đông khô) hiện đại từ Nhật Bản, sấy ở nhiệt độ âm sâu -40°C, giữ trọn 98% dinh dưỡng và màu sắc tự nhiên.</p>
    
    <h3>Đặc Điểm Nổi Bật</h3>
    <ul>
    <li><strong>Kết Cấu Giòn Xốp Kỳ Diệu:</strong> Tan ngay khi chạm lưỡi, trải nghiệm hoàn toàn mới</li>
    <li><strong>Hương Vị Đậm Đà:</strong> Gấp 10 lần dâu tươi, cô đặc tinh túy Mộc Châu</li>
    <li><strong>Màu Sắc Tự Nhiên:</strong> Đỏ rực rỡ như vừa mới hái</li>
    <li><strong>Không Thêm Chất Lạ:</strong> Không đường, không dầu mỡ, không chất bảo quản</li>
    </ul>
    
    <h3>Dinh Dưỡng Siêu Cô Đặc</h3>
    <ul>
    <li><strong>Vitamin C:</strong> 300mg/100g - Gấp 5 lần dâu tươi, gấp 5 lần cam</li>
    <li><strong>Anthocyanin:</strong> Cô đặc gấp 8 lần - Chống oxy hóa mạnh nhất</li>
    <li><strong>Folate:</strong> Cao gấp 6 lần - Tốt cho thai nhi và não bộ</li>
    <li><strong>Kali:</strong> Điều hòa huyết áp hiệu quả</li>
    </ul>
    
    <h3>Ai Nên Thử Dâu Sấy Thăng Hoa?</h3>
    <p>👑 Người thành đạt, yêu chất lượng</p>
    <p>🏋️ Gymer, vận động viên</p>
    <p>🎁 Quà tặng cao cấp dịp lễ, Tết</p>
    <p>👨‍💼 Doanh nhân, CEO</p>",
                        AuthorId = adminUser.Id,
                        IsPublished = true,
                        PublishedDate = DateTime.Now.AddDays(-3),
                        CreatedDate = DateTime.Now
                    }
                };
                context.Blogs.AddRange(blogs);
                context.SaveChanges();
             }
        }      Console.WriteLine("✅ Database seeded successfully with products, employees, users and 5 blog posts!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during seeding: {ex.Message}");
            throw;
        }
    }
}
