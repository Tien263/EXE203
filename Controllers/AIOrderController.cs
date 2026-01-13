using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Exe_Demo.Data;
using Exe_Demo.Models;

namespace Exe_Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIOrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AIOrderController> _logger;

        public AIOrderController(ApplicationDbContext context, ILogger<AIOrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCartFromAI([FromBody] AIOrderRequest request)
        {
            try
            {
                _logger.LogInformation($"AI Order Request: {string.Join(", ", request.Products.Select(p => $"{p.Name} x{p.Quantity}"))}");

                // Check if user is logged in
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Json(new
                    {
                        success = false,
                        requiresLogin = true,
                        message = "Bạn cần đăng nhập để đặt hàng. Mình sẽ chuyển bạn đến trang đăng nhập nhé! 😊",
                        redirectUrl = "/Auth/Login?returnUrl=/Cart"
                    });
                }

                // Get customer ID
                var user = await _context.Users.FindAsync(userId);
                if (user == null || user.CustomerId == null)
                {
                    return Json(new
                    {
                        success = false,
                        requiresLogin = true,
                        message = "Không tìm thấy thông tin khách hàng. Vui lòng đăng nhập lại.",
                        redirectUrl = "/Auth/Login"
                    });
                }

                var customerId = user.CustomerId.Value;
                var addedProducts = new List<string>();
                var errors = new List<string>();

                // Process each product
                foreach (var productRequest in request.Products)
                {
                    // Find product by name (case-insensitive, fuzzy match)
                    var product = await FindProductByName(productRequest.Name);

                    if (product == null)
                    {
                        errors.Add($"Không tìm thấy sản phẩm '{productRequest.Name}'");
                        continue;
                    }

                    // Check stock
                    if (product.StockQuantity < productRequest.Quantity)
                    {
                        errors.Add($"{product.ProductName}: Chỉ còn {product.StockQuantity} sản phẩm");
                        continue;
                    }

                    // Check if product already in cart
                    var existingCart = await _context.Carts
                        .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.ProductId == product.ProductId);

                    if (existingCart != null)
                    {
                        // Update quantity
                        existingCart.Quantity += productRequest.Quantity;

                        if (existingCart.Quantity > product.StockQuantity)
                        {
                            errors.Add($"{product.ProductName}: Vượt quá số lượng tồn kho");
                            continue;
                        }
                    }
                    else
                    {
                        // Add new cart item
                        var cart = new Cart
                        {
                            CustomerId = customerId,
                            ProductId = product.ProductId,
                            Quantity = productRequest.Quantity,
                            CreatedDate = DateTime.Now
                        };
                        _context.Carts.Add(cart);
                    }

                    addedProducts.Add($"{product.ProductName} x{productRequest.Quantity}");
                }

                await _context.SaveChangesAsync();

                if (addedProducts.Count > 0)
                {
                    var message = $"Đã thêm vào giỏ hàng: {string.Join(", ", addedProducts)}! 🎉";
                    if (errors.Count > 0)
                    {
                        message += $"\n\nLưu ý: {string.Join(", ", errors)}";
                    }

                    return Json(new
                    {
                        success = true,
                        message = message,
                        redirectUrl = "/Cart",
                        addedProducts = addedProducts
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Không thể thêm sản phẩm: {string.Join(", ", errors)}"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AI order");
                return Json(new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi xử lý đơn hàng. Vui lòng thử lại!"
                });
            }
        }

        [HttpGet("check-auth")]
        public IActionResult CheckAuth()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isLoggedIn = !string.IsNullOrEmpty(userIdClaim);

            return Json(new
            {
                isLoggedIn = isLoggedIn,
                userId = userIdClaim
            });
        }

        private async Task<Product?> FindProductByName(string name)
        {
            var nameLower = name.ToLower().Trim();

            // Try exact match first
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductName.ToLower() == nameLower);

            if (product != null) return product;

            // Try partial match
            product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductName.ToLower().Contains(nameLower) ||
                                         nameLower.Contains(p.ProductName.ToLower()));

            if (product != null) return product;

            // Try fuzzy match for common product names
            var productMappings = new Dictionary<string, string>
            {
                { "dâu tây sấy dẻo", "Dâu Sấy Dẻo" },
                { "dâu tây sấy", "Dâu Sấy Dẻo" },
                { "dâu sấy dẻo", "Dâu Sấy Dẻo" },
                { "dâu sấy", "Dâu Sấy Dẻo" },
                { "dâu tây", "Dâu Sấy Dẻo" },
                { "dâu", "Dâu Sấy Dẻo" },
                { "dâu tây sấy thăng hoa", "Dâu Sấy Thăng Hoa" },
                { "dâu thăng hoa", "Dâu Sấy Thăng Hoa" },
                { "mận sấy dẻo", "Mận Sấy Dẻo" },
                { "mận sấy", "Mận Sấy Dẻo" },
                { "mận", "Mận Sấy Dẻo" },
                { "xoài sấy dẻo", "Xoài Sấy Dẻo" },
                { "xoài sấy", "Xoài Sấy Dẻo" },
                { "xoài", "Xoài Sấy Dẻo" },
                { "đào sấy dẻo", "Đào Sấy Dẻo" },
                { "đào sấy", "Đào Sấy Dẻo" },
                { "đào", "Đào Sấy Dẻo" },
                { "hồng sấy dẻo", "Hồng Sấy Dẻo" },
                { "hồng sấy", "Hồng Sấy Dẻo" },
                { "hồng", "Hồng Sấy Dẻo" },
                { "mít sấy dẻo", "Mít Sấy Dẻo" },
                { "mít sấy", "Mít Sấy Dẻo" },
                { "mít", "Mít Sấy Dẻo" },
                { "chuối sấy giòn", "Chuối Sấy Giòn" },
                { "chuối sấy", "Chuối Sấy Giòn" },
                { "chuối", "Chuối Sấy Giòn" },
                { "sữa chua sấy", "Sữa Chua Sấy" },
                { "sữa chua", "Sữa Chua Sấy" }
            };

            if (productMappings.TryGetValue(nameLower, out var mappedName))
            {
                product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductName.Contains(mappedName));
            }

            return product;
        }
    }

    public class AIOrderRequest
    {
        public List<ProductRequest> Products { get; set; } = new();
    }

    public class ProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
