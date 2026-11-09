using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;

// -----------------------------------------------------------------------
// Program.cs — upgraded from .NET 6 to .NET 10
// Key changes for .NET 10:
//   • No breaking API changes in the minimal-hosting model for this scope.
//   • AddDatabaseDeveloperPageExceptionFilter() is still valid in .NET 10.
//   • UseMigrationsEndPoint() is still valid in .NET 10 (dev only).
//   • UseHsts() default value is still 30 days; adjust for production.
//   • MapRazorPages() retained for Identity UI Razor Pages (Areas/Identity).
// -----------------------------------------------------------------------

var builder = WebApplication.CreateBuilder(args);

// --- Data / EF Core ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// --- ASP.NET Core Identity (upgraded to .NET 10 packages) ---
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// --- MVC ---
builder.Services.AddControllersWithViews();

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// --- Routing ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Required for Identity UI Razor Pages (Areas/Identity)
app.MapRazorPages();

app.Run();
