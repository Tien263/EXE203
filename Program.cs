using Exe_Demo.Data;
using Exe_Demo.Services;
using Exe_Demo.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on PORT environment variable (for Render)
// Use 5241 for development, PORT env var for production
var port = Environment.GetEnvironmentVariable("PORT") ?? "5241";
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Memory Cache for performance optimization
builder.Services.AddMemoryCache();

// Add Response Caching
builder.Services.AddResponseCaching();

// Configure Response Caching options
builder.Services.AddControllersWithViews(options =>
{
    options.CacheProfiles.Add("Default30",
        new Microsoft.AspNetCore.Mvc.CacheProfile
        {
            Duration = 1800, // 30 minutes
            Location = Microsoft.AspNetCore.Mvc.ResponseCacheLocation.Any
        });
    options.CacheProfiles.Add("Never",
        new Microsoft.AspNetCore.Mvc.CacheProfile
        {
            Location = Microsoft.AspNetCore.Mvc.ResponseCacheLocation.None,
            NoStore = true
        });
});

// Add DbContext - Use SQLite in Production, SQL Server in Development
// With Query Tracking optimization
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (builder.Environment.IsProduction())
    {
        // Always use SQLite in Production
        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "mocvistore.db");
        options.UseSqlite($"Data Source={dbPath}");
        Console.WriteLine($"Using SQLite database at: {dbPath}");
    }
    else
    {
        // Use SQL Server in Development
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(connectionString))
        {
            options.UseSqlServer(connectionString);
            Console.WriteLine("Using SQL Server database");
        }
        else
        {
            // Fallback to SQLite if no connection string in Development
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "mocvistore.db");
            options.UseSqlite($"Data Source={dbPath}");
            Console.WriteLine($"Using SQLite database at: {dbPath}");
        }
    }
    
    // Performance optimization: Use NoTracking by default for read-only queries
    // Individual queries can override this when needed
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

// Register Repository Pattern & Unit of Work (SOLID: Dependency Inversion)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Services (SOLID: Single Responsibility)
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();

// Add Authentication
var authBuilder = builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
    });

// Only add Google OAuth if configured
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
{
    Console.WriteLine("✅ Google OAuth is configured");
    authBuilder.AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.CallbackPath = new PathString("/signin-google");
        options.SaveTokens = true;
        
        // Handle OAuth failures gracefully - only redirect on actual failures
        options.Events.OnRemoteFailure = context =>
        {
            var errorMessage = context.Failure?.Message ?? "Unknown error";
            
            // Log the error for debugging
            Console.WriteLine($"Google OAuth Error: {errorMessage}");
            
            // Only redirect if it's a real failure (not just user cancellation)
            if (context.Failure != null && !errorMessage.Contains("Correlation failed"))
            {
                context.Response.Redirect("/Auth/Login?error=oauth_failed&message=" + Uri.EscapeDataString(errorMessage));
                context.HandleResponse();
            }
            else
            {
                // For correlation failures, just redirect to login without error
                context.Response.Redirect("/Auth/Login");
                context.HandleResponse();
            }
            
            return Task.CompletedTask;
        };
        
        // Production-ready cookie settings with more lenient settings for development
        options.CorrelationCookie.SameSite = SameSiteMode.Lax;
        options.CorrelationCookie.SecurePolicy = builder.Environment.IsDevelopment() 
            ? CookieSecurePolicy.SameAsRequest 
            : CookieSecurePolicy.Always;
        options.CorrelationCookie.IsEssential = true;
        
        // Add required scopes
        options.Scope.Add("profile");
        options.Scope.Add("email");
    });
}
else
{
    Console.WriteLine("⚠️  Google OAuth is NOT configured - Google login will be disabled");
    Console.WriteLine("   To enable: Add ClientId and ClientSecret in appsettings.json");
}

// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure forwarded headers for HTTPS behind proxy (Render)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Apply any pending EF Core migrations to keep database schema in sync
        context.Database.Migrate();

        // Seed initial data if needed
        try
        {
            DatabaseSeeder.SeedData(context);
        }
        catch (Exception seedEx)
        {
            Console.WriteLine($"Database seeding skipped/failed: {seedEx.Message}");
        }

        Console.WriteLine("Database initialized successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing database: {ex.Message}");
    }
}

// Use forwarded headers FIRST
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

// Add Response Caching middleware (must be before UseRouting)
app.UseResponseCaching();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
