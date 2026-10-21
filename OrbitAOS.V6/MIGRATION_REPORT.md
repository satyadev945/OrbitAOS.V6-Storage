# OrbitAOS.V6 — ASP.NET Core 6.0 → .NET 8 Migration Analysis Report

## Executive Summary

**Module**: OrbitAOS.V6  
**Source Framework**: ASP.NET Core MVC on .NET 6.0 (net6.0)  
**Target Framework**: ASP.NET Core MVC on .NET 8.0 (net8.0)  
**Architecture Pattern Applied**: Clean Architecture (Domain / Application / Infrastructure / Web)  
**Analysis Date**: 2025-01-07  
**Migration Complexity**: Medium  

---

## 1. STEP-BY-STEP MIGRATION PLAN

### Phase 1 — Pre-Migration Assessment (Week 1)
- Inventory all NuGet packages and identify version gaps
- Identify all EF Core migration files and their compatibility
- Audit Identity configuration (IdentityUser vs ApplicationUser)
- Review all controller actions for sync/async patterns
- Check all Razor views for deprecated HTML helpers
- Establish baseline performance metrics

### Phase 2 — Environment Setup (Week 1)
- Install .NET 8 SDK (minimum 8.0.0) from https://dot.net/download
- Install dotnet-ef tool: `dotnet tool install --global dotnet-ef`
- Create solution file: `dotnet new sln -n OrbitAOS`
- Add project to solution: `dotnet sln add OrbitAOS.V6/OrbitAOS.V6.csproj`
- Configure CI/CD pipeline for .NET 8

### Phase 3 — Core Infrastructure Migration (Week 2)
- Update TargetFramework from net6.0 to net8.0 in .csproj
- Upgrade all NuGet packages to 8.x versions
- Implement Clean Architecture folder structure
- Create Domain layer (Entities, Interfaces, Common)
- Create Application layer (Services, DTOs, Interfaces)
- Create Infrastructure layer (Data, Identity, Repositories)
- Create Web layer (Controllers, Filters, Middleware)

### Phase 4 — Data Layer Migration (Week 2-3)
- Upgrade ApplicationDbContext to EF Core 8
- Replace IdentityUser with ApplicationUser
- Fix EF Core migration files (HasName → HasDatabaseName, nullable syntax)
- Regenerate migrations: `dotnet ef migrations add InitialCreate`
- Implement generic Repository<T> pattern
- Configure connection string with TrustServerCertificate=True

### Phase 5 — Controllers and Models Migration (Week 3)
- Convert synchronous actions to async Task<IActionResult>
- Inject IHomeService via constructor DI
- Replace ViewBag/ViewData with strongly-typed ViewModels
- Move controllers to Web/Controllers/ namespace
- Create Application/DTOs/ for ViewModels

### Phase 6 — Views Migration (Week 3)
- Update _ViewImports.cshtml with new namespaces
- Update _LoginPartial.cshtml to use ApplicationUser
- Update Index.cshtml to use HomeViewModel
- Verify Tag Helpers work correctly
- Update _Layout.cshtml copyright year to dynamic

### Phase 7 — Authentication Migration (Week 4)
- Replace AddDefaultIdentity<IdentityUser> with AddIdentity<ApplicationUser, IdentityRole>
- Configure cookie authentication options
- Set up HSTS, secure cookies, SameSite policies
- Configure lockout and password policies
- Test login/logout/register flows

### Phase 8 — Advanced Features Migration (Week 4)
- Add Serilog structured logging
- Add session state middleware
- Add response caching middleware
- Add health checks endpoint
- Add CORS configuration
- Add global exception filter
- Add request logging middleware

### Phase 9 — Testing and Validation (Week 5)
- Create unit tests for HomeController (xUnit + Moq)
- Create unit tests for HomeService
- Create integration tests for ApplicationDbContext
- Run all tests: `dotnet test`
- Verify build: `dotnet build`
- Performance baseline comparison

### Phase 10 — Deployment and Cutover (Week 6)
- Blue-green deployment setup
- Database migration: `dotnet ef database update`
- Smoke testing in staging
- Production cutover
- Monitor error rates for 48 hours
- Rollback plan activation if needed

---

## 2. FOLDER-BY-FOLDER MAPPING

| Legacy (.NET 6) | .NET 8 Clean Architecture |
|---|---|
| Controllers/ | Web/Controllers/ + Controllers/ (compat) |
| Views/ | Views/ (unchanged) |
| Models/ | Models/ + Application/DTOs/ |
| Data/ | Infrastructure/Data/ |
| Data/Migrations/ | Infrastructure/Data/Migrations/ (new) |
| Program.cs | Program.cs (upgraded) |
| appsettings.json | appsettings.json (upgraded) |
| wwwroot/ | wwwroot/ (unchanged) |
| Areas/Identity/ | Areas/Identity/ (unchanged) |
| — | Domain/Entities/ (new) |
| — | Domain/Interfaces/ (new) |
| — | Domain/Common/ (new) |
| — | Application/Interfaces/ (new) |
| — | Application/Services/ (new) |
| — | Application/DTOs/ (new) |
| — | Infrastructure/Identity/ (new) |
| — | Infrastructure/Repositories/ (new) |
| — | Web/Filters/ (new) |
| — | Web/Middleware/ (new) |
| — | Tests/Unit/Controllers/ (new) |
| — | Tests/Unit/Services/ (new) |

---

## 3. TRANSFORMATION RULEBOOK (30+ Rules)

### Rule 1 — Target Framework
**Legacy**: `<TargetFramework>net6.0</TargetFramework>`  
**NET 8**: `<TargetFramework>net8.0</TargetFramework>`  
**Explanation**: Update TFM to net8.0 for .NET 8 LTS support.

### Rule 2 — EF Core Package Version
**Legacy**: `Microsoft.EntityFrameworkCore.SqlServer Version="6.0.19"`  
**NET 8**: `Microsoft.EntityFrameworkCore.SqlServer Version="8.0.0"`  
**Explanation**: All EF Core packages must be upgraded to 8.x.

### Rule 3 — Identity Package Version
**Legacy**: `Microsoft.AspNetCore.Identity.EntityFrameworkCore Version="6.0.19"`  
**NET 8**: `Microsoft.AspNetCore.Identity.EntityFrameworkCore Version="8.0.0"`  
**Explanation**: Identity packages must match the target framework version.

### Rule 4 — EF Core Tools PrivateAssets
**Legacy**: `<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="6.0.19" />`  
**NET 8**: `<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0"><PrivateAssets>all</PrivateAssets></PackageReference>`  
**Explanation**: Tools packages should have PrivateAssets=all to prevent transitive dependency leakage.

### Rule 5 — AddDefaultIdentity → AddIdentity
**Legacy**: `builder.Services.AddDefaultIdentity<IdentityUser>(...)`  
**NET 8**: `builder.Services.AddIdentity<ApplicationUser, IdentityRole>(...).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders().AddDefaultUI()`  
**Explanation**: AddIdentity provides full role support; AddDefaultIdentity is limited.

### Rule 6 — IdentityUser → ApplicationUser
**Legacy**: `IdentityDbContext` (uses base IdentityUser)  
**NET 8**: `IdentityDbContext<ApplicationUser>` with custom ApplicationUser class  
**Explanation**: Custom ApplicationUser allows extending the user model with domain-specific properties.

### Rule 7 — Synchronous Controller Actions → Async
**Legacy**: `public IActionResult Index() { return View(); }`  
**NET 8**: `public async Task<IActionResult> Index(CancellationToken ct) { var msg = await _service.GetAsync(ct); return View(msg); }`  
**Explanation**: Async actions improve scalability and are the .NET 8 best practice.

### Rule 8 — Constructor DI in Controllers
**Legacy**: `public HomeController(ILogger<HomeController> logger) { _logger = logger; }`  
**NET 8**: `public HomeController(ILogger<HomeController> logger, IHomeService homeService) { _logger = logger; _homeService = homeService; }`  
**Explanation**: Inject application services to keep controllers thin.

### Rule 9 — EF Core HasName() → HasDatabaseName()
**Legacy**: `.HasName("RoleNameIndex")`  
**NET 8**: `.HasDatabaseName("RoleNameIndex")`  
**Explanation**: HasName() was deprecated in EF Core 5 and removed in EF Core 8.

### Rule 10 — EF Core SqlServerValueGenerationStrategy.IdentityColumn
**Legacy**: `.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)`  
**NET 8**: `.HasAnnotation("SqlServer:Identity", "1, 1")`  
**Explanation**: The old annotation syntax was replaced in EF Core 5+.

### Rule 11 — EF Core ProductVersion Annotation
**Legacy**: `.HasAnnotation("ProductVersion", "3.0.0")`  
**NET 8**: `.HasAnnotation("ProductVersion", "8.0.0")`  
**Explanation**: Model snapshot must reflect the current EF Core version.

### Rule 12 — EF Core Column Type Explicit Declaration
**Legacy**: `table.Column<string>(nullable: false)`  
**NET 8**: `table.Column<string>(type: "nvarchar(450)", nullable: false)`  
**Explanation**: EF Core 8 requires explicit type parameter in migration column definitions.

### Rule 13 — Cookie Authentication Configuration
**Legacy**: No explicit cookie configuration  
**NET 8**: `builder.Services.ConfigureApplicationCookie(options => { options.Cookie.SecurePolicy = CookieSecurePolicy.Always; options.ExpireTimeSpan = TimeSpan.FromHours(8); })`  
**Explanation**: Explicit cookie security configuration is required for production security.

### Rule 14 — HSTS Duration
**Legacy**: `app.UseHsts()` (default 30 days)  
**NET 8**: `app.UseHsts()` with `builder.Services.AddHsts(o => o.MaxAge = TimeSpan.FromDays(365))`  
**Explanation**: 1-year HSTS is the production recommendation.

### Rule 15 — Connection String TrustServerCertificate
**Legacy**: `Server=(localdb)\\mssqllocaldb;...`  
**NET 8**: `Server=(localdb)\\mssqllocaldb;...;TrustServerCertificate=True`  
**Explanation**: .NET 8 enforces SSL certificate validation by default; TrustServerCertificate=True is needed for LocalDB.

### Rule 16 — EF Core Retry on Failure
**Legacy**: `options.UseSqlServer(connectionString)`  
**NET 8**: `options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null))`  
**Explanation**: Transient fault handling is a production best practice.

### Rule 17 — Session State Registration
**Legacy**: Not configured  
**NET 8**: `builder.Services.AddDistributedMemoryCache(); builder.Services.AddSession(options => { ... }); app.UseSession();`  
**Explanation**: Session state requires explicit registration in ASP.NET Core.

### Rule 18 — Response Caching (replaces OutputCache)
**Legacy**: `[OutputCache]` attribute (System.Web)  
**NET 8**: `builder.Services.AddResponseCaching(); app.UseResponseCaching(); [ResponseCache(Duration=60)]`  
**Explanation**: OutputCache is not available in ASP.NET Core; use ResponseCaching middleware.

### Rule 19 — Global Exception Filter
**Legacy**: `FilterConfig.RegisterGlobalFilters` / `HandleErrorAttribute`  
**NET 8**: `builder.Services.AddControllersWithViews(options => options.Filters.Add<GlobalExceptionFilter>())`  
**Explanation**: Global filters are registered in Program.cs via AddControllersWithViews options.

### Rule 20 — HTTP Modules → Middleware
**Legacy**: `IHttpModule` implementations in web.config  
**NET 8**: `public class RequestLoggingMiddleware { public async Task InvokeAsync(HttpContext ctx) { ... await _next(ctx); } }`  
**Explanation**: HTTP Modules are replaced by ASP.NET Core middleware pipeline components.

### Rule 21 — Structured Logging with Serilog
**Legacy**: Basic ILogger wiring  
**NET 8**: `builder.Host.UseSerilog((ctx, svc, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration)...)`  
**Explanation**: Serilog provides structured logging with sinks for Console, File, and cloud providers.

### Rule 22 — Health Checks
**Legacy**: Not available  
**NET 8**: `builder.Services.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>(); app.MapHealthChecks("/health");`  
**Explanation**: Health checks are a .NET 8 best practice for container and cloud deployments.

### Rule 23 — ViewImports Namespace Updates
**Legacy**: `@using OrbitAOS.V6.Models`  
**NET 8**: `@using OrbitAOS.V6.Models @using OrbitAOS.V6.Application.DTOs`  
**Explanation**: New DTOs/ViewModels in Application layer must be imported in _ViewImports.cshtml.

### Rule 24 — LoginPartial IdentityUser → ApplicationUser
**Legacy**: `@inject SignInManager<IdentityUser> SignInManager`  
**NET 8**: `@inject SignInManager<ApplicationUser> SignInManager @inject UserManager<ApplicationUser> UserManager`  
**Explanation**: Must match the ApplicationUser type registered in DI.

### Rule 25 — Strongly-Typed Views
**Legacy**: `@{ ViewData["Title"] = "Home Page"; }` (no model)  
**NET 8**: `@model OrbitAOS.V6.Application.DTOs.HomeViewModel` with typed model  
**Explanation**: Strongly-typed views provide compile-time safety and IntelliSense.

### Rule 26 — Soft Delete Pattern
**Legacy**: Physical delete  
**NET 8**: `entity.IsDeleted = true; Update(entity);` with `HasQueryFilter(e => !e.IsDeleted)`  
**Explanation**: Soft delete preserves audit history and supports data recovery.

### Rule 27 — Audit Timestamps in SaveChangesAsync
**Legacy**: Manual timestamp management  
**NET 8**: Override `SaveChangesAsync` in DbContext to auto-set `UpdatedAt = DateTime.UtcNow`  
**Explanation**: Centralizes audit logic and prevents missed updates.

### Rule 28 — Anti-Forgery Cookie Security
**Legacy**: Default anti-forgery settings  
**NET 8**: `builder.Services.AddAntiforgery(o => { o.Cookie.SecurePolicy = CookieSecurePolicy.Always; o.Cookie.SameSite = SameSiteMode.Strict; })`  
**Explanation**: Explicit anti-forgery security is required for OWASP compliance.

### Rule 29 — CORS Configuration
**Legacy**: Not configured  
**NET 8**: `builder.Services.AddCors(options => { options.AddDefaultPolicy(...) }); app.UseCors();`  
**Explanation**: CORS must be explicitly configured in ASP.NET Core.

### Rule 30 — EF Core Design Package
**Legacy**: Only Tools package  
**NET 8**: Both `Microsoft.EntityFrameworkCore.Tools` and `Microsoft.EntityFrameworkCore.Design` with PrivateAssets=all  
**Explanation**: Design package is required for `dotnet ef` commands to work correctly.

### Rule 31 — Nullable Reference Types
**Legacy**: `<Nullable>enable</Nullable>` (already enabled)  
**NET 8**: Ensure all properties use `?` for nullable and `= string.Empty` for non-nullable strings  
**Explanation**: .NET 8 enforces nullable reference types more strictly.

### Rule 32 — Program.cs Top-Level Statements
**Legacy**: Top-level statements (already used)  
**NET 8**: Wrap in try/catch/finally with Serilog bootstrap logger for startup error capture  
**Explanation**: Startup errors are captured before the full logger is configured.

### Rule 33 — DbContext Options Pattern
**Legacy**: `options.UseSqlServer(connectionString)`  
**NET 8**: `options.UseSqlServer(connectionString, sqlOptions => { ... })` with lambda for SQL-specific options  
**Explanation**: SQL-specific options (retry, timeout) should be configured in the lambda.

### Rule 34 — Identity Lockout Configuration
**Legacy**: Default lockout settings  
**NET 8**: `options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); options.Lockout.MaxFailedAccessAttempts = 5;`  
**Explanation**: Explicit lockout configuration prevents brute-force attacks.

### Rule 35 — EF Core ApplyConfigurationsFromAssembly
**Legacy**: Manual entity configuration in OnModelCreating  
**NET 8**: `builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly)`  
**Explanation**: Automatically discovers all IEntityTypeConfiguration<T> implementations.

---

## 4. DATA STRATEGY WITH EF CORE SCAFFOLD COMMANDS

### Database-First Approach (Scaffold from existing DB)
```bash
# Install EF Core tools
dotnet tool install --global dotnet-ef

# Scaffold DbContext and entities from existing database
dotnet ef dbcontext scaffold \
  "Server=(localdb)\mssqllocaldb;Database=OrbitAOS;Trusted_Connection=True;TrustServerCertificate=True" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --output-dir Infrastructure/Data/Entities \
  --context-dir Infrastructure/Data \
  --context ApplicationDbContext \
  --namespace OrbitAOS.V6.Infrastructure.Data \
  --data-annotations \
  --force
```

### Code-First Approach (Migrations)
```bash
# Add initial migration
dotnet ef migrations add InitialCreate \
  --project OrbitAOS.V6.csproj \
  --startup-project OrbitAOS.V6.csproj \
  --output-dir Data/Migrations \
  --context ApplicationDbContext

# Apply migration to database
dotnet ef database update \
  --project OrbitAOS.V6.csproj \
  --startup-project OrbitAOS.V6.csproj \
  --context ApplicationDbContext

# Generate SQL script (for DBA review)
dotnet ef migrations script \
  --project OrbitAOS.V6.csproj \
  --output migration.sql \
  --idempotent

# Rollback last migration
dotnet ef database update PreviousMigrationName

# Remove last unapplied migration
dotnet ef migrations remove
```

### DbContext Configuration
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
        sqlOptions.CommandTimeout(30);
        sqlOptions.MigrationsAssembly("OrbitAOS.V6");
    }));
```

### Connection String Management
- Development: appsettings.Development.json or User Secrets
- Production: Environment variables or Azure Key Vault
- Never commit production connection strings to source control

---

## 5. AUTHENTICATION MIGRATION GUIDANCE

### A) Forms Authentication → Cookie Authentication
```csharp
// Program.cs — replaces web.config <forms> element
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
```

### B) ASP.NET Identity → ASP.NET Core Identity on .NET 8
```csharp
// Replace AddDefaultIdentity<IdentityUser> with:
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.SignIn.RequireConfirmedAccount = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();
```

---

## 6. BLOCKERS & MITIGATIONS

### Blocker 1 — EF Core Package Version Mismatch (HIGH)
**Issue**: All EF Core packages at version 6.0.19 are incompatible with .NET 8.  
**Mitigation**: Upgrade all to 8.0.0. Run `dotnet restore` after update.

### Blocker 2 — EF Core HasName() Deprecated (HIGH)
**Issue**: `HasName()` on index configuration removed in EF Core 8.  
**Mitigation**: Replace all `HasName()` calls with `HasDatabaseName()` in migration files and model snapshot.

### Blocker 3 — SqlServerValueGenerationStrategy.IdentityColumn (HIGH)
**Issue**: Old annotation syntax removed in EF Core 8.  
**Mitigation**: Replace with `.HasAnnotation("SqlServer:Identity", "1, 1")`.

### Blocker 4 — IdentityUser vs ApplicationUser (HIGH)
**Issue**: Using base IdentityUser prevents extending the user model.  
**Mitigation**: Create ApplicationUser : IdentityUser and update all DI registrations and views.

### Blocker 5 — AddDefaultIdentity Limitations (MEDIUM)
**Issue**: AddDefaultIdentity does not support roles.  
**Mitigation**: Replace with AddIdentity<ApplicationUser, IdentityRole>.

### Blocker 6 — Synchronous Controller Actions (MEDIUM)
**Issue**: Synchronous actions block thread pool threads under load.  
**Mitigation**: Convert all actions to async Task<IActionResult> with CancellationToken.

### Blocker 7 — Missing TrustServerCertificate in Connection String (MEDIUM)
**Issue**: .NET 8 enforces SSL certificate validation; LocalDB connections fail without this flag.  
**Mitigation**: Add `TrustServerCertificate=True` to connection string.

### Blocker 8 — No Session State Configured (MEDIUM)
**Issue**: Session state is not registered in the .NET 6 template.  
**Mitigation**: Add AddDistributedMemoryCache(), AddSession(), and app.UseSession().

### Blocker 9 — No Global Exception Handling (MEDIUM)
**Issue**: No global exception filter; unhandled exceptions show raw error pages.  
**Mitigation**: Implement GlobalExceptionFilter and register via AddControllersWithViews options.

### Blocker 10 — No Structured Logging (MEDIUM)
**Issue**: Basic ILogger wiring without structured logging sinks.  
**Mitigation**: Add Serilog.AspNetCore with Console and File sinks.

### Blocker 11 — No Health Checks (LOW)
**Issue**: No health check endpoint for container/cloud deployments.  
**Mitigation**: Add AddHealthChecks().AddDbContextCheck<ApplicationDbContext>() and MapHealthChecks("/health").

### Blocker 12 — No CORS Configuration (LOW)
**Issue**: CORS not configured; cross-origin requests will fail.  
**Mitigation**: Add AddCors() and UseCors() with appropriate policy.

### Blocker 13 — No Anti-Forgery Security Configuration (LOW)
**Issue**: Default anti-forgery settings may not meet security requirements.  
**Mitigation**: Configure AddAntiforgery() with SecurePolicy and SameSite settings.

### Blocker 14 — EF Core ProductVersion Mismatch in Snapshot (MEDIUM)
**Issue**: Model snapshot has ProductVersion "3.0.0" which causes EF Core 8 warnings.  
**Mitigation**: Update to "8.0.0" in ApplicationDbContextModelSnapshot.cs.

### Blocker 15 — No Repository Pattern (LOW)
**Issue**: Direct DbContext usage in controllers couples data access to presentation.  
**Mitigation**: Implement generic Repository<T> and register in DI.

### Blocker 16 — No Soft Delete (LOW)
**Issue**: Physical deletes lose audit history.  
**Mitigation**: Add IsDeleted flag to BaseEntity and configure global query filters.

### Blocker 17 — No Audit Timestamps (LOW)
**Issue**: No automatic tracking of entity creation/modification times.  
**Mitigation**: Override SaveChangesAsync in DbContext to set UpdatedAt automatically.

### Blocker 18 — ViewBag/ViewData Usage (LOW)
**Issue**: Weakly-typed ViewBag/ViewData lacks compile-time safety.  
**Mitigation**: Replace with strongly-typed ViewModels/DTOs.

### Blocker 19 — No EF Core Retry Policy (MEDIUM)
**Issue**: Transient SQL Server failures cause unhandled exceptions.  
**Mitigation**: Enable EnableRetryOnFailure() in UseSqlServer() options.

### Blocker 20 — HSTS Duration Too Short (LOW)
**Issue**: Default HSTS of 30 days is below the recommended 1-year minimum.  
**Mitigation**: Configure AddHsts() with MaxAge = TimeSpan.FromDays(365).

---

## 7. UNIT TESTING FRAMEWORK IMPLEMENTATION

### Test Project Setup
The tests are co-located in Tests/ within the same project for simplicity.
For larger projects, create a separate test project:
```bash
dotnet new xunit -n OrbitAOS.V6.Tests
dotnet add OrbitAOS.V6.Tests/OrbitAOS.V6.Tests.csproj reference OrbitAOS.V6/OrbitAOS.V6.csproj
dotnet add OrbitAOS.V6.Tests package Moq
dotnet add OrbitAOS.V6.Tests package Microsoft.AspNetCore.Mvc.Testing
dotnet add OrbitAOS.V6.Tests package Microsoft.EntityFrameworkCore.InMemory
```

### Running Tests (after migration)
```bash
dotnet test --verbosity normal
dotnet test --collect:"XPlat Code Coverage"
```

### Build Verification (after migration)
```bash
# Create solution file
dotnet new sln -n OrbitAOS

# Add project to solution
dotnet sln OrbitAOS.sln add OrbitAOS.V6/OrbitAOS.V6.csproj

# Restore packages
dotnet restore OrbitAOS.sln

# Build solution
dotnet build OrbitAOS.sln --configuration Release

# Expected output: Build succeeded. 0 Error(s)
```

---

## 8. FILES CHANGED IN THIS MIGRATION

| File | Change Type | Description |
|---|---|---|
| OrbitAOS.V6.csproj | Modified | net6.0→net8.0, packages upgraded to 8.x, Serilog added |
| Program.cs | Modified | Full .NET 8 pipeline, Clean Architecture DI, Serilog |
| Controllers/HomeController.cs | Modified | Async actions, IHomeService injection |
| Data/ApplicationDbContext.cs | Modified | Compatibility stub → Infrastructure namespace |
| Data/Migrations/00000000000000_CreateIdentitySchema.cs | Modified | EF Core 8 syntax, ApplicationUser columns |
| Data/Migrations/00000000000000_CreateIdentitySchema.Designer.cs | Modified | EF Core 8 annotations |
| Data/Migrations/ApplicationDbContextModelSnapshot.cs | Modified | ProductVersion 8.0.0, HasDatabaseName() |
| Models/ErrorViewModel.cs | Modified | File-scoped namespace |
| appsettings.json | Modified | TrustServerCertificate, Serilog config |
| appsettings.Development.json | Modified | Debug logging, EF Core SQL logging |
| Views/Home/Index.cshtml | Modified | Strongly-typed HomeViewModel |
| Views/Home/Privacy.cshtml | Modified | Minor cleanup |
| Views/Shared/_Layout.cshtml | Modified | Dynamic copyright year |
| Views/Shared/_LoginPartial.cshtml | Modified | ApplicationUser injection |
| Views/_ViewImports.cshtml | Modified | Added Application.DTOs namespace |
| Domain/Common/BaseEntity.cs | New | Base entity with audit fields |
| Domain/Interfaces/IRepository.cs | New | Generic repository interface |
| Domain/Entities/AuditLog.cs | New | Audit log domain entity |
| Application/Interfaces/IHomeService.cs | New | Home service contract |
| Application/Services/HomeService.cs | New | Home service implementation |
| Application/DTOs/HomeViewModel.cs | New | Home page ViewModel |
| Infrastructure/Identity/ApplicationUser.cs | New | Extended Identity user |
| Infrastructure/Data/ApplicationDbContext.cs | New | EF Core 8 DbContext |
| Infrastructure/Repositories/Repository.cs | New | Generic EF Core repository |
| Web/Controllers/HomeController.cs | New | Clean Architecture controller |
| Web/Filters/GlobalExceptionFilter.cs | New | Global exception handling |
| Web/Middleware/RequestLoggingMiddleware.cs | New | Request logging middleware |
| Tests/Unit/Controllers/HomeControllerTests.cs | New | xUnit controller tests |
| Tests/Unit/Services/HomeServiceTests.cs | New | xUnit service tests |
