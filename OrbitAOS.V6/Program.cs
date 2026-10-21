// ============================================================
// Program.cs – ASP.NET Core MVC on .NET 8
// Clean Architecture entry point (Web / Presentation layer)
//
// Migration notes:
//   • Upgraded from net6.0 → net8.0
//   • All package references bumped to 8.0.x
//   • Clean Architecture layers wired via DI extensions
//   • Identity confirmed-account requirement kept from legacy
//   • HSTS, HTTPS-redirect, static files, routing unchanged
// ============================================================

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using OrbitAOS.V6.Infrastructure.Extensions;
using OrbitAOS.V6.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------------
// 1. Infrastructure layer registration (DbContext, Identity, Repos)
// ------------------------------------------------------------------
builder.Services.AddInfrastructure(builder.Configuration);

// ------------------------------------------------------------------
// 2. Application layer registration (Services, Validators, MediatR)
// ------------------------------------------------------------------
builder.Services.AddApplication();

// ------------------------------------------------------------------
// 3. ASP.NET Core Identity (Web layer concern – UI scaffolding)
// ------------------------------------------------------------------
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ------------------------------------------------------------------
// 4. MVC + Razor Pages (Web layer)
// ------------------------------------------------------------------
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation(); // enables hot-reload in Development

builder.Services.AddRazorPages();

// ------------------------------------------------------------------
// 5. Build the application
// ------------------------------------------------------------------
var app = builder.Build();

// ------------------------------------------------------------------
// 6. HTTP request pipeline
// ------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    // Shows EF Core migration errors in the browser during development
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // HSTS: 30-day default – adjust for production (see https://aka.ms/aspnetcore-hsts)
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ------------------------------------------------------------------
// 7. Route mapping
// ------------------------------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Razor Pages required for Identity UI scaffolding
app.MapRazorPages();

app.Run();
