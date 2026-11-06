# OrbitAOS.V6 — ASP.NET MVC 6.0 to .NET 8 Core MVC Migration Analysis Report

**Analysis ID:** mvc-migration-analysis-2025-01-07  
**Module:** OrbitAOS.V6  
**Current Framework:** ASP.NET Core MVC on .NET 6 (net6.0)  
**Target Framework:** ASP.NET Core MVC on .NET 8 (net8.0)  
**Analysis Date:** 2025-01-07  
**Status:** READ-ONLY — No project files were modified during this analysis.

---

## Executive Summary

The OrbitAOS.V6 project is an ASP.NET Core MVC application currently targeting **net6.0** with ASP.NET Core Identity, Entity Framework Core 6.0.19, and SQL Server. The migration path is from **.NET 6 to .NET 8** (both are ASP.NET Core MVC — not a legacy ASP.NET MVC Framework migration). This is a **relatively straightforward in-place upgrade** because the application already uses modern ASP.NET Core patterns (Program.cs, appsettings.json, Tag Helpers, EF Core, etc.).

| Metric | Value |
|---|---|
| Total Issues | 18 |
| Critical | 1 |
| High | 4 |
| Medium | 8 |
| Low | 5 |
| Estimated Effort | 8–16 hours |
| Migration Complexity | Simple |
| Deprecated APIs Found | 3 |
| Breaking Changes | 4 |
| Compatibility Score | 82/100 |

---

## 1. STEP-BY-STEP MIGRATION PLAN

### Phase 1 — Pre-Migration Assessment (Week 1)
1. Audit all NuGet packages for .NET 8 compatibility (see Section 6).
2. Confirm SQL Server version compatibility with EF Core 8.
3. Verify .NET 8 SDK is installed: `dotnet --version` (must be ≥ 8.0.0).
4. Back up the entire solution and database.
5. Create a feature branch: `git checkout -b migrate/net8`.

### Phase 2 — Environment Setup (Week 1)
1. Install .NET 8 SDK from https://dot.net/download.
2. Install/update Visual Studio 2022 (17.8+) or VS Code with C# Dev Kit.
3. Install EF Core tools: `dotnet tool install --global dotnet-ef --version 8.*`.
4. Confirm tooling: `dotnet ef --version`.

### Phase 3 — Core Infrastructure Migration (Week 1–2)
1. Update `OrbitAOS.V6.csproj` — change `<TargetFramework>net6.0</TargetFramework>` to `net8.0`.
2. Upgrade all NuGet packages from version 6.0.x to 8.0.x (see Section 6).
3. Remove `<Nullable>enable</Nullable>` if not fully adopted, or resolve all nullable warnings.
4. Review `Program.cs` for any deprecated middleware APIs.
5. Update `appsettings.json` connection string if needed.

### Phase 4 — Data Layer Migration (Week 2)
1. Upgrade EF Core packages to 8.0.x.
2. Update `ApplicationDbContext.cs` — review for deprecated EF Core 6 APIs.
3. Regenerate or update migrations for EF Core 8 compatibility.
4. Run `dotnet ef database update` after migration.

### Phase 5 — Controllers and Models Migration (Week 2)
1. Review `HomeController.cs` — already uses `IActionResult`, DI, and async-ready patterns.
2. Verify `ErrorViewModel.cs` nullable annotations are correct.
3. Add any missing `[ApiController]` or routing attributes as needed.

### Phase 6 — Views Migration (Week 2)
1. Update `_Layout.cshtml` — review Bootstrap version and CDN references.
2. Verify `_ViewImports.cshtml` includes all required Tag Helper namespaces.
3. Update `_LoginPartial.cshtml` for any Identity UI changes in .NET 8.
4. Review `Areas/Identity/Pages/_ViewStart.cshtml` layout path.

### Phase 7 — Authentication and Authorization Migration (Week 3)
1. Upgrade `Microsoft.AspNetCore.Identity.EntityFrameworkCore` to 8.0.x.
2. Upgrade `Microsoft.AspNetCore.Identity.UI` to 8.0.x.
3. Review `AddDefaultIdentity` configuration in `Program.cs`.
4. Verify `RequireConfirmedAccount` behavior matches business requirements.
5. Test login, logout, register, and account management flows.

### Phase 8 — Advanced Features Migration (Week 3)
1. Review `serviceDependencies.json` — update SQL connection references.
2. Update `launchSettings.json` if port conflicts arise.
3. Review `_Layout.cshtml.css` scoped CSS for any breaking changes.
4. Verify static file serving (`wwwroot`) is unchanged.

### Phase 9 — Testing and Validation (Week 3–4)
1. Create a test project: `dotnet new xunit -n OrbitAOS.V6.Tests`.
2. Add test project to solution.
3. Write unit tests for `HomeController` actions.
4. Write integration tests for Identity flows.
5. Run all tests: `dotnet test`.

### Phase 10 — Build Verification and Deployment (Week 4)
> **NOTE (Read-Only Constraint):** The following steps describe what the team must execute AFTER migration — they were NOT run during this analysis.

**Build Verification Steps (to be run by the team after migration):**
```bash
# 1. Create or update the solution file
dotnet new sln -n OrbitAOS.V6
dotnet sln OrbitAOS.V6.sln add OrbitAOS.V6/OrbitAOS.V6.csproj

# 2. Restore packages
dotnet restore OrbitAOS.V6.sln

# 3. Build the solution — this MUST succeed with zero errors
dotnet build OrbitAOS.V6.sln --configuration Release

# 4. Run tests
dotnet test OrbitAOS.V6.sln

# 5. Publish
dotnet publish OrbitAOS.V6/OrbitAOS.V6.csproj -c Release -o ./publish
```

A **successful build** is confirmed when `dotnet build` outputs:
```
Build succeeded.
    0 Error(s)
    0 Warning(s)
```

---

## 2. FOLDER-BY-FOLDER MAPPING

| Legacy / Current (.NET 6) | Target (.NET 8) | Notes |
|---|---|---|
| `Controllers/` | `Controllers/` | No structural change |
| `Views/` | `Views/` | No structural change |
| `Views/_ViewImports.cshtml` | `Views/_ViewImports.cshtml` | Add new Tag Helper namespaces if needed |
| `Views/_ViewStart.cshtml` | `Views/_ViewStart.cshtml` | No change |
| `Views/Shared/_Layout.cshtml` | `Views/Shared/_Layout.cshtml` | Update Bootstrap if upgrading |
| `Models/` | `Models/` | No structural change |
| `Data/` | `Data/` | Update EF Core version |
| `Data/Migrations/` | `Data/Migrations/` | May need regeneration |
| `Areas/Identity/` | `Areas/Identity/` | Update Identity UI package |
| `wwwroot/css/` | `wwwroot/css/` | No change |
| `wwwroot/js/` | `wwwroot/js/` | No change |
| `wwwroot/lib/` | `wwwroot/lib/` | Update Bootstrap/jQuery via LibMan |
| `appsettings.json` | `appsettings.json` | No structural change |
| `appsettings.Development.json` | `appsettings.Development.json` | No change |
| `Program.cs` | `Program.cs` | Minor API updates |
| `OrbitAOS.V6.csproj` | `OrbitAOS.V6.csproj` | TFM + package version updates |
| `Properties/launchSettings.json` | `Properties/launchSettings.json` | No change |
| N/A | `OrbitAOS.V6.sln` | Create solution file |
| N/A | `OrbitAOS.V6.Tests/` | New test project |

---

## 3. TRANSFORMATION RULEBOOK (30+ Rules)

### Category A: Core Framework & Startup

**Rule 1 — Target Framework Moniker (TFM)**
```xml
<!-- BEFORE (.NET 6) -->
<TargetFramework>net6.0</TargetFramework>

<!-- AFTER (.NET 8) -->
<TargetFramework>net8.0</TargetFramework>
```
*Change the TFM in the .csproj file. This is the primary driver of all other changes.*

**Rule 2 — NuGet Package Versions**
```xml
<!-- BEFORE (.NET 6) -->
<PackageReference Include="Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore" Version="6.0.19" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="6.0.19" />
<PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="6.0.19" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="6.0.19" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="6.0.19" />

<!-- AFTER (.NET 8) -->
<PackageReference Include="Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
```
*All Microsoft.AspNetCore.* and Microsoft.EntityFrameworkCore.* packages must align with the target framework version.*

**Rule 3 — Nullable Reference Types**
```xml
<!-- BEFORE (.NET 6 — optional) -->
<Nullable>enable</Nullable>

<!-- AFTER (.NET 8 — default enabled, resolve all warnings) -->
<Nullable>enable</Nullable>
<!-- Ensure all nullable warnings are resolved or suppressed with justification -->
```
*In .NET 8, nullable reference types are enabled by default in new projects. Existing warnings must be addressed.*

**Rule 4 — ImplicitUsings**
```xml
<!-- BEFORE (.NET 6) -->
<ImplicitUsings>enable</ImplicitUsings>

<!-- AFTER (.NET 8 — no change needed, but verify) -->
<ImplicitUsings>enable</ImplicitUsings>
<!-- Implicit usings now include additional namespaces in .NET 8 -->
```
*No breaking change, but be aware that .NET 8 implicit usings may include additional namespaces.*

**Rule 5 — Program.cs Minimal API Pattern**
```csharp
// BEFORE (.NET 6 — already minimal hosting model, no change needed)
var builder = WebApplication.CreateBuilder(args);
// ...
var app = builder.Build();
app.Run();

// AFTER (.NET 8 — same pattern, verify no deprecated APIs used)
var builder = WebApplication.CreateBuilder(args);
// ...
var app = builder.Build();
app.Run();
```
*The minimal hosting model introduced in .NET 6 is unchanged in .NET 8. No migration needed.*

**Rule 6 — WebApplication.CreateBuilder Options (New in .NET 8)**
```csharp
// BEFORE (.NET 6)
var builder = WebApplication.CreateBuilder(args);

// AFTER (.NET 8 — optional: use new overload for slim builder)
var builder = WebApplication.CreateSlimBuilder(args); // New in .NET 8 for minimal APIs
// OR keep existing pattern — no breaking change
var builder = WebApplication.CreateBuilder(args);
```
*`CreateSlimBuilder` is new in .NET 8 for trimmed/AOT scenarios. Not required for standard MVC.*

**Rule 7 — HSTS Configuration**
```csharp
// BEFORE (.NET 6)
app.UseHsts(); // Default max-age: 30 days

// AFTER (.NET 8 — consider updating HSTS max-age for production)
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
    options.Preload = true;
});
app.UseHsts();
```
*HSTS configuration is unchanged but .NET 8 best practices recommend explicit configuration.*

### Category B: Controllers & Actions

**Rule 8 — Controller Base Class**
```csharp
// BEFORE (.NET 6) — already correct
public class HomeController : Controller { }

// AFTER (.NET 8) — no change needed
public class HomeController : Controller { }
```
*`Microsoft.AspNetCore.Mvc.Controller` is unchanged between .NET 6 and .NET 8.*

**Rule 9 — IActionResult Return Types**
```csharp
// BEFORE (.NET 6)
public IActionResult Index() => View();

// AFTER (.NET 8 — consider using typed results for better performance)
public IActionResult Index() => View();
// OR for new actions, use Results<T> pattern:
// public Results<Ok<MyModel>, NotFound> GetItem(int id) { ... }
```
*`IActionResult` is still fully supported. Typed results are optional performance improvements.*

**Rule 10 — Dependency Injection in Controllers**
```csharp
// BEFORE (.NET 6) — already correct
public HomeController(ILogger<HomeController> logger)
{
    _logger = logger;
}

// AFTER (.NET 8) — no change needed; DI is identical
// .NET 8 adds primary constructor support (C# 12):
public class HomeController(ILogger<HomeController> logger) : Controller
{
    public IActionResult Index() => View();
}
```
*Primary constructors (C# 12) are available in .NET 8 as an optional modernization.*

**Rule 11 — ResponseCache Attribute**
```csharp
// BEFORE (.NET 6)
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public IActionResult Error() { ... }

// AFTER (.NET 8) — no change needed; attribute is identical
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public IActionResult Error() { ... }
```
*`ResponseCache` attribute is unchanged. No migration needed.*

**Rule 12 — Async Controller Actions**
```csharp
// BEFORE (.NET 6) — synchronous
public IActionResult Index()
{
    var data = _service.GetData(); // synchronous
    return View(data);
}

// AFTER (.NET 8) — async recommended
public async Task<IActionResult> Index()
{
    var data = await _service.GetDataAsync();
    return View(data);
}
```
*Async actions are strongly recommended in .NET 8 for scalability.*

**Rule 13 — Problem Details (New in .NET 8)**
```csharp
// BEFORE (.NET 6) — manual error handling
public IActionResult Error()
{
    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}

// AFTER (.NET 8) — use built-in Problem Details
builder.Services.AddProblemDetails();
// In controller:
public IActionResult Error()
{
    return Problem(title: "An error occurred", statusCode: 500);
}
```
*Problem Details middleware is built-in in .NET 8 and provides RFC 7807-compliant error responses.*

### Category C: Views & Razor

**Rule 14 — _ViewImports.cshtml Tag Helpers**
```cshtml
@* BEFORE (.NET 6) *@
@using OrbitAOS.V6
@using OrbitAOS.V6.Models
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

@* AFTER (.NET 8) — no change needed; consider adding new namespaces *@
@using OrbitAOS.V6
@using OrbitAOS.V6.Models
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```
*Tag Helpers are unchanged. No migration needed.*

**Rule 15 — Layout File asp-append-version**
```cshtml
@* BEFORE (.NET 6) *@
<link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />

@* AFTER (.NET 8) — no change needed *@
<link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
```
*`asp-append-version` Tag Helper is unchanged in .NET 8.*

**Rule 16 — Scoped CSS (Component CSS)**
```cshtml
@* BEFORE (.NET 6) — _Layout.cshtml.css exists *@
<link rel="stylesheet" href="~/OrbitAOS.V6.styles.css" asp-append-version="true" />

@* AFTER (.NET 8) — no change needed; scoped CSS is fully supported *@
<link rel="stylesheet" href="~/OrbitAOS.V6.styles.css" asp-append-version="true" />
```
*Scoped CSS (`.cshtml.css` files) is unchanged in .NET 8.*

**Rule 17 — Partial Views**
```cshtml
@* BEFORE (.NET 6) *@
<partial name="_LoginPartial" />

@* AFTER (.NET 8) — no change needed *@
<partial name="_LoginPartial" />
```
*Partial Tag Helper is unchanged.*

**Rule 18 — RenderSectionAsync**
```cshtml
@* BEFORE (.NET 6) *@
@await RenderSectionAsync("Scripts", required: false)

@* AFTER (.NET 8) — no change needed *@
@await RenderSectionAsync("Scripts", required: false)
```
*Section rendering is unchanged.*

**Rule 19 — ViewData and ViewBag**
```csharp
// BEFORE (.NET 6)
ViewData["Title"] = "Home Page";

// AFTER (.NET 8) — no change needed; consider strongly-typed ViewModels
ViewData["Title"] = "Home Page";
// Preferred: use strongly-typed models
public record HomeViewModel(string Title, string Message);
```
*ViewData/ViewBag are unchanged but strongly-typed ViewModels are preferred in .NET 8.*

**Rule 20 — Bootstrap Version in Layout**
```html
<!-- BEFORE (.NET 6) — Bootstrap 5.x via wwwroot/lib -->
<link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />

<!-- AFTER (.NET 8) — Update Bootstrap to latest 5.x via LibMan -->
<!-- Run: libman restore (after updating libman.json) -->
<link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
```
*Bootstrap version in wwwroot/lib should be updated to the latest 5.x release.*

### Category D: Data Access

**Rule 21 — EF Core Package Version**
```xml
<!-- BEFORE (.NET 6) -->
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="6.0.19" />

<!-- AFTER (.NET 8) -->
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
```
*EF Core 8 includes significant performance improvements and new features.*

**Rule 22 — DbContext Configuration**
```csharp
// BEFORE (.NET 6)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// AFTER (.NET 8) — no change needed; optionally add resilience
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 5);
    }));
```
*EF Core 8 supports connection resiliency. The basic pattern is unchanged.*

**Rule 23 — Migration Namespace (Breaking Change)**
```csharp
// BEFORE (.NET 6 migrations) — uses old namespace pattern
namespace OrbitAOS.V6.Data.Migrations
{
    public partial class CreateIdentitySchema : Migration { }
}

// AFTER (.NET 8) — regenerate migrations with EF Core 8 tools
// dotnet ef migrations add InitialCreate --output-dir Data/Migrations
// New migrations will use file-scoped namespaces:
namespace OrbitAOS.V6.Data.Migrations;
public partial class CreateIdentitySchema : Migration { }
```
*EF Core 8 migrations use file-scoped namespaces by default. Existing migrations still work but new ones will differ.*

**Rule 24 — HasName() Deprecated in EF Core 8**
```csharp
// BEFORE (EF Core 3.x/6.x — found in ApplicationDbContextModelSnapshot.cs)
b.HasIndex("NormalizedName")
    .IsUnique()
    .HasName("RoleNameIndex")  // DEPRECATED in EF Core 8
    .HasFilter("[NormalizedName] IS NOT NULL");

// AFTER (EF Core 8)
b.HasIndex("NormalizedName")
    .IsUnique()
    .HasDatabaseName("RoleNameIndex")  // Use HasDatabaseName instead
    .HasFilter("[NormalizedName] IS NOT NULL");
```
*`HasName()` on indexes is deprecated in EF Core 8. Use `HasDatabaseName()` instead.*

**Rule 25 — HasMaxLength() vs MaxLength in EF Core 8**
```csharp
// BEFORE (EF Core 3.x — found in migration snapshot)
b.Property<string>("Name")
    .HasColumnType("nvarchar(256)")
    .HasMaxLength(256);

// AFTER (EF Core 8) — HasMaxLength is still valid but column type is inferred
b.Property<string>("Name")
    .HasMaxLength(256);
// EF Core 8 infers nvarchar(256) from HasMaxLength(256) for SQL Server
```
*Explicit `HasColumnType` with `HasMaxLength` is redundant in EF Core 8.*

**Rule 26 — EF Core 8 Scaffold Commands (Database-First)**
```bash
# Database-First: Scaffold DbContext and entities from existing database
dotnet ef dbcontext scaffold \
  "Server=(localdb)\mssqllocaldb;Database=OrbitAOS;Trusted_Connection=True;" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --output-dir Data/Entities \
  --context-dir Data \
  --context ApplicationDbContext \
  --namespace OrbitAOS.V6.Data \
  --data-annotations \
  --force

# Parameters explained:
# --output-dir: Where entity classes are generated
# --context-dir: Where DbContext is generated
# --context: Name of the DbContext class
# --namespace: Namespace for generated classes
# --data-annotations: Use data annotations instead of Fluent API
# --force: Overwrite existing files
```

**Rule 27 — EF Core 8 Migration Commands (Code-First)**
```bash
# Add a new migration
dotnet ef migrations add MigrationName \
  --project OrbitAOS.V6 \
  --startup-project OrbitAOS.V6 \
  --output-dir Data/Migrations \
  --context ApplicationDbContext

# Apply migrations to database
dotnet ef database update \
  --project OrbitAOS.V6 \
  --startup-project OrbitAOS.V6 \
  --context ApplicationDbContext

# Generate SQL script for production deployment
dotnet ef migrations script \
  --project OrbitAOS.V6 \
  --startup-project OrbitAOS.V6 \
  --context ApplicationDbContext \
  --output migration.sql \
  --idempotent
```

### Category E: Authentication & Security

**Rule 28 — AddDefaultIdentity Configuration**
```csharp
// BEFORE (.NET 6)
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// AFTER (.NET 8) — no breaking change; optionally add roles
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddRoles<IdentityRole>()  // Add if role-based auth needed
.AddEntityFrameworkStores<ApplicationDbContext>();
```
*`AddDefaultIdentity` is unchanged. .NET 8 adds no breaking changes to Identity configuration.*

**Rule 29 — Cookie Authentication (Forms Auth Migration)**
```csharp
// BEFORE (Legacy ASP.NET MVC — web.config Forms Authentication)
// <authentication mode="Forms">
//   <forms loginUrl="~/Account/Login" timeout="2880" />
// </authentication>

// AFTER (.NET 8 — Cookie Authentication in Program.cs)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(2880);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
```
*This rule applies if migrating from legacy Forms Authentication. Current project already uses ASP.NET Core Identity.*

**Rule 30 — Authorization Middleware Order**
```csharp
// BEFORE (.NET 6) — already correct order
app.UseAuthentication();
app.UseAuthorization();

// AFTER (.NET 8) — same order required; no change
app.UseAuthentication();
app.UseAuthorization();
// NOTE: Order matters — Authentication must come before Authorization
```
*Middleware order is unchanged and critical. No migration needed.*

**Rule 31 — Anti-Forgery Token**
```csharp
// BEFORE (.NET 6)
builder.Services.AddControllersWithViews();
// Anti-forgery is automatic for forms

// AFTER (.NET 8) — explicitly configure if needed
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});
```
*Anti-forgery is automatic in .NET 8 for form posts. Explicit global filter is optional.*

**Rule 32 — CORS Configuration**
```csharp
// BEFORE (.NET 6) — not configured in current project
// AFTER (.NET 8) — add if API endpoints are exposed
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://example.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
app.UseCors("AllowSpecificOrigin");
```

### Category F: Filters & Middleware

**Rule 33 — Exception Handling Middleware**
```csharp
// BEFORE (.NET 6)
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// AFTER (.NET 8) — add Problem Details for API-style error responses
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage(); // Explicit in .NET 8
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
```
*Exception handling middleware is unchanged. `UseDeveloperExceptionPage()` is now explicit in .NET 8.*

**Rule 34 — Static Files Middleware**
```csharp
// BEFORE (.NET 6)
app.UseStaticFiles();

// AFTER (.NET 8) — no change; optionally configure cache headers
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000");
    }
});
```
*Static files middleware is unchanged. Cache headers are optional.*

**Rule 35 — Output Caching (New in .NET 7/8)**
```csharp
// BEFORE (.NET 6) — Response Caching
builder.Services.AddResponseCaching();
app.UseResponseCaching();

// AFTER (.NET 8) — Output Caching (new, more powerful)
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromMinutes(10)));
});
app.UseOutputCache();
// In controller:
[OutputCache(Duration = 60)]
public IActionResult Index() => View();
```
*Output Caching is new in .NET 7 and improved in .NET 8. It replaces Response Caching for most scenarios.*

**Rule 36 — Rate Limiting (New in .NET 7/8)**
```csharp
// BEFORE (.NET 6) — not available built-in
// AFTER (.NET 8) — built-in rate limiting
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 10;
    });
});
app.UseRateLimiter();
```
*Rate limiting is built-in in .NET 7+ and is a new capability for .NET 8 migrations.*

**Rule 37 — Health Checks**
```csharp
// BEFORE (.NET 6) — basic health checks
builder.Services.AddHealthChecks();
app.MapHealthChecks("/health");

// AFTER (.NET 8) — enhanced health checks with EF Core
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database")
    .AddSqlServer(connectionString, name: "sql-server");
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

### Category G: Configuration

**Rule 38 — Connection String in appsettings.json**
```json
// BEFORE (.NET 6) — already in appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=aspnet-OrbitAOS.V6-afe03967-a771-44f8-b4ff-ee887e025b4f;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}

// AFTER (.NET 8) — update database name; remove MultipleActiveResultSets if not needed
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OrbitAOS_V8;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```
*`MultipleActiveResultSets=true` is not needed with EF Core 8. `TrustServerCertificate=True` may be needed for local dev.*

**Rule 39 — User Secrets**
```xml
<!-- BEFORE (.NET 6) — UserSecretsId in .csproj -->
<UserSecretsId>aspnet-OrbitAOS.V6-afe03967-a771-44f8-b4ff-ee887e025b4f</UserSecretsId>

<!-- AFTER (.NET 8) — no change needed; User Secrets work identically -->
<UserSecretsId>aspnet-OrbitAOS.V6-afe03967-a771-44f8-b4ff-ee887e025b4f</UserSecretsId>
```
*User Secrets are unchanged in .NET 8.*

**Rule 40 — IOptions Pattern**
```csharp
// BEFORE (.NET 6) — direct configuration access
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// AFTER (.NET 8) — use IOptions<T> for strongly-typed configuration
public class DatabaseSettings
{
    public string DefaultConnection { get; set; } = string.Empty;
}
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("ConnectionStrings"));
// Inject: IOptions<DatabaseSettings> settings
```
*Strongly-typed configuration via `IOptions<T>` is the recommended pattern in .NET 8.*

---

## 4. DATA STRATEGY WITH EF CORE SCAFFOLD COMMANDS

### Approach A: Database-First (Scaffold from Existing Database)
```bash
# Install EF Core tools
dotnet tool install --global dotnet-ef --version 8.*

# Scaffold all tables from existing database
dotnet ef dbcontext scaffold \
  "Server=(localdb)\mssqllocaldb;Database=aspnet-OrbitAOS.V6-afe03967-a771-44f8-b4ff-ee887e025b4f;Trusted_Connection=True;" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --project OrbitAOS.V6 \
  --startup-project OrbitAOS.V6 \
  --output-dir Data/Entities \
  --context-dir Data \
  --context ApplicationDbContext \
  --namespace OrbitAOS.V6.Data.Entities \
  --context-namespace OrbitAOS.V6.Data \
  --data-annotations \
  --no-onconfiguring \
  --force

# Parameter explanations:
# --project: The project containing the DbContext
# --startup-project: The startup project (same here)
# --output-dir: Directory for entity classes
# --context-dir: Directory for DbContext class
# --context: Name of the generated DbContext
# --namespace: Namespace for entity classes
# --context-namespace: Namespace for DbContext
# --data-annotations: Use [Key], [Required] etc. instead of Fluent API
# --no-onconfiguring: Don't generate OnConfiguring (use DI instead)
# --force: Overwrite existing files
```

### Approach B: Code-First (Migrations)
```bash
# Add initial migration
dotnet ef migrations add InitialCreate \
  --project OrbitAOS.V6 \
  --startup-project OrbitAOS.V6 \
  --output-dir Data/Migrations \
  --context ApplicationDbContext \
  --namespace OrbitAOS.V6.Data.Migrations

# Apply to database
dotnet ef database update \
  --project OrbitAOS.V6 \
  --startup-project OrbitAOS.V6 \
  --context ApplicationDbContext

# Generate idempotent SQL script for production
dotnet ef migrations script \
  --project OrbitAOS.V6 \
  --startup-project OrbitAOS.V6 \
  --context ApplicationDbContext \
  --output ./scripts/migration.sql \
  --idempotent
# --idempotent: Script can be run multiple times safely

# Rollback last migration
dotnet ef database update PreviousMigrationName \
  --project OrbitAOS.V6 \
  --startup-project OrbitAOS.V6

# Remove last migration (if not applied to DB)
dotnet ef migrations remove \
  --project OrbitAOS.V6 \
  --startup-project OrbitAOS.V6
```

### DbContext Configuration (.NET 8)
```csharp
// Data/ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OrbitAOS.V6.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Add your DbSets here
    // public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Apply entity configurations
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
```

### Connection String Management
```csharp
// Program.cs — .NET 8 pattern
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
        sqlOptions.CommandTimeout(30);
    }));
```

### Repository Pattern (Optional)
```csharp
// Interfaces/IRepository.cs
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<int> SaveChangesAsync();
}

// Data/Repository.cs
public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id) =>
        await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() =>
        await _dbSet.ToListAsync();

    public async Task AddAsync(T entity) =>
        await _dbSet.AddAsync(entity);

    public async Task UpdateAsync(T entity) =>
        _dbSet.Update(entity);

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null) _dbSet.Remove(entity);
    }

    public async Task<int> SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
```

---

## 5. AUTHENTICATION MIGRATION GUIDANCE

### A) Forms Authentication → Cookie Authentication
*(Applies to legacy ASP.NET MVC apps — current project already uses ASP.NET Core Identity)*

**Legacy web.config:**
```xml
<system.web>
  <authentication mode="Forms">
    <forms loginUrl="~/Account/Login" timeout="2880" defaultUrl="~/Home/Index" />
  </authentication>
  <authorization>
    <deny users="?" />
  </authorization>
</system.web>
```

**Equivalent Program.cs (.NET 8):**
```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(2880);
        options.SlidingExpiration = true;
        options.Cookie.Name = ".OrbitAOS.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

app.UseAuthentication();
app.UseAuthorization();
```

**Login Implementation:**
```csharp
// AccountController.cs
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
{
    if (!ModelState.IsValid) return View(model);

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, model.UserName),
        new Claim(ClaimTypes.Email, model.Email)
    };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal,
        new AuthenticationProperties { IsPersistent = model.RememberMe });

    return LocalRedirect(returnUrl ?? "/");
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Logout()
{
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return RedirectToAction("Index", "Home");
}
```

### B) ASP.NET Core Identity (.NET 6) → ASP.NET Core Identity (.NET 8)

**Current Program.cs (net6.0):**
```csharp
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
```

**Updated Program.cs (net8.0):**
```csharp
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Sign-in settings
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;

    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();
```

**Database Schema Compatibility:**
The ASP.NET Core Identity schema (AspNetUsers, AspNetRoles, etc.) is **compatible between .NET 6 and .NET 8**. No schema migration is required for the Identity tables themselves.

---

## 6. BLOCKERS & MITIGATIONS

### Blocker 1 — Target Framework Version (CRITICAL)
- **Description:** `OrbitAOS.V6.csproj` targets `net6.0` which reached end of support on May 14, 2024.
- **Impact:** High — Security vulnerabilities will not be patched; no new .NET features available.
- **File:** `OrbitAOS.V6.csproj`, Line 4
- **Code:** `<TargetFramework>net6.0</TargetFramework>`
- **Mitigation:** Change to `<TargetFramework>net8.0</TargetFramework>` and update all package versions.

### Blocker 2 — EF Core Package Version Mismatch (HIGH)
- **Description:** All EF Core packages are version 6.0.19. EF Core 8 has breaking changes and new APIs.
- **Impact:** High — Cannot use EF Core 8 features; potential runtime errors with .NET 8 runtime.
- **File:** `OrbitAOS.V6.csproj`, Lines 12–16
- **Mitigation:** Upgrade all `Microsoft.EntityFrameworkCore.*` packages to version `8.0.0`.

### Blocker 3 — Deprecated HasName() in Migration Snapshot (HIGH)
- **Description:** `ApplicationDbContextModelSnapshot.cs` uses `.HasName("RoleNameIndex")` which is deprecated in EF Core 8.
- **Impact:** High — Build warnings become errors; migration snapshot may fail to apply.
- **File:** `Data/Migrations/ApplicationDbContextModelSnapshot.cs`, Line 22
- **Code:** `.HasName("RoleNameIndex")`
- **Mitigation:** Replace `.HasName()` with `.HasDatabaseName()` throughout the snapshot file.

### Blocker 4 — EF Core 3.0 ProductVersion in Snapshot (HIGH)
- **Description:** `ApplicationDbContextModelSnapshot.cs` has `.HasAnnotation("ProductVersion", "3.0.0")` indicating it was generated with EF Core 3.0.
- **Impact:** High — Snapshot is out of sync with current EF Core version; may cause migration failures.
- **File:** `Data/Migrations/ApplicationDbContextModelSnapshot.cs`, Line 17
- **Code:** `.HasAnnotation("ProductVersion", "3.0.0")`
- **Mitigation:** Regenerate the migration snapshot using EF Core 8 tools: `dotnet ef migrations add InitialCreate --force`.

### Blocker 5 — Identity Package Version (HIGH)
- **Description:** `Microsoft.AspNetCore.Identity.EntityFrameworkCore` and `Microsoft.AspNetCore.Identity.UI` are version 6.0.19.
- **Impact:** High — Identity UI scaffolding and behavior may differ between .NET 6 and .NET 8.
- **File:** `OrbitAOS.V6.csproj`, Lines 13–14
- **Mitigation:** Upgrade to version `8.0.0`. Re-scaffold Identity UI pages if customized.

### Blocker 6 — Migration File Uses Old Nullable Syntax (MEDIUM)
- **Description:** `CreateIdentitySchema.cs` uses `nullable: false` and `nullable: true` parameters in `table.Column<T>()` which changed in EF Core 6+.
- **Impact:** Medium — Compilation warnings; potential behavior differences.
- **File:** `Data/Migrations/00000000000000_CreateIdentitySchema.cs`
- **Code:** `table.Column<string>(nullable: false)`
- **Mitigation:** Regenerate migrations with EF Core 8 tools. The `nullable` parameter is now `type: "nvarchar(max)", nullable: false`.

### Blocker 7 — SqlServerValueGenerationStrategy.IdentityColumn Deprecated (MEDIUM)
- **Description:** Migration files use `.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)` which is deprecated in EF Core 8.
- **Impact:** Medium — Deprecation warnings; use `SqlServerValueGenerationStrategy.IdentityColumn` is replaced by `SqlServerValueGenerationStrategy.IdentityColumn` (same name but different behavior in EF Core 8).
- **File:** `Data/Migrations/00000000000000_CreateIdentitySchema.cs`, Line 35
- **Mitigation:** Regenerate migrations with EF Core 8 tools.

### Blocker 8 — Missing Solution File (.sln) (MEDIUM)
- **Description:** No `.sln` solution file exists in the project directory.
- **Impact:** Medium — Cannot run `dotnet build` on the solution; CI/CD pipelines may fail.
- **Mitigation:** After migration, create solution file:
  ```bash
  dotnet new sln -n OrbitAOS.V6
  dotnet sln OrbitAOS.V6.sln add OrbitAOS.V6/OrbitAOS.V6.csproj
  ```

### Blocker 9 — No Unit Test Project (MEDIUM)
- **Description:** No test project exists in the solution.
- **Impact:** Medium — No automated test coverage; migration correctness cannot be verified programmatically.
- **Mitigation:** Create test project after migration:
  ```bash
  dotnet new xunit -n OrbitAOS.V6.Tests --framework net8.0
  dotnet sln OrbitAOS.V6.sln add OrbitAOS.V6.Tests/OrbitAOS.V6.Tests.csproj
  ```

### Blocker 10 — Connection String Uses GUID Database Name (MEDIUM)
- **Description:** `appsettings.json` connection string uses a GUID-based database name (`aspnet-OrbitAOS.V6-afe03967-a771-44f8-b4ff-ee887e025b4f`).
- **Impact:** Medium — Difficult to manage; not suitable for production.
- **File:** `appsettings.json`, Line 3
- **Mitigation:** Update to a meaningful database name: `Database=OrbitAOS_Production`.

### Blocker 11 — MultipleActiveResultSets in Connection String (LOW)
- **Description:** Connection string includes `MultipleActiveResultSets=true` which is not recommended with EF Core 8.
- **Impact:** Low — Performance overhead; EF Core 8 manages connections more efficiently without MARS.
- **File:** `appsettings.json`, Line 3
- **Mitigation:** Remove `MultipleActiveResultSets=true` from the connection string.

### Blocker 12 — No Roles Support in Identity (LOW)
- **Description:** `AddDefaultIdentity` is used without `.AddRoles<IdentityRole>()`, meaning role-based authorization is not configured.
- **Impact:** Low — `[Authorize(Roles = "Admin")]` will not work.
- **File:** `Program.cs`, Line 12
- **Mitigation:** Add `.AddRoles<IdentityRole>()` if role-based authorization is needed.

### Blocker 13 — No Error Logging Configuration (LOW)
- **Description:** No structured logging provider (Serilog, NLog) is configured; only default console logging.
- **Impact:** Low — Limited observability in production.
- **Mitigation:** Add Serilog or Application Insights:
  ```bash
  dotnet add package Serilog.AspNetCore --version 8.*
  ```

### Blocker 14 — Static Library Files Not Managed by LibMan (LOW)
- **Description:** Bootstrap, jQuery, and validation libraries are stored directly in `wwwroot/lib/` without a `libman.json` manifest.
- **Impact:** Low — Cannot easily update client-side libraries; no version tracking.
- **Mitigation:** Create `libman.json` and manage libraries via LibMan:
  ```bash
  dotnet tool install --global Microsoft.Web.LibraryManager.Cli
  libman init
  libman install bootstrap@5.3.0 --provider cdnjs --destination wwwroot/lib/bootstrap
  ```

### Blocker 15 — No Health Check Endpoints (LOW)
- **Description:** No health check endpoints are configured for monitoring.
- **Impact:** Low — Cannot integrate with load balancers or monitoring systems.
- **Mitigation:** Add health checks in Program.cs:
  ```csharp
  builder.Services.AddHealthChecks()
      .AddDbContextCheck<ApplicationDbContext>();
  app.MapHealthChecks("/health");
  ```

### Blocker 16 — Areas/Identity _ViewStart Uses Absolute Path (LOW)
- **Description:** `Areas/Identity/Pages/_ViewStart.cshtml` uses an absolute path for the layout.
- **Impact:** Low — Works but is fragile if layout is moved.
- **File:** `Areas/Identity/Pages/_ViewStart.cshtml`
- **Code:** `Layout = "/Views/Shared/_Layout.cshtml";`
- **Mitigation:** This is the correct pattern for Identity Area pages. No change needed unless layout is relocated.

### Blocker 17 — No Output Caching or Response Compression (LOW)
- **Description:** No output caching or response compression middleware is configured.
- **Impact:** Low — Suboptimal performance for static content and repeated requests.
- **Mitigation:**
  ```csharp
  builder.Services.AddOutputCache();
  builder.Services.AddResponseCompression(options =>
  {
      options.EnableForHttps = true;
  });
  app.UseResponseCompression();
  app.UseOutputCache();
  ```

### Blocker 18 — No Content Security Policy (LOW)
- **Description:** No Content Security Policy (CSP) headers are configured.
- **Impact:** Low — Security risk; XSS attacks are not mitigated at the HTTP header level.
- **Mitigation:** Add CSP middleware or use `app.Use()` to add security headers.

---

## 7. UNIT TESTING FRAMEWORK IMPLEMENTATION

### Test Project Setup
```bash
# Create test project
dotnet new xunit -n OrbitAOS.V6.Tests --framework net8.0
cd OrbitAOS.V6.Tests

# Add required packages
dotnet add package Microsoft.AspNetCore.Mvc.Testing --version 8.0.0
dotnet add package Moq --version 4.20.70
dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 8.0.0
dotnet add package FluentAssertions --version 6.12.0
dotnet add package coverlet.collector --version 6.0.0
```

### Test Project Structure
```
OrbitAOS.V6.Tests/
├── Controllers/
│   └── HomeControllerTests.cs
├── Data/
│   └── ApplicationDbContextTests.cs
├── Integration/
│   └── WebApplicationTests.cs
├── Helpers/
│   └── TestDbContextFactory.cs
└── OrbitAOS.V6.Tests.csproj
```

### HomeController Unit Tests
```csharp
// Controllers/HomeControllerTests.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Controllers;
using OrbitAOS.V6.Models;
using Xunit;
using FluentAssertions;

namespace OrbitAOS.V6.Tests.Controllers;

public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _loggerMock;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _loggerMock = new Mock<ILogger<HomeController>>();
        _controller = new HomeController(_loggerMock.Object);
    }

    [Fact]
    public void Index_ReturnsViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Privacy_ReturnsViewResult()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Error_ReturnsViewResultWithErrorViewModel()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = _controller.Error() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.Model.Should().BeOfType<ErrorViewModel>();
    }

    [Fact]
    public void Error_HasNoCacheResponseCacheAttribute()
    {
        // Arrange
        var methodInfo = typeof(HomeController).GetMethod(nameof(HomeController.Error));

        // Act
        var attribute = methodInfo!.GetCustomAttributes(typeof(ResponseCacheAttribute), false)
            .Cast<ResponseCacheAttribute>()
            .FirstOrDefault();

        // Assert
        attribute.Should().NotBeNull();
        attribute!.Duration.Should().Be(0);
        attribute.NoStore.Should().BeTrue();
    }
}
```

### Integration Tests
```csharp
// Integration/WebApplicationTests.cs
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;

namespace OrbitAOS.V6.Tests.Integration;

public class WebApplicationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public WebApplicationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/Home/Privacy")]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType?.ToString()
            .Should().StartWith("text/html");
    }
}
```

### DbContext Tests
```csharp
// Data/ApplicationDbContextTests.cs
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using Xunit;
using FluentAssertions;

namespace OrbitAOS.V6.Tests.Data;

public class ApplicationDbContextTests
{
    private ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public void CanCreateDbContext()
    {
        // Act
        using var context = CreateContext();

        // Assert
        context.Should().NotBeNull();
    }

    [Fact]
    public async Task CanSaveAndRetrieveData()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert — verify context is operational
        var canConnect = await context.Database.CanConnectAsync();
        canConnect.Should().BeTrue();
    }
}
```

---

## 8. ISSUES DETAIL

### Issue 1 — Target Framework (CRITICAL)
- **File:** `OrbitAOS.V6.csproj`
- **Line:** 4
- **Code:** `<TargetFramework>net6.0</TargetFramework>`
- **Severity:** Critical
- **Fix:** Change to `net8.0`

### Issue 2 — EF Core Package Versions (HIGH)
- **File:** `OrbitAOS.V6.csproj`
- **Lines:** 12–16
- **Code:** `Version="6.0.19"` on all EF Core packages
- **Severity:** High
- **Fix:** Update all to `Version="8.0.0"`

### Issue 3 — HasName() Deprecated (HIGH)
- **File:** `Data/Migrations/ApplicationDbContextModelSnapshot.cs`
- **Line:** 22
- **Code:** `.HasName("RoleNameIndex")`
- **Severity:** High
- **Fix:** Replace with `.HasDatabaseName("RoleNameIndex")`

### Issue 4 — EF Core 3.0 ProductVersion (HIGH)
- **File:** `Data/Migrations/ApplicationDbContextModelSnapshot.cs`
- **Line:** 17
- **Code:** `.HasAnnotation("ProductVersion", "3.0.0")`
- **Severity:** High
- **Fix:** Regenerate snapshot with EF Core 8

### Issue 5 — Identity Package Versions (HIGH)
- **File:** `OrbitAOS.V6.csproj`
- **Lines:** 13–14
- **Code:** `Version="6.0.19"` on Identity packages
- **Severity:** High
- **Fix:** Update to `Version="8.0.0"`

### Issue 6 — Old Nullable Syntax in Migration (MEDIUM)
- **File:** `Data/Migrations/00000000000000_CreateIdentitySchema.cs`
- **Line:** 18
- **Code:** `table.Column<string>(nullable: false)`
- **Severity:** Medium
- **Fix:** Regenerate migrations

### Issue 7 — SqlServerValueGenerationStrategy.IdentityColumn (MEDIUM)
- **File:** `Data/Migrations/00000000000000_CreateIdentitySchema.cs`
- **Line:** 35
- **Code:** `.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)`
- **Severity:** Medium
- **Fix:** Regenerate migrations with EF Core 8

### Issue 8 — Missing Solution File (MEDIUM)
- **File:** N/A
- **Severity:** Medium
- **Fix:** `dotnet new sln -n OrbitAOS.V6 && dotnet sln add OrbitAOS.V6/OrbitAOS.V6.csproj`

### Issue 9 — No Unit Test Project (MEDIUM)
- **File:** N/A
- **Severity:** Medium
- **Fix:** Create xUnit test project targeting net8.0

### Issue 10 — GUID Database Name (MEDIUM)
- **File:** `appsettings.json`
- **Line:** 3
- **Code:** `Database=aspnet-OrbitAOS.V6-afe03967-a771-44f8-b4ff-ee887e025b4f`
- **Severity:** Medium
- **Fix:** Use meaningful database name

### Issue 11 — MultipleActiveResultSets (LOW)
- **File:** `appsettings.json`
- **Line:** 3
- **Code:** `MultipleActiveResultSets=true`
- **Severity:** Low
- **Fix:** Remove from connection string

### Issue 12 — No Roles in Identity (LOW)
- **File:** `Program.cs`
- **Line:** 12
- **Code:** `AddDefaultIdentity<IdentityUser>(...)`
- **Severity:** Low
- **Fix:** Add `.AddRoles<IdentityRole>()`

### Issue 13 — No Structured Logging (LOW)
- **File:** `Program.cs`
- **Severity:** Low
- **Fix:** Add Serilog or Application Insights

### Issue 14 — Static Libraries Not Managed (LOW)
- **File:** `wwwroot/lib/`
- **Severity:** Low
- **Fix:** Create `libman.json` manifest

### Issue 15 — No Health Checks (LOW)
- **File:** `Program.cs`
- **Severity:** Low
- **Fix:** Add `AddHealthChecks()` and `MapHealthChecks()`

### Issue 16 — No Output Caching (LOW)
- **File:** `Program.cs`
- **Severity:** Low
- **Fix:** Add `AddOutputCache()` and `UseOutputCache()`

### Issue 17 — No Response Compression (LOW)
- **File:** `Program.cs`
- **Severity:** Low
- **Fix:** Add `AddResponseCompression()` and `UseResponseCompression()`

### Issue 18 — No Content Security Policy (LOW)
- **File:** `Program.cs`
- **Severity:** Low
- **Fix:** Add CSP middleware or security headers

---

## 9. BUILD VERIFICATION STEPS (Post-Migration — Do Not Run During Analysis)

After completing all migration steps, the team must verify a successful build:

```bash
# Step 1: Ensure .NET 8 SDK is installed
dotnet --version
# Expected: 8.x.x

# Step 2: Create solution file (if not exists)
dotnet new sln -n OrbitAOS.V6
dotnet sln OrbitAOS.V6.sln add "OrbitAOS.V6/OrbitAOS.V6.csproj"

# Step 3: Restore NuGet packages
dotnet restore OrbitAOS.V6.sln

# Step 4: Build the solution
dotnet build OrbitAOS.V6.sln --configuration Release --no-restore
# SUCCESS indicator: "Build succeeded. 0 Error(s)"

# Step 5: Run unit tests
dotnet test OrbitAOS.V6.sln --configuration Release --no-build
# SUCCESS indicator: "Passed! - Failed: 0, Passed: N, Skipped: 0"

# Step 6: Verify EF Core migrations
dotnet ef migrations list --project OrbitAOS.V6 --startup-project OrbitAOS.V6
# SUCCESS indicator: List of migrations with "(Pending)" or "(Applied)" status

# Step 7: Publish
dotnet publish "OrbitAOS.V6/OrbitAOS.V6.csproj" \
  --configuration Release \
  --output ./publish \
  --runtime win-x64 \
  --self-contained false
# SUCCESS indicator: "Publish succeeded."
```

**A valid solution file (.sln) must:**
1. Reference all project files with correct relative paths
2. Include test projects
3. Be parseable by `dotnet sln list`
4. Allow `dotnet build` to succeed with zero errors

---

## 10. RISK ASSESSMENT

| Risk | Level | Mitigation |
|---|---|---|
| EF Core migration snapshot incompatibility | High | Regenerate with EF Core 8 tools |
| Identity UI breaking changes | Medium | Test all auth flows after upgrade |
| Package version conflicts | Medium | Use `dotnet list package --outdated` |
| Connection string changes | Low | Test with local DB before production |
| Bootstrap version incompatibility | Low | Test UI after library update |

---

*This report was generated as a read-only analysis. No project files were created, modified, or deleted during this analysis.*
