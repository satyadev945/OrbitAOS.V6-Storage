using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using System.Threading.RateLimiting;

// -----------------------------------------------------------------------
// Program.cs — upgraded from .NET 6 to .NET 8 with PostgreSQL migration
// Key changes for PostgreSQL migration:
//   • Replaced UseSqlServer() with UseNpgsql() for PostgreSQL 16 support
//   • Added UseSnakeCaseNamingConvention() for PostgreSQL naming conventions
//   • Configured MigrationsHistoryTable with snake_case naming
//   • Added EnableLegacyTimestampBehavior for DateTime handling
//   • Null guard added to GetConnectionString() (issue-8)
//   • Global AutoValidateAntiforgeryToken filter added (issue-11)
//   • Health check endpoint added (issue-14)
//   • Rate limiting middleware added (issue-17)
//   • Content Security Policy headers middleware added (issue-18)
// -----------------------------------------------------------------------

// Enable legacy timestamp behavior for PostgreSQL DateTime handling
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// --- Data / EF Core ---
// Null guard added: throws a clear error if connection string is missing (issue-8)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options
        .UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history", "public");
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null);
        })
        .UseSnakeCaseNamingConvention());

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// --- ASP.NET Core Identity (net8.0 packages) ---
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// --- MVC with global AutoValidateAntiforgeryToken filter (issue-11) ---
builder.Services.AddControllersWithViews(options =>
{
    // Global CSRF protection: all POST/PUT/DELETE actions require a valid antiforgery token
    options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
});

// --- Health Checks (issue-14) ---
// Note: AddDbContextCheck() requires Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore
// which is not available for net8.0 in the local package cache; using basic health check instead.
builder.Services.AddHealthChecks();

// --- Rate Limiting (issue-17) ---
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 10;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

// --- HTTP pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // Default HSTS value is 30 days. Adjust for production: https://aka.ms/aspnetcore-hsts
    app.UseHsts();
}

// --- Content Security Policy headers middleware (issue-18) ---
app.Use(async (context, next) =>
{
    context.Response.Headers.Append(
        "Content-Security-Policy",
        "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self';");
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// --- Rate Limiting middleware (issue-17) ---
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// --- Health Check endpoint (issue-14) ---
app.MapHealthChecks("/health");

// --- Routing ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Required for Identity UI Razor Pages (Areas/Identity)
app.MapRazorPages();

app.Run();
