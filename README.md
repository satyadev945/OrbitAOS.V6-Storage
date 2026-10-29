# OrbitAOS.V6 - Clean Architecture ASP.NET Core MVC Application

## Overview

This project demonstrates a complete ASP.NET Core MVC application built with Clean Architecture principles, migrated from legacy ASP.NET MVC to .NET 8.

## Architecture

The solution follows Clean Architecture with clear separation of concerns across four layers:

### 1. Domain Layer (`OrbitAOS.V6.Domain`)
**Purpose**: Contains enterprise business rules and domain entities.

**Key Components**:
- **Entities**: Core business objects (`SampleEntity`)
- **Common**: Base classes and interfaces
  - `BaseEntity`: Base class for all entities with audit fields
  - `IAuditableEntity`: Interface for auditable entities
  - `ISoftDeletable`: Interface for soft-deletable entities
- **Exceptions**: Domain-specific exceptions
  - `DomainException`: Base exception class
  - `EntityNotFoundException`: Thrown when entity not found
  - `ValidationException`: Thrown for validation errors

**Dependencies**: None (pure .NET)

### 2. Application Layer (`OrbitAOS.V6.Application`)
**Purpose**: Contains application business logic, DTOs, and interfaces.

**Key Components**:
- **DTOs**: Data Transfer Objects for API contracts
  - `SampleDto`, `CreateSampleDto`, `UpdateSampleDto`
- **Interfaces**: Abstractions for repositories and services
  - `IRepository<T>`: Generic repository interface
  - `IUnitOfWork`: Unit of Work pattern
  - `ISampleService`: Sample service interface
- **Services**: Business logic implementations
  - `SampleService`: Sample business logic
- **Features**: CQRS pattern with MediatR
  - **Queries**: `GetAllSamplesQuery`, `GetSampleByIdQuery`
  - **Commands**: `CreateSampleCommand`, `UpdateSampleCommand`, `DeleteSampleCommand`
- **Validators**: FluentValidation validators
  - `CreateSampleDtoValidator`, `UpdateSampleDtoValidator`
- **Common**:
  - `MappingProfile`: AutoMapper configuration
  - `Result<T>`: Generic result wrapper
  - `PaginationParams`, `PagedResult<T>`: Pagination support

**Dependencies**: 
- OrbitAOS.V6.Domain
- AutoMapper
- FluentValidation
- MediatR

### 3. Infrastructure Layer (`OrbitAOS.V6.Infrastructure`)
**Purpose**: Contains data access, external services, and infrastructure concerns.

**Key Components**:
- **Data**: Database context and migrations
  - `ApplicationDbContext`: EF Core DbContext with Identity
- **Repositories**: Data access implementations
  - `Repository<T>`: Generic repository implementation
  - `EnhancedRepository<T>`: Advanced querying capabilities
  - `UnitOfWork`: Unit of Work implementation
- **Services**:
  - `CurrentUserService`: Access current user information
  - `DateTimeService`: Testable DateTime abstraction
- **Identity**: ASP.NET Core Identity configuration

**Dependencies**:
- OrbitAOS.V6.Domain
- OrbitAOS.V6.Application
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.AspNetCore.Identity.EntityFrameworkCore

### 4. Web/Presentation Layer (`OrbitAOS.V6`)
**Purpose**: ASP.NET Core MVC application with controllers and views.

**Key Components**:
- **Controllers**: MVC controllers
  - `HomeController`: Home page controller
  - `SampleController`: Sample CRUD operations
- **Views**: Razor views with Bootstrap 5
- **Models**: View models
  - `ErrorViewModel`
- **Filters**: Global filters
  - `GlobalExceptionFilter`: Centralized exception handling
  - `ValidationFilter`: FluentValidation integration
- **Middleware**: Custom middleware
  - `RequestLoggingMiddleware`: Request/response logging
- **wwwroot**: Static files (CSS, JS, images)

**Dependencies**:
- OrbitAOS.V6.Application
- OrbitAOS.V6.Infrastructure
- Microsoft.AspNetCore.Identity.UI
- FluentValidation.AspNetCore
- MediatR

### 5. Test Layer (`OrbitAOS.V6.Tests`)
**Purpose**: Unit and integration tests.

**Key Components**:
- **Controllers**: Controller tests
  - `SampleControllerTests`
- **Services**: Service tests
  - `SampleServiceTests`
- **Repositories**: Repository tests
  - `RepositoryTests`

**Testing Stack**:
- xUnit
- Moq
- FluentAssertions
- Microsoft.EntityFrameworkCore.InMemory

## Design Patterns Implemented

### 1. Repository Pattern
Generic repository for data access abstraction:
```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

### 2. Unit of Work Pattern
Manages transactions across multiple repositories:
```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### 3. CQRS Pattern (Optional)
Separation of read and write operations using MediatR:
- **Queries**: Read operations (GetAllSamplesQuery, GetSampleByIdQuery)
- **Commands**: Write operations (CreateSampleCommand, UpdateSampleCommand, DeleteSampleCommand)

### 4. Dependency Injection
All layers use constructor injection for loose coupling.

### 5. Result Pattern
Standardized response wrapper for operations:
```csharp
public class Result<T>
{
    public bool Succeeded { get; set; }
    public T? Data { get; set; }
    public string[] Errors { get; set; }
}
```

## Key Features

### 1. Entity Framework Core 8.0
- Code-First approach
- Migrations support
- SQL Server provider
- Identity integration

### 2. ASP.NET Core Identity
- User authentication and authorization
- Role-based access control
- Identity UI scaffolding

### 3. AutoMapper
- Automatic mapping between entities and DTOs
- Profile-based configuration

### 4. FluentValidation
- Declarative validation rules
- Automatic validation in MVC pipeline

### 5. MediatR
- CQRS pattern implementation
- Decoupled request/response handling

### 6. Global Exception Handling
- Centralized exception filter
- Consistent error responses
- Environment-specific error details

### 7. Audit Trail
- Automatic tracking of Created/Modified dates and users
- Soft delete support

### 8. Request Logging
- Custom middleware for request/response logging
- Performance monitoring

## Configuration

### Database Connection
Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OrbitAOS;Trusted_Connection=True;"
  }
}
```

### Apply Migrations
```bash
dotnet ef database update --project OrbitAOS.V6.Infrastructure --startup-project OrbitAOS.V6
```

### Create New Migration
```bash
dotnet ef migrations add MigrationName --project OrbitAOS.V6.Infrastructure --startup-project OrbitAOS.V6
```

## Building and Running

### Prerequisites
- .NET 8 SDK
- SQL Server or SQL Server LocalDB
- Visual Studio 2022 or VS Code

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run --project OrbitAOS.V6
```

### Run Tests
```bash
dotnet test
```

## Project Structure

```
OrbitAOS.V6/
├── OrbitAOS.V6.Domain/
│   ├── Common/
│   │   ├── BaseEntity.cs
│   │   ├── IAuditableEntity.cs
│   │   └── ISoftDeletable.cs
│   ├── Entities/
│   │   └── SampleEntity.cs
│   └── Exceptions/
│       ├── DomainException.cs
│       ├── EntityNotFoundException.cs
│       └── ValidationException.cs
├── OrbitAOS.V6.Application/
│   ├── Common/
│   │   ├── MappingProfile.cs
│   │   └── Models/
│   │       ├── Result.cs
│   │       └── PaginationModels.cs
│   ├── DTOs/
│   │   └── SampleDto.cs
│   ├── Features/
│   │   └── Samples/
│   │       ├── Commands/
│   │       └── Queries/
│   ├── Interfaces/
│   │   ├── IRepository.cs
│   │   ├── IUnitOfWork.cs
│   │   └── ISampleService.cs
│   ├── Services/
│   │   └── SampleService.cs
│   ├── Validators/
│   │   └── SampleDtoValidators.cs
│   └── DependencyInjection.cs
├── OrbitAOS.V6.Infrastructure/
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   └── Migrations/
│   ├── Repositories/
│   │   ├── Repository.cs
│   │   ├── EnhancedRepository.cs
│   │   └── UnitOfWork.cs
│   ├── Services/
│   │   ├── CurrentUserService.cs
│   │   └── DateTimeService.cs
│   └── DependencyInjection.cs
├── OrbitAOS.V6/
│   ├── Controllers/
│   │   ├── HomeController.cs
│   │   └── SampleController.cs
│   ├── Views/
│   ├── Filters/
│   │   ├── GlobalExceptionFilter.cs
│   │   └── ValidationFilter.cs
│   ├── Middleware/
│   │   └── RequestLoggingMiddleware.cs
│   ├── wwwroot/
│   ├── Program.cs
│   └── appsettings.json
└── OrbitAOS.V6.Tests/
    ├── Controllers/
    ├── Services/
    └── Repositories/
```

## Migration from Legacy ASP.NET MVC

This project was migrated from ASP.NET MVC 5 to .NET 8 Core MVC with the following transformations:

### Key Changes
1. **Global.asax → Program.cs**: Application startup and configuration
2. **web.config → appsettings.json**: Configuration management
3. **App_Start → Program.cs**: Routing, filters, and bundling configuration
4. **System.Web → ASP.NET Core**: Modern HTTP abstractions
5. **Entity Framework 6 → EF Core 8**: Modern ORM
6. **ASP.NET Identity 2.x → Core Identity**: Modern authentication

### Benefits
- ✅ Cross-platform support (Windows, Linux, macOS)
- ✅ Better performance and scalability
- ✅ Modern development experience
- ✅ Built-in dependency injection
- ✅ Improved testability
- ✅ Cloud-ready architecture

## Best Practices

### 1. Dependency Direction
- Domain has no dependencies
- Application depends only on Domain
- Infrastructure depends on Application and Domain
- Web depends on Application and Infrastructure

### 2. Separation of Concerns
- Each layer has a single responsibility
- Business logic in Application layer
- Data access in Infrastructure layer
- UI concerns in Web layer

### 3. Testability
- Interfaces for all dependencies
- Repository pattern for data access
- Service layer for business logic
- Mock-friendly design

### 4. Security
- ASP.NET Core Identity for authentication
- Anti-forgery tokens on forms
- HTTPS enforcement
- Secure configuration management

### 5. Performance
- Async/await throughout
- Response compression
- Efficient database queries
- Caching strategies (can be added)

## Adding New Features

### 1. Add New Entity
1. Create entity in `Domain/Entities`
2. Add DbSet to `ApplicationDbContext`
3. Create migration
4. Create DTOs in `Application/DTOs`
5. Create service interface and implementation
6. Create controller and views

### 2. Add New Business Logic
1. Create service interface in `Application/Interfaces`
2. Implement service in `Application/Services`
3. Register in `Application/DependencyInjection.cs`
4. Inject into controller

### 3. Add New Validation
1. Create validator in `Application/Validators`
2. Inherit from `AbstractValidator<T>`
3. Define validation rules
4. Automatic validation in MVC pipeline

## Troubleshooting

### Database Connection Issues
- Verify connection string in `appsettings.json`
- Ensure SQL Server is running
- Check firewall settings

### Migration Issues
- Delete `Migrations` folder and recreate
- Ensure correct startup project
- Check DbContext configuration

### Build Errors
- Clean solution: `dotnet clean`
- Restore packages: `dotnet restore`
- Rebuild: `dotnet build`

## Contributing

1. Follow Clean Architecture principles
2. Write unit tests for new features
3. Use async/await for I/O operations
4. Follow C# coding conventions
5. Document public APIs

## License

This project is licensed under the MIT License.

## Support

For issues and questions, please create an issue in the repository.
