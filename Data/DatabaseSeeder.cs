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
        // Check if products already exist (keep this check for products)
        if (context.Products.Any())
        {
            Console.WriteLine("Products already seeded");
        }
        else 
        {
            // Move product seeding logic here... but getting complicated to split.
            // Simplified approach: Just remove the check return, and use AddRange/Add checks inside.
            // Better: Let's check specifically for the admin user to decide if we need to seed accounts.
        }
        
        // Revised logic:
        Console.WriteLine("Seeding database...");

        if (!context.Categories.Any())
        {
             // ... (Keep existing Category seeding)
             var categories = new List<Category>
            {
                new Category { CategoryName = "Hoa Quả Sấy", Description = "Các loại hoa quả sấy khô tự nhiên" },
                new Category { CategoryName = "Hoa Quả Sấy Dẻo", Description = "Hoa quả sấy giữ độ mềm tự nhiên" },
                new Category { CategoryName = "Hoa Quả Sấy Thăng Hoa", Description = "Hoa quả sấy công nghệ thăng hoa" },
                new Category { CategoryName = "Combo Quà Tặng", Description = "Combo hoa quả sấy làm quà" }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();
        }

        if (!context.Products.Any())
        {
             // ... (Keep existing Product seeding)
             var products = new List<Product>
            {
                new Product
                {
                    ProductCode = "MIT001",
                    ProductName = "Mít Sấy Giòn",
                    CategoryId = 1,
                    Price = 150000,
                    StockQuantity = 100,
                    Description = "Mít sấy giòn tự nhiên, không chất bảo quản",
                    ImageUrl = "/images/products/mit-say.jpg",
                    IsActive = true
                },
                new Product
                {
                    ProductCode = "CHUOI001",
                    ProductName = "Chuối Sấy Dẻo",
                    CategoryId = 2,
                    Price = 120000,
                    StockQuantity = 150,
                    Description = "Chuối sấy dẻo thơm ngon, giữ nguyên vị tự nhiên",
                    ImageUrl = "/images/products/chuoi-say.jpg",
                    IsActive = true
                },
                new Product
                {
                    ProductCode = "XOAI001",
                    ProductName = "Xoài Sấy Dẻo",
                    CategoryId = 2,
                    Price = 180000,
                    StockQuantity = 80,
                    Description = "Xoài sấy dẻo chua ngọt đậm đà",
                    ImageUrl = "/images/products/xoai-say.jpg",
                    IsActive = true
                },
                new Product
                {
                    ProductCode = "DAU001",
                    ProductName = "Dâu Tây Sấy Thăng Hoa",
                    CategoryId = 3,
                    Price = 250000,
                    StockQuantity = 50,
                    Description = "Dâu tây sấy thăng hoa giữ nguyên hương vị",
                    ImageUrl = "/images/products/dau-say.jpg",
                    IsActive = true
                },
                new Product
                {
                    ProductCode = "COMBO001",
                    ProductName = "Combo Hoa Quả Sấy 5 Loại",
                    CategoryId = 4,
                    Price = 350000,
                    StockQuantity = 30,
                    Description = "Combo 5 loại hoa quả sấy đa dạng",
                    ImageUrl = "/images/products/combo-5.jpg",
                    IsActive = true
                }
            };
            context.Products.AddRange(products);
            context.SaveChanges();
        }

        if (!context.Employees.Any())
        {
            var emp1 = new Employee
            {
                EmployeeCode = "NV001",
                FullName = "Nguyễn Văn A",
                PhoneNumber = "0901234567",
                Email = "staff@mocvistore.com",
                Position = "Nhân viên bán hàng",
                Department = "Bán hàng",
                Salary = 8000000,
                HireDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-2)),
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            var emp2 = new Employee
            {
                EmployeeCode = "ADMIN001",
                FullName = "Quản Trị Viên",
                PhoneNumber = "0912345678",
                Email = "admin@mocvistore.com",
                Position = "Quản lý",
                Department = "Quản lý",
                Salary = 15000000,
                HireDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-3)),
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            context.Employees.Add(emp1);
            context.Employees.Add(emp2);
            context.SaveChanges();
        }

        // Seed Users if specific admin/staff emails don't exist
        var adminEmail = "admin@mocvistore.com";
        var staffEmail = "staff@mocvistore.com";

        if (!context.Users.Any(u => u.Email == staffEmail))
        {
             var emp1 = context.Employees.FirstOrDefault(e => e.Email == staffEmail);
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

        if (!context.Users.Any(u => u.Email == adminEmail))
        {
             var emp2 = context.Employees.FirstOrDefault(e => e.Email == adminEmail);
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
                        AuthorId = adminUser.UserId,
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
                        AuthorId = adminUser.UserId,
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
