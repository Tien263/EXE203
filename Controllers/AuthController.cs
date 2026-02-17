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
                // Hash password Ä‘á»ƒ so sÃ¡nh
                var passwordHash = HashPassword(model.Password);

                // TÃ¬m user theo email
                var user = await _context.Users
                    .Include(u => u.Customer)
                    .Include(u => u.Employee)
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.PasswordHash == passwordHash);

                if (user != null)
                {
                    // Check if user is not active (pending OTP verification)
                    if (user.IsActive == false)
                    {
                        TempData["Email"] = user.Email;
                        TempData["ErrorMessage"] = "TÃ i khoáº£n chÆ°a Ä‘Æ°á»£c kÃ­ch hoáº¡t. Vui lÃ²ng xÃ¡c thá»±c OTP.";
                        return RedirectToAction(nameof(ResendOtp));
                    }

                    // Táº¡o claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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

                    // Cáº­p nháº­t last login
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

                ModelState.AddModelError(string.Empty, "Email hoáº·c máº­t kháº©u khÃ´ng Ä‘Ãºng.");
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
                // Kiá»ƒm tra email Ä‘Ã£ tá»“n táº¡i
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email nÃ y Ä‘Ã£ Ä‘Æ°á»£c sá»­ dá»¥ng.");
                    return View(model);
                }

                try
                {
                    // Báº¯t Ä‘áº§u transaction
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Táº¡o mÃ£ OTP 6 sá»‘
                            var otpCode = new Random().Next(100000, 999999).ToString();

                            // LÆ°u OTP vÃ o database
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

                            // Táº¡o customer má»›i
                            var customer = new Customer
                            {
                                CustomerCode = GenerateCustomerCode(),
                                FullName = model.FullName,
                                PhoneNumber = model.PhoneNumber,
                                Email = model.Email,
                                Address = model.Address,
                                City = model.City,
                                CustomerType = "ThÆ°á»ng",
                                LoyaltyPoints = 0,
                                IsActive = true,
                                CreatedDate = DateTime.Now
                            };
                            _context.Customers.Add(customer);
                            await _context.SaveChangesAsync();

                            // Táº¡o user account (chÆ°a active)
                            var user = new User
                            {
                                Email = model.Email,
                                PasswordHash = HashPassword(model.Password),
                                FullName = model.FullName,
                                PhoneNumber = model.PhoneNumber,
                                Role = "Customer",
                                CustomerId = customer.CustomerId,
                                IsActive = false, // ChÆ°a active, cáº§n verify OTP
                                CreatedDate = DateTime.Now
                            };
                            _context.Users.Add(user);
                            await _context.SaveChangesAsync();

                            // Gá»­i email OTP - náº¿u lá»—i thÃ¬ rollback
                            try
                            {
                                await _emailService.SendOtpEmailAsync(model.Email, otpCode, model.FullName);
                                _logger.LogInformation($"OTP sent to {model.Email}");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Error sending OTP email: {ex.Message}");
                                // Rollback transaction náº¿u gá»­i email fail
                                await transaction.RollbackAsync();
                                ModelState.AddModelError("", "Lá»—i gá»­i email. Vui lÃ²ng thá»­ láº¡i sau.");
                                return View(model);
                            }

                            // Commit transaction
                            await transaction.CommitAsync();

                            TempData["Email"] = model.Email;
                            TempData["SuccessMessage"] = "Vui lÃ²ng kiá»ƒm tra email Ä‘á»ƒ láº¥y mÃ£ OTP!";
                            return RedirectToAction(nameof(VerifyOtp));
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            _logger.LogError($"Error during registration: {ex.Message}");
                            ModelState.AddModelError("", $"Lá»—i Ä‘Äƒng kÃ½: {ex.Message}");
                            return View(model);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unexpected error during registration: {ex.Message}");
                    ModelState.AddModelError("", "CÃ³ lá»—i xáº£y ra. Vui lÃ²ng thá»­ láº¡i!");
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
                    // TÃ¬m OTP
                    var otp = await _context.OtpVerifications
                        .Where(o => o.Email == model.Email && o.OtpCode == model.OtpCode && !o.IsUsed)
                        .OrderByDescending(o => o.CreatedAt)
                        .FirstOrDefaultAsync();

                    if (otp == null)
                    {
                        ModelState.AddModelError("OtpCode", "MÃ£ OTP khÃ´ng Ä‘Ãºng!");
                        return View(model);
                    }

                    if (otp.IsExpired)
                    {
                        ModelState.AddModelError("OtpCode", "MÃ£ OTP Ä‘Ã£ háº¿t háº¡n! Vui lÃ²ng Ä‘Äƒng kÃ½ láº¡i.");
                        return View(model);
                    }

                    // Active user first
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                    if (user != null)
                    {
                        // Sá»­ dá»¥ng transaction Ä‘á»ƒ Ä‘áº£m báº£o dá»¯ liá»‡u Ä‘Æ°á»£c lÆ°u
                        using (var transaction = await _context.Database.BeginTransactionAsync())
                        {
                            try
                            {
                                user.IsActive = true;
                                
                                // ÄÃ¡nh dáº¥u OTP Ä‘Ã£ sá»­ dá»¥ng
                                otp.IsUsed = true;
                                
                                // Save changes for both user and OTP
                                await _context.SaveChangesAsync();

                                // Gá»­i email chÃ o má»«ng
                                try
                                {
                                    await _emailService.SendWelcomeEmailAsync(model.Email, user.FullName);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError($"Error sending welcome email: {ex.Message}");
                                    // KhÃ´ng rollback náº¿u email fail vÃ¬ user Ä‘Ã£ Ä‘Æ°á»£c activate
                                }

                                await transaction.CommitAsync();

                                _logger.LogInformation($"User {user.Email} verified and activated.");

                                TempData["SuccessMessage"] = "XÃ¡c thá»±c thÃ nh cÃ´ng! Báº¡n cÃ³ thá»ƒ Ä‘Äƒng nháº­p ngay.";
                                return RedirectToAction(nameof(Login));
                            }
                            catch (Exception ex)
                            {
                                await transaction.RollbackAsync();
                                _logger.LogError($"Error verifying OTP: {ex.Message}");
                                ModelState.AddModelError("", $"Lá»—i xÃ¡c thá»±c: {ex.Message}");
                                return View(model);
                            }
                        }
                    }

                    ModelState.AddModelError("", "KhÃ´ng tÃ¬m tháº¥y tÃ i khoáº£n.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unexpected error in VerifyOtp: {ex.Message}");
                    ModelState.AddModelError("", "CÃ³ lá»—i xáº£y ra. Vui lÃ²ng thá»­ láº¡i!");
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
                    TempData["ErrorMessage"] = "Google login chÆ°a Ä‘Æ°á»£c cáº¥u hÃ¬nh. Vui lÃ²ng Ä‘Äƒng nháº­p báº±ng tÃ i khoáº£n thÃ´ng thÆ°á»ng.";
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
                    TempData["ErrorMessage"] = $"Lá»—i tá»« Google: {remoteError}";
                    return RedirectToAction(nameof(Login));
                }

                // Use "Google" scheme instead of Cookie scheme to get external login info
                var info = await HttpContext.AuthenticateAsync("Google");
                if (info?.Principal == null || !info.Succeeded)
                {
                    _logger.LogWarning("External authentication failed or no principal found");
                    TempData["ErrorMessage"] = "KhÃ´ng thá»ƒ láº¥y thÃ´ng tin tá»« Google. Vui lÃ²ng thá»­ láº¡i.";
                    return RedirectToAction(nameof(Login));
                }

                // Láº¥y thÃ´ng tin tá»« Google
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);
                var googleId = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

                _logger.LogInformation($"Google login attempt for email: {email}");

                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Email is null or empty from Google");
                    TempData["ErrorMessage"] = "KhÃ´ng thá»ƒ láº¥y email tá»« Google.";
                    return RedirectToAction(nameof(Login));
                }

                // Kiá»ƒm tra user Ä‘Ã£ tá»“n táº¡i chÆ°a
                var user = await _context.Users
                    .Include(u => u.Customer)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    _logger.LogInformation($"New Google user, redirecting to CompleteProfile: {email}");
                    // User má»›i tá»« Google â†’ Redirect Ä‘áº¿n trang nháº­p thÃ´ng tin
                    TempData["GoogleEmail"] = email;
                    TempData["GoogleName"] = name ?? email;
                    TempData["GoogleId"] = googleId;
                    return RedirectToAction(nameof(CompleteProfile));
                }

                _logger.LogInformation($"Existing user found, signing in: {email}");

                // Táº¡o claims vÃ  Ä‘Äƒng nháº­p
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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

                // Cáº­p nháº­t last login
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
                TempData["ErrorMessage"] = "CÃ³ lá»—i xáº£y ra khi Ä‘Äƒng nháº­p báº±ng Google. Vui lÃ²ng thá»­ láº¡i.";
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
                // Sá»­ dá»¥ng transaction Ä‘á»ƒ Ä‘áº£m báº£o dá»¯ liá»‡u Ä‘Æ°á»£c lÆ°u
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Táº¡o customer má»›i vá»›i Ä‘áº§y Ä‘á»§ thÃ´ng tin
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
                            CustomerType = "ThÆ°á»ng",
                            LoyaltyPoints = 0,
                            IsActive = true,
                            CreatedDate = DateTime.Now
                        };
                        _context.Customers.Add(customer);
                        await _context.SaveChangesAsync();

                        // Táº¡o user má»›i
                        var user = new User
                        {
                            Email = email,
                            FullName = model.FullName,
                            PhoneNumber = model.PhoneNumber,
                            Role = "Customer",
                            CustomerId = customer.CustomerId,
                            IsActive = true,
                            CreatedDate = DateTime.Now,
                            PasswordHash = "" // KhÃ´ng cáº§n password cho Google login
                        };
                        _context.Users.Add(user);
                        await _context.SaveChangesAsync();

                        _logger.LogInformation($"New user registered via Google with complete profile: {email}");

                        // Gá»­i email chÃ o má»«ng (khÃ´ng rollback náº¿u email fail)
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

                        // Táº¡o claims vÃ  Ä‘Äƒng nháº­p
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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

                        // Cáº­p nháº­t last login
                        user.LastLoginDate = DateTime.Now;
                        await _context.SaveChangesAsync();

                        TempData["SuccessMessage"] = "HoÃ n táº¥t Ä‘Äƒng kÃ½! ChÃ o má»«ng báº¡n Ä‘áº¿n vá»›i Má»™c Vá»‹ Store.";
                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError($"Error in CompleteProfile: {ex.Message}\n{ex.StackTrace}");
                        ModelState.AddModelError("", $"Lá»—i Ä‘Äƒng kÃ½: {ex.Message}");
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error in CompleteProfile: {ex.Message}");
                ModelState.AddModelError("", "CÃ³ lá»—i xáº£y ra. Vui lÃ²ng thá»­ láº¡i!");
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
                TempData["ErrorMessage"] = "Email khÃ´ng há»£p lá»‡.";
                return RedirectToAction(nameof(Login));
            }

            try
            {
                // Kiá»ƒm tra user tá»“n táº¡i vÃ  chÆ°a active
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive == false);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "KhÃ´ng tÃ¬m tháº¥y tÃ i khoáº£n hoáº·c tÃ i khoáº£n Ä‘Ã£ Ä‘Æ°á»£c kÃ­ch hoáº¡t.";
                    return RedirectToAction(nameof(Login));
                }

                // Sá»­ dá»¥ng transaction
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Táº¡o mÃ£ OTP má»›i
                        var otpCode = new Random().Next(100000, 999999).ToString();

                        // LÆ°u OTP vÃ o database
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

                        // Gá»­i email OTP - náº¿u fail thÃ¬ rollback
                        try
                        {
                            await _emailService.SendOtpEmailAsync(email, otpCode, user.FullName);
                            _logger.LogInformation($"OTP resent to {email}");
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            _logger.LogError($"Error resending OTP email: {ex.Message}");
                            TempData["ErrorMessage"] = "Lá»—i gá»­i email. Vui lÃ²ng thá»­ láº¡i sau.";
                            return View();
                        }

                        // Commit transaction
                        await transaction.CommitAsync();

                        TempData["Email"] = email;
                        TempData["SuccessMessage"] = "MÃ£ OTP má»›i Ä‘Ã£ Ä‘Æ°á»£c gá»­i Ä‘áº¿n email cá»§a báº¡n!";
                        return RedirectToAction(nameof(VerifyOtp));
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError($"Error in ResendOtp: {ex.Message}");
                        TempData["ErrorMessage"] = "CÃ³ lá»—i xáº£y ra. Vui lÃ²ng thá»­ láº¡i!";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error in ResendOtp: {ex.Message}");
                TempData["ErrorMessage"] = "CÃ³ lá»—i xáº£y ra. Vui lÃ²ng thá»­ láº¡i!";
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

