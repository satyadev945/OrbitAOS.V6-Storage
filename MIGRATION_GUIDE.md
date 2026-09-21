# ASP.NET MVC to .NET 8 Core MVC Migration Guide

## OrbitAOS.V6 Migration Documentation

---

## Table of Contents

1. [Migration Overview](#1-migration-overview)
2. [Step-by-Step Migration Plan](#2-step-by-step-migration-plan)
3. [Folder-by-Folder Mapping](#3-folder-by-folder-mapping)
4. [Transformation Rulebook (30+ Rules)](#4-transformation-rulebook)
5. [Data Strategy with EF Core Scaffold Commands](#5-data-strategy)
6. [Authentication Migration Guidance](#6-authentication-migration)
7. [Blockers and Mitigations (15+)](#7-blockers-and-mitigations)
8. [Unit Testing Framework Implementation](#8-unit-testing)

---

## 1. Migration Overview

| Attribute | Value |
|-----------|-------|
| Source Framework | ASP.NET MVC 3/4/5 on .NET Framework 4.5/4.6/4.7 |
| Target Framework | ASP.NET Core MVC on .NET 8 |
| Architecture | Clean Architecture (Domain/Application/Infrastructure/Web) |
| Database | SQL Server with EF Core 8 |
| Authentication | ASP.NET Core Identity 8 (Cookie Authentication) |
| Testing | xUnit + Moq + FluentAssertions |

---

## 2. Step-by-Step Migration Plan

### Phase 1: Pre-Migration Assessment (Week 1, Days 1-2)

**Tasks:**
- Inventory all controllers, views, models, and dependencies
- Analyze NuGet package compatibility with .NET 8
- Identify System.Web dependencies
- Document all routes and action methods
- Create migration checklist

**Commands:**
```bash
# Check .NET 8 SDK installation
dotnet --version

# List all packages in legacy project
dotnet list package
```

### Phase 2: Environment Setup (Week 1, Days 3-4)

**Tasks:**
- Install .NET 8 SDK
- Create Clean Architecture solution structure
- Configure project files (.csproj) with net8.0 target
- Set up project references

**Commands:**
```bash
# Create solution
dotnet new sln -n OrbitAOS.V6

# Create projects
dotnet new classlib -n OrbitAOS.V6.Domain -f net8.0
dotnet new classlib -n OrbitAOS.V6.Application -f net8.0
dotnet new classlib -n OrbitAOS.V6.Infrastructure -f net8.0
dotnet new mvc -n OrbitAOS.V6.Web -f net8.0
dotnet new xunit -n OrbitAOS.V6.Tests -f net8.0

# Add projects to solution
dotnet sln add OrbitAOS.V6.Domain/OrbitAOS.V6.Domain.csproj
dotnet sln add OrbitAOS.V6.Application/OrbitAOS.V6.Application.csproj
dotnet sln add OrbitAOS.V6.Infrastructure/OrbitAOS.V6.Infrastructure.csproj
dotnet sln add OrbitAOS.V6.Web/OrbitAOS.V6.Web.csproj
dotnet sln add OrbitAOS.V6.Tests/OrbitAOS.V6.Tests.csproj

# Add project references
dotnet add OrbitAOS.V6.Application reference OrbitAOS.V6.Domain
dotnet add OrbitAOS.V6.Infrastructure reference OrbitAOS.V6.Domain
dotnet add OrbitAOS.V6.Infrastructure reference OrbitAOS.V6.Application
dotnet add OrbitAOS.V6.Web reference OrbitAOS.V6.Application
dotnet add OrbitAOS.V6.Web reference OrbitAOS.V6.Infrastructure
```

### Phase 3: Domain Layer Migration (Week 1, Day 5)

**Tasks:**
- Create BaseEntity with audit properties
- Migrate domain entities from legacy Models folder
- Remove infrastructure concerns from entities

### Phase 4: Application Layer Migration (Week 2, Days 1-2)

**Tasks:**
- Create repository interfaces (IRepository<T>, IUnitOfWork)
- Create service interfaces (ISampleService, etc.)
- Implement service classes with business logic
- Create DTOs and AutoMapper profiles

### Phase 5: Infrastructure Layer Migration (Week 2, Days 3-4)

**Tasks:**
- Migrate ApplicationDbContext to Infrastructure layer
- Implement generic Repository<T>
- Implement UnitOfWork
- Configure ASP.NET Core Identity with ApplicationUser
- Update all packages to .NET 8 versions

### Phase 6: Presentation Layer Migration (Week 2, Day 5)

**Tasks:**
- Create Web project with ASP.NET Core MVC
- Migrate controllers (update namespaces, inject services)
- Migrate views (update Tag Helpers, _ViewImports)
- Configure Program.cs (replaces Global.asax + App_Start)
- Migrate appsettings.json (replaces web.config)

### Phase 7: Testing Implementation (Week 3, Days 1-2)

**Tasks:**
- Create xUnit test project
- Write unit tests for all services
- Write unit tests for all controllers
- Write integration tests for repositories

### Phase 8: Database Migration (Week 3, Day 3)

**Tasks:**
- Create EF Core initial migration
- Apply migration to database
- Verify schema and data integrity

### Phase 9: Build and Validation (Week 3, Day 4)

**Tasks:**
- Build entire solution: `dotnet build`
- Run all tests: `dotnet test`
- Manual testing of all features

### Phase 10: Documentation and Deployment (Week 3, Day 5)

**Tasks:**
- Update README.md
- Create deployment package
- Deploy to staging, then production

---

## 3. Folder-by-Folder Mapping

| Legacy Path | .NET 8 Path | Notes |
|-------------|-------------|-------|
| `App_Start/RouteConfig.cs` | `Program.cs` | `app.MapControllerRoute(...)` |
| `App_Start/FilterConfig.cs` | `Program.cs` | `options.Filters.Add<...>()` |
| `App_Start/BundleConfig.cs` | `wwwroot/` + LibMan | Static files in wwwroot |
| `Global.asax` | `Program.cs` | Application startup |
| `Global.asax.cs` | `Program.cs` | Middleware pipeline |
| `web.config` | `appsettings.json` | Configuration |
| `web.config (connectionStrings)` | `appsettings.json` | Connection strings |
| `web.config (system.web)` | `Program.cs` | Middleware config |
| `Controllers/` | `OrbitAOS.V6.Web/Controllers/` | Same structure |
| `Views/` | `OrbitAOS.V6.Web/Views/` | Same structure |
| `Models/` | `OrbitAOS.V6.Domain/Entities/` | Domain entities |
| `Models/ViewModels/` | `OrbitAOS.V6.Application/DTOs/` | DTOs |
| `Content/` | `wwwroot/css/` | CSS files |
| `Scripts/` | `wwwroot/js/` | JavaScript files |
| `Images/` | `wwwroot/images/` | Image files |
| `Filters/` | `OrbitAOS.V6.Web/Filters/` | Action filters |
| `Infrastructure/` | `OrbitAOS.V6.Infrastructure/` | Data access |
| `packages.config` | `*.csproj PackageReference` | NuGet packages |

---

## 4. Transformation Rulebook

### Rule 1: Update Target Framework
```xml
<!-- Legacy -->
<TargetFrameworkVersion>v4.7.2</TargetFrameworkVersion>

<!-- .NET 8 -->
<TargetFramework>net8.0</TargetFramework>
```

### Rule 2: Replace packages.config with PackageReference
```xml
<!-- Legacy packages.config -->
<package id="EntityFramework" version="6.4.4" targetFramework="net472" />

<!-- .NET 8 .csproj -->
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
```

### Rule 3: Replace Global.asax with Program.cs
```csharp
// Legacy Global.asax.cs
protected void Application_Start()
{
    AreaRegistration.RegisterAllAreas();
    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
    RouteConfig.RegisterRoutes(RouteTable.Routes);
    BundleConfig.RegisterBundles(BundleTable.Bundles);
}

// .NET 8 Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
var app = builder.Build();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
```

### Rule 4: Replace RouteConfig.cs with MapControllerRoute
```csharp
// Legacy App_Start/RouteConfig.cs
routes.MapRoute(
    name: "Default",
    url: "{controller}/{action}/{id}",
    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
);

// .NET 8 Program.cs
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

### Rule 5: Replace FilterConfig.cs with Global Filters in Program.cs
```csharp
// Legacy App_Start/FilterConfig.cs
filters.Add(new HandleErrorAttribute());

// .NET 8 Program.cs
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});
```

### Rule 6: Replace System.Web.Mvc with Microsoft.AspNetCore.Mvc
```csharp
// Legacy
using System.Web.Mvc;
public class HomeController : Controller { }

// .NET 8
using Microsoft.AspNetCore.Mvc;
public class HomeController : Controller { }
```

### Rule 7: Replace ActionResult with IActionResult
```csharp
// Legacy
public ActionResult Index() { return View(); }

// .NET 8
public IActionResult Index() { return View(); }
```

### Rule 8: Add Async/Await to Controller Actions
```csharp
// Legacy (synchronous)
public ActionResult Index()
{
    var items = db.Items.ToList();
    return View(items);
}

// .NET 8 (async)
public async Task<IActionResult> Index()
{
    var items = await _service.GetAllAsync();
    return View(items);
}
```

### Rule 9: Replace Constructor Injection Pattern
```csharp
// Legacy (DependencyResolver or property injection)
private readonly IService _service = DependencyResolver.Current.GetService<IService>();

// .NET 8 (constructor injection)
private readonly IService _service;
public HomeController(IService service) { _service = service; }
```

### Rule 10: Replace web.config with appsettings.json
```xml
<!-- Legacy web.config -->
<appSettings>
    <add key="AppName" value="OrbitAOS" />
</appSettings>
<connectionStrings>
    <add name="DefaultConnection" connectionString="..." />
</connectionStrings>
```
```json
// .NET 8 appsettings.json
{
  "AppSettings": { "AppName": "OrbitAOS" },
  "ConnectionStrings": { "DefaultConnection": "..." }
}
```

### Rule 11: Replace HttpContext.Current with IHttpContextAccessor
```csharp
// Legacy
var user = HttpContext.Current.User.Identity.Name;

// .NET 8
private readonly IHttpContextAccessor _httpContextAccessor;
var user = _httpContextAccessor.HttpContext?.User.Identity?.Name;
```

### Rule 12: Replace Session State
```csharp
// Legacy
Session["Key"] = "Value";
var value = Session["Key"] as string;

// .NET 8
HttpContext.Session.SetString("Key", "Value");
var value = HttpContext.Session.GetString("Key");
```

### Rule 13: Replace OutputCache with ResponseCache
```csharp
// Legacy
[OutputCache(Duration = 60, VaryByParam = "none")]
public ActionResult Index() { }

// .NET 8
[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
public IActionResult Index() { }
```

### Rule 14: Replace Html.Action with ViewComponents
```csharp
// Legacy
@Html.Action("RecentItems", "Sidebar")

// .NET 8 ViewComponent
@await Component.InvokeAsync("RecentItems")
```

### Rule 15: Replace IHttpModule with Middleware
```csharp
// Legacy IHttpModule
public class LoggingModule : IHttpModule
{
    public void Init(HttpApplication context)
    {
        context.BeginRequest += OnBeginRequest;
    }
}

// .NET 8 Middleware
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    public async Task InvokeAsync(HttpContext context)
    {
        // Before request
        await _next(context);
        // After request
    }
}
```

### Rule 16: Replace IHttpHandler with Endpoints
```csharp
// Legacy IHttpHandler
public class FileHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context) { }
}

// .NET 8 Endpoint
app.MapGet("/file/{name}", async (string name, HttpContext context) =>
{
    // Handle request
});
```

### Rule 17: Replace Forms Authentication with Cookie Authentication
```csharp
// Legacy web.config
// <authentication mode="Forms">
//   <forms loginUrl="~/Account/Login" timeout="2880" />
// </authentication>

// .NET 8 Program.cs
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromHours(48);
});
```

### Rule 18: Replace ASP.NET Identity 2.x with ASP.NET Core Identity
```csharp
// Legacy
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
public class ApplicationUser : IdentityUser { }

// .NET 8
using Microsoft.AspNetCore.Identity;
public class ApplicationUser : IdentityUser { }
```

### Rule 19: Replace EF6 DbContext with EF Core DbContext
```csharp
// Legacy EF6
using System.Data.Entity;
public class AppDbContext : DbContext
{
    public AppDbContext() : base("DefaultConnection") { }
}

// .NET 8 EF Core
using Microsoft.EntityFrameworkCore;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
}
```

### Rule 20: Replace EF6 Migrations with EF Core Migrations
```bash
# Legacy EF6 (Package Manager Console)
Enable-Migrations
Add-Migration InitialCreate
Update-Database

# .NET 8 EF Core (CLI)
dotnet ef migrations add InitialCreate --project OrbitAOS.V6.Infrastructure
dotnet ef database update --project OrbitAOS.V6.Infrastructure
```

### Rule 21: Replace BundleConfig with wwwroot Static Files
```csharp
// Legacy App_Start/BundleConfig.cs
bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"));

// .NET 8 - Use libman.json or direct wwwroot references
// <script src="~/lib/jquery/dist/jquery.min.js"></script>
```

### Rule 22: Replace @Html.BeginForm with asp-action Tag Helper
```html
<!-- Legacy -->
@using (Html.BeginForm("Create", "Sample", FormMethod.Post))
{
    @Html.AntiForgeryToken()
}

<!-- .NET 8 -->
<form asp-action="Create" method="post">
    <!-- AntiForgery token is automatic with Tag Helpers -->
</form>
```

### Rule 23: Replace @Html.TextBoxFor with asp-for Tag Helper
```html
<!-- Legacy -->
@Html.LabelFor(m => m.Name)
@Html.TextBoxFor(m => m.Name, new { @class = "form-control" })
@Html.ValidationMessageFor(m => m.Name)

<!-- .NET 8 -->
<label asp-for="Name" class="form-label"></label>
<input asp-for="Name" class="form-control" />
<span asp-validation-for="Name" class="text-danger"></span>
```

### Rule 24: Replace @Html.ActionLink with asp-controller/asp-action
```html
<!-- Legacy -->
@Html.ActionLink("Home", "Index", "Home")

<!-- .NET 8 -->
<a asp-controller="Home" asp-action="Index">Home</a>
```

### Rule 25: Replace @Url.Action with asp-route Tag Helpers
```html
<!-- Legacy -->
<a href="@Url.Action("Details", "Sample", new { id = item.Id })">Details</a>

<!-- .NET 8 -->
<a asp-action="Details" asp-route-id="@item.Id">Details</a>
```

### Rule 26: Replace @Html.ValidationSummary with asp-validation-summary
```html
<!-- Legacy -->
@Html.ValidationSummary(true, "", new { @class = "text-danger" })

<!-- .NET 8 -->
<div asp-validation-summary="ModelOnly" class="text-danger"></div>
```

### Rule 27: Replace @Html.Partial with <partial> Tag Helper
```html
<!-- Legacy -->
@Html.Partial("_LoginPartial")

<!-- .NET 8 -->
<partial name="_LoginPartial" />
```

### Rule 28: Replace @Html.RenderPartial with await Html.RenderPartialAsync
```html
<!-- Legacy -->
@{ Html.RenderPartial("_ValidationScripts"); }

<!-- .NET 8 -->
@{ await Html.RenderPartialAsync("_ValidationScriptsPartial"); }
```

### Rule 29: Replace Custom Action Filters
```csharp
// Legacy
public class LogActionFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext) { }
}

// .NET 8
public class ActionLoggingFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Before action
        var result = await next();
        // After action
    }
}
```

### Rule 30: Replace HandleError Attribute with Exception Filter
```csharp
// Legacy
[HandleError]
public class HomeController : Controller { }

// .NET 8 - Global exception filter in Program.cs
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});
```

### Rule 31: Replace ChildActionOnly with ViewComponents
```csharp
// Legacy
[ChildActionOnly]
public ActionResult Sidebar() { return PartialView(); }

// .NET 8 ViewComponent
public class SidebarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() { return View(); }
}
```

### Rule 32: Replace TempData Provider Configuration
```csharp
// Legacy - automatic with System.Web
// .NET 8 - requires explicit configuration
builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider();
// OR (default cookie-based)
// TempData uses cookies by default in .NET 8
```

### Rule 33: Replace Role Provider with Role Manager
```csharp
// Legacy
Roles.IsUserInRole(username, "Admin");

// .NET 8
var user = await _userManager.FindByNameAsync(username);
var isInRole = await _userManager.IsInRoleAsync(user, "Admin");
```

### Rule 34: Replace ModelState.IsValid Pattern
```csharp
// Legacy (same pattern, but return type changes)
if (!ModelState.IsValid) return View(model);

// .NET 8 (IActionResult return type)
if (!ModelState.IsValid) return View(model);
// Pattern is the same, but controller returns IActionResult
```

### Rule 35: Replace [Bind] Attribute Usage
```csharp
// Legacy
public ActionResult Create([Bind(Include = "Name,Description")] SampleModel model) { }

// .NET 8
public IActionResult Create([Bind("Name,Description")] SampleDto dto) { }
```

---

## 5. Data Strategy

### Database-First Approach (Scaffold Commands)

```bash
# Install EF Core tools
dotnet tool install --global dotnet-ef

# Scaffold DbContext from existing database
dotnet ef dbcontext scaffold \
    "Server=(localdb)\mssqllocaldb;Database=OrbitAOS;Trusted_Connection=True" \
    Microsoft.EntityFrameworkCore.SqlServer \
    --output-dir Data/Entities \
    --context-dir Data \
    --context ApplicationDbContext \
    --project OrbitAOS.V6.Infrastructure \
    --startup-project OrbitAOS.V6.Web \
    --force \
    --data-annotations

# Parameters explained:
# --output-dir: Where to place entity classes
# --context-dir: Where to place DbContext class
# --context: Name of the DbContext class
# --project: Project containing the DbContext
# --startup-project: Project with appsettings.json
# --force: Overwrite existing files
# --data-annotations: Use data annotations instead of Fluent API
```

### Code-First Approach (Migrations)

```bash
# Create initial migration
dotnet ef migrations add InitialCreate \
    --project OrbitAOS.V6.Infrastructure \
    --startup-project OrbitAOS.V6.Web \
    --output-dir Data/Migrations

# Apply migration to database
dotnet ef database update \
    --project OrbitAOS.V6.Infrastructure \
    --startup-project OrbitAOS.V6.Web

# Create migration for schema changes
dotnet ef migrations add AddSampleDescription \
    --project OrbitAOS.V6.Infrastructure \
    --startup-project OrbitAOS.V6.Web

# Remove last migration (if not applied)
dotnet ef migrations remove \
    --project OrbitAOS.V6.Infrastructure \
    --startup-project OrbitAOS.V6.Web

# Generate SQL script for production
dotnet ef migrations script \
    --output migration.sql \
    --idempotent \
    --project OrbitAOS.V6.Infrastructure \
    --startup-project OrbitAOS.V6.Web

# Rollback to specific migration
dotnet ef database update PreviousMigrationName \
    --project OrbitAOS.V6.Infrastructure \
    --startup-project OrbitAOS.V6.Web
```

### DbContext Configuration

```csharp
// Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        sqlOptions.MigrationsAssembly("OrbitAOS.V6.Infrastructure");
    });
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});
```

### Connection String Management

```json
// appsettings.json (development)
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OrbitAOS.V6;Trusted_Connection=True;TrustServerCertificate=True"
  }
}

// Production: Use environment variables
// CONNECTIONSTRINGS__DEFAULTCONNECTION=Server=prod-server;Database=OrbitAOS;...
```

---

## 6. Authentication Migration

### A) Forms Authentication → Cookie Authentication

**Legacy web.config:**
```xml
<authentication mode="Forms">
    <forms loginUrl="~/Account/Login" timeout="2880" slidingExpiration="true" />
</authentication>
<authorization>
    <deny users="?" />
</authorization>
```

**Legacy Login Action:**
```csharp
// Legacy
FormsAuthentication.SetAuthCookie(username, rememberMe);
FormsAuthentication.SignOut();
```

**.NET 8 Program.cs:**
```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(48);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

app.UseAuthentication();
app.UseAuthorization();
```

**.NET 8 Login Action:**
```csharp
// .NET 8
var claims = new List<Claim>
{
    new Claim(ClaimTypes.Name, username),
    new Claim(ClaimTypes.Email, email)
};
var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

// Logout
await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
```

### B) ASP.NET Identity 2.x → ASP.NET Core Identity

**Legacy ApplicationUser:**
```csharp
// Legacy
using Microsoft.AspNet.Identity.EntityFramework;
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
}
```

**.NET 8 ApplicationUser:**
```csharp
// .NET 8
using Microsoft.AspNetCore.Identity;
public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
```

**.NET 8 Identity Configuration:**
```csharp
// Program.cs
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
```

**Database Schema Compatibility:**
The ASP.NET Core Identity schema is compatible with ASP.NET Identity 2.x for the core tables (AspNetUsers, AspNetRoles, etc.). Additional columns in ApplicationUser require a migration.

---

## 7. Blockers and Mitigations

### Blocker 1: System.Web Namespace Removal (HIGH)
**Description:** All `System.Web` types are removed in .NET 8.
**Impact:** High - affects HttpContext, HttpRequest, HttpResponse, Session, etc.
**Mitigation:**
```csharp
// Replace HttpContext.Current
// Legacy: HttpContext.Current.User.Identity.Name
// .NET 8: Inject IHttpContextAccessor
private readonly IHttpContextAccessor _httpContextAccessor;
var userName = _httpContextAccessor.HttpContext?.User.Identity?.Name;
```

### Blocker 2: Global.asax Removal (HIGH)
**Description:** Global.asax and its event handlers are not supported.
**Impact:** High - Application_Start, Application_Error, Session_Start events lost.
**Mitigation:**
```csharp
// Replace Application_Start → Program.cs builder configuration
// Replace Application_Error → GlobalExceptionFilter or UseExceptionHandler
// Replace Session_Start → Middleware
app.UseExceptionHandler("/Home/Error");
```

### Blocker 3: Session State Changes (MEDIUM)
**Description:** Session state requires explicit configuration in .NET 8.
**Impact:** Medium - Session API changes.
**Mitigation:**
```csharp
// Program.cs
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
app.UseSession();

// Usage
HttpContext.Session.SetString("Key", "Value");
var value = HttpContext.Session.GetString("Key");
```

### Blocker 4: Synchronous I/O Operations (MEDIUM)
**Description:** Synchronous I/O is discouraged and may cause issues.
**Impact:** Medium - performance degradation, potential deadlocks.
**Mitigation:**
```csharp
// Replace synchronous with async
// Legacy: var items = db.Items.ToList();
// .NET 8: var items = await _repository.GetAllAsync();
```

### Blocker 5: web.config Configuration (HIGH)
**Description:** web.config is not used in .NET 8 (except for IIS hosting).
**Impact:** High - all configuration must be migrated.
**Mitigation:**
```json
// Migrate to appsettings.json
{
  "ConnectionStrings": { "DefaultConnection": "..." },
  "AppSettings": { "Key": "Value" }
}
```

### Blocker 6: Custom HTTP Modules (HIGH)
**Description:** IHttpModule is not supported in .NET 8.
**Impact:** High - all HTTP modules must be rewritten.
**Mitigation:**
```csharp
// Replace IHttpModule with Middleware
public class CustomMiddleware
{
    private readonly RequestDelegate _next;
    public async Task InvokeAsync(HttpContext context)
    {
        // Before request processing
        await _next(context);
        // After request processing
    }
}
app.UseMiddleware<CustomMiddleware>();
```

### Blocker 7: Custom HTTP Handlers (HIGH)
**Description:** IHttpHandler is not supported in .NET 8.
**Impact:** High - all HTTP handlers must be rewritten.
**Mitigation:**
```csharp
// Replace IHttpHandler with Minimal API endpoints
app.MapGet("/handler/{id}", async (int id, IService service) =>
{
    var result = await service.GetAsync(id);
    return Results.Ok(result);
});
```

### Blocker 8: OutputCache Attribute (MEDIUM)
**Description:** OutputCache attribute is not available in .NET 8.
**Impact:** Medium - caching behavior changes.
**Mitigation:**
```csharp
// Replace [OutputCache] with [ResponseCache]
[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "id" })]
public IActionResult Index() { }

// Program.cs
builder.Services.AddResponseCaching();
app.UseResponseCaching();
```

### Blocker 9: Child Actions (Html.Action) (MEDIUM)
**Description:** Html.Action and Html.RenderAction are not supported.
**Impact:** Medium - child action pattern must change.
**Mitigation:**
```csharp
// Replace with ViewComponents
public class SidebarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() { return View(); }
}
// View: @await Component.InvokeAsync("Sidebar")
```

### Blocker 10: .NET Framework-Only NuGet Packages (HIGH)
**Description:** Some packages only target .NET Framework.
**Impact:** High - must find .NET 8 compatible alternatives.
**Mitigation:**
```
EntityFramework 6.x → Microsoft.EntityFrameworkCore 8.0.0
System.Web.Optimization → LigerShark.WebOptimizer.Core
Microsoft.AspNet.Identity → Microsoft.AspNetCore.Identity
Newtonsoft.Json (if needed) → System.Text.Json (built-in) or Newtonsoft.Json 13.x
```

### Blocker 11: Deployment Model Changes (MEDIUM)
**Description:** .NET 8 uses Kestrel by default, not IIS.
**Impact:** Medium - deployment configuration changes.
**Mitigation:**
```json
// web.config for IIS hosting (still needed for IIS)
{
  "processPath": "dotnet",
  "arguments": ".\\OrbitAOS.V6.Web.dll",
  "stdoutLogEnabled": false,
  "hostingModel": "inprocess"
}
```

### Blocker 12: Missing TempData Provider (LOW)
**Description:** TempData requires explicit provider configuration.
**Impact:** Low - TempData may not persist between requests.
**Mitigation:**
```csharp
// Program.cs - TempData uses cookies by default
// For session-based TempData:
builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider();
```

### Blocker 13: Custom Model Binders (MEDIUM)
**Description:** Model binder API has changed in .NET 8.
**Impact:** Medium - custom model binders must be rewritten.
**Mitigation:**
```csharp
// Legacy
public class CustomModelBinder : IModelBinder
{
    public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) { }
}

// .NET 8
public class CustomModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        // Async binding
        return Task.CompletedTask;
    }
}
```

### Blocker 14: Route Constraints Syntax (LOW)
**Description:** Route constraint syntax has minor differences.
**Impact:** Low - routes may need updating.
**Mitigation:**
```csharp
// Legacy
routes.MapRoute("default", "{controller}/{action}/{id}", new { id = UrlParameter.Optional });

// .NET 8
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
```

### Blocker 15: Anti-Forgery Token Changes (LOW)
**Description:** Anti-forgery token API has changed.
**Impact:** Low - forms need updating.
**Mitigation:**
```html
<!-- Legacy -->
@Html.AntiForgeryToken()

<!-- .NET 8 - Automatic with Tag Helpers -->
<form asp-action="Create" method="post">
    <!-- Token is automatically included -->
</form>
```

### Blocker 16: ViewBag/ViewData Differences (LOW)
**Description:** ViewBag and ViewData work the same but with nullable context.
**Impact:** Low - minor code updates needed.
**Mitigation:**
```csharp
// .NET 8 - Same pattern, but use null-safe access
ViewData["Title"] = "Home";
var title = ViewData["Title"] as string ?? "Default";
```

### Blocker 17: Bundling and Minification (MEDIUM)
**Description:** System.Web.Optimization is not available.
**Impact:** Medium - bundling must be replaced.
**Mitigation:**
```bash
# Option 1: LibMan (Library Manager)
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
libman install bootstrap --provider cdnjs --destination wwwroot/lib/bootstrap

# Option 2: WebOptimizer
dotnet add package LigerShark.WebOptimizer.Core
```

---

## 8. Unit Testing Framework Implementation

### Test Project Setup

```xml
<!-- OrbitAOS.V6.Tests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="xunit" Version="2.6.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.5" />
    <PackageReference Include="Moq" Version="4.20.70" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.0" />
  </ItemGroup>
</Project>
```

### Controller Unit Test Pattern

```csharp
public class SampleControllerTests
{
    private readonly Mock<ISampleService> _mockService;
    private readonly SampleController _controller;

    public SampleControllerTests()
    {
        _mockService = new Mock<ISampleService>();
        _controller = new SampleController(_mockService.Object, Mock.Of<ILogger<SampleController>>());
        _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
    }

    [Fact]
    public async Task Index_ShouldReturnViewWithSamples()
    {
        // Arrange
        var samples = new List<SampleDto> { new SampleDto { Id = 1, Name = "Test" } };
        _mockService.Setup(s => s.GetAllSamplesAsync()).ReturnsAsync(samples);

        // Act
        var result = await _controller.Index() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.Model.Should().BeEquivalentTo(samples);
    }
}
```

### Service Unit Test Pattern

```csharp
public class SampleServiceTests
{
    private readonly Mock<IRepository<SampleEntity>> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly SampleService _service;

    public SampleServiceTests()
    {
        _mockRepo = new Mock<IRepository<SampleEntity>>();
        _mockMapper = new Mock<IMapper>();
        _mockUow = new Mock<IUnitOfWork>();
        _service = new SampleService(_mockRepo.Object, _mockMapper.Object, _mockUow.Object);
    }

    [Fact]
    public async Task GetAllSamplesAsync_ShouldReturnMappedDtos()
    {
        // Arrange
        var entities = new List<SampleEntity> { new SampleEntity { Id = 1, Name = "Test" } };
        var dtos = new List<SampleDto> { new SampleDto { Id = 1, Name = "Test" } };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
        _mockMapper.Setup(m => m.Map<IEnumerable<SampleDto>>(entities)).Returns(dtos);

        // Act
        var result = await _service.GetAllSamplesAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Test");
    }
}
```

### Integration Test Pattern (EF Core InMemory)

```csharp
public class RepositoryIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Repository<SampleEntity> _repository;

    public RepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _repository = new Repository<SampleEntity>(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistEntity()
    {
        var entity = new SampleEntity { Name = "Test", IsActive = true };
        await _repository.AddAsync(entity);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(entity.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test");
    }

    public void Dispose() => _context.Dispose();
}
```

### Running Tests

```bash
# Run all tests
dotnet test OrbitAOS.V6.sln

# Run with verbose output
dotnet test OrbitAOS.V6.sln --verbosity normal

# Run with code coverage
dotnet test OrbitAOS.V6.sln /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./coverage/

# Run specific test class
dotnet test --filter "FullyQualifiedName~SampleServiceTests"

# Run specific test method
dotnet test --filter "FullyQualifiedName~SampleServiceTests.GetAllSamplesAsync_ShouldReturnAllSamples"
```

---

## Success Criteria

- [ ] Solution builds successfully: `dotnet build OrbitAOS.V6.sln`
- [ ] All tests pass: `dotnet test OrbitAOS.V6.sln`
- [ ] No System.Web references remain
- [ ] All packages updated to .NET 8 compatible versions
- [ ] Clean Architecture layers properly separated
- [ ] Authentication fully functional
- [ ] Database migrations applied successfully
- [ ] Code coverage > 80%
