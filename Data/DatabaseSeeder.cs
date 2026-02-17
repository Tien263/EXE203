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
        // Check if products already exist
        if (context.Products.Any() || context.Users.Any())
        {
            Console.WriteLine("Database already seeded");
            return;
        }

        Console.WriteLine("Seeding database...");

        try
        {
            // Seed Categories
            var categories = new List<Category>
            {
                new Category { CategoryName = "Hoa Quáº£ Sáº¥y", Description = "CÃ¡c loáº¡i hoa quáº£ sáº¥y khÃ´ tá»± nhiÃªn" },
                new Category { CategoryName = "Hoa Quáº£ Sáº¥y Dáº»o", Description = "Hoa quáº£ sáº¥y giá»¯ Ä‘á»™ má»m tá»± nhiÃªn" },
                new Category { CategoryName = "Hoa Quáº£ Sáº¥y ThÄƒng Hoa", Description = "Hoa quáº£ sáº¥y cÃ´ng nghá»‡ thÄƒng hoa" },
                new Category { CategoryName = "Combo QuÃ  Táº·ng", Description = "Combo hoa quáº£ sáº¥y lÃ m quÃ " }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();

            // Seed Products
            var products = new List<Product>
            {
                new Product
                {
                    ProductCode = "MIT001",
                    ProductName = "MÃ­t Sáº¥y GiÃ²n",
                    CategoryId = 1,
                    Price = 150000,
                    StockQuantity = 100,
                    Description = "MÃ­t sáº¥y giÃ²n tá»± nhiÃªn, khÃ´ng cháº¥t báº£o quáº£n",
                    ImageUrl = "/images/products/mit-say.jpg",
                    IsActive = true
                },
                new Product
                {
                    ProductCode = "CHUOI001",
                    ProductName = "Chuá»‘i Sáº¥y Dáº»o",
                    CategoryId = 2,
                    Price = 120000,
                    StockQuantity = 150,
                    Description = "Chuá»‘i sáº¥y dáº»o thÆ¡m ngon, giá»¯ nguyÃªn vá»‹ tá»± nhiÃªn",
                    ImageUrl = "/images/products/chuoi-say.jpg",
                    IsActive = true
                },
                new Product
                {
                    ProductCode = "XOAI001",
                    ProductName = "XoÃ i Sáº¥y Dáº»o",
                    CategoryId = 2,
                    Price = 180000,
                    StockQuantity = 80,
                    Description = "XoÃ i sáº¥y dáº»o chua ngá»t Ä‘áº­m Ä‘Ã ",
                    ImageUrl = "/images/products/xoai-say.jpg",
                    IsActive = true
                },
                new Product
                {
                    ProductCode = "DAU001",
                    ProductName = "DÃ¢u TÃ¢y Sáº¥y ThÄƒng Hoa",
                    CategoryId = 3,
                    Price = 250000,
                    StockQuantity = 50,
                    Description = "DÃ¢u tÃ¢y sáº¥y thÄƒng hoa giá»¯ nguyÃªn hÆ°Æ¡ng vá»‹",
                    ImageUrl = "/images/products/dau-say.jpg",
                    IsActive = true
                },
                new Product
                {
                    ProductCode = "COMBO001",
                    ProductName = "Combo Hoa Quáº£ Sáº¥y 5 Loáº¡i",
                    CategoryId = 4,
                    Price = 350000,
                    StockQuantity = 30,
                    Description = "Combo 5 loáº¡i hoa quáº£ sáº¥y Ä‘a dáº¡ng",
                    ImageUrl = "/images/products/combo-5.jpg",
                    IsActive = true
                }
            };
            context.Products.AddRange(products);
            context.SaveChanges();

            // Seed Employees
            var emp1 = new Employee
            {
                EmployeeCode = "NV001",
                FullName = "Nguyá»…n VÄƒn A",
                PhoneNumber = "0901234567",
                Email = "staff@mocvistore.com",
                Position = "NhÃ¢n viÃªn bÃ¡n hÃ ng",
                Department = "BÃ¡n hÃ ng",
                Salary = 8000000,
                HireDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-2)),
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            var emp2 = new Employee
            {
                EmployeeCode = "ADMIN001",
                FullName = "Quáº£n Trá»‹ ViÃªn",
                PhoneNumber = "0912345678",
                Email = "admin@mocvistore.com",
                Position = "Quáº£n lÃ½",
                Department = "Quáº£n lÃ½",
                Salary = 15000000,
                HireDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-3)),
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            context.Employees.Add(emp1);
            context.Employees.Add(emp2);
            context.SaveChanges();

            // Seed Users with correct passwords from THONG_TIN_TAI_KHOAN.md
            var user1 = new User
            {
                Email = "staff@mocvistore.com",
                PasswordHash = HashPassword("Staff@123"),
                FullName = "Nguyá»…n VÄƒn A",
                PhoneNumber = "0901234567",
                Role = "Staff",
                EmployeeId = emp1.EmployeeId,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            var user2 = new User
            {
                Email = "admin@mocvistore.com",
                PasswordHash = HashPassword("Admin@123"),
                FullName = "Quáº£n Trá»‹ ViÃªn",
                PhoneNumber = "0912345678",
                Role = "Admin",
                EmployeeId = emp2.EmployeeId,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            context.Users.Add(user1);
            context.Users.Add(user2);
            context.SaveChanges();

            // Seed Blog Posts from AI training data
            var blogs = new List<Blog>
            {
                new Blog
                {
                    Title = "ðŸ“ DÃ¢u TÃ¢y Má»™c ChÃ¢u - Ná»¯ HoÃ ng Hoa Quáº£ Cao NguyÃªn",
                    Slug = "dau-tay-moc-chau-nu-hoang-hoa-qua-cao-nguyen",
                    ShortDescription = "KhÃ¡m phÃ¡ dÃ¢u tÃ¢y Má»™c ChÃ¢u Ä‘Æ°á»£c má»‡nh danh lÃ  'Ná»¯ hoÃ ng hoa quáº£ cao nguyÃªn' vá»›i vitamin C gáº¥p 3 láº§n cam!",
                    Content = @"<h2>Ná»¯ HoÃ ng Hoa Quáº£ Cao NguyÃªn</h2>
<p>DÃ¢u tÃ¢y Má»™c ChÃ¢u Ä‘Æ°á»£c má»‡nh danh lÃ  'Ná»¯ hoÃ ng hoa quáº£ cao nguyÃªn'! Má»—i trÃ¡i dÃ¢u Ä‘Æ°á»£c chá»n lá»c ká»¹ cÃ ng tá»« vÆ°á»n dÃ¢u Má»™c ChÃ¢u 1200m so vá»›i máº·t nÆ°á»›c biá»ƒn, nÆ¡i cÃ³ khÃ­ háº­u mÃ¡t máº» quanh nÄƒm.</p>

<h3>CÃ´ng Nghá»‡ Sáº¥y ThÃ´ng Minh</h3>
<p>Sáº¥y dáº»o á»Ÿ nhiá»‡t Ä‘á»™ tháº¥p 50-60Â°C, giá»¯ trá»n 95% vitamin C - gáº¥p 3 láº§n cam! MÃ u Ä‘á» tÆ°Æ¡i rá»±c rá»¡ 100% tá»± nhiÃªn, khÃ´ng má»™t giá»t mÃ u nhÃ¢n táº¡o.</p>

<h3>Dinh DÆ°á»¡ng Tuyá»‡t Vá»i</h3>
<ul>
<li><strong>Vitamin C:</strong> SiÃªu cao (180mg/100g) - Gáº¥p 3 láº§n cam, Ä‘Ã¡p á»©ng 200% nhu cáº§u hÃ ng ngÃ y</li>
<li><strong>Anthocyanin:</strong> Cháº¥t chá»‘ng oxy hÃ³a máº¡nh tá»« mÃ u Ä‘á» tá»± nhiÃªn - Báº£o vá»‡ tim máº¡ch</li>
<li><strong>Folate (Vitamin B9):</strong> Cao - Tá»‘t cho phá»¥ ná»¯ mang thai vÃ  nÃ£o bá»™</li>
<li><strong>Cháº¥t xÆ¡:</strong> 3.5g/100g - GiÃºp no lÃ¢u, há»— trá»£ giáº£m cÃ¢n hiá»‡u quáº£</li>
</ul>

<h3>Lá»£i Ãch Sá»©c Khá»e</h3>
<p><strong>ðŸ’ª TÄƒng CÆ°á»ng Miá»…n Dá»‹ch VÆ°á»£t Trá»™i</strong> - Vitamin C siÃªu cao giÃºp cÆ¡ thá»ƒ chá»‘ng láº¡i virus, cáº£m cÃºm</p>
<p><strong>âœ¨ LÃ m Äáº¹p Da Tá»« BÃªn Trong</strong> - Chá»‘ng oxy hÃ³a máº¡nh, giáº£m nÃ¡m, sáº¡m, da sÃ¡ng má»‹n tá»± nhiÃªn</p>
<p><strong>â¤ï¸ Báº£o Vá»‡ Tim Máº¡ch</strong> - Anthocyanin giáº£m cholesterol xáº¥u, ngÄƒn ngá»«a Ä‘á»™t quá»µ</p>

<h3>CÃ¡ch DÃ¹ng DÃ¢u TÃ¢y Sáº¥y</h3>
<ul>
<li>ðŸµ Ä‚n váº·t trá»±c tiáº¿p - Thay tháº¿ káº¹o, bÃ¡nh khÃ´ng lÃ nh máº¡nh</li>
<li>ðŸ¥¤ Pha trÃ  dÃ¢u detox - NgÃ¢m vá»›i nÆ°á»›c áº¥m, thÃªm máº­t ong</li>
<li>ðŸ¨ Topping yogurt/kem - Trang trÃ­ Ä‘áº¹p máº¯t, tÄƒng dinh dÆ°á»¡ng</li>
<li>ðŸŽ‚ LÃ m bÃ¡nh, trang trÃ­ mÃ³n Äƒn - MÃ u Ä‘á» tá»± nhiÃªn báº¯t máº¯t</li>
</ul>",
                    AuthorId = user2.Id,
                    IsPublished = true,
                    PublishedDate = DateTime.Now.AddDays(-5),
                    CreatedDate = DateTime.Now
                },
                new Blog
                {
                    Title = "ðŸŒŸ CÃ´ng Nghá»‡ Freeze-Dried - DÃ¢u Sáº¥y ThÄƒng Hoa Cao Cáº¥p",
                    Slug = "cong-nghe-freeze-dried-dau-say-thang-hoa-cao-cap",
                    ShortDescription = "KhÃ¡m phÃ¡ cÃ´ng nghá»‡ freeze-dried hiá»‡n Ä‘áº¡i tá»« Nháº­t Báº£n giá»¯ 98% dinh dÆ°á»¡ng vÃ  táº¡o káº¿t cáº¥u giÃ²n xá»‘p ká»³ diá»‡u!",
                    Content = @"<h2>Äá»‰nh Cao CÃ´ng Nghá»‡ - DÃ¢u Sáº¥y ThÄƒng Hoa</h2>
<p>Báº¡n Ä‘Ã£ bao giá» thá»­ dÃ¢u tÃ¢y 'tan nhÆ° tuyáº¿t' trong miá»‡ng chÆ°a? ÄÃ¢y lÃ  sáº£n pháº©m CAO Cáº¤P NHáº¤T cá»§a Má»™c Vá»‹!</p>

<h3>CÃ´ng Nghá»‡ Freeze-Dried Nháº­t Báº£n</h3>
<p>Sá»­ dá»¥ng cÃ´ng nghá»‡ Freeze-Dried (sáº¥y Ä‘Ã´ng khÃ´) hiá»‡n Ä‘áº¡i tá»« Nháº­t Báº£n, sáº¥y á»Ÿ nhiá»‡t Ä‘á»™ Ã¢m sÃ¢u -40Â°C, giá»¯ trá»n 98% dinh dÆ°á»¡ng vÃ  mÃ u sáº¯c tá»± nhiÃªn.</p>

<h3>Äáº·c Äiá»ƒm Ná»•i Báº­t</h3>
<ul>
<li><strong>Káº¿t Cáº¥u GiÃ²n Xá»‘p Ká»³ Diá»‡u:</strong> Tan ngay khi cháº¡m lÆ°á»¡i, tráº£i nghiá»‡m hoÃ n toÃ n má»›i</li>
<li><strong>HÆ°Æ¡ng Vá»‹ Äáº­m ÄÃ :</strong> Gáº¥p 10 láº§n dÃ¢u tÆ°Æ¡i, cÃ´ Ä‘áº·c tinh tÃºy Má»™c ChÃ¢u</li>
<li><strong>MÃ u Sáº¯c Tá»± NhiÃªn:</strong> Äá» rá»±c rá»¡ nhÆ° vá»«a má»›i hÃ¡i</li>
<li><strong>KhÃ´ng ThÃªm Cháº¥t Láº¡:</strong> KhÃ´ng Ä‘Æ°á»ng, khÃ´ng dáº§u má»¡, khÃ´ng cháº¥t báº£o quáº£n</li>
</ul>

<h3>Dinh DÆ°á»¡ng SiÃªu CÃ´ Äáº·c</h3>
<ul>
<li><strong>Vitamin C:</strong> 300mg/100g - Gáº¥p 5 láº§n dÃ¢u tÆ°Æ¡i, gáº¥p 5 láº§n cam</li>
<li><strong>Anthocyanin:</strong> CÃ´ Ä‘áº·c gáº¥p 8 láº§n - Chá»‘ng oxy hÃ³a máº¡nh nháº¥t</li>
<li><strong>Folate:</strong> Cao gáº¥p 6 láº§n - Tá»‘t cho thai nhi vÃ  nÃ£o bá»™</li>
<li><strong>Kali:</strong> Äiá»u hÃ²a huyáº¿t Ã¡p hiá»‡u quáº£</li>
</ul>

<h3>Ai NÃªn Thá»­ DÃ¢u Sáº¥y ThÄƒng Hoa?</h3>
<p>ðŸ‘‘ NgÆ°á»i thÃ nh Ä‘áº¡t, yÃªu cháº¥t lÆ°á»£ng</p>
<p>ðŸ‹ï¸ Gymer, váº­n Ä‘á»™ng viÃªn</p>
<p>ðŸŽ QuÃ  táº·ng cao cáº¥p dá»‹p lá»…, Táº¿t</p>
<p>ðŸ‘¨â€ðŸ’¼ Doanh nhÃ¢n, CEO</p>",
                    AuthorId = user2.Id,
                    IsPublished = true,
                    PublishedDate = DateTime.Now.AddDays(-3),
                    CreatedDate = DateTime.Now
                },
                new Blog
                {
                    Title = "ðŸ‘ Máº­n Má»™c ChÃ¢u - Vá»‹ Chua Ngá»t Äá»‰nh Cao & Cháº¥t XÆ¡ Cao Nháº¥t",
                    Slug = "man-moc-chau-vi-chua-ngot-dinh-cao-chat-xo-cao-nhat",
                    ShortDescription = "Máº­n háº­u Má»™c ChÃ¢u vá»›i vá»‹ chua ngá»t cÃ¢n báº±ng hoÃ n háº£o, cháº¥t xÆ¡ cao nháº¥t trong cÃ¡c loáº¡i sáº¥y!",
                    Content = @"<h2>Máº­n Háº­u Má»™c ChÃ¢u - KÃ½ á»¨c Ngá»t NgÃ o</h2>
<p>Báº¡n nhá»› vá»‹ máº­n háº­u Má»™c ChÃ¢u thuá»Ÿ nhá» chá»©? Giá» Ä‘Ã¢y, Má»™c Vá»‹ Ä‘Ã£ 'Ä‘Ã³ng gÃ³i' cáº£ kÃ½ á»©c áº¥y vÃ o tá»«ng miáº¿ng máº­n sáº¥y dáº»o!</p>

<h3>Vá»‹ Chua Ngá»t KÃ­ch ThÃ­ch Vá»‹ GiÃ¡c</h3>
<p>Máº­n háº­u thu hoáº¡ch thÃ¡ng 4-6, chá»n trÃ¡i chÃ­n vá»«a tá»›i, mÃ u tÃ­m Ä‘en tá»± nhiÃªn. Sáº¥y dáº»o giá»¯ nguyÃªn vá»‹ chua thanh kÃ­ch thÃ­ch vá»‹ giÃ¡c, ngá»t mÃ¡t háº­u vá»‹. Má»m dáº»o dai dai, thÆ¡m mÃ¹i máº­n chÃ­n.</p>

<h3>Dinh DÆ°á»¡ng Tuyá»‡t Vá»i</h3>
<ul>
<li><strong>Vitamin C:</strong> Ráº¥t cao (85mg/100g) - TÄƒng cÆ°á»ng miá»…n dá»‹ch máº¡nh máº½</li>
<li><strong>Vitamin A:</strong> Tá»‘t cho máº¯t, da sÃ¡ng khá»e</li>
<li><strong>Anthocyanin:</strong> Cháº¥t chá»‘ng oxy hÃ³a tá»« mÃ u tÃ­m tá»± nhiÃªn</li>
<li><strong>Kali:</strong> Äiá»u hÃ²a huyáº¿t Ã¡p, tá»‘t cho tim</li>
<li><strong>Cháº¥t XÆ¡:</strong> 4.2g/100g - CAO NHáº¤T trong cÃ¡c loáº¡i sáº¥y, há»— trá»£ tiÃªu hÃ³a cá»±c tá»‘t</li>
</ul>

<h3>Lá»£i Ãch Sá»©c Khá»e</h3>
<p><strong>ðŸ’ª TÄƒng CÆ°á»ng Miá»…n Dá»‹ch</strong> - Vitamin C cao, phÃ²ng chá»‘ng cáº£m cÃºm</p>
<p><strong>ðŸš½ Há»— Trá»£ TiÃªu HÃ³a Máº¡nh Máº½</strong> - Cháº¥t xÆ¡ cao, chá»‘ng tÃ¡o bÃ³n hiá»‡u quáº£</p>
<p><strong>âœ¨ LÃ m Äáº¹p Da Tá»± NhiÃªn</strong> - Chá»‘ng oxy hÃ³a, giáº£m má»¥n, da sÃ¡ng</p>
<p><strong>âš–ï¸ Há»— Trá»£ Giáº£m CÃ¢n</strong> - Ãt calo (220 kcal/100g), no lÃ¢u</p>

<h3>Gá»£i Ã Sá»­ Dá»¥ng</h3>
<ul>
<li>ðŸ¬ Ä‚n váº·t trá»±c tiáº¿p - Giáº£i khÃ¡t, giáº£i ngÃ¡n tuyá»‡t vá»i</li>
<li>ðŸµ NgÃ¢m trÃ  máº­n - ThÃªm Ä‘Æ°á»ng phÃ¨n, uá»‘ng mÃ¡t láº¡nh</li>
<li>ðŸ² Náº¥u chÃ¨ máº­n - MÃ³n trÃ¡ng miá»‡ng truyá»n thá»‘ng</li>
<li>ðŸ° LÃ m má»©t, nhÃ¢n bÃ¡nh - Vá»‹ chua ngá»t Ä‘á»™c Ä‘Ã¡o</li>
</ul>",
                    AuthorId = user2.Id,
                    IsPublished = true,
                    PublishedDate = DateTime.Now.AddDays(-2),
                    CreatedDate = DateTime.Now
                },
                new Blog
                {
                    Title = "ðŸ¥­ XoÃ i Má»™c ChÃ¢u - Ngá»t NgÃ o, ThÆ¡m Ná»©c & GiÃ u Vitamin A",
                    Slug = "xoai-moc-chau-ngot-ngao-thom-nuc-giau-vitamin-a",
                    ShortDescription = "XoÃ i Má»™c ChÃ¢u sáº¥y dáº»o giá»¯ nguyÃªn mÃ u vÃ ng tá»± nhiÃªn, ngá»t thanh vÃ  Ä‘áº·c biá»‡t tá»‘t cho máº¯t!",
                    Content = @"<h2>XoÃ i Má»™c ChÃ¢u - HÆ°Æ¡ng Vá»‹ Nhiá»‡t Äá»›i</h2>
<p>XoÃ i Má»™c ChÃ¢u cÃ³ vá»‹ ngá»t Ä‘áº­m Ä‘Ã , thÆ¡m ná»©c mÃ¹i xoÃ i chÃ­n. Sáº¥y dáº»o giá»¯ nguyÃªn mÃ u vÃ ng tá»± nhiÃªn, má»m dai, khÃ´ng khÃ´ cá»©ng.</p>

<h3>Äáº·c Äiá»ƒm Ná»•i Báº­t</h3>
<ul>
<li><strong>MÃ u VÃ ng Tá»± NhiÃªn:</strong> KhÃ´ng táº©m Ä‘Æ°á»ng, khÃ´ng cháº¥t báº£o quáº£n</li>
<li><strong>Vá»‹ Ngá»t Äáº­m ÄÃ :</strong> ThÆ¡m ná»©c, ngon cá»±c ká»³</li>
<li><strong>Káº¿t Cáº¥u Má»m Dai:</strong> KhÃ´ng khÃ´ cá»©ng, dá»… cháº¿u</li>
</ul>

<h3>GiÃ u Vitamin A Cho Máº¯t Khá»e</h3>
<ul>
<li><strong>Vitamin A:</strong> Ráº¥t cao - Tá»‘t cho máº¯t, da</li>
<li><strong>Vitamin C:</strong> Cao - TÄƒng miá»…n dá»‹ch</li>
<li><strong>Beta-Carotene:</strong> Chá»‘ng oxy hÃ³a máº¡nh</li>
<li><strong>Cháº¥t XÆ¡:</strong> Há»— trá»£ tiÃªu hÃ³a tá»‘t</li>
</ul>

<h3>Lá»£i Ãch Sá»©c Khá»e</h3>
<p><strong>ðŸ‘ï¸ Tá»‘t Cho Máº¯t:</strong> Vitamin A cao, báº£o vá»‡ thá»‹ lá»±c</p>
<p><strong>âœ¨ LÃ m Äáº¹p Da:</strong> Beta-carotene giÃºp da sÃ¡ng khá»e</p>
<p><strong>ðŸ’ª TÄƒng Miá»…n Dá»‹ch:</strong> Vitamin C phÃ²ng bá»‡nh</p>
<p><strong>ðŸš½ Há»— Trá»£ TiÃªu HÃ³a:</strong> Cháº¥t xÆ¡ cao</p>

<h3>PhÃ¹ Há»£p Cho Ai?</h3>
<p>ðŸ‘¶ Tráº» em - Vitamin A giÃºp phÃ¡t triá»ƒn máº¯t</p>
<p>ðŸ‘¨â€ðŸ‘©â€ðŸ‘§â€ðŸ‘¦ Gia Ä‘Ã¬nh - An toÃ n cho cáº£ nhÃ </p>
<p>ðŸ’¼ DÃ¢n vÄƒn phÃ²ng - Snack lÃ nh máº¡nh</p>
<p>ðŸŽ QuÃ  táº·ng - GiÃ¡ há»£p lÃ½</p>",
                    AuthorId = user2.Id,
                    IsPublished = true,
                    PublishedDate = DateTime.Now.AddDays(-1),
                    CreatedDate = DateTime.Now
                },
                new Blog
                {
                    Title = "ðŸŒ¿ CÃ¢u Chuyá»‡n Má»™c Vá»‹ - Tá»« NÃ´ng DÃ¢n TÃ¢y Báº¯c Äáº¿n ThÆ°Æ¡ng Hiá»‡u Cao Cáº¥p",
                    Slug = "cau-chuyen-moc-vi-tu-nong-dan-tay-bac-den-thuong-hieu-cao-cap",
                    ShortDescription = "KhÃ¡m phÃ¡ hÃ nh trÃ¬nh cá»§a Má»™c Vá»‹ - thÆ°Æ¡ng hiá»‡u hoa quáº£ sáº¥y cao cáº¥p tá»« Má»™c ChÃ¢u, SÆ¡n La!",
                    Content = @"<h2>Má»™c Vá»‹ - HÆ°Æ¡ng Vá»‹ NguyÃªn SÆ¡ Má»™c ChÃ¢u</h2>
<p>Má»™c Vá»‹ lÃ  thÆ°Æ¡ng hiá»‡u hoa quáº£ sáº¥y cao cáº¥p tá»« Má»™c ChÃ¢u, SÆ¡n La. 'Má»™c' gá»£i Ä‘áº¿n sá»± má»™c máº¡c, tá»± nhiÃªn, nguyÃªn báº£n, gáº¯n vá»›i hÃ¬nh áº£nh nÃºi rá»«ng TÃ¢y Báº¯c. 'Vá»‹' lÃ  hÆ°Æ¡ng vá»‹, tráº£i nghiá»‡m khi thÆ°á»Ÿng thá»©c. Má»™c Vá»‹ = HÆ°Æ¡ng vá»‹ Má»™c ChÃ¢u, nháº¥n máº¡nh sá»± nguyÃªn báº£n, chÃ¢n tháº­t tá»« thiÃªn nhiÃªn.</p>

<h3>CÃ¢u Chuyá»‡n Ra Äá»i</h3>
<p>á»ž Má»™c ChÃ¢u, má»—i mÃ¹a quáº£ chÃ­n mang trong mÃ¬nh náº¯ng, giÃ³ vÃ  Ä‘áº¥t lÃ nh, nhÆ°ng hÆ°Æ¡ng vá»‹ áº¥y thÆ°á»ng khÃ³ giá»¯ trá»n váº¹n. Má»™c Vá»‹ ra Ä‘á»i Ä‘á»ƒ nÃ­u láº¡i khoáº£nh kháº¯c áº¥y â€“ giá»¯ nguyÃªn mÃ u sáº¯c, hÆ°Æ¡ng thÆ¡m vÃ  báº£n sáº¯c cá»§a cao nguyÃªn trong tá»«ng lÃ¡t hoa quáº£ sáº¥y. KhÃ´ng chá»‰ lÃ  mÃ³n Äƒn, Má»™c Vá»‹ cÃ²n lÃ  cÃ¢u chuyá»‡n vá» bÃ n tay ngÆ°á»i nÃ´ng dÃ¢n, vá» sá»± nÃ¢ng niu trong cháº¿ biáº¿n vÃ  khÃ¡t vá»ng Ä‘Æ°a nÃ´ng sáº£n Viá»‡t vÆ°Æ¡n xa.</p>

<h3>GiÃ¡ Trá»‹ Cá»‘t LÃµi</h3>
<p><strong>ðŸŒ± Giá»¯ Trá»n Tá»± NhiÃªn</strong><br/>HÆ°Æ¡ng vá»‹ ngon nháº¥t Ä‘áº¿n tá»« sá»± nguyÃªn báº£n. Tá»«ng lÃ¡t hoa quáº£ sáº¥y Ä‘á»u Ä‘Æ°á»£c lÃ m ra tá»« trÃ¡i chÃ­n Má»™c ChÃ¢u, giá»¯ nguyÃªn mÃ u sáº¯c, hÆ°Æ¡ng vá»‹ vÃ  dÆ°á»¡ng cháº¥t mÃ  thiÃªn nhiÃªn ban táº·ng.</p>

<p><strong>ðŸ”ï¸ TÃ´n Vinh Báº£n Sáº¯c</strong><br/>Má»—i sáº£n pháº©m khÃ´ng chá»‰ lÃ  mÃ³n Äƒn, mÃ  cÃ²n lÃ  cÃ¢u chuyá»‡n vá» nÃºi rá»«ng, vá» con ngÆ°á»i TÃ¢y Báº¯c cáº§n máº«n. ChÃºng tÃ´i muá»‘n Ä‘á»ƒ má»—i miáº¿ng hoa quáº£ sáº¥y Ä‘á»u mang hÆ¡i thá»Ÿ vÃ¹ng cao, gá»£i nhá»› Ä‘áº¿n báº£n sáº¯c Viá»‡t Nam.</p>

<p><strong>âœ… Cam Káº¿t Cháº¥t LÆ°á»£ng</strong><br/>Tá»« khÃ¢u chá»n nguyÃªn liá»‡u Ä‘áº¿n quy trÃ¬nh cháº¿ biáº¿n, chÃºng tÃ´i Ä‘áº·t sá»± minh báº¡ch vÃ  an toÃ n lÃªn hÃ ng Ä‘áº§u. Cháº¥t lÆ°á»£ng bá»n vá»¯ng chÃ­nh lÃ  cÃ¡ch chÃºng tÃ´i xÃ¢y dá»±ng niá»m tin lÃ¢u dÃ i vá»›i khÃ¡ch hÃ ng.</p>

<p><strong>ðŸš€ SÃ¡ng Táº¡o Äá»ƒ Lan Tá»a</strong><br/>ChÃºng tÃ´i káº¿t há»£p cÃ´ng nghá»‡ sáº¥y hiá»‡n Ä‘áº¡i vá»›i tinh hoa truyá»n thá»‘ng, Ä‘á»ƒ Ä‘áº·c sáº£n vÃ¹ng miá»n khÃ´ng chá»‰ Ä‘Æ°á»£c báº£o tá»“n mÃ  cÃ²n cÃ³ cÆ¡ há»™i Ä‘áº¿n gáº§n hÆ¡n vá»›i ngÆ°á»i tiÃªu dÃ¹ng kháº¯p cáº£ nÆ°á»›c.</p>

<h3>Sá»© Má»‡nh & Táº§m NhÃ¬n</h3>
<p><strong>Sá»© Má»‡nh:</strong> Lan tá»a báº£n sáº¯c Má»™c ChÃ¢u â€“ vÃ¹ng Ä‘áº¥t cá»§a nhá»¯ng mÃ¹a quáº£ ngá»t. Má»—i sáº£n pháº©m hoa quáº£ sáº¥y khÃ´ng chá»‰ giá»¯ trá»n váº¹n hÆ°Æ¡ng vá»‹ nguyÃªn sÆ¡ vÃ  dinh dÆ°á»¡ng thiÃªn nhiÃªn, mÃ  cÃ²n lÃ  nhá»‹p cáº§u káº¿t ná»‘i con ngÆ°á»i Viá»‡t vá»›i tÃ¬nh yÃªu quÃª hÆ°Æ¡ng, tá»± hÃ o vá» báº£n sáº¯c TÃ¢y Báº¯c.</p>

<p><strong>Táº§m NhÃ¬n:</strong> Trá»Ÿ thÃ nh thÆ°Æ¡ng hiá»‡u tiÃªn phong vá» hoa quáº£ sáº¥y vÃ¹ng miá»n, biá»ƒu tÆ°á»£ng cho sá»± káº¿t há»£p giá»¯a truyá»n thá»‘ng vÃ  cÃ´ng nghá»‡ hiá»‡n Ä‘áº¡i. Tá»« dÃ¢u sáº¥y Má»™c ChÃ¢u â€“ sáº£n pháº©m khÃ¡c biá»‡t Ä‘áº§u tiÃªn â€“ chÃºng tÃ´i khÃ¡t vá»ng trá»Ÿ thÃ nh 'Ä‘áº¡i sá»© nÃ´ng sáº£n Viá»‡t', gÃ³p pháº§n nÃ¢ng táº§m giÃ¡ trá»‹ nÃ´ng sáº£n vÃ  áº©m thá»±c Viá»‡t trÃªn thá»‹ trÆ°á»ng quá»‘c táº¿.</p>

<h3>Nhá»¯ng Äiá»ƒm KhÃ¡c Biá»‡t</h3>
<ul>
<li>âœ… TiÃªn phong Ä‘á»™c quyá»n â€“ ThÆ°Æ¡ng hiá»‡u Ä‘áº§u tiÃªn Ä‘Æ°a dÃ¢u tÃ¢y Má»™c ChÃ¢u sáº¥y ra thá»‹ trÆ°á»ng</li>
<li>âœ… Nguá»“n gá»‘c chuáº©n vÃ¹ng miá»n â€“ Trá»±c tiáº¿p tá»« cao nguyÃªn Má»™c ChÃ¢u 1200m</li>
<li>âœ… Bao bÃ¬ xanh bá»n vá»¯ng â€“ Giáº¥y phÃ¢n há»§y sinh há»c, báº£o vá»‡ mÃ´i trÆ°á»ng</li>
<li>âœ… CÃ´ng nghá»‡ sáº¥y hiá»‡n Ä‘áº¡i â€“ Giá»¯ 98% dinh dÆ°á»¡ng, khÃ´ng cháº¥t báº£o quáº£n</li>
<li>âœ… LiÃªn tá»¥c Ä‘á»•i má»›i â€“ Nhiá»u phiÃªn báº£n, combo mix cho khÃ¡ch hÃ ng chá»n lá»±a</li>
</ul>",
                    AuthorId = user2.Id,
                    IsPublished = true,
                    PublishedDate = DateTime.Now,
                    CreatedDate = DateTime.Now
                }
            };

            context.Blogs.AddRange(blogs);
            context.SaveChanges();

            Console.WriteLine("âœ… Database seeded successfully with products, employees, users and 5 blog posts!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during seeding: {ex.Message}");
            throw;
        }
    }
}

