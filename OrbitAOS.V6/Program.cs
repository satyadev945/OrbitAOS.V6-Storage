using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Application.Services;
using OrbitAOS.V6.Infrastructure.Middleware;

// ============================================================
// Program.cs – ASP.NET Core MVC on .NET 8
// Replaces: Global.asax, App_Start/RouteConfig.cs,
//           App_Start/FilterConfig.cs, App_Start/BundleConfig.cs
// ============================================================

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------------
// 1. DATA LAYER – EF Core 8 + ASP.NET Core Identity
// ------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ------------------------------------------------------------------
// 2. IDENTITY – ASP.NET Core Identity (replaces ASP.NET Identity 2.x)
// ------------------------------------------------------------------
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    // Password policy (replaces web.config membership settings)
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// ------------------------------------------------------------------
// 3. COOKIE AUTHENTICATION (replaces Forms Authentication)
// ------------------------------------------------------------------
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// ------------------------------------------------------------------
// 4. MVC + RAZOR PAGES (replaces System.Web.Mvc)
// ------------------------------------------------------------------
builder.Services.AddControllersWithViews(options =>
{
    // Global filters (replaces App_Start/FilterConfig.cs)
    // options.Filters.Add<CustomExceptionFilter>();
});
builder.Services.AddRazorPages();

// ------------------------------------------------------------------
// 5. SESSION STATE (replaces System.Web.SessionState)
// ------------------------------------------------------------------
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ------------------------------------------------------------------
// 6. RESPONSE CACHING (replaces [OutputCache] attribute)
// ------------------------------------------------------------------
builder.Services.AddResponseCaching();

// ------------------------------------------------------------------
// 7. APPLICATION SERVICES (Dependency Injection – replaces DependencyResolver)
// ------------------------------------------------------------------
builder.Services.AddScoped<IHomeService, HomeService>();

// ------------------------------------------------------------------
// 8. HTTP CONTEXT ACCESSOR (replaces HttpContext.Current)
// ------------------------------------------------------------------
builder.Services.AddHttpContextAccessor();

// ------------------------------------------------------------------
// 9. LOGGING (built-in, replaces log4net / ELMAH)
// ------------------------------------------------------------------
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ============================================================
// BUILD THE APPLICATION
// ============================================================
var app = builder.Build();

// ------------------------------------------------------------------
// 10. MIDDLEWARE PIPELINE (replaces HTTP Modules / Global.asax events)
// ------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    // Replaces Application_Error in Global.asax
    app.UseExceptionHandler("/Home/Error");
    // HSTS (HTTP Strict Transport Security)
    app.UseHsts();
}

// Custom request logging middleware (replaces IHttpModule)
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

// Static files (replaces /Content and /Scripts folders)
app.UseStaticFiles();

app.UseRouting();

// Response caching middleware
app.UseResponseCaching();

// Session middleware
app.UseSession();

// Authentication & Authorization (replaces FormsAuthentication)
app.UseAuthentication();
app.UseAuthorization();

// ------------------------------------------------------------------
// 11. ROUTING (replaces App_Start/RouteConfig.cs)
// ------------------------------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
