using Exe_Demo.Data;
using Exe_Demo.Models.ViewModels;
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

            // Cáº­p nháº­t thÃ´ng tin User
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            // Cáº­p nháº­t thÃ´ng tin Customer
            if (user.Customer != null)
            {
                user.Customer.FullName = model.FullName;
                user.Customer.PhoneNumber = model.PhoneNumber ?? string.Empty;
                user.Customer.Address = model.Address;
                user.Customer.City = model.City;
                user.Customer.District = model.District;
                user.Customer.Ward = model.Ward;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cáº­p nháº­t thÃ´ng tin thÃ nh cÃ´ng!";
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
                TempData["ErrorMessage"] = "Vui lÃ²ng chá»n áº£nh!";
                return RedirectToAction(nameof(Index));
            }

            // Kiá»ƒm tra file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(profileImage.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                TempData["ErrorMessage"] = "Chá»‰ cháº¥p nháº­n file áº£nh (.jpg, .jpeg, .png, .gif)!";
                return RedirectToAction(nameof(Index));
            }

            // Kiá»ƒm tra kÃ­ch thÆ°á»›c (max 5MB)
            if (profileImage.Length > 5 * 1024 * 1024)
            {
                TempData["ErrorMessage"] = "KÃ­ch thÆ°á»›c áº£nh khÃ´ng Ä‘Æ°á»£c vÆ°á»£t quÃ¡ 5MB!";
                return RedirectToAction(nameof(Index));
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // Táº¡o thÆ° má»¥c náº¿u chÆ°a cÃ³
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // XÃ³a áº£nh cÅ© náº¿u cÃ³
                if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfileImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Táº¡o tÃªn file unique
                var uniqueFileName = $"{userId}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // LÆ°u file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(fileStream);
                }

                // Cáº­p nháº­t database
                user.ProfileImageUrl = $"/uploads/profiles/{uniqueFileName}";
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cáº­p nháº­t áº£nh Ä‘áº¡i diá»‡n thÃ nh cÃ´ng!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile image");
                TempData["ErrorMessage"] = "CÃ³ lá»—i xáº£y ra khi upload áº£nh!";
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

            // Láº¥y danh sÃ¡ch Ä‘Æ¡n hÃ ng cá»§a khÃ¡ch hÃ ng
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

