using Exe_Demo.Data;
using Exe_Demo.Models.ViewModels;
using Exe_Demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Exe_Demo.Controllers
{
    [Authorize]
    public class ProfileController(ApplicationDbContext context, ILogger<ProfileController> logger) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<ProfileController> _logger = logger;

        // GET: Profile/Index
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = await _context.Users
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = new ProfileViewModel
            {
                UserId = user.Id,
                CustomerId = user.CustomerId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CustomerCode = user.Customer?.CustomerCode,
                CustomerType = user.Customer?.CustomerType,
                LoyaltyPoints = user.Customer?.LoyaltyPoints,
                Address = user.Customer?.Address,
                City = user.Customer?.City,
                District = user.Customer?.District,
                Ward = user.Customer?.Ward,
                CreatedDate = user.CreatedDate,
                LastLoginDate = user.LastLoginDate,
                ProfileImageUrl = user.ProfileImageUrl
            };

            return View(model);
        }

        // GET: Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = await _context.Users
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = new ProfileViewModel
            {
                UserId = user.Id,
                CustomerId = user.CustomerId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Customer?.Address,
                City = user.Customer?.City,
                District = user.Customer?.District,
                Ward = user.Customer?.Ward
            };

            return View(model);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = await _context.Users
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Cập nhật thông tin User
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            // Cập nhật thông tin Customer
            if (user.Customer == null)
            {
                // Create new customer if not exists
                var lastCustomer = await _context.Customers
                    .OrderByDescending(c => c.CustomerId)
                    .FirstOrDefaultAsync();

                int nextNumber = 1;
                if (lastCustomer != null && !string.IsNullOrEmpty(lastCustomer.CustomerCode))
                {
                    var numberPart = lastCustomer.CustomerCode.Replace("KH", "");
                    if (int.TryParse(numberPart, out int lastNumber))
                    {
                        nextNumber = lastNumber + 1;
                    }
                }

                user.Customer = new Customer
                {
                    CustomerCode = $"KH{nextNumber:D4}",
                    Email = user.Email,
                    CustomerType = "Thường",
                    LoyaltyPoints = 0,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };
                _context.Customers.Add(user.Customer);
            }

            user.Customer.FullName = model.FullName;
            user.Customer.PhoneNumber = model.PhoneNumber ?? string.Empty;
            user.Customer.Address = model.Address;
            user.Customer.City = model.City;
            user.Customer.District = model.District;
            user.Customer.Ward = model.Ward;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Profile/UploadProfileImage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfileImage(IFormFile profileImage)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (profileImage == null || profileImage.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ảnh!";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(profileImage.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                TempData["ErrorMessage"] = "Chỉ chấp nhận file ảnh (.jpg, .jpeg, .png, .gif)!";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra kích thước (max 5MB)
            if (profileImage.Length > 5 * 1024 * 1024)
            {
                TempData["ErrorMessage"] = "Kích thước ảnh không được vượt quá 5MB!";
                return RedirectToAction(nameof(Index));
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // Tạo thư mục nếu chưa có
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfileImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Tạo tên file unique
                var uniqueFileName = $"{userId}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Lưu file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(fileStream);
                }

                // Cập nhật database
                user.ProfileImageUrl = $"/uploads/profiles/{uniqueFileName}";
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật ảnh đại diện thành công!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile image");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi upload ảnh!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Profile/MyOrders
        public async Task<IActionResult> MyOrders()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = await _context.Users
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.CustomerId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Lấy danh sách đơn hàng của khách hàng
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.CustomerId == user.CustomerId)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();

            return View(orders);
        }
    }
}
