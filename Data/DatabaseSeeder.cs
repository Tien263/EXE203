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
        Console.WriteLine("--> [SEEDER] Starting SeedData process...");
        
        try
        {
            // AGGRESSIVE CLEANUP: Clear old data to ensure new paths are used
            if (context.Database.IsNpgsql())
            {
                try
                {
                    Console.WriteLine("--> [SEEDER] Attempting to TRUNCATE Products and Categories (Postgres)...");
                    // Try with double quotes (case-sensitive) first
                    context.Database.ExecuteSqlRaw("TRUNCATE TABLE \"Products\", \"Categories\" RESTART IDENTITY CASCADE;");
                    Console.WriteLine("--> [SEEDER] Successfully truncated Products and Categories.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> [SEEDER] TRUNCATE error (likely case sensitivity): {ex.Message}");
                    try
                    {
                        Console.WriteLine("--> [SEEDER] Retrying TRUNCATE with lowercase names...");
                        context.Database.ExecuteSqlRaw("TRUNCATE TABLE products, categories RESTART IDENTITY CASCADE;");
                        Console.WriteLine("--> [SEEDER] Successfully truncated products and categories (lowercase).");
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine($"--> [SEEDER] Final TRUNCATE attempt failed: {ex2.Message}");
                    }
                }
            }
            else if (context.Database.IsSqlServer())
            {
                Console.WriteLine("--> [SEEDER] Found SQL Server. Cleaning up via RemoveRange...");
                context.Products.RemoveRange(context.Products);
                context.Categories.RemoveRange(context.Categories);
                context.SaveChanges();
            }
            else if (context.Database.IsSqlite())
            {
                Console.WriteLine("--> [SEEDER] Found SQLite. Cleaning up via RemoveRange...");
                context.Products.RemoveRange(context.Products);
                context.Categories.RemoveRange(context.Categories);
                context.SaveChanges();
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
            context.SaveChanges();

            // 2. Seed Products
        Console.WriteLine("--> Seeding products...");
        var categories = context.Categories.ToList();
        var catDeo = categories.First(c => c.CategoryName.Contains("Sấy Dẻo", StringComparison.OrdinalIgnoreCase));
        var catGion = categories.First(c => c.CategoryName.Contains("Sấy Giòn", StringComparison.OrdinalIgnoreCase));
        var catThangHoa = categories.First(c => c.CategoryName.Contains("Thăng Hoa", StringComparison.OrdinalIgnoreCase));
        var catMini = categories.First(c => c.CategoryName.Contains("Mini Size", StringComparison.OrdinalIgnoreCase));

            // ADD PRODUCTS
            if (!context.Products.Any())
            {
                Console.WriteLine("--> [SEEDER] Seeding exactly 18 products...");
                
                var products = new List<Product>
                {
                    // 1. SÁº¤Y GIÃ’N (6 sáº£n pháº©m)
                    new Product { 
                        ProductName = "XoÃ i Sáº¥y Giá»n", 
                        ProductCode = "XOAI-GION-01",
                        Price = 55000, 
                        OriginalPrice = 65000,
                        StockQuantity = 100, 
                        Unit = "GÃ³i", 
                        Weight = "100g",
                        ImageUrl = "/images/products/xoai-say.jpg", // Corrected path
                        CategoryId = categories.First(c => c.CategoryName == "Sản Phẩm Sấy Giòn").CategoryId,
                        IsActive = true,
                        IsNew = true,
                        Rating = 5.0m,
                        Description = "XoÃ i sáº¥y giá»n thÆ¡m ngon, giá»¯ trá» n hÆ°Æ¡ng vá»‹ tá»± nhiÃªn."
                    },
                    new Product { 
                        ProductName = "MÃ­t Sáº¥y Giá»n", 
                        ProductCode = "MIT-GION-01",
                        Price = 45000, 
                        OriginalPrice = 50000,
                        StockQuantity = 100, 
                        Unit = "GÃ³i", 
                        Weight = "150g",
                        ImageUrl = "/images/products/mit-say.jpg", // Corrected path
                        CategoryId = categories.First(c => c.CategoryName == "Sản Phẩm Sấy Giòn").CategoryId,
                        IsActive = true,
                        IsNew = true,
                        Rating = 4.8m,
                        Description = "MÃ­t sáº¥y giá»n vÃ ng á»‘m, giÃ²n tan."
                    },
                    new Product { 
                        ProductName = "Chuá»‘i Sáº¥y Giá»n", 
                        ProductCode = "CHUOI-GION-01",
                        Price = 35000, 
                        OriginalPrice = 40000,
                        StockQuantity = 150, 
                        Unit = "GÃ³i", 
                        Weight = "200g",
                        ImageUrl = "/images/products/chuoi-say.jpg", // Corrected path
                        CategoryId = categories.First(c => c.CategoryName == "Sản Phẩm Sấy Giòn").CategoryId,
                        IsActive = true,
                        Rating = 4.7m,
                        Description = "Chuá»‘i sáº¥y giá»n truyá» n thá»‘ng."
                    },
                    new Product { 
                        ProductName = "Khoai Lang Sáº¥y Giá»n", 
                        ProductCode = "KHOAI-GION-01",
                        Price = 40000, 
                        OriginalPrice = 45000,
                        StockQuantity = 80, 
                        Unit = "GÃ³i", 
                        Weight = "150g",
                        ImageUrl = "/images/products/khoai-lang-say.jpg", // Corrected path
                        CategoryId = categories.First(c => c.CategoryName == "Sản Phẩm Sấy Giòn").CategoryId,
                        IsActive = true,
                        Rating = 4.6m,
                        Description = "Khoai lang sáº¥y giá»n tá»± nhiÃªn."
                    },
                    new Product { 
                        ProductName = "Tháº­p Cáº©m Sáº¥y Giá»n", 
                        ProductCode = "THAP-CAM-01",
                        Price = 60000, 
                        OriginalPrice = 70000,
                        StockQuantity = 200, 
                        Unit = "GÃ³i", 
                        Weight = "250g",
                        ImageUrl = "/images/products/thap-cam-say.jpg", // Corrected path
                        CategoryId = categories.First(c => c.CategoryName == "Sản Phẩm Sấy Giòn").CategoryId,
                        IsActive = true,
                        Rating = 4.9m,
                        Description = "Tháº­p cáº©m cÃ¡c loáº¡i cÅ© quáº£ sáº¥y giá»n."
                    },
                    new Product { 
                        ProductName = "Tháº­p Cáº©m Sáº¥y Giá»n Mini", 
                        ProductCode = "THAP-CAM-MINI",
                        Price = 25000, 
                        OriginalPrice = 30000,
                        StockQuantity = 300, 
                        Unit = "GÃ³i", 
                        Weight = "100g",
                        ImageUrl = "/images/products/thap-cam-say.jpg", // Corrected path
                        CategoryId = categories.First(c => c.CategoryName == "Sản Phẩm Sấy Giòn").CategoryId,
                        IsActive = true,
                        Rating = 4.8m,
                        Description = "TÃºi nhá»  tiá»‡n lá»£i."
                    },

                    // 2. Sáº¤Y THÄ‚NG HOA (6 sáº£n pháº©m)
                    new Product { 
                        ProductName = "DÃ¢u Sáº¥y ThÄƒng Hoa", 
                        ProductCode = "DAU-TH-01",
                        Price = 120000, 
                        OriginalPrice = 140000,
                        StockQuantity = 50, 
                        Unit = "Há»™p", 
                        Weight = "50g",
                        ImageUrl = "/images/products/dau-say.jpg", // Corrected path
                        CategoryId = categories.First(c => c.CategoryName == "Sản Phẩm Sấy Thăng Hoa").CategoryId,
                        IsActive = true,
                        IsNew = true,
                        Rating = 5.0m,
                        Description = "DÃ¢u tÃ¢y sáº¥y thÄƒng hoa cao cáº¥p."
                    },
                    new Product { 
                        ProductName = "Sá»¯a Chua Sáº¥y ThÄƒng Hoa", 
                        ProductCode = "SUA-CHUA-TH-01",
                        Price = 85000, 
                        OriginalPrice = 95000,
                        StockQuantity = 70, 
                        Unit = "GÃ³i", 
                        Weight = "45g",
                        ImageUrl = "/images/products/sua-chua-say.jpg", // Corrected path
                        CategoryId = categories.First(c => c.CategoryName == "Sản Phẩm Sấy Thăng Hoa").CategoryId,
                        IsActive = true,
                        IsNew = true,
                        Rating = 4.9m,
                        Description = "ViÃªn sá»¯a chua sáº¥y giÃ²n tan, bá»• dÆ°á»¡ng."
                    },
                    new Product { 
                        ProductName = "Na Sáº¥y ThÄƒng Hoa", 
                        ProductCode = "NA-TH-01",
                        Price = 150000, 
                        OriginalPrice = 170000,
                        StockQuantity = 30, 
                        Unit = "Há»™p", 
                        Weight = "50g",
                        ImageUrl = "/images/products/na-say.jpg", // Corrected path
                        CategoryId = categories.First(c => c.CategoryName == "Sản Phẩm Sấy Thăng Hoa").CategoryId,
                        IsActive = true,
                        Rating = 4.9m,
                        Description = "Na sáº¥y thÄƒ hoa giá»¯ nguyÃªn cáº¥u trÃºc vÃ  dÆ°á»¡ng cháº¥t."
                    },
                    new Product { 
                        ProductName = "Sáº§u RiÃªng Sáº¥y ThÄƒng Hoa", 
                        ProductCode = "SAU-RIENG-TH-01",
                        Price = 180000, 
                        OriginalPrice = 200000,
                        StockQuantity = 40, 
                        Unit = "Há»™p", 
                        Weight = "80g",
                        ImageUrl = "/images/products/sau-rieng-say.jpg", // Corrected path
                        CategoryId = categories.First(c => c.CategoryName == "Sản Phẩm Sấy Thăng Hoa").CategoryId,
                        IsActive = true,
                        Rating = 5.0m,
                        Description = "Sáº§u riÃªng sáº¥y thÄƒng hoa thÆ¡m ná»©c."
                    },
                    new Product { 
                        ProductName = "NhÃ£n Sáº¥y ThÄƒng Hoa", 
                        ProductCode = "NHAN-TH-01",
                        Price = 110000, 
                        OriginalPrice = 125000,
                        StockQuantity = 60, 
                        Unit = "GÃ³i", 
                        Weight = "100g",
                        ImageUrl = "/images/products/nhan-say.jpg", // Corrected path
                        CategoryId = categories.First(c => c.CategoryName == "Sản Phẩm Sấy Thăng Hoa").CategoryId,
                        IsActive = true,
                        Rating = 4.7m,
                        Description = "CÆ¡i nhÃ£n sáº¥y thÄƒng hoa ngá» t thanh."
                    },
                    new Product { 
                        ProductName = "Cam Sáº¥y ThÄƒng Hoa", 
                        ProductCode = "CAM-TH-01",
                        Price = 75000, 
                        OriginalPrice = 85000,
                        StockQuantity = 90, 
                        Unit = "GÃ³i", 
                        Weight = "100g",
                        ImageUrl = "/images/products/cam-say.jpg", // Corrected path
                        CategoryId = categories.First(c => c.CategoryName == "Sản Phẩm Sấy Thăng Hoa").CategoryId,
                        IsActive = true,
                        Rating = 4.6m,
                        Description = "LÃ¡t cam sáº¥y thÄƒng hoa dÃ¹ng pha trÃ  hoáº·c Äƒn trá»±c tiáº¿p."
                    },

                    // 3. Sáº¤Y DáºmultiO (6 sáº£n pháº©m)
                    new Product { 
                        ProductName = "XoÃ i Sáº¥y Dáº»o", 
                        ProductCode = "XOAI-DEO-01",
                        Price = 65000, 
                        OriginalPrice = 75000,
                        StockQuantity = 120, 
                        Unit = "GÃ³i", 
                        Weight = "200g",
                        ImageUrl = "/images/products/xoai-say.jpg", // Corrected path
                        CategoryId = categories.First(c => c.CategoryName == "Sản Phẩm Sấy Dẻo").CategoryId,
                        IsActive = true,
                        Rating = 4.9m,
                        Description = "XoÃ i sáº¥y dáº»o chua ngá» t, dai ngon."
                    },
                    new Product { 
                        ProductName = "Máº­n Sáº¥y Dáº»o", 
                        ProductCode = "MAN-DEO-01",
                        Price = 65000, 
                        OriginalPrice = 75000,
                        StockQuantity = 100, 
                        Unit = "GÃ³i", 
                        Weight = "200g",
                        ImageUrl = "/images/products/man-say.jpg", // Corrected path
                        CategoryId = categories.First(c => c.CategoryName == "Sản Phẩm Sấy Dẻo").CategoryId,
                        IsActive = true,
                        Rating = 5.0m,
                        Description = "Máº­n sáº¥y dáº»o khÃ´ng háº¡t."
                    },
                    new Product { 
                        ProductName = "Ä Ã o Sáº¥y Dáº»o", 
                        ProductCode = "DAO-DEO-01",
                        Price = 70000, 
                        OriginalPrice = 80000,
                        StockQuantity = 80, 
                        Unit = "GÃ³i", 
                        Weight = "150g",
                        ImageUrl = "/images/products/dao-say.jpg", // Corrected path
                        CategoryId = categories.First(c => c.CategoryName == "Sản Phẩm Sấy Dẻo").CategoryId,
                        IsActive = true,
                        Rating = 4.8m,
                        Description = "Ä Ã o sáº¥y dáº»o thÆ¡m lÃ«ng."
                    },
                    new Product { 
                        ProductName = "DÆ°á»£u Sáº¥y Dáº»o", 
                        ProductCode = "DAU-DEO-01",
                        Price = 90000, 
                        OriginalPrice = 110000,
                        StockQuantity = 60, 
                        Unit = "GÃ³i", 
                        Weight = "100g",
                        ImageUrl = "/images/products/dau-say.jpg", // Corrected path
                        CategoryId = categories.First(c => c.CategoryName == "Sản Phẩm Sấy Dẻo").CategoryId,
                        IsActive = true,
                        Rating = 4.9m,
                        Description = "DÃ¢u tÃ¢y sáº¥y dáº»o nguyÃªn trÃ¡i."
                    },
                    new Product { 
                        ProductName = "Há»“ng Sáº¥y Dáº»o", 
                        ProductCode = "HONG-DEO-01",
                        Price = 130000, 
                        OriginalPrice = 150000,
                        StockQuantity = 50, 
                        Unit = "Há»™p", 
                        Weight = "250g",
                        ImageUrl = "/images/products/hong-say.jpg", // Corrected path
                        CategoryId = categories.First(c => c.CategoryName == "Sản Phẩm Sấy Dẻo").CategoryId,
                        IsActive = true,
                        Rating = 5.0m,
                        Description = "Há»“ng treo giÃ³ sáº¥y dáº»o Ä Ã  Láº¡t."
                    },
                    new Product { 
                        ProductName = "MÃ­t Sáº¥y Dáº»o", 
                        ProductCode = "MIT-DEO-01",
                        Price = 75000, 
                        OriginalPrice = 85000,
                        StockQuantity = 90, 
                        Unit = "GÃ³i", 
                        Weight = "150g",
                        ImageUrl = "/images/products/mit-say.jpg", // Corrected path
                        CategoryId = categories.First(c => c.CategoryName == "Sản Phẩm Sấy Dẻo").CategoryId,
                        IsActive = true,
                        Rating = 4.7m,
                        Description = "MÃ­t sáº¥y dáº»o ngá» t lán."
                    }
                };

                foreach (var p in products)
                {
                    Console.WriteLine($"--> [SEEDER] Adding: {p.ProductName} | Path: {p.ImageUrl}");
                }
                
                context.Products.AddRange(products);
                context.SaveChanges();
                Console.WriteLine("--> [SEEDER] All 18 products saved successfully.");
            }
            else
            {
                Console.WriteLine($"--> [SEEDER] Products already exist ({context.Products.Count()}). Skipping product seed.");
            }
        
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
        Console.WriteLine("✅ Database seeded successfully with products, employees, users and 5 blog posts!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during seeding: {ex.Message}");
            throw;
        }
    }
}
