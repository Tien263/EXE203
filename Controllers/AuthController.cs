using Exe_Demo.Data;
using Exe_Demo.Models;
using Exe_Demo.Models.ViewModels;
using Exe_Demo.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Exe_Demo.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthController> _logger;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, ILogger<AuthController> logger, IEmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
            _configuration = configuration;
        }

        // GET: Auth/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // Hash password để so sánh
                var passwordHash = HashPassword(model.Password);

                // Tìm user theo email (có tracking để update LastLogin)
                // Tìm user theo email trước
                var user = await _context.Users
                    .AsTracking()
                    .Include(u => u.Customer)
                    .Include(u => u.Employee)
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Email không tồn tại trong hệ thống.");
                    return View(model);
                }

                if (user.PasswordHash != passwordHash)
                {
                    ModelState.AddModelError(string.Empty, $"Sai mật khẩu. (Hash trong DB: {user.PasswordHash?.Substring(0, 10)}... vs Hash nhập: {passwordHash.Substring(0, 10)}...)");
                    return View(model);
                }

                if (user != null)
                {
                    // Check if user is not active (pending OTP verification)
                    if (user.IsActive == false)
                    {
                        TempData["Email"] = user.Email;
                        TempData["ErrorMessage"] = "Tài khoản chưa được kích hoạt. Vui lòng xác thực OTP.";
                        return RedirectToAction(nameof(ResendOtp));
                    }

                    // Tạo claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role ?? "Customer")
                    };

                    if (user.CustomerId.HasValue)
                    {
                        claims.Add(new Claim("CustomerId", user.CustomerId.Value.ToString()));
                    }

                    if (user.EmployeeId.HasValue)
                    {
                        claims.Add(new Claim("EmployeeId", user.EmployeeId.Value.ToString()));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(2)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // Cập nhật last login
                    user.LastLoginDate = DateTime.Now;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"User {user.Email} logged in.");

                    // Redirect based on role
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    
                    // Redirect Staff/Admin to Dashboard
                    if (user.Role == "Staff" || user.Role == "Admin")
                    {
                        return RedirectToAction("Dashboard", "Staff");
                    }
                    
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            }

            return View(model);
        }

        // GET: Auth/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra email đã tồn tại
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                    return View(model);
                }

                try
                {
                    // Bắt đầu transaction
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Tạo mã OTP 6 số
                            var otpCode = new Random().Next(100000, 999999).ToString();

                            // Lưu OTP vào database
                            var otpVerification = new OtpVerification
                            {
                                Email = model.Email,
                                OtpCode = otpCode,
                                CreatedAt = DateTime.Now,
                                ExpiresAt = DateTime.Now.AddMinutes(5),
                                IsUsed = false
                            };
                            _context.OtpVerifications.Add(otpVerification);
                            await _context.SaveChangesAsync();

                            // Tạo customer mới
                            var customer = new Customer
                            {
                                CustomerCode = GenerateCustomerCode(),
                                FullName = model.FullName,
                                PhoneNumber = model.PhoneNumber,
                                Email = model.Email,
                                Address = model.Address,
                                City = model.City,
                                CustomerType = "Thường",
                                LoyaltyPoints = 0,
                                IsActive = true,
                                CreatedDate = DateTime.Now
                            };
                            _context.Customers.Add(customer);
                            await _context.SaveChangesAsync();

                            // Tạo user account (chưa active)
                            var user = new User
                            {
                                Email = model.Email,
                                PasswordHash = HashPassword(model.Password),
                                FullName = model.FullName,
                                PhoneNumber = model.PhoneNumber,
                                Role = "Customer",
                                CustomerId = customer.CustomerId,
                                IsActive = false, // Chưa active, cần verify OTP
                                CreatedDate = DateTime.Now
                            };
                            _context.Users.Add(user);
                            await _context.SaveChangesAsync();

                            // Gửi email OTP - nếu lỗi thì rollback
                            try
                            {
                                await _emailService.SendOtpEmailAsync(model.Email, otpCode, model.FullName);
                                _logger.LogInformation($"OTP sent to {model.Email}");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Error sending OTP email: {ex.Message}");
                                // Rollback transaction nếu gửi email fail
                                await transaction.RollbackAsync();
                                ModelState.AddModelError("", $"Lỗi gửi email OTP: {ex.Message}");
                                if (ex.InnerException != null)
                                {
                                     ModelState.AddModelError("", $"Chi tiết: {ex.InnerException.Message}");
                                }
                                return View(model);
                            }

                            // Commit transaction
                            await transaction.CommitAsync();

                            TempData["Email"] = model.Email;
                            TempData["SuccessMessage"] = "Vui lòng kiểm tra email để lấy mã OTP!";
                            return RedirectToAction(nameof(VerifyOtp));
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            _logger.LogError($"Error during registration: {ex}");
                            ModelState.AddModelError("", $"Lỗi đăng ký: {ex.Message}");
                            
                            var inner = ex.InnerException;
                            while (inner != null)
                            {
                                ModelState.AddModelError("", $"Chi tiết SQL: {inner.Message}");
                                inner = inner.InnerException;
                            }
                            
                            return View(model);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unexpected error during registration: {ex.Message}");
                    ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại!");
                    return View(model);
                }
            }

            return View(model);
        }

        // GET: Auth/VerifyOtp
        [HttpGet]
        public IActionResult VerifyOtp()
        {
            var email = TempData["Email"]?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction(nameof(Register));
            }

            TempData.Keep("Email");
            return View(new VerifyOtpViewModel { Email = email });
        }

        // POST: Auth/VerifyOtp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm OTP (Kèm tracking để update)
                    var otp = await _context.OtpVerifications
                        .AsTracking()
                        .Where(o => o.Email == model.Email && o.OtpCode == model.OtpCode && !o.IsUsed)
                        .OrderByDescending(o => o.CreatedAt)
                        .FirstOrDefaultAsync();

                    if (otp == null)
                    {
                        ModelState.AddModelError("OtpCode", "Mã OTP không đúng!");
                        return View(model);
                    }

                    if (otp.IsExpired)
                    {
                        ModelState.AddModelError("OtpCode", "Mã OTP đã hết hạn! Vui lòng đăng ký lại.");
                        return View(model);
                    }

                    // Active user first (Kèm tracking để update)
                    var user = await _context.Users.AsTracking().FirstOrDefaultAsync(u => u.Email == model.Email);
                    if (user != null)
                    {
                        // Sử dụng transaction để đảm bảo dữ liệu được lưu
                        using (var transaction = await _context.Database.BeginTransactionAsync())
                        {
                            try
                            {
                                user.IsActive = true;
                                
                                // Đánh dấu OTP đã sử dụng
                                otp.IsUsed = true;
                                
                                // Save changes for both user and OTP
                                await _context.SaveChangesAsync();

                                // Gửi email chào mừng
                                try
                                {
                                    await _emailService.SendWelcomeEmailAsync(model.Email, user.FullName);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError($"Error sending welcome email: {ex.Message}");
                                    // Không rollback nếu email fail vì user đã được activate
                                }

                                await transaction.CommitAsync();

                                _logger.LogInformation($"User {user.Email} verified and activated.");

                                TempData["SuccessMessage"] = "Xác thực thành công! Bạn có thể đăng nhập ngay.";
                                return RedirectToAction(nameof(Login));
                            }
                            catch (Exception ex)
                            {
                                await transaction.RollbackAsync();
                                _logger.LogError($"Error verifying OTP: {ex.Message}");
                                ModelState.AddModelError("", $"Lỗi xác thực: {ex.Message}");
                                return View(model);
                            }
                        }
                    }

                    ModelState.AddModelError("", "Không tìm thấy tài khoản.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unexpected error in VerifyOtp: {ex.Message}");
                    ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại!");
                }
            }

            return View(model);
        }

        // POST: Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }

        // GET: Auth/ExternalLogin
        [HttpGet]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            // Check if Google OAuth is configured
            if (provider == "Google")
            {
                var googleClientId = _configuration["Authentication:Google:ClientId"];
                if (string.IsNullOrEmpty(googleClientId))
                {
                    TempData["ErrorMessage"] = "Google login chưa được cấu hình. Vui lòng đăng nhập bằng tài khoản thông thường.";
                    return RedirectToAction(nameof(Login));
                }
            }
            
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl });
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };
            return Challenge(properties, provider);
        }

        // GET: Auth/ExternalLoginCallback
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            try
            {
                if (remoteError != null)
                {
                    _logger.LogWarning($"Remote error from Google: {remoteError}");
                    TempData["ErrorMessage"] = $"Lỗi từ Google: {remoteError}";
                    return RedirectToAction(nameof(Login));
                }

                // Use "Google" scheme instead of Cookie scheme to get external login info
                var info = await HttpContext.AuthenticateAsync("Google");
                if (info?.Principal == null || !info.Succeeded)
                {
                    _logger.LogWarning("External authentication failed or no principal found");
                    TempData["ErrorMessage"] = "Không thể lấy thông tin từ Google. Vui lòng thử lại.";
                    return RedirectToAction(nameof(Login));
                }

                // Lấy thông tin từ Google
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);
                var googleId = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

                _logger.LogInformation($"Google login attempt for email: {email}");

                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Email is null or empty from Google");
                    TempData["ErrorMessage"] = "Không thể lấy email từ Google.";
                    return RedirectToAction(nameof(Login));
                }

                // Kiểm tra user đã tồn tại chưa
                var user = await _context.Users
                    .Include(u => u.Customer)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    _logger.LogInformation($"New Google user, redirecting to CompleteProfile: {email}");
                    // User mới từ Google → Redirect đến trang nhập thông tin
                    TempData["GoogleEmail"] = email;
                    TempData["GoogleName"] = name ?? email;
                    TempData["GoogleId"] = googleId;
                    return RedirectToAction(nameof(CompleteProfile));
                }

                _logger.LogInformation($"Existing user found, signing in: {email}");

                // Tạo claims và đăng nhập
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role ?? "Customer")
                };

                if (user.CustomerId.HasValue)
                {
                    claims.Add(new Claim("CustomerId", user.CustomerId.Value.ToString()));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Cập nhật last login
                user.LastLoginDate = DateTime.Now;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User {user.Email} logged in via Google successfully.");

                // Redirect
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ExternalLoginCallback: {ex.Message}\n{ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi đăng nhập bằng Google. Vui lòng thử lại.";
                return RedirectToAction(nameof(Login));
            }
        }

        // GET: Auth/CompleteProfile
        [HttpGet]
        public IActionResult CompleteProfile()
        {
            var email = TempData["GoogleEmail"]?.ToString();
            var name = TempData["GoogleName"]?.ToString();

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction(nameof(Login));
            }

            TempData.Keep("GoogleEmail");
            TempData.Keep("GoogleName");
            TempData.Keep("GoogleId");

            return View(new CompleteProfileViewModel
            {
                Email = email,
                FullName = name ?? email
            });
        }

        // POST: Auth/CompleteProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteProfile(CompleteProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = TempData["GoogleEmail"]?.ToString();
            var name = TempData["GoogleName"]?.ToString();

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction(nameof(Login));
            }

            try
            {
                // Sử dụng transaction để đảm bảo dữ liệu được lưu
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Tạo customer mới với đầy đủ thông tin
                        var customer = new Customer
                        {
                            CustomerCode = GenerateCustomerCode(),
                            FullName = model.FullName,
                            Email = email,
                            PhoneNumber = model.PhoneNumber,
                            Address = model.Address,
                            City = model.City,
                            District = model.District,
                            Ward = model.Ward,
                            CustomerType = "Thường",
                            LoyaltyPoints = 0,
                            IsActive = true,
                            CreatedDate = DateTime.Now
                        };
                        _context.Customers.Add(customer);
                        await _context.SaveChangesAsync();

                        // Tạo user mới
                        var user = new User
                        {
                            Email = email,
                            FullName = model.FullName,
                            PhoneNumber = model.PhoneNumber,
                            Role = "Customer",
                            CustomerId = customer.CustomerId,
                            IsActive = true,
                            CreatedDate = DateTime.Now,
                            PasswordHash = "" // Không cần password cho Google login
                        };
                        _context.Users.Add(user);
                        await _context.SaveChangesAsync();

                        _logger.LogInformation($"New user registered via Google with complete profile: {email}");

                        // Gửi email chào mừng (không rollback nếu email fail)
                        try
                        {
                            await _emailService.SendWelcomeEmailAsync(email, model.FullName);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error sending welcome email: {ex.Message}");
                        }

                        // Commit transaction
                        await transaction.CommitAsync();

                        // Tạo claims và đăng nhập
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                            new Claim(ClaimTypes.Name, user.FullName),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Role, user.Role ?? "Customer")
                        };

                        if (user.CustomerId.HasValue)
                        {
                            claims.Add(new Claim("CustomerId", user.CustomerId.Value.ToString()));
                        }

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        // Cập nhật last login
                        user.LastLoginDate = DateTime.Now;
                        await _context.SaveChangesAsync();

                        TempData["SuccessMessage"] = "Hoàn tất đăng ký! Chào mừng bạn đến với Mộc Vị Store.";
                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError($"Error in CompleteProfile: {ex.Message}\n{ex.StackTrace}");
                        ModelState.AddModelError("", $"Lỗi đăng ký: {ex.Message}");
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error in CompleteProfile: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại!");
                return View(model);
            }
        }

        // GET: Auth/ResendOtp
        [HttpGet]
        public IActionResult ResendOtp()
        {
            var email = TempData["Email"]?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction(nameof(Login));
            }

            TempData.Keep("Email");
            ViewBag.Email = email;
            return View();
        }

        // POST: Auth/ResendOtp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendOtp(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Email không hợp lệ.";
                return RedirectToAction(nameof(Login));
            }

            try
            {
                // Kiểm tra user tồn tại và chưa active
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive == false);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy tài khoản hoặc tài khoản đã được kích hoạt.";
                    return RedirectToAction(nameof(Login));
                }

                // Sử dụng transaction
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Tạo mã OTP mới
                        var otpCode = new Random().Next(100000, 999999).ToString();

                        // Lưu OTP vào database
                        var otpVerification = new OtpVerification
                        {
                            Email = email,
                            OtpCode = otpCode,
                            CreatedAt = DateTime.Now,
                            ExpiresAt = DateTime.Now.AddMinutes(5),
                            IsUsed = false
                        };
                        _context.OtpVerifications.Add(otpVerification);
                        await _context.SaveChangesAsync();

                        // Gửi email OTP - nếu fail thì rollback
                        try
                        {
                            await _emailService.SendOtpEmailAsync(email, otpCode, user.FullName);
                            _logger.LogInformation($"OTP resent to {email}");
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            _logger.LogError($"Error resending OTP email: {ex.Message}");
                            TempData["ErrorMessage"] = "Lỗi gửi email. Vui lòng thử lại sau.";
                            return View();
                        }

                        // Commit transaction
                        await transaction.CommitAsync();

                        TempData["Email"] = email;
                        TempData["SuccessMessage"] = "Mã OTP mới đã được gửi đến email của bạn!";
                        return RedirectToAction(nameof(VerifyOtp));
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError($"Error in ResendOtp: {ex.Message}");
                        TempData["ErrorMessage"] = "Có lỗi xảy ra. Vui lòng thử lại!";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error in ResendOtp: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra. Vui lòng thử lại!";
                return View();
            }
        }

        // Helper methods
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private string GenerateCustomerCode()
        {
            var lastCustomer = _context.Customers
                .OrderByDescending(c => c.CustomerId)
                .FirstOrDefault();

            int nextNumber = 1;
            if (lastCustomer != null && !string.IsNullOrEmpty(lastCustomer.CustomerCode))
            {
                var numberPart = lastCustomer.CustomerCode.Replace("KH", "");
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"KH{nextNumber:D4}";
        }
    }
}
