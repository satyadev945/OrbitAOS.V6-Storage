# ASP.NET MVC to .NET 8 Core MVC Migration Guide

## Executive Summary

This document provides a comprehensive guide for migrating legacy ASP.NET MVC 3/4/5 applications to ASP.NET Core MVC on .NET 8 with Clean Architecture.

## Table of Contents

1. [Migration Overview](#migration-overview)
2. [Prerequisites](#prerequisites)
3. [Architecture Transformation](#architecture-transformation)
4. [Step-by-Step Migration Plan](#step-by-step-migration-plan)
5. [Transformation Rules](#transformation-rules)
6. [Data Migration Strategy](#data-migration-strategy)
7. [Authentication Migration](#authentication-migration)
8. [Common Blockers and Solutions](#common-blockers-and-solutions)
9. [Testing Strategy](#testing-strategy)
10. [Deployment](#deployment)

## Migration Overview

### Source Framework
- **ASP.NET MVC 3/4/5** on .NET Framework 4.5/4.6/4.7
- **Entity Framework 6.x**
- **ASP.NET Identity 2.x** (MVC 5)
- **System.Web** dependencies

### Target Framework
- **ASP.NET Core MVC** on .NET 8
- **Entity Framework Core 8.0**
- **ASP.NET Core Identity**
- **Modern HTTP abstractions**

### Architecture Pattern
**Clean Architecture** with four layers:
1. **Domain Layer**: Entities, value objects, domain logic
2. **Application Layer**: Business logic, DTOs, interfaces
3. **Infrastructure Layer**: Data access, external services
4. **Web Layer**: Controllers, views, UI concerns

## Prerequisites

### Development Environment
- ✅ .NET 8 SDK (8.0.0 or later)
- ✅ Visual Studio 2022 (17.8+) or VS Code
- ✅ SQL Server 2019+ or SQL Server LocalDB
- ✅ Git for version control

### Install .NET 8 SDK
```bash
# Download from https://dot.net/download
# Verify installation
dotnet --version
```

### Install EF Core Tools
```bash
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
```

## Architecture Transformation

### Legacy ASP.NET MVC Structure
```
LegacyApp/
├── App_Start/
│   ├── RouteConfig.cs
│   ├── FilterConfig.cs
│   └── BundleConfig.cs
├── Controllers/
├── Models/
├── Views/
├── Content/
├── Scripts/
├── Global.asax
├── web.config
└── packages.config
```

### Clean Architecture Structure (.NET 8)
```
ModernApp/
├── Domain/
│   ├── Entities/
│   ├── Common/
│   └── Exceptions/
├── Application/
│   ├── DTOs/
│   ├── Interfaces/
│   ├── Services/
│   ├── Features/ (CQRS)
│   └── Validators/
├── Infrastructure/
│   ├── Data/
│   ├── Repositories/
│   └── Services/
├── Web/
│   ├── Controllers/
│   ├── Views/
│   ├── wwwroot/
│   ├── Filters/
│   ├── Middleware/
│   └── Program.cs
└── Tests/
```

## Step-by-Step Migration Plan

### Phase 1: Assessment and Planning (Week 1)

#### 1.1 Inventory Current Application
```bash
# Scan for System.Web dependencies
grep -r "using System.Web" --include="*.cs"

# Find all controllers
find . -name "*Controller.cs"

# List all NuGet packages
cat packages.config
```

#### 1.2 Identify Dependencies
- List all NuGet packages
- Identify System.Web usage
- Document custom HTTP modules/handlers
- Review authentication mechanism
- Catalog third-party libraries

#### 1.3 Create Migration Checklist
- [ ] Controllers count: ___
- [ ] Views count: ___
- [ ] Models count: ___
- [ ] Custom filters: ___
- [ ] HTTP modules: ___
- [ ] Authentication type: ___
- [ ] Database: ___
- [ ] Third-party packages: ___

### Phase 2: Environment Setup (Week 1)

#### 2.1 Create Solution Structure
```bash
# Create solution
dotnet new sln -n OrbitAOS.V6

# Create Domain project
dotnet new classlib -n OrbitAOS.V6.Domain -f net8.0
dotnet sln add OrbitAOS.V6.Domain

# Create Application project
dotnet new classlib -n OrbitAOS.V6.Application -f net8.0
dotnet sln add OrbitAOS.V6.Application

# Create Infrastructure project
dotnet new classlib -n OrbitAOS.V6.Infrastructure -f net8.0
dotnet sln add OrbitAOS.V6.Infrastructure

# Create Web project
dotnet new mvc -n OrbitAOS.V6 -f net8.0 --auth Individual
dotnet sln add OrbitAOS.V6

# Create Test project
dotnet new xunit -n OrbitAOS.V6.Tests -f net8.0
dotnet sln add OrbitAOS.V6.Tests
```

#### 2.2 Add Project References
```bash
# Application references Domain
dotnet add OrbitAOS.V6.Application reference OrbitAOS.V6.Domain

# Infrastructure references Application and Domain
dotnet add OrbitAOS.V6.Infrastructure reference OrbitAOS.V6.Application
dotnet add OrbitAOS.V6.Infrastructure reference OrbitAOS.V6.Domain

# Web references Application and Infrastructure
dotnet add OrbitAOS.V6 reference OrbitAOS.V6.Application
dotnet add OrbitAOS.V6 reference OrbitAOS.V6.Infrastructure

# Tests reference all projects
dotnet add OrbitAOS.V6.Tests reference OrbitAOS.V6.Domain
dotnet add OrbitAOS.V6.Tests reference OrbitAOS.V6.Application
dotnet add OrbitAOS.V6.Tests reference OrbitAOS.V6.Infrastructure
dotnet add OrbitAOS.V6.Tests reference OrbitAOS.V6
```

#### 2.3 Install NuGet Packages

**Domain Layer** (No dependencies)
```bash
cd OrbitAOS.V6.Domain
# No packages needed - pure .NET
```

**Application Layer**
```bash
cd OrbitAOS.V6.Application
dotnet add package AutoMapper --version 13.0.1
dotnet add package FluentValidation --version 11.9.0
dotnet add package MediatR --version 12.2.0
```

**Infrastructure Layer**
```bash
cd OrbitAOS.V6.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.0
```

**Web Layer**
```bash
cd OrbitAOS.V6
dotnet add package Microsoft.AspNetCore.Identity.UI --version 8.0.0
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 13.0.1
dotnet add package FluentValidation.AspNetCore --version 11.3.0
dotnet add package MediatR --version 12.2.0
```

**Test Layer**
```bash
cd OrbitAOS.V6.Tests
dotnet add package xunit --version 2.6.2
dotnet add package xunit.runner.visualstudio --version 2.5.4
dotnet add package Moq --version 4.20.70
dotnet add package FluentAssertions --version 6.12.0
dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 8.0.0
```

### Phase 3: Domain Layer Migration (Week 2)

#### 3.1 Create Base Entity
```csharp
// Domain/Common/BaseEntity.cs
public abstract class BaseEntity : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string? DeletedBy { get; set; }
}
```

#### 3.2 Migrate Entities
```csharp
// Legacy: Models/Product.cs
public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Modern: Domain/Entities/Product.cs
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    
    // Domain logic
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ValidationException(nameof(Price), "Price must be greater than zero");
        
        Price = newPrice;
    }
}
```

#### 3.3 Create Domain Exceptions
```csharp
// Domain/Exceptions/DomainException.cs
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}

// Domain/Exceptions/EntityNotFoundException.cs
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object key)
        : base($"Entity '{entityName}' with key '{key}' was not found.") { }
}
```

### Phase 4: Application Layer Migration (Week 2-3)

#### 4.1 Create DTOs
```csharp
// Application/DTOs/ProductDto.cs
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
}

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
}

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
}
```

#### 4.2 Create Validators
```csharp
// Application/Validators/ProductValidators.cs
public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero");
    }
}
```

#### 4.3 Create Service Interfaces
```csharp
// Application/Interfaces/IProductService.cs
public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto> CreateProductAsync(CreateProductDto dto);
    Task UpdateProductAsync(int id, UpdateProductDto dto);
    Task DeleteProductAsync(int id);
}
```

#### 4.4 Implement Services
```csharp
// Application/Services/ProductService.cs
public class ProductService : IProductService
{
    private readonly IRepository<Product> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(
        IRepository<Product> repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(entities);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity != null ? _mapper.Map<ProductDto>(entity) : null;
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var entity = _mapper.Map<Product>(dto);
        entity.CreatedDate = DateTime.UtcNow;
        
        var created = await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<ProductDto>(created);
    }

    public async Task UpdateProductAsync(int id, UpdateProductDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new EntityNotFoundException(nameof(Product), id);

        _mapper.Map(dto, entity);
        entity.ModifiedDate = DateTime.UtcNow;
        
        await _repository.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int id)
    {
        var exists = await _repository.ExistsAsync(id);
        if (!exists)
            throw new EntityNotFoundException(nameof(Product), id);

        await _repository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
```

#### 4.5 Configure AutoMapper
```csharp
// Application/Common/MappingProfile.cs
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
    }
}
```

### Phase 5: Infrastructure Layer Migration (Week 3-4)

#### 5.1 Create DbContext
```csharp
// Infrastructure/Data/ApplicationDbContext.cs
public class ApplicationDbContext : IdentityDbContext
{
    private readonly ICurrentUserService? _currentUserService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService? currentUserService = null)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && 
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;
            var currentUser = _currentUserService?.UserName ?? "System";
            
            if (entry.State == EntityState.Added)
            {
                entity.CreatedDate = DateTime.UtcNow;
                entity.CreatedBy = currentUser;
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.ModifiedDate = DateTime.UtcNow;
                entity.ModifiedBy = currentUser;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
```

#### 5.2 Implement Repository Pattern
```csharp
// Infrastructure/Repositories/Repository.cs
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        return entity != null;
    }
}
```

#### 5.3 Implement Unit of Work
```csharp
// Infrastructure/Repositories/UnitOfWork.cs
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
```

#### 5.4 Configure Dependency Injection
```csharp
// Infrastructure/DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database context
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Identity
        services.AddDefaultIdentity<IdentityUser>(options => 
            options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>();

        // Repository pattern
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddTransient<IDateTime, DateTimeService>();

        return services;
    }
}
```

### Phase 6: Web Layer Migration (Week 4-5)

#### 6.1 Migrate Controllers
```csharp
// Legacy: Controllers/ProductController.cs
public class ProductController : Controller
{
    private readonly ProductService _service;

    public ActionResult Index()
    {
        var products = _service.GetAll();
        return View(products);
    }

    [HttpPost]
    public ActionResult Create(Product product)
    {
        if (ModelState.IsValid)
        {
            _service.Create(product);
            return RedirectToAction("Index");
        }
        return View(product);
    }
}

// Modern: Controllers/ProductController.cs
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(
        IProductService productService,
        ILogger<ProductController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return View("Error");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await _productService.CreateProductAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            ModelState.AddModelError("", "An error occurred while creating the product");
            return View(dto);
        }
    }
}
```

#### 6.2 Configure Program.cs
```csharp
// Program.cs
using OrbitAOS.V6.Infrastructure;
using OrbitAOS.V6.Application;
using OrbitAOS.V6.Filters;
using OrbitAOS.V6.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure layer
builder.Services.AddInfrastructure(builder.Configuration);

// Application layer
builder.Services.AddApplication();

// MVC with filters
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
    options.Filters.Add<ValidationFilter>();
})
.AddFluentValidation();

// Response compression
builder.Services.AddResponseCompression();

// Razor Pages (for Identity UI)
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRequestLogging();
app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
```

#### 6.3 Migrate Configuration
```json
// Legacy: web.config
<configuration>
  <appSettings>
    <add key="Setting1" value="Value1" />
  </appSettings>
  <connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Server=...;Database=...;" />
  </connectionStrings>
</configuration>

// Modern: appsettings.json
{
  "AppSettings": {
    "Setting1": "Value1"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Phase 7: Data Migration (Week 5)

#### 7.1 Database-First Approach (Scaffold from Existing Database)
```bash
# Install EF Core tools
dotnet tool install --global dotnet-ef

# Scaffold DbContext and entities from existing database
dotnet ef dbcontext scaffold \
  "Server=(localdb)\\mssqllocaldb;Database=LegacyDb;Trusted_Connection=True;" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --project OrbitAOS.V6.Infrastructure \
  --startup-project OrbitAOS.V6 \
  --context-dir Data \
  --output-dir Data/Entities \
  --context ApplicationDbContext \
  --force \
  --data-annotations \
  --use-database-names \
  --no-onconfiguring
```

#### 7.2 Code-First Approach (Create New Database)
```bash
# Create initial migration
dotnet ef migrations add InitialCreate \
  --project OrbitAOS.V6.Infrastructure \
  --startup-project OrbitAOS.V6 \
  --output-dir Data/Migrations

# Update database
dotnet ef database update \
  --project OrbitAOS.V6.Infrastructure \
  --startup-project OrbitAOS.V6

# Add new migration after model changes
dotnet ef migrations add AddProductTable \
  --project OrbitAOS.V6.Infrastructure \
  --startup-project OrbitAOS.V6

# Remove last migration (if not applied)
dotnet ef migrations remove \
  --project OrbitAOS.V6.Infrastructure \
  --startup-project OrbitAOS.V6

# Generate SQL script
dotnet ef migrations script \
  --project OrbitAOS.V6.Infrastructure \
  --startup-project OrbitAOS.V6 \
  --output migration.sql
```

### Phase 8: Authentication Migration (Week 6)

#### 8.1 Forms Authentication → Cookie Authentication
```csharp
// Legacy: web.config
<authentication mode="Forms">
  <forms loginUrl="~/Account/Login" timeout="2880" />
</authentication>

// Modern: Program.cs
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(2880);
        options.SlidingExpiration = true;
    });
```

#### 8.2 ASP.NET Identity 2.x → Core Identity
```csharp
// Legacy: ApplicationUser
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

// Modern: Same structure, different base class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

// Configure Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();
```

### Phase 9: Testing (Week 6-7)

#### 9.1 Unit Tests
```csharp
// Tests/Services/ProductServiceTests.cs
public class ProductServiceTests
{
    private readonly Mock<IRepository<Product>> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockRepository = new Mock<IRepository<Product>>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _productService = new ProductService(
            _mockRepository.Object,
            _mockUnitOfWork.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnAllProducts()
    {
        // Arrange
        var entities = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", Price = 10.00m },
            new Product { Id = 2, Name = "Product 2", Price = 20.00m }
        };

        var dtos = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Product 1", Price = 10.00m },
            new ProductDto { Id = 2, Name = "Product 2", Price = 20.00m }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
        _mockMapper.Setup(m => m.Map<IEnumerable<ProductDto>>(entities)).Returns(dtos);

        // Act
        var result = await _productService.GetAllProductsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(dtos);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }
}
```

#### 9.2 Integration Tests
```csharp
// Tests/Controllers/ProductControllerTests.cs
public class ProductControllerTests
{
    private readonly Mock<IProductService> _mockService;
    private readonly Mock<ILogger<ProductController>> _mockLogger;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _mockService = new Mock<IProductService>();
        _mockLogger = new Mock<ILogger<ProductController>>();
        _controller = new ProductController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Index_ShouldReturnViewWithProducts()
    {
        // Arrange
        var products = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Product 1", Price = 10.00m }
        };
        _mockService.Setup(s => s.GetAllProductsAsync()).ReturnsAsync(products);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeAssignableTo<IEnumerable<ProductDto>>().Subject;
        model.Should().HaveCount(1);
    }
}
```

### Phase 10: Deployment (Week 7-8)

#### 10.1 Build and Publish
```bash
# Build solution
dotnet build --configuration Release

# Run tests
dotnet test

# Publish application
dotnet publish OrbitAOS.V6 \
  --configuration Release \
  --output ./publish \
  --runtime win-x64 \
  --self-contained false

# Create deployment package
cd publish
zip -r ../OrbitAOS.V6.zip *
```

#### 10.2 Database Deployment
```bash
# Generate SQL script for production
dotnet ef migrations script \
  --project OrbitAOS.V6.Infrastructure \
  --startup-project OrbitAOS.V6 \
  --idempotent \
  --output production-migration.sql

# Apply to production database
sqlcmd -S ProductionServer -d ProductionDb -i production-migration.sql
```

## Transformation Rules

### Rule 1: Global.asax → Program.cs
**Legacy:**
```csharp
// Global.asax.cs
public class MvcApplication : System.Web.HttpApplication
{
    protected void Application_Start()
    {
        AreaRegistration.RegisterAllAreas();
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);
    }
}
```

**Modern:**
```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
var app = builder.Build();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
```

### Rule 2: web.config → appsettings.json
**Legacy:**
```xml
<appSettings>
  <add key="ApiKey" value="12345" />
</appSettings>
```

**Modern:**
```json
{
  "AppSettings": {
    "ApiKey": "12345"
  }
}
```

**Access:**
```csharp
// Legacy
string apiKey = ConfigurationManager.AppSettings["ApiKey"];

// Modern
string apiKey = _configuration["AppSettings:ApiKey"];
// Or with IOptions pattern
public class AppSettings
{
    public string ApiKey { get; set; }
}
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
```

### Rule 3: ActionResult → IActionResult
**Legacy:**
```csharp
public ActionResult Index()
{
    return View();
}
```

**Modern:**
```csharp
public IActionResult Index()
{
    return View();
}
```

### Rule 4: Async Actions
**Legacy:**
```csharp
public ActionResult GetProducts()
{
    var products = _service.GetAll();
    return View(products);
}
```

**Modern:**
```csharp
public async Task<IActionResult> GetProducts()
{
    var products = await _service.GetAllAsync();
    return View(products);
}
```

### Rule 5: HttpContext Access
**Legacy:**
```csharp
var userId = HttpContext.Current.User.Identity.Name;
```

**Modern:**
```csharp
// Inject IHttpContextAccessor
private readonly IHttpContextAccessor _httpContextAccessor;

public MyService(IHttpContextAccessor httpContextAccessor)
{
    _httpContextAccessor = httpContextAccessor;
}

var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
```

### Rule 6: Session State
**Legacy:**
```csharp
Session["Key"] = value;
var value = Session["Key"];
```

**Modern:**
```csharp
// Configure in Program.cs
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
app.UseSession();

// Usage
HttpContext.Session.SetString("Key", value);
var value = HttpContext.Session.GetString("Key");
```

### Rule 7: OutputCache → ResponseCache
**Legacy:**
```csharp
[OutputCache(Duration = 60)]
public ActionResult Index()
{
    return View();
}
```

**Modern:**
```csharp
[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
public IActionResult Index()
{
    return View();
}
```

### Rule 8: Child Actions → View Components
**Legacy:**
```csharp
[ChildActionOnly]
public ActionResult Menu()
{
    return PartialView();
}

// In view
@Html.Action("Menu")
```

**Modern:**
```csharp
// ViewComponents/MenuViewComponent.cs
public class MenuViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}

// In view
@await Component.InvokeAsync("Menu")
```

### Rule 9: Custom Filters
**Legacy:**
```csharp
public class CustomAuthFilter : AuthorizeAttribute
{
    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
        return base.AuthorizeCore(httpContext);
    }
}
```

**Modern:**
```csharp
public class CustomAuthFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Authorization logic
    }
}
```

### Rule 10: Bundling and Minification
**Legacy:**
```csharp
// App_Start/BundleConfig.cs
bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
    "~/Scripts/jquery-{version}.js"));
```

**Modern:**
```bash
# Use LibMan or npm
libman install jquery@3.6.0 --provider cdnjs --destination wwwroot/lib/jquery

# Or WebOptimizer
builder.Services.AddWebOptimizer(pipeline =>
{
    pipeline.AddJavaScriptBundle("/js/bundle.js", "js/site.js", "js/app.js");
});
```

## Common Blockers and Solutions

### Blocker 1: System.Web Dependencies
**Issue**: System.Web namespace not available in .NET Core

**Solution**:
- `HttpContext.Current` → Inject `IHttpContextAccessor`
- `HttpUtility.HtmlEncode` → `System.Net.WebUtility.HtmlEncode`
- `Server.MapPath` → `IWebHostEnvironment.ContentRootPath`

### Blocker 2: Entity Framework 6 → EF Core
**Issue**: EF6 APIs not compatible with EF Core

**Solution**:
- Replace `DbContext` with EF Core version
- Update LINQ queries (some methods changed)
- Rewrite migrations using EF Core tools

### Blocker 3: ASP.NET Identity Schema
**Issue**: Identity tables have different schema

**Solution**:
- Use compatibility mode or migrate schema
- Update user/role queries
- Test authentication thoroughly

### Blocker 4: Missing NuGet Packages
**Issue**: Some packages don't support .NET Core

**Solution**:
- Find .NET Core alternatives
- Rewrite functionality if no alternative exists
- Use .NET Standard libraries when possible

### Blocker 5: Synchronous I/O
**Issue**: Synchronous I/O operations may be disabled

**Solution**:
- Convert all I/O to async/await
- Enable synchronous I/O if absolutely necessary (not recommended)

## Testing Strategy

### Unit Testing
- Test services with mocked dependencies
- Test controllers with mocked services
- Test validators independently
- Aim for 80%+ code coverage

### Integration Testing
- Test API endpoints end-to-end
- Test database operations with in-memory database
- Test authentication flows

### Performance Testing
- Load testing with realistic data
- Compare with legacy application
- Identify bottlenecks

## Deployment

### IIS Deployment
1. Install .NET 8 Hosting Bundle
2. Create application pool (.NET CLR Version: No Managed Code)
3. Deploy published files
4. Configure connection strings
5. Test application

### Azure App Service
```bash
# Deploy to Azure
az webapp up \
  --name OrbitAOS \
  --resource-group MyResourceGroup \
  --runtime "DOTNET|8.0"
```

### Docker Deployment
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OrbitAOS.V6/OrbitAOS.V6.csproj", "OrbitAOS.V6/"]
RUN dotnet restore "OrbitAOS.V6/OrbitAOS.V6.csproj"
COPY . .
WORKDIR "/src/OrbitAOS.V6"
RUN dotnet build "OrbitAOS.V6.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrbitAOS.V6.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrbitAOS.V6.dll"]
```

## Success Criteria

- ✅ All features migrated and working
- ✅ All tests passing
- ✅ Performance comparable or better
- ✅ Security audit passed
- ✅ Documentation complete
- ✅ Team trained on new stack

## Conclusion

This migration guide provides a comprehensive roadmap for transforming legacy ASP.NET MVC applications to modern .NET 8 Core MVC with Clean Architecture. Follow the phases systematically, test thoroughly, and maintain clear communication with stakeholders throughout the process.

For questions or issues, refer to:
- [Official .NET Documentation](https://docs.microsoft.com/dotnet/)
- [ASP.NET Core Migration Guide](https://docs.microsoft.com/aspnet/core/migration/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core/)
