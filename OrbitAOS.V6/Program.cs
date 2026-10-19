// ============================================================
// Program.cs - Application Entry Point (.NET 8 Clean Architecture)
// Replaces: Global.asax, App_Start/RouteConfig.cs,
//           App_Start/FilterConfig.cs, App_Start/BundleConfig.cs
// ============================================================
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using OrbitAOS.V6.Domain.Interfaces;
using OrbitAOS.V6.Infrastructure.Repositories;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------------------
// 1. DATA LAYER - EF Core DbContext (upgraded to .NET 8)
// -------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found in appsettings.json.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        // Enable retry on transient failures (recommended for production)
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });
});

// Developer exception page for EF Core migrations (development only)
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// -------------------------------------------------------
// 2. IDENTITY - ASP.NET Core Identity (.NET 8)
//    Migrated from ASP.NET Identity 2.x / Forms Authentication
// -------------------------------------------------------
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Sign-in requirements
    options.SignIn.RequireConfirmedAccount = true;

    // Password policy (align with legacy web.config membership settings)
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Cookie authentication configuration (replaces Forms Authentication from web.config)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// -------------------------------------------------------
// 3. CLEAN ARCHITECTURE - Dependency Injection Registration
//    Domain Interfaces → Infrastructure Implementations
//    Application Services → Business Logic Layer
// -------------------------------------------------------

// Domain / Infrastructure layer registrations
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Application service layer registrations
builder.Services.AddScoped<IHomeService, HomeService>();

// -------------------------------------------------------
// 4. MVC SERVICES - Controllers + Views + Razor Pages
// -------------------------------------------------------
builder.Services.AddControllersWithViews(options =>
{
    // Global filters can be added here (replaces App_Start/FilterConfig.cs)
    // options.Filters.Add<GlobalExceptionFilter>();
});

// Razor Pages required for Identity UI scaffolding
builder.Services.AddRazorPages();

// -------------------------------------------------------
// 5. ADDITIONAL SERVICES
// -------------------------------------------------------

// Session support (replaces System.Web.SessionState)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// HTTP Context Accessor (replaces HttpContext.Current from System.Web)
builder.Services.AddHttpContextAccessor();

// Response caching (replaces [OutputCache] attribute from System.Web.Mvc)
builder.Services.AddResponseCaching();

// -------------------------------------------------------
// 6. BUILD THE APPLICATION
// -------------------------------------------------------
var app = builder.Build();

// -------------------------------------------------------
// 7. MIDDLEWARE PIPELINE (replaces HTTP Modules from web.config)
// -------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    // Show detailed EF Core migration errors in development
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    // Production error handling (replaces customErrors in web.config)
    app.UseExceptionHandler("/Home/Error");

    // HTTP Strict Transport Security (HSTS)
    // Default is 30 days; adjust for production: https://aka.ms/aspnetcore-hsts
    app.UseHsts();
}

// Redirect HTTP to HTTPS (replaces requireSSL in web.config)
app.UseHttpsRedirection();

// Serve static files from wwwroot (replaces /Content, /Scripts folders)
app.UseStaticFiles();

// Routing middleware (replaces App_Start/RouteConfig.cs)
app.UseRouting();

// Response caching middleware
app.UseResponseCaching();

// Session middleware (must be after UseRouting, before UseAuthentication)
app.UseSession();

// Authentication middleware (replaces Forms Authentication module)
app.UseAuthentication();

// Authorization middleware
app.UseAuthorization();

// -------------------------------------------------------
// 8. ENDPOINT ROUTING (replaces RouteConfig.cs MapRoute)
// -------------------------------------------------------

// Default MVC controller route (preserves legacy {controller}/{action}/{id?} pattern)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Razor Pages for Identity UI (Account/Manage pages)
app.MapRazorPages();

app.Run();
