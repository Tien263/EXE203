using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exe_Demo.Data;
using Exe_Demo.Models;
using Exe_Demo.Models.ViewModels;
using Exe_Demo.Services;
using System.Security.Claims;

namespace Exe_Demo.Controllers
{
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly Services.ICacheService _cacheService;

        public StaffController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, Services.ICacheService cacheService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _cacheService = cacheService;
        }


        // Kiểm tra quyền Staff
        private bool IsStaff()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            return role == "Staff" || role == "Admin";
        }

        private int? GetEmployeeId()
        {
            var employeeIdClaim = User.FindFirst("EmployeeId")?.Value;
            if (int.TryParse(employeeIdClaim, out int employeeId))
            {
                return employeeId;
            }
            return null;
        }

        // ==================== DASHBOARD ====================
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                if (!IsStaff())
                {
                    return RedirectToAction("Login", "Auth");
                }

                var employeeId = GetEmployeeId();
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

                var today = DateTime.Today;
                var startOfMonth = new DateTime(today.Year, today.Month, 1);

                // Client-Side Evaluation for SQLite Compatibility

                // Doanh thu hôm nay (Fetch data first, then Sum)
                var todayOrdersList = await _context.Orders
                    .Where(o => o.CreatedDate.HasValue && 
                                o.CreatedDate.Value.Date == today &&
                                o.PaymentStatus == "Đã thanh toán")
                    .Select(o => o.FinalAmount)
                    .ToListAsync();
                var todayRevenue = todayOrdersList.Sum();

                // Doanh thu tháng này
                var monthOrdersList = await _context.Orders
                    .Where(o => o.CreatedDate.HasValue && 
                                o.CreatedDate.Value >= startOfMonth &&
                                o.PaymentStatus == "Đã thanh toán")
                    .Select(o => o.FinalAmount)
                    .ToListAsync();
                var monthRevenue = monthOrdersList.Sum();

                // Top sản phẩm bán chạy (Fetch detail first, then GroupBy in memory)
                var monthOrderDetails = await _context.OrderDetails
                    .Include(od => od.Product)
                    .Where(od => od.Order.CreatedDate.HasValue && od.Order.CreatedDate.Value >= startOfMonth)
                    .ToListAsync();

                var topSellingProducts = monthOrderDetails
                    .GroupBy(od => new { od.ProductId, od.Product?.ProductName, od.Product?.ImageUrl })
                    .Select(g => new ProductSalesViewModel
                    {
                        ProductId = g.Key.ProductId,
                        ProductName = g.Key.ProductName ?? "Unknown",
                        ImageUrl = g.Key.ImageUrl,
                        TotalSold = g.Sum(od => od.Quantity),
                        Revenue = g.Sum(od => od.TotalPrice)
                    })
                    .OrderByDescending(p => p.TotalSold)
                    .Take(5)
                    .ToList();

                var model = new StaffDashboardViewModel
                {
                    EmployeeName = employee?.FullName ?? "Staff",
                    EmployeeCode = employee?.EmployeeCode ?? "",
                    Position = employee?.Position ?? "",

                    TodayRevenue = todayRevenue,
                    MonthRevenue = monthRevenue,
                    
                    // Đơn hàng hôm nay
                    TodayOrders = await _context.Orders
                        .CountAsync(o => o.CreatedDate.HasValue && o.CreatedDate.Value.Date == today),

                    // Đơn hàng tháng này
                    MonthOrders = await _context.Orders
                        .CountAsync(o => o.CreatedDate.HasValue && o.CreatedDate.Value >= startOfMonth),

                    // Đơn hàng chờ xử lý
                    PendingOrders = await _context.Orders
                        .CountAsync(o => o.OrderStatus == "Chờ xác nhận" || o.OrderStatus == "Đang xử lý"),

                    // Sản phẩm sắp hết hàng
                    LowStockProducts = await _context.Products
                        .CountAsync(p => p.StockQuantity <= p.MinStockLevel),

                    // Đơn hàng gần đây
                    RecentOrders = await _context.Orders
                        .Include(o => o.Customer)
                        .OrderByDescending(o => o.CreatedDate)
                        .Take(10)
                        .ToListAsync(),

                    TopSellingProducts = topSellingProducts
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return Content($"LỖI DASHBOARD: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}");
            }
        }

        // ==================== QUẢN LÝ SẢN PHẨM ====================
        [HttpGet]
        public async Task<IActionResult> Products(int page = 1, string? search = null, int? category = null, string? stock = null)
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            int pageSize = 20;
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search) || p.ProductCode.Contains(search));
            }

            // Lọc theo danh mục
            if (category.HasValue)
            {
                query = query.Where(p => p.CategoryId == category.Value);
            }

            // Lọc theo tồn kho
            if (!string.IsNullOrEmpty(stock))
            {
                if (stock == "low")
                {
                    query = query.Where(p => p.StockQuantity <= p.MinStockLevel && p.StockQuantity > 0);
                }
                else if (stock == "out")
                {
                    query = query.Where(p => p.StockQuantity == 0);
                }
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var products = await query
                .OrderByDescending(p => p.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new ProductManagementViewModel
            {
                Products = products,
                Categories = await _context.Categories.Where(c => c.IsActive == true).ToListAsync(),
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchTerm = search,
                CategoryFilter = category,
                StockFilter = stock
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = new ProductFormViewModel
            {
                Categories = await _context.Categories.Where(c => c.IsActive == true).ToListAsync(),
                IsActive = true,
                MinStockLevel = 10,
                Unit = "gói"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(ProductFormViewModel model)
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra mã sản phẩm đã tồn tại
                if (await _context.Products.AnyAsync(p => p.ProductCode == model.ProductCode))
                {
                    ModelState.AddModelError("ProductCode", "Mã sản phẩm đã tồn tại");
                    model.Categories = await _context.Categories.Where(c => c.IsActive == true).ToListAsync();
                    return View(model);
                }

                var product = new Product
                {
                    ProductCode = model.ProductCode,
                    ProductName = model.ProductName,
                    CategoryId = model.CategoryId,
                    Description = model.Description,
                    ShortDescription = model.ShortDescription,
                    Price = model.Price,
                    OriginalPrice = model.OriginalPrice,
                    CostPrice = model.CostPrice,
                    DiscountPercent = model.DiscountPercent,
                    StockQuantity = model.StockQuantity,
                    MinStockLevel = model.MinStockLevel ?? 10,
                    Unit = model.Unit ?? "gói",
                    Weight = model.Weight,
                    ImageUrl = model.ImageUrl,
                    IsActive = model.IsActive,
                    IsFeatured = model.IsFeatured,
                    IsNew = model.IsNew,
                    ViewCount = 0,
                    SoldCount = 0,
                    Rating = 0,
                    CreatedDate = DateTime.Now
                };

                // Handle Image Upload
                if (model.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    product.ImageUrl = "/images/products/" + uniqueFileName;
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Clear Cache
                await _cacheService.RemoveAsync("FeaturedProducts");
                await _cacheService.RemoveAsync("NewProducts");
                await _cacheService.RemoveByPrefixAsync("ProductList_");

                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                return RedirectToAction(nameof(Products));
            }

            model.Categories = await _context.Categories.Where(c => c.IsActive == true).ToListAsync();
            return View(model);
        }




        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductFormViewModel
            {
                ProductId = product.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                Description = product.Description,
                ShortDescription = product.ShortDescription,
                Price = product.Price,
                OriginalPrice = product.OriginalPrice,
                CostPrice = product.CostPrice,
                DiscountPercent = product.DiscountPercent,
                StockQuantity = product.StockQuantity ?? 0,
                MinStockLevel = product.MinStockLevel,
                Unit = product.Unit,
                Weight = product.Weight,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive ?? true,
                IsFeatured = product.IsFeatured ?? false,
                IsNew = product.IsNew ?? false,
                Categories = await _context.Categories.Where(c => c.IsActive == true).ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        // [ValidateAntiForgeryToken] // Temporary disabled for debugging
        public async Task<IActionResult> EditProduct(ProductFormViewModel model)
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (ModelState.IsValid)
            {
                var product = await _context.Products.FindAsync(model.ProductId);
                if (product == null)
                {
                    return NotFound();
                }

                // Kiểm tra mã sản phẩm đã tồn tại (trừ sản phẩm hiện tại)
                if (await _context.Products.AnyAsync(p => p.ProductCode == model.ProductCode && p.ProductId != model.ProductId))
                {
                    ModelState.AddModelError("ProductCode", "Mã sản phẩm đã tồn tại");
                    model.Categories = await _context.Categories.Where(c => c.IsActive == true).ToListAsync();
                    return View(model);
                }

                product.ProductCode = model.ProductCode;
                product.ProductName = model.ProductName;
                product.CategoryId = model.CategoryId;
                product.Description = model.Description;
                product.ShortDescription = model.ShortDescription;
                product.Price = model.Price;
                product.OriginalPrice = model.OriginalPrice;
                product.CostPrice = model.CostPrice;
                product.DiscountPercent = model.DiscountPercent;
                product.StockQuantity = model.StockQuantity;
                product.MinStockLevel = model.MinStockLevel;
                product.Unit = model.Unit;
                product.Weight = model.Weight;
                
                // Handle Image Upload
                Console.WriteLine($"[DEBUG] EditProduct POST. ID: {model.ProductId}");
                Console.WriteLine($"[DEBUG] Content Type: {Request.ContentType}");
                
                // Fallback: Try retrieval from Request.Form if binding failed
                if (model.ImageFile == null && Request.Form.Files.Count > 0)
                {
                    model.ImageFile = Request.Form.Files["ImageFile"] ?? Request.Form.Files[0];
                    Console.WriteLine($"[DEBUG] Manually retrieved ImageFile from Request.Form.Files. Name: {model.ImageFile?.FileName}");
                }
                
                Console.WriteLine($"[DEBUG] Has ImageFile: {model.ImageFile != null}");
                
                if (model.ImageFile != null)
                {
                    Console.WriteLine($"[DEBUG] File Name: {model.ImageFile.FileName}, Length: {model.ImageFile.Length}");
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    product.ImageUrl = "/images/products/" + uniqueFileName;
                }
                else if (!string.IsNullOrEmpty(model.ImageUrl))
                {
                    product.ImageUrl = model.ImageUrl;
                }
                
                product.IsActive = model.IsActive;
                product.IsFeatured = model.IsFeatured;
                product.IsNew = model.IsNew;
                product.UpdatedDate = DateTime.Now;

                // FIX: Explicitly update entity because Global NoTracking is enabled
                _context.Products.Update(product);

                await _context.SaveChangesAsync();

                // Clear Cache
                await _cacheService.RemoveAsync("FeaturedProducts");
                await _cacheService.RemoveAsync("NewProducts");
                await _cacheService.RemoveByPrefixAsync("ProductList_");
                await _cacheService.RemoveAsync($"Product_{product.ProductId}");
                await _cacheService.RemoveAsync($"Product_Details_{product.ProductId}");

                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction(nameof(Products));
            }
            else 
            {
                // Debug ModelState Errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine($"[ERROR] EditProduct POST Invalid ModelState: {string.Join(", ", errors)}");
            }

            model.Categories = await _context.Categories.Where(c => c.IsActive == true).ToListAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductApi(int id)
        {
            if (!IsStaff())
            {
                return Json(new { success = false, message = "Không có quyền truy cập" });
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
            }

            // Kiểm tra xem sản phẩm có trong đơn hàng nào không
            var hasOrders = await _context.OrderDetails.AnyAsync(od => od.ProductId == id);
            if (hasOrders)
            {
                // Chỉ vô hiệu hóa thay vì xóa
                product.IsActive = false;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Đã vô hiệu hóa sản phẩm" });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Xóa sản phẩm thành công" });
        }

        [HttpGet]
        public IActionResult DeleteProduct(int id)
        {
            // Fallback for old cached HTML links
            TempData["ErrorMessage"] = "Hệ thống đã cập nhật. Vui lòng tải lại trang (Ctrl + F5) để sử dụng tính năng Xóa mới.";
            return RedirectToAction(nameof(Products));
        }

        // ==================== QUẢN LÝ ĐỚN HÀNG ====================
        [HttpGet]
        public async Task<IActionResult> Orders(int page = 1, string? search = null, string? status = null, 
            string? paymentStatus = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            int pageSize = 20;
            var query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o => o.OrderCode.Contains(search) || 
                                        o.CustomerName.Contains(search) || 
                                        o.CustomerPhone.Contains(search));
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.OrderStatus == status);
            }

            // Lọc theo trạng thái thanh toán
            if (!string.IsNullOrEmpty(paymentStatus))
            {
                query = query.Where(o => o.PaymentStatus == paymentStatus);
            }

            // Lọc theo ngày
            if (fromDate.HasValue)
            {
                query = query.Where(o => o.CreatedDate >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                var endDate = toDate.Value.AddDays(1);
                query = query.Where(o => o.CreatedDate < endDate);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var orders = await query
                .OrderByDescending(o => o.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new OrderManagementViewModel
            {
                Orders = orders,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchTerm = search,
                StatusFilter = status,
                PaymentStatusFilter = paymentStatus,
                FromDate = fromDate,
                ToDate = toDate
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetail(int id)
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            var orderDetails = await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderId == id)
                .ToListAsync();

            var model = new OrderDetailViewModel
            {
                Order = order,
                OrderDetails = orderDetails,
                Customer = order.Customer,
                Employee = order.Employee
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusViewModel model)
        {
            if (!IsStaff())
            {
                return Json(new { success = false, message = "Không có quyền truy cập" });
            }

            // Use AsTracking to ensure changes are tracked and saved
            var order = await _context.Orders
                .AsTracking()
                .FirstOrDefaultAsync(o => o.OrderId == model.OrderId);
                
            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
            }

            // Lưu trạng thái cũ để kiểm tra
            var oldStatus = order.OrderStatus;
            
            order.OrderStatus = model.OrderStatus;
            if (!string.IsNullOrEmpty(model.PaymentStatus))
            {
                order.PaymentStatus = model.PaymentStatus;
            }
            if (!string.IsNullOrEmpty(model.Note))
            {
                order.Note = model.Note;
            }
            order.UpdatedDate = DateTime.Now;

            if (model.OrderStatus == "Đã hoàn thành")
            {
                order.CompletedDate = DateTime.Now;
                
                // Cộng điểm tích lũy khi đơn hàng hoàn thành (chỉ cộng 1 lần)
                if (oldStatus != "Đã hoàn thành" && order.CustomerId.HasValue)
                {
                    var customer = await _context.Customers
                        .AsTracking()
                        .FirstOrDefaultAsync(c => c.CustomerId == order.CustomerId.Value);
                        
                    if (customer != null)
                    {
                        // Quy tắc: 10.000đ = 1 điểm
                        int pointsToAdd = (int)(order.FinalAmount / 10000);
                        customer.LoyaltyPoints = (customer.LoyaltyPoints ?? 0) + pointsToAdd;
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
        }

        // ==================== BÁN HÀNG TRỰC TIẾP ====================
        [HttpGet]
        public async Task<IActionResult> DirectSale(string? search = null, int? category = null)
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive == true && p.StockQuantity > 0)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search) || p.ProductCode.Contains(search));
            }

            if (category.HasValue)
            {
                query = query.Where(p => p.CategoryId == category.Value);
            }

            var model = new DirectSaleViewModel
            {
                Products = await query.Take(50).ToListAsync(),
                Categories = await _context.Categories.Where(c => c.IsActive == true).ToListAsync(),
                SearchTerm = search,
                CategoryFilter = category
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDirectSaleOrder([FromBody] CreateDirectSaleOrderViewModel model)
        {
            if (!IsStaff())
            {
                return Json(new { success = false, message = "Không có quyền truy cập" });
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
            }

            var employeeId = GetEmployeeId();
            if (!employeeId.HasValue)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin nhân viên" });
            }

            // Tạo mã đơn hàng
            var orderCode = "ORD" + DateTime.Now.ToString("yyyyMMddHHmmss");

            // Tính tổng tiền
            decimal totalAmount = 0;
            foreach (var item in model.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || product.StockQuantity < item.Quantity)
                {
                    return Json(new { success = false, message = $"Sản phẩm {product?.ProductName} không đủ hàng" });
                }

                var itemTotal = item.Price * item.Quantity;
                if (item.DiscountPercent > 0)
                {
                    itemTotal = itemTotal * (100 - item.DiscountPercent) / 100;
                }
                totalAmount += itemTotal;
            }

            var finalAmount = totalAmount - model.DiscountAmount;

            // Tạo đơn hàng
            var order = new Order
            {
                OrderCode = orderCode,
                CustomerId = model.CustomerId,
                EmployeeId = employeeId.Value,
                OrderType = "Trực tiếp",
                CustomerName = model.CustomerName,
                CustomerEmail = model.CustomerEmail,
                CustomerPhone = model.CustomerPhone,
                TotalAmount = totalAmount,
                DiscountAmount = model.DiscountAmount,
                FinalAmount = finalAmount,
                PaymentMethod = model.PaymentMethod,
                PaymentStatus = "Đã thanh toán",
                OrderStatus = "Đã hoàn thành",
                Note = model.Note,
                CreatedDate = DateTime.Now,
                CompletedDate = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Tạo chi tiết đơn hàng và cập nhật tồn kho
            foreach (var item in model.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                
                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    ProductName = product!.ProductName,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    DiscountPercent = item.DiscountPercent,
                    TotalPrice = item.Price * item.Quantity * (100 - item.DiscountPercent) / 100
                };

                _context.OrderDetails.Add(orderDetail);

                // Cập nhật tồn kho
                product.StockQuantity -= item.Quantity;
                product.SoldCount = (product.SoldCount ?? 0) + item.Quantity;
            }

            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                message = "Tạo đơn hàng thành công", 
                orderId = order.OrderId,
                orderCode = order.OrderCode
            });
        }

        [HttpGet]
        public async Task<IActionResult> SearchCustomer(string phone)
        {
            if (!IsStaff())
            {
                return Json(new { success = false, message = "Không có quyền truy cập" });
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.PhoneNumber == phone);

            if (customer == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng" });
            }

            return Json(new { 
                success = true, 
                customer = new {
                    customerId = customer.CustomerId,
                    fullName = customer.FullName,
                    email = customer.Email,
                    phoneNumber = customer.PhoneNumber,
                    loyaltyPoints = customer.LoyaltyPoints
                }
            });
        }

        // ==================== THỐNG KÊ DOANH SỐ ====================
        [HttpGet]
        public async Task<IActionResult> SalesReport(DateTime? fromDate = null, DateTime? toDate = null, string reportType = "daily")
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            // Mặc định là tháng hiện tại
            if (!fromDate.HasValue)
            {
                fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }
            if (!toDate.HasValue)
            {
                toDate = DateTime.Now;
            }

            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ThenInclude(p => p.Category)
                .Where(o => o.CreatedDate >= fromDate && o.CreatedDate <= toDate && o.PaymentStatus == "Đã thanh toán")
                .ToListAsync();

            var model = new SalesReportViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                ReportType = reportType,
                TotalRevenue = orders.Sum(o => o.FinalAmount),
                TotalOrders = orders.Count,
                TotalProducts = orders.SelectMany(o => o.OrderDetails).Sum(od => od.Quantity)
            };

            // Tính tổng chi phí và lợi nhuận
            foreach (var order in orders)
            {
                foreach (var detail in order.OrderDetails)
                {
                    var costPrice = detail.Product?.CostPrice ?? 0;
                    model.TotalCost += costPrice * detail.Quantity;
                }
            }
            model.TotalProfit = model.TotalRevenue - model.TotalCost;
            model.AverageOrderValue = model.TotalOrders > 0 ? model.TotalRevenue / model.TotalOrders : 0;

            // Doanh thu theo ngày
            model.RevenueByDate = orders
                .GroupBy(o => o.CreatedDate!.Value.Date)
                .Select(g => new RevenueByDateViewModel
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.FinalAmount),
                    OrderCount = g.Count()
                })
                .OrderBy(r => r.Date)
                .ToList();

            // Top sản phẩm bán chạy
            model.TopProducts = orders
                .SelectMany(o => o.OrderDetails)
                .GroupBy(od => new { od.ProductId, od.Product!.ProductCode, od.Product.ProductName, od.Product.ImageUrl, od.Product.CostPrice })
                .Select(g => new TopProductViewModel
                {
                    ProductId = g.Key.ProductId,
                    ProductCode = g.Key.ProductCode,
                    ProductName = g.Key.ProductName,
                    ImageUrl = g.Key.ImageUrl,
                    QuantitySold = g.Sum(od => od.Quantity),
                    Revenue = g.Sum(od => od.TotalPrice),
                    Profit = g.Sum(od => od.TotalPrice) - (g.Key.CostPrice ?? 0) * g.Sum(od => od.Quantity)
                })
                .OrderByDescending(p => p.QuantitySold)
                .Take(10)
                .ToList();

            // Doanh thu theo danh mục
            var categoryRevenue = orders
                .SelectMany(o => o.OrderDetails)
                .GroupBy(od => new { od.Product!.CategoryId, od.Product.Category!.CategoryName })
                .Select(g => new RevenueByCategoryViewModel
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    Revenue = g.Sum(od => od.TotalPrice),
                    OrderCount = g.Select(od => od.OrderId).Distinct().Count()
                })
                .ToList();

            var totalCategoryRevenue = categoryRevenue.Sum(c => c.Revenue);
            foreach (var item in categoryRevenue)
            {
                item.Percentage = totalCategoryRevenue > 0 ? (item.Revenue / totalCategoryRevenue * 100) : 0;
            }
            model.RevenueByCategory = [.. categoryRevenue.OrderByDescending(c => c.Revenue)];

            // Doanh thu theo phương thức thanh toán
            var paymentRevenue = orders
                .GroupBy(o => o.PaymentMethod)
                .Select(g => new RevenueByPaymentMethodViewModel
                {
                    PaymentMethod = g.Key ?? "Chưa xác định",
                    Amount = g.Sum(o => o.FinalAmount),
                    OrderCount = g.Count()
                })
                .ToList();

            var totalPaymentRevenue = paymentRevenue.Sum(p => p.Amount);
            foreach (var item in paymentRevenue)
            {
                item.Percentage = totalPaymentRevenue > 0 ? (item.Amount / totalPaymentRevenue * 100) : 0;
            }
            model.RevenueByPaymentMethod = [.. paymentRevenue.OrderByDescending(p => p.Amount)];

            return View(model);
        }

        // ==================== QUẢN LÝ VOUCHER ====================
        [HttpGet]
        public async Task<IActionResult> Vouchers(int page = 1, string? search = null, string? status = null)
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            int pageSize = 20;
            var query = _context.Vouchers.AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(v => v.VoucherCode.Contains(search) || (v.VoucherName != null && v.VoucherName.Contains(search)));
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "active")
                {
                    query = query.Where(v => v.IsActive == true && v.ValidTo >= DateTime.Now);
                }
                else if (status == "expired")
                {
                    query = query.Where(v => v.ValidTo < DateTime.Now);
                }
                else if (status == "inactive")
                {
                    query = query.Where(v => v.IsActive == false);
                }
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var vouchers = await query
                .OrderByDescending(v => v.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchTerm = search;
            ViewBag.StatusFilter = status;

            return View(vouchers);
        }

        [HttpGet]
        public IActionResult CreateVoucher()
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVoucher(Voucher model)
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra mã voucher đã tồn tại
                if (await _context.Vouchers.AnyAsync(v => v.VoucherCode == model.VoucherCode))
                {
                    ModelState.AddModelError("VoucherCode", "Mã voucher đã tồn tại");
                    return View(model);
                }

                model.CreatedDate = DateTime.Now;
                model.UsedCount = 0;
                model.IsActive = true;

                _context.Vouchers.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thêm voucher thành công!";
                return RedirectToAction(nameof(Vouchers));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditVoucher(int id)
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }

            return View(voucher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVoucher(Voucher model)
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (ModelState.IsValid)
            {
                var voucher = await _context.Vouchers.FindAsync(model.VoucherId);
                if (voucher == null)
                {
                    return NotFound();
                }

                // Kiểm tra mã voucher đã tồn tại (trừ voucher hiện tại)
                if (await _context.Vouchers.AnyAsync(v => v.VoucherCode == model.VoucherCode && v.VoucherId != model.VoucherId))
                {
                    ModelState.AddModelError("VoucherCode", "Mã voucher đã tồn tại");
                    return View(model);
                }

                voucher.VoucherCode = model.VoucherCode;
                voucher.VoucherName = model.VoucherName;
                voucher.DiscountType = model.DiscountType;
                voucher.DiscountValue = model.DiscountValue;
                voucher.MinOrderAmount = model.MinOrderAmount;
                voucher.MaxDiscountAmount = model.MaxDiscountAmount;
                voucher.UsageLimit = model.UsageLimit;
                voucher.ValidFrom = model.ValidFrom;
                voucher.ValidTo = model.ValidTo;
                voucher.IsActive = model.IsActive;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật voucher thành công!";
                return RedirectToAction(nameof(Vouchers));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVoucher(int id)
        {
            if (!IsStaff())
            {
                return Json(new { success = false, message = "Không có quyền truy cập" });
            }

            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return Json(new { success = false, message = "Không tìm thấy voucher" });
            }

            // Chỉ vô hiệu hóa thay vì xóa
            voucher.IsActive = false;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã vô hiệu hóa voucher" });
        }

        // ==================== QUẢN LÝ ĐIỂM TÍCH LŨY ====================
        [HttpGet]
        public async Task<IActionResult> LoyaltyPoints(int page = 1, string? search = null)
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            int pageSize = 20;
            var query = _context.Customers.AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => (c.FullName != null && c.FullName.Contains(search)) || 
                                        (c.PhoneNumber != null && c.PhoneNumber.Contains(search)) || 
                                        (c.Email != null && c.Email.Contains(search)));
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var customers = await query
                .OrderByDescending(c => c.LoyaltyPoints)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchTerm = search;

            return View(customers);
        }

        [HttpGet]
        public async Task<IActionResult> LoyaltyPointsHistory(int customerId)
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                return NotFound();
            }

            var history = await _context.LoyaltyPointsHistories
                .Where(h => h.CustomerId == customerId)
                .OrderByDescending(h => h.CreatedDate)
                .ToListAsync();

            ViewBag.Customer = customer;
            return View(history);
        }

        [HttpPost]
        public async Task<IActionResult> AdjustLoyaltyPoints([FromBody] AdjustLoyaltyPointsViewModel model)
        {
            if (!IsStaff())
            {
                return Json(new { success = false, message = "Không có quyền truy cập" });
            }

            // Use AsTracking to ensure changes are saved
            var customer = await _context.Customers
                .AsTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == model.CustomerId);
                
            if (customer == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng" });
            }

            // Cập nhật điểm
            customer.LoyaltyPoints = (customer.LoyaltyPoints ?? 0) + model.Points;

            // Lưu lịch sử
            var history = new LoyaltyPointsHistory
            {
                CustomerId = model.CustomerId,
                Points = model.Points,
                TransactionType = model.Points > 0 ? "Cộng điểm" : "Trừ điểm",
                Description = model.Reason,
                CreatedDate = DateTime.Now
            };

            _context.LoyaltyPointsHistories.Add(history);
            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                message = "Cập nhật điểm thành công",
                newPoints = customer.LoyaltyPoints
            });
        }

        // ==================== QUẢN LÝ BLOG ====================
        [HttpGet]
        public async Task<IActionResult> Blogs(int page = 1, string? search = null, string? status = null)
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            int pageSize = 20;
            var query = _context.Blogs.Include(b => b.Author).AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.Title.Contains(search) || b.Content.Contains(search));
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "published")
                {
                    query = query.Where(b => b.IsPublished == true);
                }
                else if (status == "draft")
                {
                    query = query.Where(b => b.IsPublished == false);
                }
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var blogs = await query
                .OrderByDescending(b => b.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchTerm = search;
            ViewBag.StatusFilter = status;

            return View(blogs);
        }

        [HttpGet]
        public IActionResult CreateBlog()
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBlog(Blog model)
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (ModelState.IsValid)
            {
                // Xử lý upload ảnh
                // Fallback: Try retrieval from Request.Form if model binding failed
                if (model.ImageFile == null && Request.Form.Files.Count > 0)
                {
                    model.ImageFile = Request.Form.Files["ImageFile"] ?? Request.Form.Files[0];
                }

                if (model.ImageFile != null)
                {
                    string uniqueFileName = UploadFile(model.ImageFile);
                    model.ImageUrl = "/images/blog/" + uniqueFileName;
                }

                // Tạo slug từ title
                model.Slug = GenerateSlug(model.Title);

                // Kiểm tra slug đã tồn tại
                if (await _context.Blogs.AnyAsync(b => b.Slug == model.Slug))
                {
                    model.Slug = model.Slug + "-" + DateTime.Now.Ticks;
                }

                var employeeId = GetEmployeeId();
                var employee = await _context.Employees.FindAsync(employeeId);

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                model.AuthorId = userIdClaim?.Value != null ? 
                                int.Parse(userIdClaim.Value) : null;
                model.AuthorName = employee?.FullName ?? User.Identity?.Name;
                model.CreatedDate = DateTime.Now;
                model.ViewCount = 0;

                if (model.IsPublished == true)
                {
                    model.PublishedDate = DateTime.Now;
                }

                _context.Blogs.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thêm bài viết thành công!";
                return RedirectToAction(nameof(Blogs));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditBlog(int id)
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBlog(Blog model)
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (ModelState.IsValid)
            {
                var blog = await _context.Blogs.FindAsync(model.BlogId);
                if (blog == null)
                {
                    return NotFound();
                }

                // Xử lý upload ảnh mới
                // Fallback: Try retrieval from Request.Form if model binding failed
                if (model.ImageFile == null && Request.Form.Files.Count > 0)
                {
                    model.ImageFile = Request.Form.Files["ImageFile"] ?? Request.Form.Files[0];
                }

                if (model.ImageFile != null)
                {
                    // Xóa ảnh cũ nếu có (tùy chọn)
                    // if (!string.IsNullOrEmpty(blog.ImageUrl)) { 删除旧图片 logic }

                    string uniqueFileName = UploadFile(model.ImageFile);
                    blog.ImageUrl = "/images/blog/" + uniqueFileName;
                }
                else if (!string.IsNullOrEmpty(model.ImageUrl))
                {
                    // Nếu nhập URL trực tiếp
                    blog.ImageUrl = model.ImageUrl;
                }

                // Cập nhật slug nếu title thay đổi
                if (blog.Title != model.Title)
                {
                    model.Slug = GenerateSlug(model.Title);
                    
                    // Kiểm tra slug đã tồn tại (trừ blog hiện tại)
                    if (await _context.Blogs.AnyAsync(b => b.Slug == model.Slug && b.BlogId != model.BlogId))
                    {
                        model.Slug = model.Slug + "-" + DateTime.Now.Ticks;
                    }
                    blog.Slug = model.Slug;
                }

                blog.Title = model.Title;
                blog.Content = model.Content;
                blog.ShortDescription = model.ShortDescription;
                blog.AuthorName = model.AuthorName;
                blog.IsPublished = model.IsPublished;
                blog.UpdatedDate = DateTime.Now;

                // Nếu chuyển từ draft sang published
                if (blog.IsPublished == true && (blog.PublishedDate == null))
                {
                    blog.PublishedDate = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật bài viết thành công!";
                return RedirectToAction(nameof(Blogs));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            if (!IsStaff())
            {
                return Json(new { success = false, message = "Không có quyền truy cập" });
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bài viết" });
            }

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Xóa bài viết thành công" });
        }

        private string UploadFile(IFormFile file)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "blog");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return uniqueFileName;
        }

        // Helper method để tạo slug
        private static string GenerateSlug(string title)
        {
            if (string.IsNullOrEmpty(title)) return string.Empty;

            // Chuyển về chữ thường
            string slug = title.ToLower();

            // Thay thế các ký tự có dấu
            slug = slug.Replace("á", "a").Replace("à", "a").Replace("ả", "a").Replace("ã", "a").Replace("ạ", "a")
                       .Replace("ă", "a").Replace("ắ", "a").Replace("ằ", "a").Replace("ẳ", "a").Replace("ẵ", "a").Replace("ặ", "a")
                       .Replace("â", "a").Replace("ấ", "a").Replace("ầ", "a").Replace("ẩ", "a").Replace("ẫ", "a").Replace("ậ", "a")
                       .Replace("đ", "d")
                       .Replace("é", "e").Replace("è", "e").Replace("ẻ", "e").Replace("ẽ", "e").Replace("ẹ", "e")
                       .Replace("ê", "e").Replace("ế", "e").Replace("ề", "e").Replace("ể", "e").Replace("ễ", "e").Replace("ệ", "e")
                       .Replace("í", "i").Replace("ì", "i").Replace("ỉ", "i").Replace("ĩ", "i").Replace("ị", "i")
                       .Replace("ó", "o").Replace("ò", "o").Replace("ỏ", "o").Replace("õ", "o").Replace("ọ", "o")
                       .Replace("ô", "o").Replace("ố", "o").Replace("ồ", "o").Replace("ổ", "o").Replace("ỗ", "o").Replace("ộ", "o")
                       .Replace("ơ", "o").Replace("ớ", "o").Replace("ờ", "o").Replace("ở", "o").Replace("ỡ", "o").Replace("ợ", "o")
                       .Replace("ú", "u").Replace("ù", "u").Replace("ủ", "u").Replace("ũ", "u").Replace("ụ", "u")
                       .Replace("ư", "u").Replace("ứ", "u").Replace("ừ", "u").Replace("ử", "u").Replace("ữ", "u").Replace("ự", "u")
                       .Replace("ý", "y").Replace("ỳ", "y").Replace("ỷ", "y").Replace("ỹ", "y").Replace("ỵ", "y");

            // Thay thế khoảng trắng và ký tự đặc biệt bằng dấu gạch ngang
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-");
            slug = slug.Trim('-');

            return slug;
        }

        // ==================== EXCEL EXPORT/IMPORT ====================
        
        [HttpGet]
        public IActionResult ExportOrders()
        {
            if (!IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ExportOrdersToExcel(string period, DateTime? startDate, DateTime? endDate)
        {
            if (!IsStaff())
            {
                return Json(new { success = false, message = "Không có quyền truy cập" });
            }

            DateTime start, end;

            switch (period)
            {
                case "today":
                    start = DateTime.Today;
                    end = DateTime.Today.AddDays(1).AddSeconds(-1);
                    break;
                case "week":
                    start = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                    end = start.AddDays(7).AddSeconds(-1);
                    break;
                case "month":
                    start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    end = start.AddMonths(1).AddSeconds(-1);
                    break;
                case "custom":
                    if (!startDate.HasValue || !endDate.HasValue)
                    {
                        return Json(new { success = false, message = "Vui lòng chọn ngày bắt đầu và kết thúc" });
                    }
                    start = startDate.Value;
                    end = endDate.Value.AddDays(1).AddSeconds(-1);
                    break;
                default:
                    return Json(new { success = false, message = "Khoảng thời gian không hợp lệ" });
            }

            var excelService = new ExcelOrderService(_context);
            var excelData = await excelService.ExportOrdersToExcel(start, end);
            var fileName = $"DonHang_{start:yyyyMMdd}_{end:yyyyMMdd}.xlsx";

            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public async Task<IActionResult> ImportOrdersFromExcel(IFormFile file)
        {
            if (!IsStaff())
            {
                return Json(new { success = false, message = "Không có quyền truy cập" });
            }

            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "Vui lòng chọn file Excel" });
            }

            if (!file.FileName.EndsWith(".xlsx"))
            {
                return Json(new { success = false, message = "Chỉ chấp nhận file .xlsx" });
            }

            try
            {
                using var stream = file.OpenReadStream();
                var excelService = new ExcelOrderService(_context);
                var result = await excelService.ImportOrdersFromExcel(stream);
                    
                    if (result.failed > 0)
                    {
                        return Json(new 
                        { 
                            success = true, 
                            message = $"Cập nhật thành công {result.success} đơn, thất bại {result.failed} đơn",
                            errors = result.errors
                        });
                    }

                return Json(new { success = true, message = $"Cập nhật thành công {result.success} đơn hàng" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
    }

    // ViewModel cho Adjust Loyalty Points
    public class AdjustLoyaltyPointsViewModel
    {
        public int CustomerId { get; set; }
        public int Points { get; set; }
        public string Reason { get; set; } = null!;
    }
}
