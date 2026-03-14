using Exe_Demo.Data;
using Exe_Demo.Services;
using Exe_Demo.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
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
// builder.Services.AddControllersWithViews(); // Removed duplicate call

// Use Distributed Memory Cache for Session state (Much faster for single-node deployments and eliminates connection timeouts)
builder.Services.AddDistributedMemoryCache();
Console.WriteLine("--> Using Distributed Memory Cache for Sessions and Performance");


// Add Response Caching
// builder.Services.AddResponseCaching(); // DISABLE CACHING TO FIX AUTH ISSUE

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
// Add DbContext - Prioritize SQLite for Docker/VPS deployments without a dedicated SQL server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    // Check for PostgreSQL (Npgsql) keywords
    bool isPostgres = !string.IsNullOrEmpty(connectionString) && 
                     (connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase) || 
                      connectionString.Contains("Port=", StringComparison.OrdinalIgnoreCase) ||
                      connectionString.Contains("Username=", StringComparison.OrdinalIgnoreCase) ||
                      connectionString.Contains("SSL Mode=", StringComparison.OrdinalIgnoreCase));

    // Check for SQL Server (SqlClient) keywords
    // We favor PostgreSQL if both are present in some weird string, as it's more common in Docker
    bool isSqlServer = !isPostgres && !string.IsNullOrEmpty(connectionString) && 
                      (connectionString.Contains("SQLEXPRESS", StringComparison.OrdinalIgnoreCase) || 
                       connectionString.Contains("localdb", StringComparison.OrdinalIgnoreCase) ||
                       connectionString.Contains("TrustServerCertificate", StringComparison.OrdinalIgnoreCase) ||
                       connectionString.Contains("Trusted_Connection", StringComparison.OrdinalIgnoreCase) ||
                       connectionString.Contains("Integrated Security", StringComparison.OrdinalIgnoreCase) ||
                       connectionString.Contains("Initial Catalog=", StringComparison.OrdinalIgnoreCase));

    if (isPostgres)
    {
        Console.WriteLine("--> [DB_MODE] Selected Provider: PostgreSQL");
        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null);
        });
    }
    else if (isSqlServer)
    {
        Console.WriteLine("--> [DB_MODE] Selected Provider: SQL Server");
        options.UseSqlServer(connectionString);
    }
    else
    {
        // Fallback to SQLite (Perfect for single-node Docker deployments like this one)
        var dbFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "DbStorage");
        if (!Directory.Exists(dbFolderPath))
        {
            Directory.CreateDirectory(dbFolderPath);
        }
        var dbPath = Path.Combine(dbFolderPath, "mocvistore.db");
        Console.WriteLine($"--> [DB_MODE] Selected Provider: SQLite (Path: {dbPath})");
        options.UseSqlite($"Data Source={dbPath}");
    }
    
    if (string.IsNullOrEmpty(connectionString)) {
        Console.WriteLine("--> [WARNING] DefaultConnection string is EMPTY or NULL. Using SQLite fallback.");
    }
    
    // Performance optimization: Use NoTracking by default for read-only queries
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
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddScoped<Microsoft.AspNetCore.Identity.IPasswordHasher<Exe_Demo.Models.User>, Microsoft.AspNetCore.Identity.PasswordHasher<Exe_Demo.Models.User>>();
builder.Services.AddScoped<Exe_Demo.Helpers.StaffAccountHelper>();

// Add HttpClient for AI Proxy
builder.Services.AddHttpClient();

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
        
        // Configure cookies to work with HTTP (not just HTTPS)
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
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
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; 
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

// Add Data Protection to persist keys across restarts (Fix CryptographicException)
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "dataprotection-keys")));

var app = builder.Build();

// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Self-Healing Mechanism for SQLite
        // If we are using SQLite and the schema is fundamentally broken (e.g., from an old version before Identity)
        // We need to nuke it and recreate it because SQL Server migrations can't run on SQLite
        bool needsRecreate = false;
        if (context.Database.IsSqlite())
        {
             try 
             {
                 // Try a simple query that requires the latest schema (e.g., checking for Identity columns)
                 context.Users.FirstOrDefault();
             }
             catch(Exception)
             {
                 Console.WriteLine("--> [WARNING] Obsolete SQLite schema detected. Initiating database nuke & rebuild...");
                 needsRecreate = true;
             }
        }

        if (needsRecreate)
        {
             context.Database.EnsureDeleted();
             context.Database.EnsureCreated();
             Console.WriteLine("--> [SUCCESS] Obsolete SQLite database was successfully wiped and recreated with the latest schema.");
        }
        else 
        {
             context.Database.EnsureCreated();
        }

        // MANUAL PATCH: Fix missing column in Production (PostgreSQL uses double quotes)
        try 
        {
            if (context.Database.IsNpgsql())
            {
                 context.Database.ExecuteSqlRaw("ALTER TABLE \"Reviews\" ADD COLUMN IF NOT EXISTS \"MediaUrl\" TEXT;");
            }
            else 
            {
                 // For SQL Server/SQLite, check if column exists before adding
                 try
                 {
                     context.Database.ExecuteSqlRaw("ALTER TABLE Reviews ADD COLUMN MediaUrl TEXT;");
                 }
                 catch (Exception) { /* Column already exists - safe to ignore */ }
            }
        } 
        catch (Exception ex) { Console.WriteLine($"MediaUrl column patch: {ex.Message} (safe to ignore)"); }

        // Seed initial data if needed
        try
        {
            DatabaseSeeder.SeedData(context);
            Console.WriteLine("--> Database seeded successfully.");
        }
        catch (Exception seedEx)
        {
            Console.WriteLine($"--> Database seeding error: {seedEx.Message}");
        }

        Console.WriteLine("Database initialized successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing database: {ex.Message}");
    }
}

// Handle forwarded headers from Nginx reverse proxy (MUST be before other middleware)
app.UseForwardedHeaders();

// Configure static files with cache control
if (app.Environment.IsDevelopment())
{
    // In development: disable caching to always get fresh files
    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            // Disable caching for JS and CSS files in development
            if (ctx.File.Name.EndsWith(".js") || ctx.File.Name.EndsWith(".css"))
            {
                ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
                ctx.Context.Response.Headers.Append("Pragma", "no-cache");
                ctx.Context.Response.Headers.Append("Expires", "0");
            }
        }
    });
}
else
{
    // In production: enable caching for performance
    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            // Cache static files for 7 days in production
            ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=604800");
        }
    });
}

// Add Response Caching middleware (must be before UseRouting)
// app.UseResponseCaching(); // DISABLE CACHING TO FIX AUTH ISSUE

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
