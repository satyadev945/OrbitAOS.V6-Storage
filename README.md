# OrbitAOS - ASP.NET MVC to .NET 8 Migration

## Overview

This project has been migrated from **ASP.NET MVC on .NET 6** to **ASP.NET Core MVC on .NET 8** using **Clean Architecture** with a layered structure.

## Architecture

```
OrbitAOS.V6.sln
├── src/
│   ├── OrbitAOS.Domain          # Domain entities, interfaces, business rules
│   ├── OrbitAOS.Application     # Business logic, DTOs, service interfaces
│   ├── OrbitAOS.Infrastructure  # EF Core 8, Identity, repository implementations
│   └── OrbitAOS.Web             # ASP.NET Core MVC .NET 8 web application
└── tests/
    └── OrbitAOS.Tests           # xUnit unit tests (Domain, Application, Infrastructure, Web)
```

## Layer Responsibilities

### OrbitAOS.Domain
- `BaseEntity` - Common audit fields (Id, CreatedAt, UpdatedAt)
- `UserProfile` - Domain entity
- `IRepository<T>` - Generic repository interface
- `IUserProfileRepository` - Specific repository interface
- **No external dependencies**

### OrbitAOS.Application
- `UserProfileDto` - Data Transfer Object
- `IUserProfileService` - Service interface
- `UserProfileService` - Business logic implementation
- `DependencyInjection` - Service registration extension
- **Depends on**: Domain

### OrbitAOS.Infrastructure
- `ApplicationDbContext` - EF Core 8 + ASP.NET Core Identity
- `Repository<T>` - Generic EF Core 8 repository
- `UserProfileRepository` - Specific repository implementation
- `DependencyInjection` - Infrastructure service registration
- **Depends on**: Domain, Application
- **Packages**: EF Core 8.0.12, Identity.EntityFrameworkCore 8.0.12

### OrbitAOS.Web
- `HomeController` - Landing page, privacy, error handling
- `UserProfileController` - CRUD operations for user profiles
- Razor Views with Tag Helpers
- ASP.NET Core Identity UI (scaffolded)
- **Depends on**: Application, Infrastructure
- **Target**: net8.0

## Migration Changes

### From net6.0 to net8.0
| Legacy (net6.0) | Modern (.NET 8) |
|---|---|
| `net6.0` target framework | `net8.0` target framework |
| EF Core 6.0.19 | EF Core 8.0.12 |
| Identity 6.0.19 | Identity 8.0.12 |
| Single-project structure | Clean Architecture (4 layers) |
| No repository pattern | Generic + specific repositories |
| No service layer | Application service layer |
| No domain layer | Domain entities + interfaces |

### Key Migration Patterns Applied

1. **Target Framework**: `net6.0` → `net8.0`
2. **Package Versions**: All packages updated to 8.x
3. **EF Core Migrations**: Regenerated with EF Core 8 syntax (no deprecated `nullable: false`)
4. **Repository Pattern**: Introduced generic `IRepository<T>` and specific repositories
5. **Service Layer**: `IUserProfileService` / `UserProfileService` for business logic
6. **Dependency Injection**: Layer-specific `DependencyInjection` extension classes
7. **FindAsync**: Updated to use collection expression `[id]` syntax (EF Core 8)
8. **MigrationsAssembly**: Uses `Assembly.GetName().Name` (null-safe)

## Building the Solution

```bash
# Restore packages
dotnet restore OrbitAOS.V6.sln

# Build all projects
dotnet build OrbitAOS.V6.sln

# Run unit tests
dotnet test OrbitAOS.V6.sln
```

## Database Setup

```bash
# Apply migrations (from solution root)
dotnet ef database update --project src/OrbitAOS.Infrastructure --startup-project src/OrbitAOS.Web

# Add new migration
dotnet ef migrations add <MigrationName> --project src/OrbitAOS.Infrastructure --startup-project src/OrbitAOS.Web

# Scaffold from existing database (Database-First)
dotnet ef dbcontext scaffold "Server=...;Database=...;" Microsoft.EntityFrameworkCore.SqlServer \
  --project src/OrbitAOS.Infrastructure \
  --startup-project src/OrbitAOS.Web \
  --output-dir Data/Entities \
  --context ApplicationDbContext \
  --force
```

## Unit Tests

The test project (`OrbitAOS.Tests`) covers:

| Test Class | Layer | Coverage |
|---|---|---|
| `DomainEntityTests` | Domain | Entity defaults, inheritance, property assignment |
| `UserProfileServiceTests` | Application | CRUD operations, mapping, logging |
| `RepositoryTests` | Infrastructure | Generic repository CRUD with in-memory DB |
| `UserProfileRepositoryTests` | Infrastructure | Specific repository queries |
| `HomeControllerTests` | Web | Controller actions, view results, error model |
| `UserProfileControllerTests` | Web | Full CRUD controller actions, redirects, not-found |

## Authentication

Uses ASP.NET Core Identity with:
- `IdentityUser` (default)
- Email confirmation required
- Scaffolded Identity UI pages (`/Areas/Identity/Pages/`)
- Cookie authentication middleware

## Configuration

`appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OrbitAOS-net8;..."
  }
}
```
