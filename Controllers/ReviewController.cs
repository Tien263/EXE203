using Exe_Demo.Data;
using Exe_Demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Exe_Demo.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReviewController> _logger;
        private readonly IWebHostEnvironment _environment;

        public ReviewController(ApplicationDbContext context, ILogger<ReviewController> logger, IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment, IFormFile? reviewImage)
        {
            try
            {
                // Get current user
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                {
                    return RedirectToAction("Login", "Auth");
                }

                // Verify product exists
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return NotFound();
                }

                // Check if user has purchased this product (Optional validation)
                // For now, allow any logged-in user to review to simplify testing

                var review = new Review
                {
                    ProductId = productId,
                    CustomerId = userId,
                    // Get customer name from definition
                    CustomerName = User.FindFirstValue(ClaimTypes.Name) ?? "Khách hàng",
                    Rating = rating,
                    Comment = comment,
                    CreatedDate = DateTime.Now,
                    IsApproved = true // Auto approve for now
                };

                // Handle Image Upload
                if (reviewImage != null && reviewImage.Length > 0)
                {
                    // Validation
                    if (!reviewImage.ContentType.StartsWith("image/"))
                    {
                        TempData["Error"] = "Chỉ chấp nhận file ảnh.";
                        return RedirectToAction("Details", "Product", new { id = productId });
                    }

                    if (reviewImage.Length > 5 * 1024 * 1024) // 5MB
                    {
                        TempData["Error"] = "Ảnh quá lớn (tối đa 5MB).";
                        return RedirectToAction("Details", "Product", new { id = productId });
                    }

                    // Save file
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "images", "reviews");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + reviewImage.FileName;
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await reviewImage.CopyToAsync(fileStream);
                    }

                    review.MediaUrl = "/images/reviews/" + uniqueFileName;
                }

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đánh giá của bạn đã được gửi thành công!";
                return RedirectToAction("Details", "Product", new { id = productId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding review");
                TempData["Error"] = "Có lỗi xảy ra khi gửi đánh giá.";
                return RedirectToAction("Details", "Product", new { id = productId });
            }
        }
    }
}
