// =============================================================================
// Program.cs - ASP.NET Core MVC .NET 8 Entry Point
// MVC-Only Architecture (Single Project)
//
// Migration Notes:
//   - Upgraded from net7.0 to net8.0
//   - Replaces legacy Global.asax, App_Start/RouteConfig.cs,
//     App_Start/FilterConfig.cs, App_Start/BundleConfig.cs
//   - Uses ASP.NET Core Identity (replaces Forms Authentication)
//   - Configures EF Core 8 (replaces EF6)
//   - Adds AutoMapper, FluentValidation, MediatR
// =============================================================================

using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Domain.Interfaces;
using OrbitAOS.V6.Filters;
using OrbitAOS.V6.Infrastructure.Persistence;
using OrbitAOS.V6.Infrastructure.Repositories;
using OrbitAOS.V6.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------------------------------------
// 1. LOGGING (.NET 8)
//    Migration: log4net/NLog → Microsoft.Extensions.Logging
// -------------------------------------------------------------------------
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// -------------------------------------------------------------------------
// 2. DATABASE & EF CORE 8 CONFIGURATION
//    Migration: web.config connectionStrings → appsettings.json
//    Migration: EF6 DbContext → EF Core 8 DbContext
// -------------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        // .NET 8 EF Core: enable retry on transient failures
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });

    // Development-only: enable sensitive data logging for debugging
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// EF Core developer page exception filter (.NET 8)
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// -------------------------------------------------------------------------
// 3. ASP.NET CORE IDENTITY CONFIGURATION
//    Migration: ASP.NET Identity 2.x → ASP.NET Core Identity on .NET 8
//    Migration: Forms Authentication → Cookie Authentication
// -------------------------------------------------------------------------
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Password policy (replaces web.config membership settings)
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // Sign-in settings
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;

    // Lockout settings (replaces web.config lockout configuration)
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Cookie Authentication configuration
// Migration: Forms Authentication (web.config) → Cookie Authentication (Program.cs)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// -------------------------------------------------------------------------
// 4. MVC & RAZOR PAGES CONFIGURATION
//    Migration: System.Web.Mvc → Microsoft.AspNetCore.Mvc
// -------------------------------------------------------------------------
builder.Services.AddControllersWithViews(options =>
{
    // Global exception filter (replaces App_Start/FilterConfig.cs)
    options.Filters.Add<GlobalExceptionFilter>();
});

builder.Services.AddRazorPages();

// Register FluentValidation validators from the current assembly
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// -------------------------------------------------------------------------
// 5. MEDIATR FOR CQRS PATTERN
// -------------------------------------------------------------------------
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// -------------------------------------------------------------------------
// 6. AUTOMAPPER FOR OBJECT MAPPING
// -------------------------------------------------------------------------
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// -------------------------------------------------------------------------
// 7. REPOSITORY & UNIT OF WORK PATTERN
//    Replaces direct DbContext usage in controllers
// -------------------------------------------------------------------------
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// -------------------------------------------------------------------------
// 8. SESSION CONFIGURATION
//    Migration: System.Web.SessionState → ASP.NET Core Session
// -------------------------------------------------------------------------
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// -------------------------------------------------------------------------
// 9. RESPONSE CACHING
//    Migration: [OutputCache] attribute → Response Caching middleware
// -------------------------------------------------------------------------
builder.Services.AddResponseCaching();

// -------------------------------------------------------------------------
// 10. HEALTH CHECKS (.NET 8 best practice)
// -------------------------------------------------------------------------
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

// =========================================================================
// BUILD THE APPLICATION
// =========================================================================
var app = builder.Build();

// -------------------------------------------------------------------------
// 11. HTTP REQUEST PIPELINE
//     Migration: Global.asax Application_Start → Program.cs middleware
//     Migration: HTTP Modules → Middleware
// -------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    // Migration: Application_Error in Global.asax → UseExceptionHandler
    app.UseExceptionHandler("/Home/Error");
    // HSTS: .NET 8 security best practice
    app.UseHsts();
}

// Custom exception handling middleware
app.UseMiddleware<OrbitAOS.V6.Middleware.ExceptionHandlingMiddleware>();

// HTTPS Redirection (replaces web.config HTTPS redirect rules)
app.UseHttpsRedirection();

// Static Files (replaces /Content and /Scripts folders)
// Migration: BundleConfig.cs → wwwroot static files
app.UseStaticFiles();

// Response Caching middleware
// Migration: [OutputCache] → Response Caching
app.UseResponseCaching();

// Routing middleware
// Migration: App_Start/RouteConfig.cs → UseRouting + MapControllerRoute
app.UseRouting();

// Session middleware
// Migration: System.Web.SessionState → UseSession
app.UseSession();

// Authentication & Authorization
// Migration: FormsAuthentication → UseAuthentication + UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

// -------------------------------------------------------------------------
// 12. ROUTE CONFIGURATION
//     Migration: App_Start/RouteConfig.cs → MapControllerRoute
// -------------------------------------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Razor Pages for Identity UI
app.MapRazorPages();

// Health check endpoint
app.MapHealthChecks("/health");

// =========================================================================
// RUN THE APPLICATION
// =========================================================================
app.Run();
