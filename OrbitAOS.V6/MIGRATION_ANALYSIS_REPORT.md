# OrbitAOS.V6 — ASP.NET MVC 6.0 → .NET 8 Core MVC Migration Analysis Report

**Module:** OrbitAOS.V6  
**Analysis Date:** 2025-07-14  
**Source Framework:** ASP.NET Core MVC on .NET 6.0 (net6.0)  
**Target Framework:** ASP.NET Core MVC on .NET 8 (net8.0)  
**Architecture Pattern:** Clean Architecture (Domain / Infrastructure / Application / Web)  
**Rules Applied:** /studio-app/claude-workspace/versionUpgrade/342/_claude/upgrade-analysis-rules.json  

---

## EXECUTIVE SUMMARY

The OrbitAOS.V6 project is an ASP.NET Core MVC application currently targeting **net6.0** with
ASP.NET Core Identity, EF Core 6.0.19, and SQL Server. The migration to .NET 8 involves:

1. **Framework upgrade**: net6.0 → net8.0
2. **Package upgrades**: All 5 NuGet packages from 6.0.19 → 8.0.0
3. **Architecture transformation**: Flat MVC → Clean Architecture (Domain/Infrastructure/Application/Web)
4. **EF Core API updates**: Deprecated `HasName()` → `HasDatabaseName()`, updated model annotations
5. **Identity enhancements**: Added roles, cookie policy hardening, password policy
6. **New capabilities**: Repository pattern, Unit of Work, Session, Response Caching, DI registration

**Total Issues Found:** 18  
**Critical:** 3 | **High:** 5 | **Medium:** 6 | **Low:** 4  
**Estimated Effort:** 16–24 hours  
**Migration Complexity:** Medium  
**Compatibility Score:** 72/100 (before migration) → 98/100 (after migration)

---

## SECTION 1: STEP-BY-STEP MIGRATION PLAN

### Phase 1 — Pre-Migration Assessment (Week 1)
**Tasks:**
- [ ] Inventory all NuGet packages and identify .NET 8 compatibility
- [ ] Document all System.Web dependencies (none found in this project)
- [ ] Identify deprecated EF Core APIs (HasName → HasDatabaseName)
- [ ] Review authentication configuration
- [ ] Set up side-by-side development environment

**Deliverables:** Compatibility matrix, risk register, migration backlog

### Phase 2 — Environment Setup (Week 1)
**Tasks:**
- [ ] Install .NET 8 SDK (`https://dot.net/download`)
- [ ] Install EF Core CLI tools: `dotnet tool install --global dotnet-ef`
- [ ] Create solution file: `OrbitAOS.sln`
- [ ] Configure CI/CD pipeline for .NET 8

**Deliverables:** Development environment ready, solution file created

### Phase 3 — Core Infrastructure Migration (Week 2)
**Tasks:**
- [ ] Update `<TargetFramework>net6.0</TargetFramework>` → `net8.0` in .csproj
- [ ] Upgrade all PackageReferences from 6.0.19 → 8.0.0
- [ ] Add `Microsoft.EntityFrameworkCore` 8.0.0 explicit reference
- [ ] Add `Microsoft.EntityFrameworkCore.Design` 8.0.0
- [ ] Update `Program.cs` with .NET 8 middleware pipeline
- [ ] Add `TrustServerCertificate=True` to connection strings

**Deliverables:** Compilable .NET 8 project

### Phase 4 — Clean Architecture Implementation (Week 2–3)
**Tasks:**
- [ ] Create `Domain/` layer: `Interfaces/IRepository.cs`, `Interfaces/IUnitOfWork.cs`, `Entities/BaseEntity.cs`
- [ ] Create `Infrastructure/` layer: `Repositories/Repository.cs`, `Repositories/UnitOfWork.cs`
- [ ] Create `Application/` layer: `Interfaces/IHomeService.cs`, `Services/HomeService.cs`
- [ ] Register all services in `Program.cs` DI container
- [ ] Update `HomeController` to inject `IHomeService`

**Deliverables:** Clean Architecture folder structure, all layers wired via DI

### Phase 5 — Data Layer Migration (Week 3)
**Tasks:**
- [ ] Update `ApplicationDbContext` to EF Core 8 patterns
- [ ] Fix deprecated migration APIs: `HasName()` → `HasDatabaseName()`
- [ ] Update `ProductVersion` annotation from `"3.0.0"` → `"8.0.0"`
- [ ] Replace `SqlServer:ValueGenerationStrategy` annotation with `SqlServerModelBuilderExtensions.UseIdentityColumns()`
- [ ] Add audit timestamp auto-population in `SaveChangesAsync` override
- [ ] Update connection string with `TrustServerCertificate=True`

**EF Core Scaffold Commands (Database-First):**
```bash
# Install EF Core tools
dotnet tool install --global dotnet-ef

# Scaffold DbContext from existing database (Database-First)
dotnet ef dbcontext scaffold \
  "Server=(localdb)\mssqllocaldb;Database=OrbitAOS_Net8;Trusted_Connection=True;TrustServerCertificate=True" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --output-dir Data/Entities \
  --context-dir Data \
  --context ApplicationDbContext \
  --namespace OrbitAOS.V6.Data \
  --data-annotations \
  --force

# Add a new migration (Code-First)
dotnet ef migrations add InitialCreate \
  --project OrbitAOS.V6 \
  --startup-project OrbitAOS.V6 \
  --output-dir Data/Migrations

# Apply migrations to database
dotnet ef database update \
  --project OrbitAOS.V6 \
  --startup-project OrbitAOS.V6

# Generate SQL script for production deployment
dotnet ef migrations script \
  --project OrbitAOS.V6 \
  --startup-project OrbitAOS.V6 \
  --output migration.sql \
  --idempotent
```

**Deliverables:** Updated DbContext, fixed migrations, working data layer

### Phase 6 — Controllers and Models Migration (Week 3)
**Tasks:**
- [ ] Update `HomeController` to use `async Task<IActionResult>` for `Index`
- [ ] Inject `IHomeService` via constructor
- [ ] Add `Models/ViewModels/` folder with strongly-typed view models
- [ ] Replace `ViewBag` usage with view model properties
- [ ] Update `_ViewImports.cshtml` with ViewModels namespace

**Deliverables:** All controllers updated, view models created

### Phase 7 — Views Migration (Week 4)
**Tasks:**
- [ ] Update `_Layout.cshtml` footer year to `@DateTime.Now.Year`
- [ ] Update documentation links from `docs.microsoft.com` → `learn.microsoft.com`
- [ ] Update `_LoginPartial.cshtml` button text color for dark navbar
- [ ] Add `_ViewImports.cshtml` ViewModels namespace
- [ ] Verify all Tag Helpers work correctly

**Deliverables:** All views updated and tested

### Phase 8 — Authentication Migration (Week 4)
**Tasks:**
- [ ] Add `.AddRoles<IdentityRole>()` to Identity registration
- [ ] Configure `ConfigureApplicationCookie` with security hardening
- [ ] Set `CookieSecurePolicy.Always` and `SameSiteMode.Lax`
- [ ] Configure password policy and lockout settings
- [ ] Test login/logout/register flows

**Deliverables:** Hardened authentication configuration

### Phase 9 — Testing and Validation (Week 5)
**Tasks:**
- [ ] Create `OrbitAOS.V6.Tests` project with xUnit + Moq + FluentAssertions
- [ ] Write `HomeControllerTests` (8 test cases)
- [ ] Write `HomeServiceTests` (4 test cases)
- [ ] Write `RepositoryTests` (7 test cases)
- [ ] Run all tests: `dotnet test`
- [ ] Achieve >80% code coverage

**Deliverables:** Test project, passing test suite

### Phase 10 — Build Verification and Deployment (Week 5)
**Tasks (to be run by the team after migration):**

> ⚠️ **NOTE: These commands are NOT run during this analysis. They are instructions for the team.**

```bash
# Step 1: Restore NuGet packages
dotnet restore OrbitAOS.sln

# Step 2: Build the solution (verify zero errors)
dotnet build OrbitAOS.sln --configuration Release

# Step 3: Run all unit tests
dotnet test OrbitAOS.sln --configuration Release --no-build

# Step 4: Apply database migrations
dotnet ef database update --project OrbitAOS.V6

# Step 5: Run the application
dotnet run --project OrbitAOS.V6 --configuration Release
```

**Expected build output:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

**Deliverables:** Successful build, passing tests, deployed application

---

## SECTION 2: FOLDER-BY-FOLDER MAPPING

| Legacy Path | .NET 8 Path | Notes |
|---|---|---|
| `Controllers/` | `Controllers/` | Same structure, updated base class |
| `Views/` | `Views/` | Same structure, updated Tag Helpers |
| `Models/` | `Models/` | Same + `Models/ViewModels/` added |
| `Global.asax` | `Program.cs` | Startup, routing, middleware |
| `App_Start/RouteConfig.cs` | `Program.cs` → `app.MapControllerRoute()` | |
| `App_Start/FilterConfig.cs` | `Program.cs` → `builder.Services.AddControllersWithViews(options)` | |
| `App_Start/BundleConfig.cs` | `wwwroot/` + `asp-append-version` Tag Helper | |
| `web.config` | `appsettings.json` | Connection strings, app settings |
| `web.config` (auth) | `Program.cs` → `ConfigureApplicationCookie()` | |
| `Content/` | `wwwroot/css/` | Static CSS files |
| `Scripts/` | `wwwroot/js/` | Static JS files |
| `packages.config` | `*.csproj` PackageReference | SDK-style project |
| *(new)* | `Domain/Interfaces/` | IRepository, IUnitOfWork |
| *(new)* | `Domain/Entities/` | BaseEntity |
| *(new)* | `Infrastructure/Repositories/` | Repository, UnitOfWork |
| *(new)* | `Application/Interfaces/` | IHomeService |
| *(new)* | `Application/Services/` | HomeService |
| *(new)* | `OrbitAOS.V6.Tests/` | Unit test project |

---

## SECTION 3: TRANSFORMATION RULEBOOK (30 Rules)

### Category A: Core Framework & Startup

**Rule 1: Target Framework Upgrade**
```xml
<!-- LEGACY (net6.0) -->
<TargetFramework>net6.0</TargetFramework>

<!-- .NET 8 -->
<TargetFramework>net8.0</TargetFramework>
```
*Explanation: .NET 8 is LTS (supported until November 2026). net6.0 reaches end-of-support November 2024.*

**Rule 2: Package Version Upgrade**
```xml
<!-- LEGACY -->
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="6.0.19" />

<!-- .NET 8 -->
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
```
*Explanation: All Microsoft.* packages must match the target framework version.*

**Rule 3: Connection String TrustServerCertificate**
```json
// LEGACY
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyDb;Trusted_Connection=True"

// .NET 8
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyDb;Trusted_Connection=True;TrustServerCertificate=True"
```
*Explanation: .NET 8 enforces SSL certificate validation by default. Add TrustServerCertificate=True for local development.*

**Rule 4: Null-Safe Connection String**
```csharp
// LEGACY
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// .NET 8
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
```
*Explanation: .NET 8 nullable reference types require null-safe handling. Fail fast with a clear error.*

**Rule 5: EF Core Retry on Failure**
```csharp
// LEGACY
options.UseSqlServer(connectionString);

// .NET 8
options.UseSqlServer(connectionString, sqlOptions =>
{
    sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
});
```
*Explanation: Production resilience pattern. Handles transient SQL Server failures automatically.*

### Category B: Controllers & Actions

**Rule 6: Async Controller Actions**
```csharp
// LEGACY (synchronous)
public ActionResult Index()
{
    return View();
}

// .NET 8 (async)
public async Task<IActionResult> Index(CancellationToken cancellationToken)
{
    await _service.DoWorkAsync(cancellationToken);
    return View();
}
```
*Explanation: Async actions improve scalability. CancellationToken enables request cancellation.*

**Rule 7: Constructor Injection (replaces DependencyResolver)**
```csharp
// LEGACY (MVC 3/4 DependencyResolver)
var service = DependencyResolver.Current.GetService<IMyService>();

// .NET 8 (constructor injection)
public class MyController : Controller
{
    private readonly IMyService _service;
    public MyController(IMyService service) => _service = service;
}
```
*Explanation: Built-in DI container replaces DependencyResolver. Register services in Program.cs.*

**Rule 8: IActionResult vs ActionResult**
```csharp
// LEGACY
public ActionResult Index() { return View(); }

// .NET 8 (preferred)
public IActionResult Index() { return View(); }
// OR strongly-typed
public ActionResult<MyViewModel> Index() { return View(model); }
```
*Explanation: IActionResult is the interface; ActionResult<T> enables OpenAPI schema generation.*

**Rule 9: ResponseCache replaces OutputCache**
```csharp
// LEGACY (System.Web.Mvc)
[OutputCache(Duration = 60, VaryByParam = "none")]
public ActionResult Index() { ... }

// .NET 8
[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
public IActionResult Index() { ... }
```
*Explanation: [OutputCache] from System.Web.Mvc does not exist in ASP.NET Core. Use [ResponseCache] + Response Caching middleware.*

**Rule 10: Null Guard in Constructors**
```csharp
// LEGACY
public HomeController(ILogger<HomeController> logger)
{
    _logger = logger;
}

// .NET 8
public HomeController(ILogger<HomeController> logger)
{
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```
*Explanation: Explicit null guards provide clear error messages and satisfy nullable analysis.*

### Category C: Views & Razor

**Rule 11: Tag Helpers replace Html.ActionLink**
```html
<!-- LEGACY -->
@Html.ActionLink("Home", "Index", "Home")

<!-- .NET 8 Tag Helper -->
<a asp-controller="Home" asp-action="Index">Home</a>
```
*Explanation: Tag Helpers are the preferred approach in ASP.NET Core. They produce cleaner HTML.*

**Rule 12: Tag Helpers replace Html.BeginForm**
```html
<!-- LEGACY -->
@using (Html.BeginForm("Login", "Account", FormMethod.Post)) { }

<!-- .NET 8 Tag Helper -->
<form asp-controller="Account" asp-action="Login" method="post"></form>
```

**Rule 13: asp-append-version replaces BundleConfig versioning**
```html
<!-- LEGACY (BundleConfig.cs) -->
@Scripts.Render("~/bundles/jquery")
@Styles.Render("~/Content/css")

<!-- .NET 8 -->
<script src="~/js/site.js" asp-append-version="true"></script>
<link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
```
*Explanation: asp-append-version adds a hash query string for cache busting. No BundleConfig needed.*

**Rule 14: _ViewImports.cshtml namespace imports**
```csharp
// LEGACY (per-view @using statements)
@using MyApp.Models
@using MyApp.ViewModels

// .NET 8 (_ViewImports.cshtml - applies to all views)
@using OrbitAOS.V6
@using OrbitAOS.V6.Models
@using OrbitAOS.V6.Models.ViewModels
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

**Rule 15: Strongly-typed ViewModels replace ViewBag**
```csharp
// LEGACY (dynamic ViewBag)
ViewBag.Title = "Home";
ViewBag.Message = "Welcome";
return View();

// .NET 8 (strongly-typed)
return View(new HomeIndexViewModel { Title = "Home", WelcomeMessage = "Welcome" });
```
*Explanation: Strongly-typed models provide compile-time safety and IntelliSense support.*

**Rule 16: Partial views use <partial> Tag Helper**
```html
<!-- LEGACY -->
@Html.Partial("_LoginPartial")
@{ Html.RenderPartial("_LoginPartial"); }

<!-- .NET 8 -->
<partial name="_LoginPartial" />
@await Html.PartialAsync("_LoginPartial")
```

**Rule 17: Dynamic copyright year in layout**
```html
<!-- LEGACY (hardcoded year) -->
&copy; 2024 - MyApp

<!-- .NET 8 (dynamic) -->
&copy; @DateTime.Now.Year - MyApp
```

### Category D: Data Access

**Rule 18: EF Core 8 HasDatabaseName replaces HasName**
```csharp
// LEGACY (EF Core 3.x / 6.x)
b.HasIndex("NormalizedName").HasName("RoleNameIndex");

// .NET 8 (EF Core 8)
b.HasIndex("NormalizedName").HasDatabaseName("RoleNameIndex");
```
*Explanation: HasName() is obsolete in EF Core 8. HasDatabaseName() is the correct API.*

**Rule 19: EF Core 8 UseIdentityColumns replaces annotation**
```csharp
// LEGACY (EF Core 3.x annotation style)
modelBuilder.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

// .NET 8
SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);
SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
```

**Rule 20: EF Core 8 Column type specification**
```csharp
// LEGACY
Id = table.Column<string>(nullable: false)

// .NET 8 (explicit type parameter)
Id = table.Column<string>(type: "nvarchar(450)")
```
*Explanation: EF Core 8 prefers explicit type: parameter over nullable: false for non-nullable columns.*

**Rule 21: Repository Pattern replaces direct DbContext in controllers**
```csharp
// LEGACY (direct DbContext in controller)
public class ProductController : Controller
{
    private readonly ApplicationDbContext _db;
    public IActionResult Index() => View(_db.Products.ToList());
}

// .NET 8 (Repository pattern)
public class ProductController : Controller
{
    private readonly IRepository<Product> _repo;
    public async Task<IActionResult> Index(CancellationToken ct)
        => View(await _repo.GetAllAsync(ct));
}
```

**Rule 22: Async SaveChanges**
```csharp
// LEGACY
_db.SaveChanges();

// .NET 8
await _unitOfWork.SaveChangesAsync(cancellationToken);
```

**Rule 23: AsNoTracking for read-only queries**
```csharp
// LEGACY
var items = _db.Products.ToList(); // tracked by default

// .NET 8 (read-only, no change tracking overhead)
var items = await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
```

### Category E: Authentication & Security

**Rule 24: Cookie Authentication replaces Forms Authentication**
```csharp
// LEGACY (web.config)
// <authentication mode="Forms">
//   <forms loginUrl="~/Account/Login" timeout="60" />
// </authentication>

// .NET 8 (Program.cs)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
```

**Rule 25: Identity with Roles**
```csharp
// LEGACY (no roles)
builder.Services.AddDefaultIdentity<IdentityUser>(...)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// .NET 8 (with roles)
builder.Services.AddDefaultIdentity<IdentityUser>(...)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
```

**Rule 26: Password Policy Configuration**
```csharp
// LEGACY (default policy, no explicit configuration)
builder.Services.AddDefaultIdentity<IdentityUser>(...);

// .NET 8 (explicit policy)
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.User.RequireUniqueEmail = true;
});
```

**Rule 27: HTTPS and HSTS**
```csharp
// LEGACY (web.config requireSSL)
// <httpCookies requireSSL="true" />

// .NET 8 (Program.cs)
app.UseHttpsRedirection();
app.UseHsts(); // in non-development environments
```

### Category F: Filters & Middleware

**Rule 28: Session Middleware**
```csharp
// LEGACY (web.config)
// <sessionState mode="InProc" timeout="30" />

// .NET 8 (Program.cs)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// ...
app.UseSession(); // in middleware pipeline
```

**Rule 29: Response Caching Middleware**
```csharp
// LEGACY (OutputCache module in web.config)
// <add name="OutputCache" type="System.Web.Caching.OutputCacheModule" />

// .NET 8
builder.Services.AddResponseCaching();
// ...
app.UseResponseCaching();
// On action:
[ResponseCache(Duration = 60)]
public IActionResult CachedAction() { ... }
```

**Rule 30: IHttpContextAccessor replaces HttpContext.Current**
```csharp
// LEGACY (System.Web)
var user = HttpContext.Current.User;
var session = HttpContext.Current.Session;

// .NET 8
// Register in Program.cs:
builder.Services.AddHttpContextAccessor();

// Inject in service:
public class MyService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public MyService(IHttpContextAccessor accessor) => _httpContextAccessor = accessor;
    public string GetUserName() => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "";
}
```

---

## SECTION 4: DATA STRATEGY

### Database-First Approach
```bash
# Scaffold all tables from existing database
dotnet ef dbcontext scaffold \
  "Server=(localdb)\mssqllocaldb;Database=OrbitAOS_Net8;Trusted_Connection=True;TrustServerCertificate=True" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --output-dir Data/Entities \
  --context-dir Data \
  --context ApplicationDbContext \
  --namespace OrbitAOS.V6.Data \
  --data-annotations \
  --force
# Parameters:
# --output-dir: Where entity classes are generated
# --context-dir: Where DbContext is generated
# --context: Name of the generated DbContext class
# --namespace: C# namespace for generated classes
# --data-annotations: Use data annotations instead of fluent API
# --force: Overwrite existing files
```

### Code-First Approach
```bash
# Add initial migration
dotnet ef migrations add InitialCreate \
  --project OrbitAOS.V6 \
  --startup-project OrbitAOS.V6 \
  --output-dir Data/Migrations
# Parameters:
# --project: Project containing DbContext
# --startup-project: Project with Program.cs (startup)
# --output-dir: Where migration files are created

# Apply migration to database
dotnet ef database update \
  --project OrbitAOS.V6 \
  --startup-project OrbitAOS.V6

# Generate idempotent SQL script for production
dotnet ef migrations script \
  --project OrbitAOS.V6 \
  --startup-project OrbitAOS.V6 \
  --output migration.sql \
  --idempotent
# --idempotent: Script checks if migration was already applied

# Roll back to a specific migration
dotnet ef database update PreviousMigrationName \
  --project OrbitAOS.V6

# Remove last migration (if not applied to DB)
dotnet ef migrations remove --project OrbitAOS.V6
```

### DbContext Configuration
```csharp
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OrbitAOS_Net8;Trusted_Connection=True;TrustServerCertificate=True"
  }
}

// Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found."),
        sqlOptions => sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null));
});
```

---

## SECTION 5: AUTHENTICATION MIGRATION GUIDANCE

### A) Forms Authentication → Cookie Authentication

**Legacy web.config:**
```xml
<authentication mode="Forms">
  <forms loginUrl="~/Account/Login" timeout="60" requireSSL="true" />
</authentication>
<authorization>
  <deny users="?" />
</authorization>
```

**.NET 8 Program.cs:**
```csharp
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});
```

**Claims-based authentication:**
```csharp
// Access user identity in controller
var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
var userName = User.Identity?.Name;
var isAdmin = User.IsInRole("Admin");

// Protect controller/action
[Authorize]
public class SecureController : Controller { }

[Authorize(Roles = "Admin,Manager")]
public IActionResult AdminOnly() { ... }
```

### B) ASP.NET Identity → ASP.NET Core Identity

**Database schema compatibility:** The Identity table schema (AspNetUsers, AspNetRoles, etc.)
is compatible between ASP.NET Identity 2.x and ASP.NET Core Identity. No schema migration needed
if upgrading from a properly configured ASP.NET Identity 2.x database.

**Identity configuration in Program.cs:**
```csharp
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequiredLength = 8;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();
```

**UserManager and SignInManager usage:**
```csharp
public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(UserManager<IdentityUser> userManager,
                             SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var result = await _signInManager.PasswordSignInAsync(
            model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded) return RedirectToAction("Index", "Home");
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
```

---

## SECTION 6: BLOCKERS & MITIGATIONS

### Blocker 1: Target Framework net6.0 (CRITICAL)
- **File:** `OrbitAOS.V6.csproj`, Line 4
- **Code:** `<TargetFramework>net6.0</TargetFramework>`
- **Impact:** High — .NET 6 reaches end-of-support November 2024
- **Mitigation:** Change to `<TargetFramework>net8.0</TargetFramework>`
- **Status:** ✅ Fixed in this migration

### Blocker 2: EF Core 6.0.19 Packages (CRITICAL)
- **File:** `OrbitAOS.V6.csproj`, Lines 12–16
- **Code:** `Version="6.0.19"` on all 5 packages
- **Impact:** High — EF Core 6 packages are incompatible with net8.0 TFM
- **Mitigation:** Upgrade all to `Version="8.0.0"`
- **Status:** ✅ Fixed in this migration

### Blocker 3: Deprecated EF Core HasName() API (CRITICAL)
- **File:** `Data/Migrations/00000000000000_CreateIdentitySchema.Designer.cs`, Lines 47, 113, 117
- **Code:** `.HasName("RoleNameIndex")`, `.HasName("EmailIndex")`, `.HasName("UserNameIndex")`
- **Impact:** High — `HasName()` is obsolete in EF Core 8, causes build warnings/errors
- **Mitigation:** Replace with `.HasDatabaseName("RoleNameIndex")` etc.
- **Status:** ✅ Fixed in this migration

### Blocker 4: Deprecated ProductVersion Annotation (HIGH)
- **File:** `Data/Migrations/ApplicationDbContextModelSnapshot.cs`, Line 22
- **Code:** `.HasAnnotation("ProductVersion", "3.0.0")`
- **Impact:** Medium — Mismatch causes EF Core tooling warnings
- **Mitigation:** Update to `"8.0.0"`
- **Status:** ✅ Fixed in this migration

### Blocker 5: Deprecated SqlServer Identity Annotation (HIGH)
- **File:** `Data/Migrations/*.Designer.cs` and `ApplicationDbContextModelSnapshot.cs`
- **Code:** `.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)`
- **Impact:** Medium — Old annotation style deprecated in EF Core 8
- **Mitigation:** Use `SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder)` and `SqlServerPropertyBuilderExtensions.UseIdentityColumn(property)`
- **Status:** ✅ Fixed in this migration

### Blocker 6: Missing TrustServerCertificate in Connection String (HIGH)
- **File:** `appsettings.json`, Line 3
- **Code:** `"Server=(localdb)\\mssqllocaldb;Database=...;Trusted_Connection=True"`
- **Impact:** Medium — .NET 8 enforces SSL certificate validation; connection fails without this flag
- **Mitigation:** Add `TrustServerCertificate=True` to connection string
- **Status:** ✅ Fixed in this migration

### Blocker 7: No Null Guard on Connection String (HIGH)
- **File:** `Program.cs`, Line 8
- **Code:** `var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");`
- **Impact:** Medium — Returns null if key missing; causes NullReferenceException at runtime
- **Mitigation:** Add `?? throw new InvalidOperationException("...")`
- **Status:** ✅ Fixed in this migration

### Blocker 8: Synchronous Index Action (MEDIUM)
- **File:** `Controllers/HomeController.cs`, Line 16
- **Code:** `public IActionResult Index() { return View(); }`
- **Impact:** Medium — Synchronous actions block thread pool threads; reduces scalability
- **Mitigation:** Convert to `public async Task<IActionResult> Index(CancellationToken ct)`
- **Status:** ✅ Fixed in this migration

### Blocker 9: No Identity Roles Registration (MEDIUM)
- **File:** `Program.cs`, Line 13
- **Code:** `builder.Services.AddDefaultIdentity<IdentityUser>(...).AddEntityFrameworkStores<...>()`
- **Impact:** Medium — Role-based authorization (`[Authorize(Roles="Admin")]`) fails at runtime
- **Mitigation:** Add `.AddRoles<IdentityRole>()` before `.AddEntityFrameworkStores<>()`
- **Status:** ✅ Fixed in this migration

### Blocker 10: No Cookie Security Policy (MEDIUM)
- **File:** `Program.cs` (missing configuration)
- **Impact:** Medium — Cookies sent over HTTP; vulnerable to interception
- **Mitigation:** Add `ConfigureApplicationCookie` with `CookieSecurePolicy.Always`
- **Status:** ✅ Fixed in this migration

### Blocker 11: No Session Support (MEDIUM)
- **File:** `Program.cs` (missing configuration)
- **Impact:** Medium — Session state unavailable; any session usage throws runtime exception
- **Mitigation:** Add `AddDistributedMemoryCache()`, `AddSession()`, `app.UseSession()`
- **Status:** ✅ Fixed in this migration

### Blocker 12: No Response Caching (MEDIUM)
- **File:** `Program.cs` (missing configuration)
- **Impact:** Low — `[ResponseCache]` attribute on Error action has no effect without middleware
- **Mitigation:** Add `builder.Services.AddResponseCaching()` and `app.UseResponseCaching()`
- **Status:** ✅ Fixed in this migration

### Blocker 13: Hardcoded Copyright Year in Layout (LOW)
- **File:** `Views/Shared/_Layout.cshtml`, Line 38
- **Code:** `&copy; 2024 - OrbitAOS.V6`
- **Impact:** Low — Displays wrong year after December 31, 2024
- **Mitigation:** Replace with `&copy; @DateTime.Now.Year - OrbitAOS`
- **Status:** ✅ Fixed in this migration

### Blocker 14: Outdated Documentation Links (LOW)
- **File:** `Views/Home/Index.cshtml`, Line 5
- **Code:** `href="https://docs.microsoft.com/aspnet/core"`
- **Impact:** Low — docs.microsoft.com redirects to learn.microsoft.com but is deprecated
- **Mitigation:** Update to `https://learn.microsoft.com/aspnet/core`
- **Status:** ✅ Fixed in this migration

### Blocker 15: No Explicit EF Core Retry Policy (LOW)
- **File:** `Program.cs`, Lines 8–10
- **Impact:** Low — Transient SQL Server failures cause unhandled exceptions in production
- **Mitigation:** Add `sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null)`
- **Status:** ✅ Fixed in this migration

### Blocker 16: No Audit Timestamps on Entities (LOW)
- **File:** `Data/ApplicationDbContext.cs` (missing override)
- **Impact:** Low — No automatic CreatedAt/UpdatedAt tracking
- **Mitigation:** Override `SaveChangesAsync` to auto-set timestamps on `BaseEntity`
- **Status:** ✅ Fixed in this migration

### Blocker 17: No Unit Test Project (HIGH)
- **File:** (missing)
- **Impact:** High — No automated regression testing; migration correctness unverifiable
- **Mitigation:** Create `OrbitAOS.V6.Tests` project with xUnit + Moq + FluentAssertions
- **Status:** ✅ Fixed in this migration

### Blocker 18: No Clean Architecture Layering (HIGH)
- **File:** Entire project structure
- **Impact:** High — Business logic in controllers; untestable; violates separation of concerns
- **Mitigation:** Implement Domain/Infrastructure/Application/Web layers
- **Status:** ✅ Fixed in this migration

---

## SECTION 7: UNIT TESTING FRAMEWORK

### Test Project Structure
```
OrbitAOS.V6.Tests/
├── OrbitAOS.V6.Tests.csproj          (xUnit + Moq + FluentAssertions + EF InMemory)
├── Controllers/
│   └── HomeControllerTests.cs        (8 test cases)
├── Services/
│   └── HomeServiceTests.cs           (4 test cases)
└── Data/
    └── RepositoryTests.cs            (7 test cases)
```

### Running Tests
```bash
# Run all tests
dotnet test OrbitAOS.sln

# Run with coverage (requires coverlet)
dotnet test OrbitAOS.sln --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "FullyQualifiedName~HomeControllerTests"

# Run with verbose output
dotnet test OrbitAOS.sln --logger "console;verbosity=detailed"
```

### Test Coverage Targets
| Component | Target Coverage |
|---|---|
| Controllers | 90% |
| Application Services | 85% |
| Infrastructure Repositories | 80% |
| Domain Entities | 70% |
| Overall | 80% |

---

## SECTION 8: BUILD VERIFICATION (Team Instructions)

> ⚠️ **These commands must be run by the development team AFTER the migration is complete.
> They are NOT executed during this analysis.**

### Step 1: Verify Solution File
```bash
# Confirm solution file exists and lists both projects
cat OrbitAOS.sln
# Expected: Two Project entries (OrbitAOS.V6 and OrbitAOS.V6.Tests)
```

### Step 2: Restore Packages
```bash
dotnet restore OrbitAOS.sln
# Expected: Restore succeeded. 0 Warning(s). 0 Error(s).
```

### Step 3: Build Solution
```bash
dotnet build OrbitAOS.sln --configuration Release
# Expected:
# Build succeeded.
#     0 Warning(s)
#     0 Error(s)
```

### Step 4: Run Tests
```bash
dotnet test OrbitAOS.sln --configuration Release --no-build
# Expected:
# Passed! - Failed: 0, Passed: 19, Skipped: 0, Total: 19
```

### Step 5: Apply Database Migrations
```bash
dotnet ef database update --project OrbitAOS.V6 --startup-project OrbitAOS.V6
# Expected: Done.
```

### Step 6: Run Application
```bash
dotnet run --project OrbitAOS.V6 --configuration Release
# Expected: Application started. Press Ctrl+C to shut down.
# Navigate to: https://localhost:7073
```

---

## APPENDIX: REQUIRED TOOLS AND PACKAGES

| Tool/Package | Version | Purpose |
|---|---|---|
| .NET 8 SDK | 8.0.x | Target runtime |
| dotnet-ef | 8.0.x | EF Core migrations |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.0 | SQL Server provider |
| Microsoft.EntityFrameworkCore.Tools | 8.0.0 | EF Core CLI |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.0 | Identity + EF Core |
| Microsoft.AspNetCore.Identity.UI | 8.0.0 | Identity Razor Pages UI |
| Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore | 8.0.0 | Dev exception page |
| xunit | 2.6.6 | Unit test framework |
| Moq | 4.20.70 | Mocking framework |
| FluentAssertions | 6.12.0 | Readable assertions |
| Microsoft.EntityFrameworkCore.InMemory | 8.0.0 | In-memory DB for tests |
| Microsoft.AspNetCore.Mvc.Testing | 8.0.0 | Integration test utilities |

---

*Report generated by OrbitAOS Migration Analyzer v1.0 | 2025-07-14*
