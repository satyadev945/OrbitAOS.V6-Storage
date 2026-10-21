// ============================================================
// Program.cs — .NET 8 ASP.NET Core MVC entry point
// Replaces: Global.asax, Global.asax.cs, App_Start/RouteConfig.cs,
//           App_Start/FilterConfig.cs, App_Start/BundleConfig.cs
// Architecture: Clean Architecture (Domain / Application / Infrastructure / Web)
// ============================================================

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Infrastructure.Data;
using OrbitAOS.V6.Infrastructure.Identity;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Application.Services;
using OrbitAOS.V6.Infrastructure.Repositories;
using OrbitAOS.V6.Web.Middleware;
using Serilog;

// ── Serilog bootstrap logger (captures startup errors) ──────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ── Serilog full configuration from appsettings.json ────────────────────
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("Logs/orbitaos-.log", rollingInterval: RollingInterval.Day));

    // ── Database / EF Core ──────────────────────────────────────────────────
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            // Enable retry-on-transient-failure for resilience
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });
        // Enable sensitive data logging only in Development
        if (builder.Environment.IsDevelopment())
            options.EnableSensitiveDataLogging();
    });

    // Developer exception page for EF Core migrations
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    // ── ASP.NET Core Identity ────────────────────────────────────────────────
    // Upgraded from AddDefaultIdentity to AddIdentity for full role support
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Password policy (adjust to match legacy requirements)
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;

        // Sign-in policy
        options.SignIn.RequireConfirmedAccount = true;
        options.SignIn.RequireConfirmedEmail = true;

        // Lockout policy
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // User settings
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI(); // Scaffolded Razor Pages for Identity UI

    // ── Cookie Authentication (replaces Forms Authentication) ───────────────
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.LoginPath = "/Identity/Account/Login";
        options.LogoutPath = "/Identity/Account/Logout";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    });

    // ── Session State ────────────────────────────────────────────────────────
    builder.Services.AddDistributedMemoryCache(); // Use Redis in production
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

    // ── Application Services (Clean Architecture — Application Layer) ────────
    builder.Services.AddScoped<IHomeService, HomeService>();

    // ── Infrastructure Repositories (Clean Architecture — Infrastructure Layer)
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

    // ── MVC with Views ───────────────────────────────────────────────────────
    builder.Services.AddControllersWithViews(options =>
    {
        // Global exception filter (replaces FilterConfig.cs)
        options.Filters.Add<Web.Filters.GlobalExceptionFilter>();
    });

    // ── Razor Pages (for Identity UI scaffolded pages) ───────────────────────
    builder.Services.AddRazorPages();

    // ── Health Checks ────────────────────────────────────────────────────────
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<ApplicationDbContext>();

    // ── Response Caching (replaces [OutputCache] attribute) ─────────────────
    builder.Services.AddResponseCaching();

    // ── CORS (configure as needed) ───────────────────────────────────────────
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
            policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
                  .AllowAnyHeader()
                  .AllowAnyMethod());
    });

    // ── Anti-forgery ─────────────────────────────────────────────────────────
    builder.Services.AddAntiforgery(options =>
    {
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

    // ────────────────────────────────────────────────────────────────────────
    var app = builder.Build();
    // ────────────────────────────────────────────────────────────────────────

    // ── HTTP Pipeline ────────────────────────────────────────────────────────
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseMigrationsEndPoint();
    }
    else
    {
        // Production error handler
        app.UseExceptionHandler("/Home/Error");
        // HSTS — 1 year for production (was 30 days in .NET 6 template)
        app.UseHsts();
    }

    // ── Custom Middleware (replaces HTTP Modules) ────────────────────────────
    app.UseMiddleware<RequestLoggingMiddleware>();

    app.UseHttpsRedirection();

    // Static files served from wwwroot (replaces /Content, /Scripts)
    app.UseStaticFiles();

    app.UseRouting();

    app.UseCors();

    app.UseResponseCaching();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseSession();

    // ── Health Check endpoint ────────────────────────────────────────────────
    app.MapHealthChecks("/health");

    // ── MVC Routes (replaces App_Start/RouteConfig.cs) ───────────────────────
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // ── Razor Pages (Identity UI) ────────────────────────────────────────────
    app.MapRazorPages();

    // ── Serilog request logging ──────────────────────────────────────────────
    app.UseSerilogRequestLogging();

    Log.Information("OrbitAOS.V6 application starting on .NET 8");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
