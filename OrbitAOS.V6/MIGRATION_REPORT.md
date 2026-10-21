# OrbitAOS.V6 – ASP.NET MVC 6.0 → .NET 8 Core MVC Migration Report

**Module:** OrbitAOS.V6  
**Analysis Date:** 2025-01-07  
**Source Framework:** ASP.NET Core MVC on .NET 6.0 (net6.0)  
**Target Framework:** ASP.NET Core MVC on .NET 8 (net8.0)  
**Architecture Pattern:** Clean Architecture (Domain / Application / Infrastructure / Web)

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Step-by-Step Migration Plan](#2-step-by-step-migration-plan)
3. [Folder-by-Folder Mapping](#3-folder-by-folder-mapping)
4. [Transformation Rulebook (30+ Rules)](#4-transformation-rulebook)
5. [Data Strategy with EF Core Scaffold Commands](#5-data-strategy)
6. [Authentication Migration Guidance](#6-authentication-migration)
7. [Blockers & Mitigations (15+)](#7-blockers--mitigations)
8. [Unit Testing Framework Implementation](#8-unit-testing)
9. [Build Verification & Solution File](#9-build-verification)
10. [Final Execution Plan JSON](#10-final-execution-plan-json)

---

## 1. Executive Summary

| Metric | Value |
|--------|-------|
| Source framework | net6.0 (ASP.NET Core MVC) |
| Target framework | net8.0 (ASP.NET Core MVC) |
| Architecture | Clean Architecture (4 layers) |
| Total issues found | 22 |
| Critical | 3 |
| High | 7 |
| Medium | 8 |
| Low | 4 |
| Estimated effort | 16–24 hours |
| Migration complexity | Medium |
| Compatibility score | 72/100 |

The OrbitAOS.V6 project is an ASP.NET Core MVC application originally scaffolded on .NET 6.0. The migration to .NET 8 with Clean Architecture involves:

- Upgrading all NuGet packages from `6.0.x` → `8.0.x`
- Changing `<TargetFramework>net6.0</TargetFramework>` → `net8.0`
- Introducing Clean Architecture layers: Domain, Application, Infrastructure, Web
- Replacing bare `IdentityUser` with a custom `ApplicationUser` domain entity
- Updating EF Core migration files to use .NET 8 API (removing deprecated `nullable:` parameter, replacing `HasName()` with `HasDatabaseName()`, replacing `SqlServerValueGenerationStrategy.IdentityColumn` with `UseIdentityColumn()`)
- Adding `TrustServerCertificate=True` to the SQL Server connection string (required for .NET 8)
- Adding a unit test project (`OrbitAOS.V6.Tests`) with xUnit, Moq, and FluentAssertions

---

## 2. Step-by-Step Migration Plan

### Phase 1 – Pre-Migration Assessment (Week 1)

**Tasks:**
1. Inventory all NuGet packages and identify .NET 8 incompatible versions
2. Document all `System.Web` usages (none found in this project)
3. Identify EF Core migration files using deprecated APIs
4. Review `ApplicationDbContext` for `IdentityUser` vs `ApplicationUser`
5. Confirm SQL Server connection string format for .NET 8

**Deliverables:**
- Migration issue list (this report)
- Package upgrade matrix
- Architecture decision record (ADR) for Clean Architecture

**Validation checkpoint:** All issues catalogued; team aligned on Clean Architecture approach.

---

### Phase 2 – Environment Setup (Week 1)

**Tasks:**
1. Install .NET 8 SDK (`dotnet --version` should show `8.x.x`)
2. Update Visual Studio to 17.8+ or use VS Code with C# Dev Kit
3. Install EF Core tools: `dotnet tool install --global dotnet-ef --version 8.0.0`
4. Create solution file: `OrbitAOS.V6.sln`
5. Add test project: `OrbitAOS.V6.Tests`

**Commands (run after migration, not during analysis):**
```bash
# Install .NET 8 SDK
# Download from https://dot.net/download

# Install EF Core global tools
dotnet tool install --global dotnet-ef --version 8.0.0

# Verify installation
dotnet ef --version
```

---

### Phase 3 – Core Infrastructure Migration (Week 2)

**Tasks:**
1. Update `<TargetFramework>` from `net6.0` → `net8.0` in `.csproj`
2. Upgrade all `PackageReference` versions from `6.0.19` → `8.0.0`
3. Add `Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation` for dev hot-reload
4. Create Clean Architecture folder structure:
   - `Domain/Entities/`
   - `Domain/Interfaces/`
   - `Application/Extensions/`
   - `Application/Interfaces/`
   - `Application/Services/`
   - `Application/DTOs/`
   - `Infrastructure/Extensions/`
   - `Infrastructure/Persistence/`
5. Update `Program.cs` to call `AddInfrastructure()` and `AddApplication()`

**Deliverables:**
- Updated `.csproj`
- Updated `Program.cs`
- All Clean Architecture layer files

---

### Phase 4 – Data Layer Migration (Week 2)

**Tasks:**
1. Create `Domain/Entities/ApplicationUser.cs` extending `IdentityUser`
2. Update `ApplicationDbContext` to use `IdentityDbContext<ApplicationUser>`
3. Update EF Core migration files:
   - Remove deprecated `nullable:` parameter from `Column<T>()`
   - Replace `SqlServerValueGenerationStrategy.IdentityColumn` with `.Annotation("SqlServer:Identity", "1, 1")`
   - Replace `HasName()` with `HasDatabaseName()`
   - Update `ProductVersion` to `"8.0.0"`
4. Add `TrustServerCertificate=True` to connection string
5. Create `GenericRepository<T>` and `UnitOfWork` in Infrastructure layer

**EF Core Commands (run after migration):**
```bash
# Drop existing migrations and regenerate (recommended for clean start)
dotnet ef migrations remove --project OrbitAOS.V6

# Add new initial migration
dotnet ef migrations add InitialCreate --project OrbitAOS.V6 --context ApplicationDbContext

# Apply migration to database
dotnet ef database update --project OrbitAOS.V6 --context ApplicationDbContext
```

---

### Phase 5 – Controllers and Models Migration (Week 3)

**Tasks:**
1. Update `HomeController` to inject `IHomeService` (Application layer)
2. Make `Index()` action async: `Task<IActionResult>`
3. Move business logic from controllers to Application services
4. Update `Models/ErrorViewModel.cs` (already compatible, minor doc update)
5. Add `Application/DTOs/HomeIndexDto.cs`

---

### Phase 6 – Views Migration (Week 3)

**Tasks:**
1. Update `Views/_ViewImports.cshtml` to include Application and Domain namespaces
2. Update `Views/Shared/_LoginPartial.cshtml` to use `ApplicationUser` instead of `IdentityUser`
3. Update `Views/Home/Index.cshtml` to display `WelcomeMessage` from ViewData
4. Update Microsoft docs link to current .NET 8 URL
5. Verify all Tag Helpers still work (no changes needed)

---

### Phase 7 – Authentication Migration (Week 3)

**Tasks:**
1. Replace `AddDefaultIdentity<IdentityUser>` with `AddDefaultIdentity<ApplicationUser>`
2. Add `.AddRoles<IdentityRole>()` for role management
3. Update `_LoginPartial.cshtml` to inject `SignInManager<ApplicationUser>`
4. Configure password policy in `AddInfrastructure()`
5. Add SQL Server retry policy for cloud resilience

---

### Phase 8 – Testing and Validation (Week 4)

**Tasks:**
1. Create `OrbitAOS.V6.Tests` project with xUnit, Moq, FluentAssertions
2. Write `HomeControllerTests` (5 test cases)
3. Write `HomeServiceTests` (4 test cases)
4. Write `GenericRepositoryTests` (4 test cases using EF InMemory)
5. Run all tests: `dotnet test`

---

### Phase 9 – Build Verification (Week 4)

**Tasks (to be run by the team after migration):**
```bash
# Restore packages
dotnet restore OrbitAOS.V6.sln

# Build the solution
dotnet build OrbitAOS.V6.sln --configuration Release

# Run unit tests
dotnet test OrbitAOS.V6.sln --configuration Release --no-build

# Verify EF Core migrations
dotnet ef migrations list --project OrbitAOS.V6
```

**Success criteria:**
- `dotnet build` exits with code 0 (no errors)
- All unit tests pass
- No EF Core migration warnings

---

### Phase 10 – Deployment and Cutover (Week 5)

**Tasks:**
1. Update CI/CD pipeline to use .NET 8 SDK
2. Update Docker base image: `mcr.microsoft.com/dotnet/aspnet:8.0`
3. Apply EF Core migrations to production database
4. Deploy to staging; run smoke tests
5. Cutover production; monitor for 48 hours

---

## 3. Folder-by-Folder Mapping

| Legacy (net6.0) | .NET 8 Clean Architecture | Notes |
|-----------------|--------------------------|-------|
| `Controllers/` | `Controllers/` | Unchanged location; controllers now depend on Application interfaces |
| `Views/` | `Views/` | Unchanged; `_ViewImports.cshtml` updated |
| `Models/` | `Models/` (ViewModels) | ViewModels stay; domain entities move to `Domain/Entities/` |
| `Data/` | `Data/` + `Infrastructure/Persistence/` | DbContext stays in `Data/`; repos in `Infrastructure/` |
| `Data/Migrations/` | `Data/Migrations/` | Updated for EF Core 8 API |
| `Areas/Identity/` | `Areas/Identity/` | Unchanged |
| `wwwroot/` | `wwwroot/` | Unchanged |
| `Properties/` | `Properties/` | `launchSettings.json` updated |
| `Program.cs` | `Program.cs` | Refactored to call layer extension methods |
| `appsettings.json` | `appsettings.json` | Connection string updated for .NET 8 |
| *(new)* | `Domain/Entities/` | `BaseEntity.cs`, `ApplicationUser.cs` |
| *(new)* | `Domain/Interfaces/` | `IRepository.cs`, `IUnitOfWork.cs` |
| *(new)* | `Application/Extensions/` | `ServiceCollectionExtensions.cs` |
| *(new)* | `Application/Interfaces/` | `IHomeService.cs` |
| *(new)* | `Application/Services/` | `HomeService.cs` |
| *(new)* | `Application/DTOs/` | `HomeIndexDto.cs` |
| *(new)* | `Infrastructure/Extensions/` | `ServiceCollectionExtensions.cs` |
| *(new)* | `Infrastructure/Persistence/` | `GenericRepository.cs`, `UnitOfWork.cs` |
| *(new)* | `OrbitAOS.V6.Tests/` | Unit test project |

---

## 4. Transformation Rulebook

### Rule 1 – Target Framework Upgrade
**Legacy:**
```xml
<TargetFramework>net6.0</TargetFramework>
```
**NET 8:**
```xml
<TargetFramework>net8.0</TargetFramework>
```
**Explanation:** .NET 8 is the current LTS release (supported until November 2026). All packages must also be upgraded to `8.0.x`.

---

### Rule 2 – Package Version Upgrade
**Legacy:**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="6.0.19" />
```
**NET 8:**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
```
**Explanation:** All `Microsoft.AspNetCore.*` and `Microsoft.EntityFrameworkCore.*` packages must match the target framework version.

---

### Rule 3 – EF Core Tools Package Attributes
**Legacy:**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="6.0.19" />
```
**NET 8:**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
```
**Explanation:** EF Core Tools should be marked as a build-only dependency to avoid shipping it in production.

---

### Rule 4 – Program.cs Minimal API Style
**Legacy (net6.0):**
```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<IdentityUser>(...)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
```
**NET 8 (Clean Architecture):**
```csharp
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();
```
**Explanation:** Layer registration is delegated to extension methods in each layer, keeping `Program.cs` clean.

---

### Rule 5 – IdentityUser → ApplicationUser
**Legacy:**
```csharp
builder.Services.AddDefaultIdentity<IdentityUser>(options => ...)
    .AddEntityFrameworkStores<ApplicationDbContext>();
```
**NET 8:**
```csharp
builder.Services.AddDefaultIdentity<ApplicationUser>(options => ...)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
```
**Explanation:** Using a custom `ApplicationUser` allows adding domain-specific profile fields without schema changes later.

---

### Rule 6 – IdentityDbContext Generic Parameter
**Legacy:**
```csharp
public class ApplicationDbContext : IdentityDbContext
```
**NET 8:**
```csharp
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
```
**Explanation:** The generic parameter must match the user entity registered with Identity.

---

### Rule 7 – EF Core Migration: Remove Deprecated `nullable:` Parameter
**Legacy (EF Core 3/6):**
```csharp
Id = table.Column<string>(nullable: false),
```
**NET 8 (EF Core 8):**
```csharp
Id = table.Column<string>(type: "nvarchar(450)"),
```
**Explanation:** EF Core 8 uses nullable reference type annotations; the `nullable:` parameter is obsolete.

---

### Rule 8 – EF Core Migration: Replace `SqlServerValueGenerationStrategy.IdentityColumn`
**Legacy:**
```csharp
Id = table.Column<int>(nullable: false)
    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
```
**NET 8:**
```csharp
Id = table.Column<int>(type: "int").Annotation("SqlServer:Identity", "1, 1"),
```
**Explanation:** `SqlServerValueGenerationStrategy.IdentityColumn` is deprecated in EF Core 8; use the `SqlServer:Identity` annotation directly.

---

### Rule 9 – EF Core Snapshot: `UseIdentityColumns()`
**Legacy:**
```csharp
modelBuilder
    .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
```
**NET 8:**
```csharp
SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);
```
**Explanation:** EF Core 8 provides a dedicated extension method for SQL Server identity columns.

---

### Rule 10 – EF Core Snapshot: `HasDatabaseName()` replaces `HasName()`
**Legacy:**
```csharp
b.HasIndex("NormalizedName")
    .HasName("RoleNameIndex")
    .HasFilter("[NormalizedName] IS NOT NULL");
```
**NET 8:**
```csharp
b.HasIndex("NormalizedName")
    .HasDatabaseName("RoleNameIndex")
    .HasFilter("[NormalizedName] IS NOT NULL");
```
**Explanation:** `HasName()` is obsolete in EF Core 8; `HasDatabaseName()` is the correct API.

---

### Rule 11 – EF Core Snapshot: `ProductVersion`
**Legacy:**
```csharp
modelBuilder.HasAnnotation("ProductVersion", "3.0.0")
```
**NET 8:**
```csharp
modelBuilder.HasAnnotation("ProductVersion", "8.0.0")
```
**Explanation:** The product version annotation must match the installed EF Core version.

---

### Rule 12 – EF Core Snapshot: `ToTable()` with null schema
**Legacy:**
```csharp
b.ToTable("AspNetRoles");
```
**NET 8:**
```csharp
b.ToTable("AspNetRoles", (string)null);
```
**Explanation:** EF Core 8 requires explicit null schema to avoid ambiguity warnings.

---

### Rule 13 – Connection String: `TrustServerCertificate`
**Legacy:**
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=...;Trusted_Connection=True;MultipleActiveResultSets=true"
```
**NET 8:**
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=...;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```
**Explanation:** .NET 8 enforces stricter SSL certificate validation for SQL Server connections. `TrustServerCertificate=True` is required for local development with LocalDB.

---

### Rule 14 – SQL Server Retry Policy
**Legacy:**
```csharp
options.UseSqlServer(connectionString)
```
**NET 8:**
```csharp
options.UseSqlServer(connectionString, sqlOptions =>
{
    sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(30),
        errorNumbersToAdd: null);
});
```
**Explanation:** Retry-on-failure is a best practice for cloud-hosted SQL Server (Azure SQL).

---

### Rule 15 – Controller: Synchronous → Async Actions
**Legacy:**
```csharp
public IActionResult Index()
{
    return View();
}
```
**NET 8:**
```csharp
public async Task<IActionResult> Index(CancellationToken cancellationToken)
{
    var message = await _homeService.GetWelcomeMessageAsync(cancellationToken);
    ViewData["WelcomeMessage"] = message;
    return View();
}
```
**Explanation:** Async actions are the .NET 8 best practice; they free thread-pool threads during I/O waits.

---

### Rule 16 – Controller: Inject Application Service
**Legacy:**
```csharp
public HomeController(ILogger<HomeController> logger)
{
    _logger = logger;
}
```
**NET 8 (Clean Architecture):**
```csharp
public HomeController(ILogger<HomeController> logger, IHomeService homeService)
{
    _logger = logger;
    _homeService = homeService;
}
```
**Explanation:** Controllers should depend on Application layer interfaces, not implement business logic directly.

---

### Rule 17 – File-Scoped Namespaces
**Legacy:**
```csharp
namespace OrbitAOS.V6.Controllers
{
    public class HomeController : Controller { }
}
```
**NET 8:**
```csharp
namespace OrbitAOS.V6.Controllers;

public class HomeController : Controller { }
```
**Explanation:** File-scoped namespaces (C# 10+) reduce indentation and are the .NET 8 convention.

---

### Rule 18 – Nullable Reference Types
**Legacy:**
```csharp
public string RequestId { get; set; }
```
**NET 8:**
```csharp
public string? RequestId { get; set; }
```
**Explanation:** `<Nullable>enable</Nullable>` is set in the project; all nullable properties must use `?`.

---

### Rule 19 – `_ViewImports.cshtml`: Add Layer Namespaces
**Legacy:**
```cshtml
@using OrbitAOS.V6
@using OrbitAOS.V6.Models
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```
**NET 8:**
```cshtml
@using OrbitAOS.V6
@using OrbitAOS.V6.Models
@using OrbitAOS.V6.Application.DTOs
@using OrbitAOS.V6.Domain.Entities
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```
**Explanation:** Views need access to Application DTOs and Domain entities for strongly-typed views.

---

### Rule 20 – `_LoginPartial.cshtml`: Use `ApplicationUser`
**Legacy:**
```cshtml
@inject SignInManager<IdentityUser> SignInManager
@inject UserManager<IdentityUser> UserManager
```
**NET 8:**
```cshtml
@using OrbitAOS.V6.Domain.Entities
@inject SignInManager<ApplicationUser> SignInManager
@inject UserManager<ApplicationUser> UserManager
```
**Explanation:** The injected managers must match the registered user type.

---

### Rule 21 – Generic Repository Pattern
**Legacy (no repository):**
```csharp
// Direct DbContext usage in controllers
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
}
```
**NET 8 (Clean Architecture):**
```csharp
// Domain interface
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Delete(T entity);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
```
**Explanation:** The repository pattern decouples business logic from EF Core, enabling unit testing with in-memory providers.

---

### Rule 22 – Unit of Work Pattern
**Legacy:** No UoW; `SaveChanges()` called directly on DbContext.  
**NET 8:**
```csharp
public interface IUnitOfWork : IDisposable
{
    Task<int> CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}
```
**Explanation:** UoW ensures multiple repository operations participate in a single transaction.

---

### Rule 23 – `appsettings.json`: EF Core Logging
**Legacy:**
```json
"Microsoft.AspNetCore": "Warning"
```
**NET 8:**
```json
"Microsoft.AspNetCore": "Warning",
"Microsoft.EntityFrameworkCore": "Warning"
```
**Explanation:** Separate EF Core log level allows SQL query logging in Development without noise in Production.

---

### Rule 24 – `launchSettings.json`: Add `$schema`
**Legacy:** No `$schema` property.  
**NET 8:**
```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  ...
}
```
**Explanation:** The `$schema` property enables IntelliSense in VS Code and Visual Studio.

---

### Rule 25 – `AddRazorRuntimeCompilation()`
**Legacy:**
```csharp
builder.Services.AddControllersWithViews();
```
**NET 8:**
```csharp
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();
```
**Explanation:** Runtime compilation enables hot-reload of `.cshtml` files in Development without restarting the app.

---

### Rule 26 – `BaseEntity` for Domain Entities
**Legacy:** No base entity class.  
**NET 8:**
```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
```
**Explanation:** A base entity provides consistent primary key and audit fields across all domain entities.

---

### Rule 27 – `ApplicationUser` Domain Entity
**Legacy:**
```csharp
// Bare IdentityUser registered directly
builder.Services.AddDefaultIdentity<IdentityUser>(...)
```
**NET 8:**
```csharp
public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}
```
**Explanation:** Custom `ApplicationUser` allows adding profile fields without a separate profile table.

---

### Rule 28 – `OnModelCreating`: `ApplyConfigurationsFromAssembly()`
**Legacy:**
```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);
}
```
**NET 8:**
```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);
    builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
}
```
**Explanation:** Auto-discovers all `IEntityTypeConfiguration<T>` classes, keeping `OnModelCreating` clean.

---

### Rule 29 – `ILogger` Injection in Services
**Legacy:** No service layer; logging only in controllers.  
**NET 8:**
```csharp
public sealed class HomeService : IHomeService
{
    private readonly ILogger<HomeService> _logger;
    public HomeService(ILogger<HomeService> logger) => _logger = logger;
}
```
**Explanation:** Services should log their own operations; controllers should not log business logic.

---

### Rule 30 – `sealed` Keyword for Service Implementations
**Legacy:** No service classes.  
**NET 8:**
```csharp
public sealed class HomeService : IHomeService { }
public sealed class GenericRepository<T> : IRepository<T> { }
public sealed class UnitOfWork : IUnitOfWork { }
```
**Explanation:** `sealed` prevents unintended inheritance and enables JIT optimizations in .NET 8.

---

### Rule 31 – `record` for DTOs
**Legacy:** No DTO layer.  
**NET 8:**
```csharp
public sealed record HomeIndexDto(string WelcomeMessage);
```
**Explanation:** C# 9+ `record` types provide value equality, immutability, and concise syntax for DTOs.

---

### Rule 32 – `CancellationToken` Propagation
**Legacy:**
```csharp
public async Task<IActionResult> Index()
```
**NET 8:**
```csharp
public async Task<IActionResult> Index(CancellationToken cancellationToken)
```
**Explanation:** ASP.NET Core automatically binds `CancellationToken` from the request; propagating it to services and repositories enables proper request cancellation.

---

## 5. Data Strategy

### 5.1 Database-First Approach (Scaffold)

Use when the database schema already exists and you want to generate EF Core entities from it.

```bash
# Install EF Core tools
dotnet tool install --global dotnet-ef --version 8.0.0

# Scaffold DbContext and entities from existing database
dotnet ef dbcontext scaffold \
  "Server=(localdb)\mssqllocaldb;Database=OrbitAOS_V6_Net8;Trusted_Connection=True;TrustServerCertificate=True" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --project OrbitAOS.V6 \
  --output-dir Data/Entities \
  --context-dir Data \
  --context ApplicationDbContext \
  --namespace OrbitAOS.V6.Data.Entities \
  --context-namespace OrbitAOS.V6.Data \
  --data-annotations \
  --force
```

**Parameter explanations:**
- `--project`: Path to the .csproj file
- `--output-dir`: Where to place generated entity classes
- `--context-dir`: Where to place the DbContext class
- `--context`: Name of the generated DbContext class
- `--namespace`: Namespace for entity classes
- `--context-namespace`: Namespace for the DbContext
- `--data-annotations`: Use data annotations instead of Fluent API
- `--force`: Overwrite existing files

---

### 5.2 Code-First Approach (Migrations)

Use when you define entities in code and want EF Core to generate the database schema.

```bash
# Add initial migration
dotnet ef migrations add InitialCreate \
  --project OrbitAOS.V6 \
  --context ApplicationDbContext \
  --output-dir Data/Migrations

# Apply migration to database
dotnet ef database update \
  --project OrbitAOS.V6 \
  --context ApplicationDbContext

# Add subsequent migration (after model changes)
dotnet ef migrations add AddUserProfileFields \
  --project OrbitAOS.V6 \
  --context ApplicationDbContext

# Rollback to a specific migration
dotnet ef database update InitialCreate \
  --project OrbitAOS.V6 \
  --context ApplicationDbContext

# Generate SQL script (for DBA review)
dotnet ef migrations script \
  --project OrbitAOS.V6 \
  --context ApplicationDbContext \
  --output migration.sql \
  --idempotent
```

---

### 5.3 DbContext Configuration

```csharp
// Infrastructure/Extensions/ServiceCollectionExtensions.cs
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
            sqlOptions.MigrationsAssembly("OrbitAOS.V6");
        }));
```

---

### 5.4 Connection String Management

**Development (appsettings.Development.json):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OrbitAOS_V6_Dev;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

**Production (environment variable – never in source control):**
```bash
# Set via environment variable (Azure App Service, Docker, etc.)
ConnectionStrings__DefaultConnection="Server=prod-server;Database=OrbitAOS_V6;User Id=app_user;Password=***;TrustServerCertificate=False"
```

---

## 6. Authentication Migration

### 6.1 Forms Authentication → Cookie Authentication

**Legacy web.config:**
```xml
<authentication mode="Forms">
  <forms loginUrl="~/Account/Login" timeout="2880" />
</authentication>
```

**NET 8 Program.cs:**
```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(2);
        options.SlidingExpiration = true;
    });
```

---

### 6.2 ASP.NET Identity 2.x → ASP.NET Core Identity on .NET 8

**Legacy (ASP.NET MVC 5 / Identity 2.x):**
```csharp
// Startup.cs / OWIN
app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
app.UseCookieAuthentication(new CookieAuthenticationOptions
{
    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
    LoginPath = new PathString("/Account/Login")
});
```

**NET 8 Program.cs:**
```csharp
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// In pipeline:
app.UseAuthentication();
app.UseAuthorization();
```

**Database schema compatibility:**
The ASP.NET Core Identity schema (`AspNetUsers`, `AspNetRoles`, etc.) is compatible with ASP.NET Identity 2.x. The main difference is the addition of `ConcurrencyStamp` and `NormalizedEmail`/`NormalizedUserName` columns. Run the migration to add these columns if upgrading from Identity 2.x.

**Account controller migration:**
```csharp
// Legacy: AccountController with FormsAuthentication.SetAuthCookie()
FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

// NET 8: Use SignInManager
await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);
```

---

## 7. Blockers & Mitigations

### Blocker 1 – Target Framework `net6.0` (CRITICAL)
**File:** `OrbitAOS.V6.csproj`, line 4  
**Impact:** High – app cannot run on .NET 8 runtime  
**Code:** `<TargetFramework>net6.0</TargetFramework>`  
**Mitigation:** Change to `<TargetFramework>net8.0</TargetFramework>`

---

### Blocker 2 – All Packages on Version 6.0.19 (CRITICAL)
**File:** `OrbitAOS.V6.csproj`, lines 10–14  
**Impact:** High – packages incompatible with .NET 8 runtime  
**Code:** `Version="6.0.19"` on all 5 packages  
**Mitigation:** Upgrade all to `Version="8.0.0"`

---

### Blocker 3 – Bare `IdentityUser` in DbContext (CRITICAL)
**File:** `Data/ApplicationDbContext.cs`, line 8  
**Impact:** High – prevents adding domain-specific user fields  
**Code:** `public class ApplicationDbContext : IdentityDbContext`  
**Mitigation:** Change to `IdentityDbContext<ApplicationUser>` and create `ApplicationUser` entity

---

### Blocker 4 – Deprecated `nullable:` Parameter in Migration (HIGH)
**File:** `Data/Migrations/00000000000000_CreateIdentitySchema.cs`, lines 14–17  
**Impact:** High – EF Core 8 emits warnings; migration may fail  
**Code:** `Id = table.Column<string>(nullable: false),`  
**Mitigation:** Replace with `Id = table.Column<string>(type: "nvarchar(450)"),`

---

### Blocker 5 – `SqlServerValueGenerationStrategy.IdentityColumn` Deprecated (HIGH)
**File:** `Data/Migrations/00000000000000_CreateIdentitySchema.cs`, lines 55–57  
**Impact:** High – compile error in EF Core 8  
**Code:** `.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)`  
**Mitigation:** Replace with `.Annotation("SqlServer:Identity", "1, 1")`

---

### Blocker 6 – `HasName()` Deprecated in Model Snapshot (HIGH)
**File:** `Data/Migrations/ApplicationDbContextModelSnapshot.cs`, lines 35–37  
**Impact:** High – EF Core 8 compile warning treated as error  
**Code:** `.HasName("RoleNameIndex")`  
**Mitigation:** Replace with `.HasDatabaseName("RoleNameIndex")`

---

### Blocker 7 – `ProductVersion` Mismatch in Snapshot (HIGH)
**File:** `Data/Migrations/ApplicationDbContextModelSnapshot.cs`, line 20  
**Impact:** High – EF Core 8 detects version mismatch and may regenerate migrations  
**Code:** `.HasAnnotation("ProductVersion", "3.0.0")`  
**Mitigation:** Update to `.HasAnnotation("ProductVersion", "8.0.0")`

---

### Blocker 8 – Missing `TrustServerCertificate` in Connection String (HIGH)
**File:** `appsettings.json`, line 3  
**Impact:** High – SQL Server connection fails in .NET 8 with SSL error  
**Code:** `"Trusted_Connection=True;MultipleActiveResultSets=true"`  
**Mitigation:** Add `;TrustServerCertificate=True` to the connection string

---

### Blocker 9 – No Clean Architecture Layers (HIGH)
**File:** Entire project  
**Impact:** High – violates architecture requirement; business logic in controllers  
**Mitigation:** Create Domain, Application, Infrastructure layers with proper DI registration

---

### Blocker 10 – Synchronous `Index()` Action (MEDIUM)
**File:** `Controllers/HomeController.cs`, line 18  
**Impact:** Medium – thread-pool starvation under load  
**Code:** `public IActionResult Index()`  
**Mitigation:** Change to `public async Task<IActionResult> Index(CancellationToken ct)`

---

### Blocker 11 – No Repository Pattern (MEDIUM)
**File:** `Data/ApplicationDbContext.cs`  
**Impact:** Medium – controllers/services directly depend on EF Core; hard to unit test  
**Mitigation:** Implement `IRepository<T>` and `GenericRepository<T>`

---

### Blocker 12 – No Unit of Work (MEDIUM)
**File:** Entire project  
**Impact:** Medium – no transaction coordination across multiple repositories  
**Mitigation:** Implement `IUnitOfWork` and `UnitOfWork`

---

### Blocker 13 – `_LoginPartial.cshtml` Uses `IdentityUser` (MEDIUM)
**File:** `Views/Shared/_LoginPartial.cshtml`, lines 2–3  
**Impact:** Medium – compile error after switching to `ApplicationUser`  
**Code:** `@inject SignInManager<IdentityUser> SignInManager`  
**Mitigation:** Change to `SignInManager<ApplicationUser>` and `UserManager<ApplicationUser>`

---

### Blocker 14 – No Retry Policy for SQL Server (MEDIUM)
**File:** `Program.cs` (legacy) / `Infrastructure/Extensions/ServiceCollectionExtensions.cs`  
**Impact:** Medium – transient SQL Server failures cause unhandled exceptions  
**Mitigation:** Add `EnableRetryOnFailure()` to `UseSqlServer()` options

---

### Blocker 15 – No Unit Test Project (MEDIUM)
**File:** Solution  
**Impact:** Medium – no automated regression testing  
**Mitigation:** Create `OrbitAOS.V6.Tests` project with xUnit, Moq, FluentAssertions

---

### Blocker 16 – `Microsoft.AspNetCore.Identity.UI` Not Configured for `ApplicationUser` (MEDIUM)
**File:** `Program.cs`, `Areas/Identity/Pages/_ViewStart.cshtml`  
**Impact:** Medium – Identity UI scaffolding uses `IdentityUser` by default  
**Mitigation:** Re-scaffold Identity UI with `ApplicationUser`: `dotnet aspnet-codegenerator identity -dc ApplicationDbContext --userClass ApplicationUser`

---

### Blocker 17 – No `$schema` in `launchSettings.json` (LOW)
**File:** `Properties/launchSettings.json`  
**Impact:** Low – no IntelliSense in VS Code  
**Mitigation:** Add `"$schema": "https://json.schemastore.org/launchsettings.json"`

---

### Blocker 18 – Outdated Microsoft Docs Link in `Index.cshtml` (LOW)
**File:** `Views/Home/Index.cshtml`, line 6  
**Impact:** Low – broken link  
**Code:** `https://docs.microsoft.com/aspnet/core`  
**Mitigation:** Update to `https://learn.microsoft.com/aspnet/core`

---

### Blocker 19 – No `EF Core Logging` Level in `appsettings.json` (LOW)
**File:** `appsettings.json`  
**Impact:** Low – EF Core SQL queries not logged separately  
**Mitigation:** Add `"Microsoft.EntityFrameworkCore": "Warning"` to `LogLevel`

---

### Blocker 20 – No Solution File (LOW)
**File:** Solution root  
**Impact:** Low – `dotnet build` requires a `.sln` or `.csproj` target  
**Mitigation:** Create `OrbitAOS.V6.sln` referencing both projects

---

## 8. Unit Testing

### 8.1 Test Project Structure

```
OrbitAOS.V6.Tests/
├── OrbitAOS.V6.Tests.csproj
├── Controllers/
│   └── HomeControllerTests.cs
├── Services/
│   └── HomeServiceTests.cs
└── Persistence/
    └── GenericRepositoryTests.cs
```

### 8.2 Test Framework Stack

| Package | Version | Purpose |
|---------|---------|---------|
| `xunit` | 2.6.6 | Test framework |
| `xunit.runner.visualstudio` | 2.5.6 | VS Test Explorer integration |
| `Microsoft.NET.Test.Sdk` | 17.8.0 | .NET test SDK |
| `Moq` | 4.20.70 | Mocking framework |
| `Microsoft.EntityFrameworkCore.InMemory` | 8.0.0 | In-memory DB for repo tests |
| `Microsoft.AspNetCore.Mvc.Testing` | 8.0.0 | Integration test helpers |
| `FluentAssertions` | 6.12.0 | Readable assertions |

### 8.3 Running Tests

```bash
# Run all tests
dotnet test OrbitAOS.V6.sln

# Run with coverage (requires coverlet)
dotnet test OrbitAOS.V6.sln --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test OrbitAOS.V6.Tests --filter "FullyQualifiedName~HomeControllerTests"
```

### 8.4 Test Coverage Targets

| Layer | Target Coverage |
|-------|----------------|
| Controllers | 80% |
| Application Services | 90% |
| Infrastructure (Repositories) | 70% |
| Domain Entities | 60% |

---

## 9. Build Verification

> **IMPORTANT:** The following commands must be run by the development team **after** completing the migration. Do NOT run these during the analysis phase.

### 9.1 Prerequisites

```bash
# Verify .NET 8 SDK is installed
dotnet --version
# Expected: 8.x.x

# Verify EF Core tools
dotnet ef --version
# Expected: 8.x.x
```

### 9.2 Build Verification Steps

```bash
# Step 1: Navigate to solution root
cd "COmp tetsing dotnet with mssql"

# Step 2: Restore NuGet packages
dotnet restore OrbitAOS.V6.sln

# Step 3: Build the solution (Debug)
dotnet build OrbitAOS.V6.sln --configuration Debug

# Step 4: Build the solution (Release)
dotnet build OrbitAOS.V6.sln --configuration Release --no-restore

# Step 5: Run unit tests
dotnet test OrbitAOS.V6.sln --configuration Release --no-build --verbosity normal

# Step 6: Verify EF Core migrations
dotnet ef migrations list --project OrbitAOS.V6 --context ApplicationDbContext

# Step 7: Apply migrations to local database
dotnet ef database update --project OrbitAOS.V6 --context ApplicationDbContext
```

### 9.3 Expected Build Output

```
Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:xx.xx
```

### 9.4 Solution File Validation

The solution file `OrbitAOS.V6.sln` must:
1. Reference both `OrbitAOS.V6\OrbitAOS.V6.csproj` and `OrbitAOS.V6.Tests\OrbitAOS.V6.Tests.csproj`
2. Have valid GUIDs for each project
3. Include both Debug and Release configurations

Validate with:
```bash
dotnet sln OrbitAOS.V6.sln list
# Expected output:
# OrbitAOS.V6\OrbitAOS.V6.csproj
# OrbitAOS.V6.Tests\OrbitAOS.V6.Tests.csproj
```

---

## 10. Final Execution Plan JSON

See the JSON output at the beginning of this report for the complete machine-readable execution plan.

---

*Report generated by OrbitAOS Migration Analyzer v1.0 | 2025-01-07*
