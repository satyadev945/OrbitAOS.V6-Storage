using Microsoft.AspNetCore.Identity;
using OrbitAOS.Application.Common;
using OrbitAOS.Infrastructure;
using OrbitAOS.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// ── Infrastructure layer (EF Core 8, repositories) ───────────────────────────
builder.Services.AddInfrastructureServices(builder.Configuration);

// ── Application layer (business services) ────────────────────────────────────
builder.Services.AddApplicationServices();

// ── ASP.NET Core Identity (upgraded to net8.0) ───────────────────────────────
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 8;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ── MVC with Views ────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews(options =>
{
    // Global anti-forgery token validation for all POST/PUT/DELETE actions
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});

// ── Response caching ──────────────────────────────────────────────────────────
builder.Services.AddResponseCaching();

// ── HTTP context accessor (for accessing HttpContext in services) ─────────────
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ── HTTP request pipeline ─────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // HSTS: 30-day default; adjust for production as needed.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// ── Security headers middleware ───────────────────────────────────────────────
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseResponseCaching();

// ── Routing: conventional MVC + Razor Pages (Identity UI) ────────────────────
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

// Expose Program class for integration testing
public partial class Program { }
