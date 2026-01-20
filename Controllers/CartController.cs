using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Exe_Demo.Data;
using Exe_Demo.Models;
using Exe_Demo.Helpers;
using Exe_Demo.Services;

namespace Exe_Demo.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public CartController(ApplicationDbContext context, ILogger<CartController> logger, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Get customer ID from user
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.CustomerId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .ThenInclude(p => p.Category)
                .Where(c => c.CustomerId == user.CustomerId)
                .ToListAsync();

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            try
            {
                _logger.LogInformation($"AddToCart called with productId: {productId}, quantity: {quantity}");
                
                // Check product exists
                var product = await _context.Products.FindAsync(productId);
                
                _logger.LogInformation($"Product found: {product != null}, ProductId in DB: {product?.ProductId}");
                
                if (product == null)
                {
                    // Try to find all products to debug
                    var allProducts = await _context.Products.Select(p => p.ProductId).ToListAsync();
                    _logger.LogWarning($"Product {productId} not found. Available products: {string.Join(", ", allProducts)}");
                    
                    return Json(new { success = false, message = $"Sản phẩm không tồn tại! (ID: {productId})" });
                }

                // Check stock
                if (product.StockQuantity < quantity)
                {
                    return Json(new { success = false, message = $"Chỉ còn {product.StockQuantity} sản phẩm!" });
                }

                // Get customer ID or session ID
                int? customerId = null;
                string? sessionId = null;

                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    var user = await _context.Users.FindAsync(userId);
                    customerId = user?.CustomerId;
                }

                if (customerId == null)
                {
                    // Guest user - use session
                    sessionId = HttpContext.Session.GetString("CartSessionId");
                    if (string.IsNullOrEmpty(sessionId))
                    {
                        sessionId = Guid.NewGuid().ToString();
                        HttpContext.Session.SetString("CartSessionId", sessionId);
                    }
                }

                // Check if product already in cart
                var existingCart = await _context.Carts
                    .AsTracking()
                    .FirstOrDefaultAsync(c => 
                        (customerId.HasValue && c.CustomerId == customerId && c.ProductId == productId) ||
                        (!customerId.HasValue && c.SessionId == sessionId && c.ProductId == productId));

                if (existingCart != null)
                {
                    // Update quantity
                    existingCart.Quantity += quantity;
                    
                    // Check stock again
                    if (existingCart.Quantity > product.StockQuantity)
                    {
                        return Json(new { success = false, message = $"Chỉ còn {product.StockQuantity} sản phẩm!" });
                    }
                }
                else
                {
                    // Add new cart item
                    var cart = new Cart
                    {
                        CustomerId = customerId,
                        SessionId = sessionId,
                        ProductId = productId,
                        Quantity = quantity,
                        CreatedDate = DateTime.Now
                    };
                    _context.Carts.Add(cart);
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã thêm vào giỏ hàng!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to cart");
                return Json(new { success = false, message = "Có lỗi xảy ra. Vui lòng thử lại!" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartId, int quantity)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập!" });
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.CustomerId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập!" });
            }

            // Lấy giỏ hàng
            // Use AsTracking to ensure changes are saved
            var cart = await _context.Carts
                .AsTracking()
                .FirstOrDefaultAsync(c => c.CartId == cartId && c.CustomerId == user.CustomerId);

            if (cart == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng!" });
            }

            if (quantity <= 0)
            {
                _context.Carts.Remove(cart);
            }
            else
            {
                cart.Quantity = quantity;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập!" });
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.CustomerId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập!" });
            }

            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.CartId == cartId && c.CustomerId == user.CustomerId);

            if (cart == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng!" });
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        public async Task<IActionResult> GetCartCount()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Json(0);
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.CustomerId == null)
            {
                return Json(0);
            }

            var count = await _context.Carts
                .Where(c => c.CustomerId == user.CustomerId)
                .SumAsync(c => c.Quantity);

            return Json(count);
        }

        [HttpPost]
        public async Task<IActionResult> ApplyVoucher(string voucherCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(voucherCode))
                {
                    return Json(new { success = false, message = "Vui lòng nhập mã voucher" });
                }

                // Tìm voucher
                var voucher = await _context.Vouchers
                    .FirstOrDefaultAsync(v => v.VoucherCode.ToUpper() == voucherCode.ToUpper());

                if (voucher == null)
                {
                    return Json(new { success = false, message = "Mã voucher không tồn tại" });
                }

                // Kiểm tra voucher có active không
                if (voucher.IsActive != true)
                {
                    return Json(new { success = false, message = "Mã voucher không còn hiệu lực" });
                }

                // Kiểm tra thời gian hiệu lực
                var now = DateTime.Now;
                if (voucher.ValidFrom.HasValue && now < voucher.ValidFrom.Value)
                {
                    return Json(new { success = false, message = "Mã voucher chưa đến thời gian sử dụng" });
                }

                if (voucher.ValidTo.HasValue && now > voucher.ValidTo.Value)
                {
                    return Json(new { success = false, message = "Mã voucher đã hết hạn" });
                }

                // Kiểm tra số lần sử dụng
                if (voucher.UsageLimit.HasValue && voucher.UsageLimit > 0)
                {
                    if (voucher.UsedCount >= voucher.UsageLimit)
                    {
                        return Json(new { success = false, message = "Mã voucher đã hết lượt sử dụng" });
                    }
                }

                // Lấy tổng tiền giỏ hàng
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập" });
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null || user.CustomerId == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin khách hàng" });
                }

                var cartItems = await _context.Carts
                    .Include(c => c.Product)
                    .Where(c => c.CustomerId == user.CustomerId)
                    .ToListAsync();

                decimal totalAmount = cartItems.Sum(c => c.Quantity * c.Product.Price);

                // Kiểm tra đơn hàng tối thiểu
                if (voucher.MinOrderAmount.HasValue && totalAmount < voucher.MinOrderAmount.Value)
                {
                    return Json(new { 
                        success = false, 
                        message = $"Đơn hàng tối thiểu {voucher.MinOrderAmount.Value:N0}đ để sử dụng mã này" 
                    });
                }

                // Tính giảm giá
                decimal discountAmount = 0;
                if (voucher.DiscountType == "Percent")
                {
                    discountAmount = totalAmount * voucher.DiscountValue / 100;
                    
                    // Áp dụng giảm tối đa nếu có
                    if (voucher.MaxDiscountAmount.HasValue && discountAmount > voucher.MaxDiscountAmount.Value)
                    {
                        discountAmount = voucher.MaxDiscountAmount.Value;
                    }
                }
                else if (voucher.DiscountType == "Fixed")
                {
                    discountAmount = voucher.DiscountValue;
                }

                decimal finalAmount = totalAmount - discountAmount;
                if (finalAmount < 0) finalAmount = 0;

                return Json(new { 
                    success = true, 
                    message = "Áp dụng mã giảm giá thành công",
                    voucherCode = voucher.VoucherCode,
                    voucherName = voucher.VoucherName,
                    discountAmount = discountAmount,
                    finalAmount = finalAmount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error applying voucher: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi áp dụng voucher" });
            }
        }

        public async Task<IActionResult> Checkout()
        {
            // Kiểm tra đăng nhập
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt hàng!";
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Checkout", "Cart") });
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.CustomerId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt hàng!";
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Checkout", "Cart") });
            }

            // Lấy thông tin customer
            var customer = await _context.Customers.FindAsync(user.CustomerId);
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin khách hàng!";
                return RedirectToAction("Index", "Cart");
            }

            // Lấy giỏ hàng
            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .ThenInclude(p => p.Category)
                .Where(c => c.CustomerId == user.CustomerId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống!";
                return RedirectToAction("Index", "Cart");
            }

            // Truyền dữ liệu sang view
            ViewBag.Customer = customer;
            ViewBag.CartItems = cartItems;
            ViewBag.Total = cartItems.Sum(c => c.Quantity * c.Product.Price);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string FullName, string Phone, string Email, string Address, string Note, string PaymentMethod, string? VoucherCode, decimal? DiscountAmount, decimal? FinalAmount)
        {
            // Kiểm tra đăng nhập
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt hàng!";
                return RedirectToAction("Login", "Auth");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.CustomerId == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin khách hàng!";
                return RedirectToAction("Index", "Cart");
            }

            // Lấy giỏ hàng
            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.CustomerId == user.CustomerId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống!";
                return RedirectToAction("Index", "Cart");
            }

            try
            {
                // Bắt đầu transaction để đảm bảo dữ liệu được lưu toàn vẹn
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Tính tổng tiền
                        decimal totalAmount = cartItems.Sum(c => c.Quantity * c.Product.Price);
                        
                        // Áp dụng giảm giá từ voucher nếu có
                        decimal finalOrderAmount = FinalAmount ?? totalAmount;
                        decimal discountApplied = DiscountAmount ?? 0;

                        // Tạo đơn hàng
                        var order = new Models.Order
                        {
                            OrderCode = "ORD" + DateTime.Now.Ticks.ToString().Substring(0, 10),
                            CustomerId = user.CustomerId,
                            CustomerName = FullName,
                            CustomerEmail = Email,
                            CustomerPhone = Phone,
                            ShippingAddress = Address,
                            TotalAmount = totalAmount,
                            FinalAmount = finalOrderAmount,
                            PaymentMethod = PaymentMethod,
                            PaymentStatus = PaymentMethod == "COD" ? "Pending" : "Pending",
                            OrderStatus = "Pending",
                            Note = Note,
                            CreatedDate = DateTime.Now
                        };
                        
                        // Cập nhật voucher usage nếu có
                        if (!string.IsNullOrEmpty(VoucherCode))
                        {
                            var voucher = await _context.Vouchers
                                .FirstOrDefaultAsync(v => v.VoucherCode == VoucherCode);
                            if (voucher != null)
                            {
                                voucher.UsedCount = (voucher.UsedCount ?? 0) + 1;
                                _context.Vouchers.Update(voucher);
                            }
                        }

                        // Thêm đơn hàng
                        _context.Orders.Add(order);
                        await _context.SaveChangesAsync();

                        // Tạo chi tiết đơn hàng
                        foreach (var item in cartItems)
                        {
                            var orderDetail = new Models.OrderDetail
                            {
                                OrderId = order.OrderId,
                                ProductId = item.ProductId,
                                ProductName = item.Product.ProductName,
                                Quantity = item.Quantity,
                                Price = item.Product.Price,
                                DiscountPercent = item.Product.DiscountPercent ?? 0,
                                TotalPrice = item.Quantity * item.Product.Price
                            };
                            _context.OrderDetails.Add(orderDetail);
                        }
                        await _context.SaveChangesAsync();

                        // Xóa giỏ hàng
                        _context.Carts.RemoveRange(cartItems);
                        await _context.SaveChangesAsync();

                        // Commit transaction
                        await transaction.CommitAsync();

                        _logger.LogInformation($"Đơn hàng #{order.OrderCode} được tạo thành công cho khách hàng ID: {user.CustomerId}");

                        // Load lại order với OrderDetails để gửi email
                        var orderWithDetails = await _context.Orders
                            .Include(o => o.OrderDetails)
                            .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

                        // Gửi email xác nhận đơn hàng
                        if (orderWithDetails != null)
                        {
                            try
                            {
                                await _emailService.SendOrderConfirmationEmailAsync(orderWithDetails);
                                _logger.LogInformation($"Email xác nhận đơn hàng #{orderWithDetails.OrderCode} đã được gửi đến {orderWithDetails.CustomerEmail}");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Lỗi gửi email xác nhận đơn hàng: {ex.Message}");
                                // Không throw exception để không ảnh hưởng đến quá trình đặt hàng
                            }
                        }

                        // Xử lý theo phương thức thanh toán
                        if (PaymentMethod == "Bank")
                        {
                            // Chuyển hướng đến trang hiển thị thông tin chuyển khoản
                            return RedirectToAction("BankTransferInfo", new { orderId = order.OrderId });
                        }
                        else if (PaymentMethod == "COD")
                        {
                            // Tính điểm sẽ nhận được
                            int pointsToEarn = (int)(order.FinalAmount / 10000);
                            string pointsMessage = pointsToEarn > 0 
                                ? $" Sau khi giao hàng thành công, bạn sẽ nhận được {pointsToEarn} điểm tích lũy (10.000đ = 1 điểm)." 
                                : "";
                            
                            TempData["SuccessMessage"] = $"Đặt hàng thành công! Mã đơn hàng: #{order.OrderCode}. Chúng tôi sẽ liên hệ với bạn sớm nhất. Thông tin chi tiết đã được gửi qua email.{pointsMessage}";
                            return RedirectToAction("Index", "Home");
                        }

                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError($"Lỗi khi tạo đơn hàng: {ex.Message}\n{ex.StackTrace}");
                        TempData["ErrorMessage"] = $"Có lỗi xảy ra khi đặt hàng: {ex.Message}";
                        return RedirectToAction("Index", "Cart");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi không mong muốn khi đặt hàng: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra. Vui lòng thử lại!";
                return RedirectToAction("Index", "Cart");
            }
        }

        public async Task<IActionResult> BankTransferInfo(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng!";
                return RedirectToAction("Index", "Home");
            }

            // Truyền thông tin ngân hàng từ appsettings.json
            ViewBag.BankName = _configuration["BankTransfer:BankName"];
            ViewBag.AccountNumber = _configuration["BankTransfer:AccountNumber"];
            ViewBag.AccountName = _configuration["BankTransfer:AccountName"];
            ViewBag.BankBranch = _configuration["BankTransfer:BankBranch"];

            return View(order);
        }
        
        // Debug endpoint to check products
        [HttpGet]
        public async Task<IActionResult> DebugProducts()
        {
            var products = await _context.Products
                .Select(p => new { 
                    p.ProductId, 
                    p.ProductName, 
                    p.Price, 
                    p.StockQuantity 
                })
                .ToListAsync();
                
            return Json(new { 
                count = products.Count, 
                products = products 
            });
        }
        
        // Test add to cart with specific product
        [HttpGet]
        public async Task<IActionResult> TestAddToCart(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            
            return Json(new {
                productId = productId,
                found = product != null,
                productName = product?.ProductName,
                price = product?.Price,
                stock = product?.StockQuantity
            });
        }
    }
}
