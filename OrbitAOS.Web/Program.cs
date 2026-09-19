using Microsoft.AspNetCore.Identity;
using OrbitAOS.Application;
using OrbitAOS.Infrastructure;
using OrbitAOS.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------
// Logging - .NET 8 built-in logging (replaces legacy log4net/NLog)
// -----------------------------------------------------------------------
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// -----------------------------------------------------------------------
// Infrastructure layer: EF Core 8, repositories, unit of work
// Replaces legacy EntityFramework 6.x with EF Core 8.0.0
// -----------------------------------------------------------------------
builder.Services.AddInfrastructureServices(builder.Configuration);

// -----------------------------------------------------------------------
// Application layer: business services
// -----------------------------------------------------------------------
builder.Services.AddApplicationServices();

// -----------------------------------------------------------------------
// ASP.NET Core Identity (upgraded from net6.0 to net8.0)
// Replaces legacy ASP.NET Identity 2.x / Forms Authentication
// -----------------------------------------------------------------------
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// -----------------------------------------------------------------------
// MVC with views - ASP.NET Core MVC on .NET 8
// Replaces legacy System.Web.Mvc
// -----------------------------------------------------------------------
builder.Services.AddControllersWithViews();

// -----------------------------------------------------------------------
// Session support (ASP.NET Core session replaces legacy System.Web.SessionState)
// -----------------------------------------------------------------------
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// -----------------------------------------------------------------------
// Response caching - replaces legacy OutputCache attribute
// -----------------------------------------------------------------------
builder.Services.AddResponseCaching();

var app = builder.Build();

// -----------------------------------------------------------------------
// HTTP request pipeline
// Replaces legacy Global.asax Application_Start and HTTP Modules
// -----------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // HSTS: 30-day default; adjust for production
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseResponseCaching();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// -----------------------------------------------------------------------
// Routing: replaces legacy App_Start/RouteConfig.cs
// -----------------------------------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
