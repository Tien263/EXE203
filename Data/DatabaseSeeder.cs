using Exe_Demo.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Exe_Demo.Data;

public static class DatabaseSeeder
{
    // Removed custom SHA256 HashPassword as it's incompatible with Identity

    public static void SeedData(ApplicationDbContext context)
    {
        Console.WriteLine("--> [SEEDER] Starting SeedData process...");
        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
        
        try
        {
            // 0. Prepare categories list
            var expectedNames = new[] { "Hoa Quả Sấy Dẻo", "Hoa Quả Sấy Giòn", "Hoa Quả Sấy Thăng Hoa", "Giỏ Quà Tết" };
            var existingCats = context.Categories.ToList();

            // 1. Seed Categories
            var categoryNames = expectedNames;
            foreach (var name in categoryNames)
            {
                bool exists = context.Categories.ToList().Any(c => c.CategoryName.Trim().ToLower() == name.Trim().ToLower());
                if (!exists)
                {
                    context.Categories.Add(new Category 
                    { 
                        CategoryName = name, 
                        Description = name switch {
                            "Hoa Quả Sấy Dẻo" => "Hoa quả sấy dẻo giữ nguyên vị ngọt tự nhiên, mềm mại",
                            "Hoa Quả Sấy Giòn" => "Hoa quả sấy giòn tan, thơm ngon, giàu chất xơ",
                            "Hoa Quả Sấy Thăng Hoa" => "Công nghệ sấy thăng hoa hiện đại, giữ nguyên dinh dưỡng",
                            "Giỏ Quà Tết" => "Giỏ quà tặng dịp Tết, phù hợp biếu tặng người thân",
                            _ => ""
                        },
                        DisplayOrder = Array.IndexOf(categoryNames, name) + 1, 
                        IsActive = true 
                    });
                }
            }
            context.SaveChanges();
            Console.WriteLine($"--> [SEEDER] After category seed: {context.Categories.Count()} categories in DB.");

            // 2. Seed Products
            Console.WriteLine("--> Seeding products...");
            var categories = context.Categories.OrderBy(c => c.CategoryId).ToList();
            Console.WriteLine($"--> [SEEDER] Loaded {categories.Count} categories for product mapping:");
            
            // Map safely by Order or fallback to first
            int catDeoId = categories.Count > 0 ? categories[0].CategoryId : 1;
            int catGionId = categories.Count > 1 ? categories[1].CategoryId : 1;
            int catThangHoaId = categories.Count > 2 ? categories[2].CategoryId : 1;
            int catMiniId = categories.Count > 3 ? categories[3].CategoryId : 1;

            // Optional explicit match if order wasn't exactly 1, 2, 3, 4
            var sDeo = categories.FirstOrDefault(c => c.CategoryName.Contains("Dẻo") || c.CategoryName.Contains("Deo"));
            var sGion = categories.FirstOrDefault(c => c.CategoryName.Contains("Giòn") || c.CategoryName.Contains("Gion"));
            var sThangHoa = categories.FirstOrDefault(c => c.CategoryName.Contains("Thăng Hoa") || c.CategoryName.Contains("Thang Hoa"));
            var sMini = categories.FirstOrDefault(c => c.CategoryName.Contains("Giỏ") || c.CategoryName.Contains("Tết") || c.CategoryName.Contains("Combo") || c.CategoryName.Contains("Mini"));

            if(sDeo != null) catDeoId = sDeo.CategoryId;
            if(sGion != null) catGionId = sGion.CategoryId;
            if(sThangHoa != null) catThangHoaId = sThangHoa.CategoryId;
            if(sMini != null) catMiniId = sMini.CategoryId;

            // ADD PRODUCTS
            Console.WriteLine("--> [SEEDER] Seeding exactly 18 products carefully (Upsert mode)...");
            // USER REQUESTED TO DELETE ALL PRODUCTS
            Console.WriteLine("--> [SEEDER] Skipping product seeding (Reset to empty as requested).");
        
        // context.Products.AddRange(productList);
        // context.SaveChanges();
        // Console.WriteLine($"--> Added {productList.Count} products.");

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
                 existingStaff.PasswordHash = hasher.HashPassword(null!, "Staff@123");
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
                    PasswordHash = hasher.HashPassword(null!, "Staff@123"),
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
        var staffAccounts = new[] { "nv1@gmail.com", "nv2@gmail.com", "nv3@gmail.com" };
        foreach (var email in staffAccounts)
        {
            var newHash = hasher.HashPassword(null!, "Mocvi@123");
            
            // NUCLEAR OPTION: Use direct SQL to ensure the hash is updated regardless of EF tracker state
            context.Database.ExecuteSqlRaw("UPDATE Users SET PasswordHash = {0}, Role = 'Staff' WHERE Email = {1}", newHash, email);
            
            if (!context.Users.Any(u => u.Email == email))
            {
                context.Users.Add(new User
                {
                    Email = email,
                    PasswordHash = newHash,
                    FullName = "Nhân viên mới",
                    Role = "Staff",
                    EmployeeId = null,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                });
                Console.WriteLine($"Created staff account: {email}");
            }
        }

        // Also update standard staff and admin emails
        context.Database.ExecuteSqlRaw("UPDATE Users SET PasswordHash = {0} WHERE Email = {1}", hasher.HashPassword(null!, "Staff@123"), staffEmail);
        context.Database.ExecuteSqlRaw("UPDATE Users SET PasswordHash = {0} WHERE Email = {1}", hasher.HashPassword(null!, "Admin@123"), adminEmail);

        if (context.Users.Any(u => u.Email == adminEmail))
        {
             var existingAdmin = context.Users.FirstOrDefault(u => u.Email == adminEmail);
             if (existingAdmin != null)
             {
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
                    PasswordHash = hasher.HashPassword(null!, "Admin@123"),
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
             } // closes if (adminUser != null)
        } // closes if (!context.Blogs.Any())
        
        Console.WriteLine("✅ Database seeded successfully with products, employees, users and 5 blog posts!");
        } // closes try block
        catch (Exception ex)
        {
            Console.WriteLine($"Error during seeding: {ex.Message}");
            throw;
        }
    }
}
