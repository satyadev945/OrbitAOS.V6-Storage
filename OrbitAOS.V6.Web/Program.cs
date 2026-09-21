using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Application.Common;
using OrbitAOS.V6.Web.Configuration;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Application.Services;
using OrbitAOS.V6.Infrastructure.Data;
using OrbitAOS.V6.Infrastructure.Identity;
using OrbitAOS.V6.Infrastructure.Repositories;
using OrbitAOS.V6.Web.Filters;
using OrbitAOS.V6.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────────────────────
// 0. Strongly-typed configuration (replaces web.config appSettings)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

// ─────────────────────────────────────────────────────────────────────────────
// 1. Database Context (replaces web.config connectionStrings + EF6 DbContext)
// ─────────────────────────────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ─────────────────────────────────────────────────────────────────────────────
// 2. ASP.NET Core Identity (replaces Forms Authentication + ASP.NET Identity 2.x)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    // Password policy (replaces web.config membership settings)
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 4;

    // Lockout policy
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User policy
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ─────────────────────────────────────────────────────────────────────────────
// 3. Cookie Authentication Settings (replaces Forms Authentication in web.config)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Name = "OrbitAOS.Auth";
});

// ─────────────────────────────────────────────────────────────────────────────
// 4. AutoMapper (replaces manual property mapping in legacy controllers)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ─────────────────────────────────────────────────────────────────────────────
// 5. Repository Pattern and Unit of Work (replaces direct DbContext in controllers)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ─────────────────────────────────────────────────────────────────────────────
// 6. Application Services (business logic layer)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddScoped<ISampleService, SampleService>();

// ─────────────────────────────────────────────────────────────────────────────
// 7. MVC with Global Filters (replaces FilterConfig.cs in App_Start)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews(options =>
{
    // Global exception filter (replaces Application_Error in Global.asax)
    options.Filters.Add<GlobalExceptionFilter>();
    // Global action logging filter (replaces legacy action filters)
    options.Filters.Add<ActionLoggingFilter>();
});

builder.Services.AddRazorPages();

// ─────────────────────────────────────────────────────────────────────────────
// 8. Session Support (replaces System.Web.SessionState)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.Name = "OrbitAOS.Session";
});

// ─────────────────────────────────────────────────────────────────────────────
// 9. Response Caching (replaces OutputCache attribute)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddResponseCaching();

// ─────────────────────────────────────────────────────────────────────────────
// 10. HTTP Context Accessor (replaces HttpContext.Current in legacy code)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();

// ─────────────────────────────────────────────────────────────────────────────
// 11. Logging (replaces log4net / NLog configuration in web.config)
// ─────────────────────────────────────────────────────────────────────────────
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
if (!builder.Environment.IsDevelopment())
{
    builder.Logging.AddEventLog();
}

var app = builder.Build();

// ─────────────────────────────────────────────────────────────────────────────
// 12. HTTP Request Pipeline (replaces Global.asax Application_Start and HTTP Modules)
// ─────────────────────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    // Replaces Application_Error in Global.asax
    app.UseExceptionHandler("/Home/Error");
    // HSTS: 30 days in production (replaces web.config HTTPS redirect)
    app.UseHsts();
}

// Custom request logging middleware (replaces IHttpModule BeginRequest/EndRequest)
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Response caching middleware
app.UseResponseCaching();

// Session middleware (must be before UseAuthentication)
app.UseSession();

// Authentication and Authorization (replaces FormsAuthentication)
app.UseAuthentication();
app.UseAuthorization();

// ─────────────────────────────────────────────────────────────────────────────
// 13. Route Configuration (replaces RouteConfig.cs in App_Start)
// ─────────────────────────────────────────────────────────────────────────────
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
