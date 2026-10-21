# OrbitAOS.V6 - ASP.NET MVC 6.0 to .NET 7 Core MVC Migration Report
# Clean Architecture Implementation

## MIGRATION SUMMARY

**Source**: ASP.NET Core MVC on net6.0  
**Target**: ASP.NET Core MVC on .NET 7 (net7.0)  
**Architecture**: Clean Architecture (Domain / Application / Infrastructure / Web)  
**Date**: 2025-01-07  

---

## PHASE 1: PRE-MIGRATION ASSESSMENT

### Current State (net6.0)
- TargetFramework: net6.0
- All packages at version 6.0.19
- Single-layer MVC structure (no Clean Architecture)
- EF Core 6 with outdated migration syntax (EF Core 3.0 annotations)
- No Repository pattern
- No Unit of Work pattern
- No CQRS/MediatR
- No FluentValidation
- No AutoMapper

### Target State (net7.0)
- TargetFramework: net7.0
- All packages upgraded to 7.0.x
- Clean Architecture: Domain / Application / Infrastructure / Web layers
- EF Core 7 with updated migration syntax
- Generic Repository pattern
- Unit of Work pattern
- MediatR for CQRS
- FluentValidation for input validation
- AutoMapper for object mapping

---

## PHASE 2: ENVIRONMENT SETUP

### Required Tools
```bash
# Install .NET 7 SDK
# Download from: https://dotnet.microsoft.com/download/dotnet/7.0

# Verify installation
dotnet --version  # Should show 7.x.x

# Install EF Core CLI tools
dotnet tool install --global dotnet-ef --version 7.0.20

# Verify EF tools
dotnet ef --version
```

### Solution File Creation (Run AFTER migration)
```bash
# Create solution file
dotnet new sln -n OrbitAOS.V6

# Add project to solution
dotnet sln OrbitAOS.V6.sln add OrbitAOS.V6/OrbitAOS.V6.csproj

# Verify solution
dotnet sln OrbitAOS.V6.sln list
```

### Build Verification (Run AFTER migration)
```bash
# Restore packages
dotnet restore OrbitAOS.V6/OrbitAOS.V6.csproj

# Build the project
dotnet build OrbitAOS.V6/OrbitAOS.V6.csproj --configuration Release

# Expected output:
# Build succeeded.
# 0 Warning(s)
# 0 Error(s)
```

---

## PHASE 3: CLEAN ARCHITECTURE FOLDER STRUCTURE

### New Structure Created
```
OrbitAOS.V6/
├── Domain/                          ← NEW: Domain Layer
│   ├── Entities/
│   │   └── BaseEntity.cs            ← Base entity with audit fields
│   └── Interfaces/
│       ├── IRepository.cs           ← Generic repository interface
│       └── IUnitOfWork.cs           ← Unit of Work interface
│
├── Application/                     ← NEW: Application Layer
│   ├── ApplicationServiceExtensions.cs
│   ├── Common/
│   │   ├── Interfaces/
│   │   │   └── IApplicationDbContext.cs
│   │   ├── Mappings/
│   │   │   └── MappingProfile.cs    ← AutoMapper profiles
│   │   └── Models/
│   │       └── Result.cs            ← Result pattern
│
├── Infrastructure/                  ← NEW: Infrastructure Layer
│   ├── Persistence/
│   │   └── ApplicationDbContext.cs  ← EF Core 7 DbContext
│   ├── Repositories/
│   │   └── Repository.cs            ← Generic repository implementation
│   └── Services/
│       └── UnitOfWork.cs            ← Unit of Work implementation
│
├── Controllers/                     ← Web Layer (updated)
│   └── HomeController.cs
├── Views/                           ← Web Layer (updated)
├── Models/                          ← Web Layer ViewModels
├── Middleware/                      ← NEW: Custom middleware
│   └── ExceptionHandlingMiddleware.cs
├── Filters/                         ← NEW: Action filters
│   └── GlobalExceptionFilter.cs
├── Data/                            ← Legacy compatibility layer
│   ├── ApplicationDbContext.cs      ← Alias to Infrastructure DbContext
│   └── Migrations/                  ← Updated to EF Core 7 syntax
├── wwwroot/                         ← Static files (unchanged)
├── Program.cs                       ← Updated for .NET 7 + Clean Architecture
├── OrbitAOS.V6.csproj               ← Upgraded to net7.0
└── appsettings.json                 ← Updated configuration
```

### Legacy → .NET 7 Folder Mapping
| Legacy Folder | .NET 7 Equivalent | Notes |
|---|---|---|
| /Controllers | /Controllers | Same, updated to async |
| /Views | /Views | Same, Tag Helpers added |
| /Models | /Models + /Domain/Entities | Split: ViewModels stay, Entities move to Domain |
| /App_Start/RouteConfig.cs | Program.cs (MapControllerRoute) | Inline in Program.cs |
| /App_Start/FilterConfig.cs | Program.cs (AddControllersWithViews) | Inline in Program.cs |
| /App_Start/BundleConfig.cs | wwwroot/ + libman.json | Static files in wwwroot |
| /Content | /wwwroot/css | Static files |
| /Scripts | /wwwroot/js | Static files |
| /Global.asax | Program.cs | Middleware pipeline |
| /web.config | appsettings.json | Configuration |
| N/A | /Domain | NEW: Domain layer |
| N/A | /Application | NEW: Application layer |
| N/A | /Infrastructure | NEW: Infrastructure layer |
| N/A | /Middleware | NEW: Custom middleware |
| N/A | /Filters | NEW: Action filters |

---

## PHASE 4: TRANSFORMATION RULEBOOK (30+ Rules)

### RULE 01: Target Framework Upgrade
**Legacy (net6.0):**
```xml
<TargetFramework>net6.0</TargetFramework>
```
**NET 7:**
```xml
<TargetFramework>net7.0</TargetFramework>
```
**Explanation**: Upgrades the project to .NET 7 runtime and SDK.

### RULE 02: Package Version Upgrade
**Legacy:**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="6.0.19" />
```
**NET 7:**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="7.0.20" />
```
**Explanation**: All Microsoft.* packages must be upgraded to 7.0.x versions.

### RULE 03: Program.cs - Application Entry Point
**Legacy (net6.0 minimal hosting):**
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
var app = builder.Build();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
```
**NET 7 (with Clean Architecture):**
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null)));
builder.Services.AddDefaultIdentity<IdentityUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
```
**Explanation**: .NET 7 Program.cs adds Clean Architecture service registrations, EF Core retry logic, and FluentValidation.

### RULE 04: EF Core Migration Syntax - Column Type
**Legacy (EF Core 3.0):**
```csharp
Id = table.Column<string>(nullable: false)
```
**NET 7 (EF Core 7):**
```csharp
Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
```
**Explanation**: EF Core 7 requires explicit `type:` parameter in migration column definitions.

### RULE 05: EF Core Migration - Identity Column Annotation
**Legacy (EF Core 3.0):**
```csharp
.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
```
**NET 7 (EF Core 7):**
```csharp
.Annotation("SqlServer:Identity", "1, 1")
```
**Explanation**: EF Core 7 uses the simplified `SqlServer:Identity` annotation.

### RULE 06: EF Core Model Snapshot - HasName → HasDatabaseName
**Legacy (EF Core 3.0):**
```csharp
b.HasIndex("NormalizedName").IsUnique().HasName("RoleNameIndex")
```
**NET 7 (EF Core 7):**
```csharp
b.HasIndex("NormalizedName").IsUnique().HasDatabaseName("RoleNameIndex")
```
**Explanation**: `HasName()` is deprecated in EF Core 7; use `HasDatabaseName()`.

### RULE 07: EF Core Model Snapshot - ProductVersion
**Legacy:**
```csharp
modelBuilder.HasAnnotation("ProductVersion", "3.0.0")
```
**NET 7:**
```csharp
modelBuilder.HasAnnotation("ProductVersion", "7.0.20")
```
**Explanation**: ProductVersion must match the installed EF Core version.

### RULE 08: EF Core Model Snapshot - UseIdentityColumns
**Legacy:**
```csharp
modelBuilder.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
```
**NET 7:**
```csharp
SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);
```
**Explanation**: EF Core 7 uses extension method syntax for SQL Server identity columns.

### RULE 09: EF Core - ToTable with null schema
**Legacy:**
```csharp
b.ToTable("AspNetRoles")
```
**NET 7:**
```csharp
b.ToTable("AspNetRoles", (string)null)
```
**Explanation**: EF Core 7 requires explicit null schema parameter to avoid warnings.

### RULE 10: Controller - Async Actions
**Legacy:**
```csharp
public IActionResult Index() { return View(); }
```
**NET 7:**
```csharp
public async Task<IActionResult> Index()
{
    var result = await _mediator.Send(new GetIndexQuery());
    return View(result);
}
```
**Explanation**: .NET 7 best practice is async/await for all I/O-bound operations.

### RULE 11: Controller - Constructor Injection
**Legacy (DependencyResolver):**
```csharp
public HomeController()
{
    _service = DependencyResolver.Current.GetService<IMyService>();
}
```
**NET 7:**
```csharp
public HomeController(IMyService service, ILogger<HomeController> logger)
{
    _service = service;
    _logger = logger;
}
```
**Explanation**: .NET 7 uses built-in DI container; no DependencyResolver needed.

### RULE 12: OutputCache → ResponseCache
**Legacy:**
```csharp
[OutputCache(Duration = 60, VaryByParam = "none")]
public ActionResult Index() { ... }
```
**NET 7:**
```csharp
[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
public IActionResult Index() { ... }
```
**Explanation**: `[OutputCache]` from System.Web.Mvc is replaced by `[ResponseCache]` in ASP.NET Core.

### RULE 13: Html.ActionLink → Tag Helper
**Legacy:**
```html
@Html.ActionLink("Home", "Index", "Home", null, new { @class = "nav-link" })
```
**NET 7:**
```html
<a class="nav-link" asp-controller="Home" asp-action="Index">Home</a>
```
**Explanation**: Tag Helpers replace HtmlHelper extension methods in ASP.NET Core views.

### RULE 14: Html.Partial → Partial Tag Helper
**Legacy:**
```html
@Html.Partial("_LoginPartial")
```
**NET 7:**
```html
<partial name="_LoginPartial" />
```
**Explanation**: The `<partial>` Tag Helper replaces `@Html.Partial()` in ASP.NET Core.

### RULE 15: Html.BeginForm → Form Tag Helper
**Legacy:**
```html
@using (Html.BeginForm("Login", "Account", FormMethod.Post)) { }
```
**NET 7:**
```html
<form asp-controller="Account" asp-action="Login" method="post"> </form>
```
**Explanation**: Form Tag Helper replaces `Html.BeginForm()`.

### RULE 16: Html.ValidationSummary → Tag Helper
**Legacy:**
```html
@Html.ValidationSummary(true, "", new { @class = "text-danger" })
```
**NET 7:**
```html
<div asp-validation-summary="ModelOnly" class="text-danger"></div>
```
**Explanation**: `asp-validation-summary` Tag Helper replaces `Html.ValidationSummary()`.

### RULE 17: Html.LabelFor → Tag Helper
**Legacy:**
```html
@Html.LabelFor(m => m.Email, new { @class = "control-label" })
```
**NET 7:**
```html
<label asp-for="Email" class="control-label"></label>
```
**Explanation**: `asp-for` Tag Helper on `<label>` replaces `Html.LabelFor()`.

### RULE 18: Html.TextBoxFor → Tag Helper
**Legacy:**
```html
@Html.TextBoxFor(m => m.Email, new { @class = "form-control" })
```
**NET 7:**
```html
<input asp-for="Email" class="form-control" />
```
**Explanation**: `asp-for` Tag Helper on `<input>` replaces `Html.TextBoxFor()`.

### RULE 19: Html.ValidationMessageFor → Tag Helper
**Legacy:**
```html
@Html.ValidationMessageFor(m => m.Email, "", new { @class = "text-danger" })
```
**NET 7:**
```html
<span asp-validation-for="Email" class="text-danger"></span>
```
**Explanation**: `asp-validation-for` Tag Helper replaces `Html.ValidationMessageFor()`.

### RULE 20: Session State
**Legacy (System.Web):**
```csharp
Session["UserId"] = userId;
var userId = Session["UserId"];
```
**NET 7:**
```csharp
// In Program.cs: builder.Services.AddSession(); app.UseSession();
HttpContext.Session.SetString("UserId", userId.ToString());
var userId = HttpContext.Session.GetString("UserId");
```
**Explanation**: ASP.NET Core session requires explicit registration and uses typed methods.

### RULE 21: HttpContext Access
**Legacy:**
```csharp
var user = System.Web.HttpContext.Current.User;
var request = System.Web.HttpContext.Current.Request;
```
**NET 7:**
```csharp
// In controllers: HttpContext.User, HttpContext.Request (direct access)
// In services: inject IHttpContextAccessor
private readonly IHttpContextAccessor _httpContextAccessor;
var user = _httpContextAccessor.HttpContext?.User;
```
**Explanation**: `System.Web.HttpContext.Current` is not available in ASP.NET Core.

### RULE 22: Forms Authentication → Cookie Authentication
**Legacy (web.config):**
```xml
<authentication mode="Forms">
  <forms loginUrl="~/Account/Login" timeout="2880" />
</authentication>
```
**NET 7 (Program.cs):**
```csharp
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});
```
**Explanation**: Forms Authentication is replaced by Cookie Authentication middleware.

### RULE 23: Global.asax Application_Start → Program.cs
**Legacy (Global.asax.cs):**
```csharp
protected void Application_Start()
{
    AreaRegistration.RegisterAllAreas();
    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
    RouteConfig.RegisterRoutes(RouteTable.Routes);
    BundleConfig.RegisterBundles(BundleTable.Bundles);
}
```
**NET 7 (Program.cs):**
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews(options => { /* global filters */ });
var app = builder.Build();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
```
**Explanation**: All Global.asax startup code moves to Program.cs.

### RULE 24: RouteConfig → MapControllerRoute
**Legacy (App_Start/RouteConfig.cs):**
```csharp
routes.MapRoute(name: "Default", url: "{controller}/{action}/{id}", defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
```
**NET 7 (Program.cs):**
```csharp
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
```
**Explanation**: Route registration moves from RouteConfig.cs to Program.cs.

### RULE 25: FilterConfig → AddControllersWithViews
**Legacy (App_Start/FilterConfig.cs):**
```csharp
public static void RegisterGlobalFilters(GlobalFilterCollection filters)
{
    filters.Add(new HandleErrorAttribute());
}
```
**NET 7 (Program.cs):**
```csharp
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});
```
**Explanation**: Global filter registration moves to Program.cs AddControllersWithViews().

### RULE 26: BundleConfig → wwwroot Static Files
**Legacy (App_Start/BundleConfig.cs):**
```csharp
bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"));
bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/bootstrap.css"));
```
**NET 7 (wwwroot + _Layout.cshtml):**
```html
<link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
<script src="~/lib/jquery/dist/jquery.min.js"></script>
```
**Explanation**: Bundling/minification replaced by static files in wwwroot served directly.

### RULE 27: web.config → appsettings.json
**Legacy (web.config):**
```xml
<appSettings>
  <add key="MyKey" value="MyValue" />
</appSettings>
<connectionStrings>
  <add name="DefaultConnection" connectionString="..." />
</connectionStrings>
```
**NET 7 (appsettings.json):**
```json
{
  "MyKey": "MyValue",
  "ConnectionStrings": {
    "DefaultConnection": "..."
  }
}
```
**Explanation**: All configuration moves from web.config to appsettings.json.

### RULE 28: Configuration Access
**Legacy:**
```csharp
var value = ConfigurationManager.AppSettings["MyKey"];
var connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
```
**NET 7:**
```csharp
// Inject IConfiguration
var value = _configuration["MyKey"];
var connStr = _configuration.GetConnectionString("DefaultConnection");
// Or use IOptions<T> pattern
```
**Explanation**: ConfigurationManager is replaced by IConfiguration dependency injection.

### RULE 29: HTTP Modules → Middleware
**Legacy (IHttpModule):**
```csharp
public class LoggingModule : IHttpModule
{
    public void Init(HttpApplication context)
    {
        context.BeginRequest += OnBeginRequest;
    }
    private void OnBeginRequest(object sender, EventArgs e) { /* log */ }
}
```
**NET 7 (Middleware):**
```csharp
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    public LoggingMiddleware(RequestDelegate next) { _next = next; }
    public async Task InvokeAsync(HttpContext context)
    {
        // Before request
        await _next(context);
        // After request
    }
}
// Register: app.UseMiddleware<LoggingMiddleware>();
```
**Explanation**: IHttpModule is replaced by ASP.NET Core middleware.

### RULE 30: Child Actions → ViewComponents
**Legacy:**
```html
@Html.Action("RecentPosts", "Blog")
@Html.RenderAction("Navigation", "Shared")
```
**NET 7:**
```html
@await Component.InvokeAsync("RecentPosts")
<vc:recent-posts></vc:recent-posts>
```
**Explanation**: Child actions (Html.Action/RenderAction) are replaced by ViewComponents.

### RULE 31: ASP.NET Identity 2.x → ASP.NET Core Identity
**Legacy (AccountController):**
```csharp
var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
var signInManager = new SignInManager<ApplicationUser>(userManager, ...);
```
**NET 7:**
```csharp
// Injected via DI
public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
{
    _userManager = userManager;
    _signInManager = signInManager;
}
```
**Explanation**: UserManager/SignInManager are registered in DI and injected into controllers.

### RULE 32: EF6 → EF Core 7 DbContext
**Legacy (EF6):**
```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() : base("DefaultConnection") { }
    public DbSet<Product> Products { get; set; }
}
```
**NET 7 (EF Core 7):**
```csharp
public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<Product> Products => Set<Product>();
}
```
**Explanation**: EF Core 7 uses DbContextOptions injection instead of connection string in constructor.

### RULE 33: Repository Pattern (Clean Architecture)
**Legacy (direct DbContext in controller):**
```csharp
public class ProductController : Controller
{
    private ApplicationDbContext _db = new ApplicationDbContext();
    public ActionResult Index() { return View(_db.Products.ToList()); }
}
```
**NET 7 (Repository + MediatR):**
```csharp
public class ProductController : Controller
{
    private readonly IMediator _mediator;
    public ProductController(IMediator mediator) { _mediator = mediator; }
    public async Task<IActionResult> Index()
    {
        var products = await _mediator.Send(new GetProductsQuery());
        return View(products);
    }
}
```
**Explanation**: Clean Architecture separates data access into Repository/Infrastructure layer.

### RULE 34: Nullable Reference Types
**Legacy:**
```csharp
public string Name { get; set; }
public string RequestId { get; set; }
```
**NET 7 (with Nullable enabled):**
```csharp
public string Name { get; set; } = string.Empty;
public string? RequestId { get; set; }
```
**Explanation**: .NET 7 enables nullable reference types by default; properties need explicit nullability.

### RULE 35: IActionResult Return Types
**Legacy:**
```csharp
public ActionResult Index() { return View(); }
public JsonResult GetData() { return Json(data, JsonRequestBehavior.AllowGet); }
```
**NET 7:**
```csharp
public IActionResult Index() { return View(); }
public IActionResult GetData() { return Json(data); }  // No JsonRequestBehavior needed
```
**Explanation**: `ActionResult` is replaced by `IActionResult`; `JsonRequestBehavior` is removed.

---

## PHASE 5: DATA STRATEGY WITH EF CORE 7

### Database-First Scaffold Commands
```bash
# Scaffold DbContext and entities from existing database
dotnet ef dbcontext scaffold \
  "Server=(localdb)\mssqllocaldb;Database=OrbitAOS;Trusted_Connection=True;" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --output-dir Infrastructure/Persistence/Entities \
  --context-dir Infrastructure/Persistence \
  --context ApplicationDbContext \
  --namespace OrbitAOS.V6.Infrastructure.Persistence \
  --data-annotations \
  --force

# Parameters explained:
# --output-dir: Where to place generated entity classes
# --context-dir: Where to place the DbContext class
# --context: Name of the generated DbContext class
# --namespace: Namespace for generated classes
# --data-annotations: Use data annotations instead of Fluent API
# --force: Overwrite existing files
```

### Code-First Migration Commands
```bash
# Add initial migration
dotnet ef migrations add InitialCreate \
  --project OrbitAOS.V6 \
  --startup-project OrbitAOS.V6 \
  --output-dir Data/Migrations \
  --context OrbitAOS.V6.Infrastructure.Persistence.ApplicationDbContext

# Apply migrations to database
dotnet ef database update \
  --project OrbitAOS.V6 \
  --startup-project OrbitAOS.V6 \
  --context OrbitAOS.V6.Infrastructure.Persistence.ApplicationDbContext

# Generate SQL script for production deployment
dotnet ef migrations script \
  --project OrbitAOS.V6 \
  --startup-project OrbitAOS.V6 \
  --output migration.sql \
  --idempotent

# Parameters explained:
# --project: The project containing the DbContext
# --startup-project: The project with Program.cs
# --output-dir: Where to place migration files
# --context: Fully qualified DbContext class name
# --idempotent: Generate script that can be run multiple times safely
```

### DbContext Configuration (EF Core 7)
```csharp
// In Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        sqlOptions.CommandTimeout(30);
        sqlOptions.MigrationsAssembly("OrbitAOS.V6");
    });
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});
```

---

## PHASE 6: AUTHENTICATION MIGRATION

### A) Forms Authentication → Cookie Authentication

**Legacy web.config:**
```xml
<system.web>
  <authentication mode="Forms">
    <forms loginUrl="~/Account/Login" timeout="2880" slidingExpiration="true" />
  </authentication>
  <authorization>
    <deny users="?" />
  </authorization>
</system.web>
```

**NET 7 Program.cs:**
```csharp
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});
app.UseAuthentication();
app.UseAuthorization();
```

### B) ASP.NET Identity 2.x → ASP.NET Core Identity

**Legacy (ApplicationUser):**
```csharp
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```

**NET 7 (ApplicationUser):**
```csharp
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
```

**Legacy Identity Registration:**
```csharp
// Startup.cs (OWIN)
app.CreatePerOwinContext(ApplicationDbContext.Create);
app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
app.UseCookieAuthentication(new CookieAuthenticationOptions { AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie });
```

**NET 7 Identity Registration:**
```csharp
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.SignIn.RequireConfirmedAccount = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();
```

---

## PHASE 7: BLOCKERS & MITIGATIONS

### BLOCKER 01: net6.0 → net7.0 Target Framework
- **Impact**: Critical
- **Description**: Project targets net6.0; must be upgraded to net7.0
- **Mitigation**: Update `<TargetFramework>net7.0</TargetFramework>` in .csproj ✅ DONE

### BLOCKER 02: EF Core 6 → EF Core 7 Package Versions
- **Impact**: Critical
- **Description**: All EF Core packages at 6.0.19; must be 7.0.x
- **Mitigation**: Updated all packages to 7.0.20 in .csproj ✅ DONE

### BLOCKER 03: EF Core 3.0 Migration Syntax
- **Impact**: High
- **Description**: Migration files use EF Core 3.0 syntax (nullable parameter, old annotations)
- **Mitigation**: Updated migration files to EF Core 7 syntax ✅ DONE

### BLOCKER 04: Deprecated HasName() in Model Snapshot
- **Impact**: High
- **Description**: `HasName()` is deprecated in EF Core 7; causes build warnings/errors
- **Mitigation**: Replaced with `HasDatabaseName()` in snapshot ✅ DONE

### BLOCKER 05: Old SqlServer:ValueGenerationStrategy Annotation
- **Impact**: High
- **Description**: EF Core 3.0 annotation format not compatible with EF Core 7
- **Mitigation**: Updated to `SqlServer:Identity` annotation and `UseIdentityColumns()` ✅ DONE

### BLOCKER 06: No Clean Architecture Layers
- **Impact**: High
- **Description**: Single-layer MVC structure; no Domain/Application/Infrastructure separation
- **Mitigation**: Created Domain, Application, Infrastructure layers ✅ DONE

### BLOCKER 07: No Repository Pattern
- **Impact**: Medium
- **Description**: Direct DbContext usage in controllers; not testable
- **Mitigation**: Created IRepository<T> and Repository<T> ✅ DONE

### BLOCKER 08: No Unit of Work Pattern
- **Impact**: Medium
- **Description**: No transaction coordination across repositories
- **Mitigation**: Created IUnitOfWork and UnitOfWork ✅ DONE

### BLOCKER 09: No CQRS/MediatR
- **Impact**: Medium
- **Description**: No separation of commands and queries; fat controllers
- **Mitigation**: Added MediatR package and registered in Program.cs ✅ DONE

### BLOCKER 10: No FluentValidation
- **Impact**: Medium
- **Description**: Only DataAnnotations validation; limited for complex rules
- **Mitigation**: Added FluentValidation.AspNetCore package ✅ DONE

### BLOCKER 11: No AutoMapper
- **Impact**: Medium
- **Description**: Manual property mapping between entities and ViewModels
- **Mitigation**: Added AutoMapper and MappingProfile ✅ DONE

### BLOCKER 12: No Custom Middleware
- **Impact**: Medium
- **Description**: No centralized exception handling middleware
- **Mitigation**: Created ExceptionHandlingMiddleware ✅ DONE

### BLOCKER 13: No Action Filters
- **Impact**: Low
- **Description**: No global exception filter for MVC actions
- **Mitigation**: Created GlobalExceptionFilter ✅ DONE

### BLOCKER 14: Hardcoded Copyright Year in Layout
- **Impact**: Low
- **Description**: Static year in footer; needs dynamic year
- **Mitigation**: Updated to `@DateTime.Now.Year` in _Layout.cshtml ✅ DONE

### BLOCKER 15: Missing TrustServerCertificate in Connection String
- **Impact**: Medium
- **Description**: .NET 7 SQL Server connections require TrustServerCertificate=True for local dev
- **Mitigation**: Added TrustServerCertificate=True to connection string ✅ DONE

### BLOCKER 16: No Health Check Endpoint
- **Impact**: Low
- **Description**: No health check endpoint for monitoring
- **Mitigation**: Added AddHealthChecks() and MapHealthChecks("/health") ✅ DONE

### BLOCKER 17: No Session Configuration
- **Impact**: Medium
- **Description**: Session not configured; required for stateful features
- **Mitigation**: Added AddSession() and UseSession() in Program.cs ✅ DONE

### BLOCKER 18: No Response Caching Middleware
- **Impact**: Low
- **Description**: No response caching middleware registered
- **Mitigation**: Added AddResponseCaching() and UseResponseCaching() ✅ DONE

---

## PHASE 8: UNIT TESTING FRAMEWORK

### Test Project Setup
```bash
# Create xUnit test project
dotnet new xunit -n OrbitAOS.V6.Tests --framework net7.0

# Add to solution
dotnet sln OrbitAOS.V6.sln add OrbitAOS.V6.Tests/OrbitAOS.V6.Tests.csproj

# Add project reference
dotnet add OrbitAOS.V6.Tests/OrbitAOS.V6.Tests.csproj reference OrbitAOS.V6/OrbitAOS.V6.csproj

# Add test packages
dotnet add OrbitAOS.V6.Tests package Moq --version 4.20.70
dotnet add OrbitAOS.V6.Tests package Microsoft.EntityFrameworkCore.InMemory --version 7.0.20
dotnet add OrbitAOS.V6.Tests package FluentAssertions --version 6.12.0
```

### Example Controller Test
```csharp
using Moq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrbitAOS.V6.Controllers;
using Xunit;

public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _mockLogger;
    private readonly Mock<IMediator> _mockMediator;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _mockLogger = new Mock<ILogger<HomeController>>();
        _mockMediator = new Mock<IMediator>();
        _controller = new HomeController(_mockLogger.Object, _mockMediator.Object);
    }

    [Fact]
    public async Task Index_ReturnsViewResult()
    {
        // Act
        var result = await _controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
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
        var result = _controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<ErrorViewModel>(viewResult.Model);
    }
}
```

### Example Repository Test
```csharp
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Infrastructure.Persistence;
using OrbitAOS.V6.Infrastructure.Repositories;
using Xunit;

public class RepositoryTests
{
    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEntities()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new Repository<SomeEntity>(context);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
    }
}
```

---

## PHASE 9: BUILD VERIFICATION (Run AFTER migration)

### Step 1: Restore Packages
```bash
cd "OrbitAOS.V6"
dotnet restore
# Expected: Restore completed successfully
```

### Step 2: Build Project
```bash
dotnet build --configuration Release
# Expected:
#   Build succeeded.
#   0 Warning(s)
#   0 Error(s)
```

### Step 3: Create Solution File
```bash
cd ..
dotnet new sln -n OrbitAOS.V6
dotnet sln OrbitAOS.V6.sln add "OrbitAOS.V6/OrbitAOS.V6.csproj"
dotnet build OrbitAOS.V6.sln --configuration Release
```

### Step 4: Run Tests
```bash
dotnet test OrbitAOS.V6.Tests --configuration Release --verbosity normal
# Expected: All tests pass
```

### Step 5: EF Core Migrations
```bash
dotnet ef database update --project OrbitAOS.V6 --startup-project OrbitAOS.V6
# Expected: Database updated successfully
```

---

## PHASE 10: DEPLOYMENT & CUTOVER

### Production Deployment Checklist
- [ ] Update connection string in production appsettings.json
- [ ] Set ASPNETCORE_ENVIRONMENT=Production
- [ ] Run `dotnet ef database update` against production database
- [ ] Configure HTTPS certificate
- [ ] Set up health check monitoring at /health
- [ ] Configure logging (Application Insights, Serilog, etc.)
- [ ] Blue-green deployment for zero-downtime cutover
- [ ] Rollback plan: Keep net6.0 deployment available for 48 hours

### Rollback Plan
1. Keep the net6.0 deployment running in parallel
2. Use load balancer to switch traffic
3. If issues arise, redirect traffic back to net6.0
4. Investigate and fix issues before re-attempting cutover

---

## SUCCESS CRITERIA

- [ ] Project builds successfully with `dotnet build` targeting net7.0
- [ ] All EF Core 7 migrations apply without errors
- [ ] ASP.NET Core Identity works (login/logout/register)
- [ ] All existing routes respond correctly
- [ ] Clean Architecture layers are properly separated
- [ ] Repository pattern is implemented
- [ ] Unit tests pass with >80% coverage
- [ ] Health check endpoint responds at /health
- [ ] No System.Web references remain
- [ ] All packages are .NET 7 compatible
