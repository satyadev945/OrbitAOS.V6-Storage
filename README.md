# OrbitAOS.V6 - ASP.NET Core MVC on .NET 8

## Overview

OrbitAOS.V6 is a fully migrated ASP.NET Core MVC application running on **.NET 8** with **Clean Architecture**. This project was migrated from ASP.NET MVC 6.0 (net6.0) to ASP.NET Core MVC on .NET 8 (net8.0).

## Architecture

This solution follows **Clean Architecture** with the following layers:

```
OrbitAOS.V6.sln
├── OrbitAOS.V6.Domain          # Domain entities and base classes (no dependencies)
├── OrbitAOS.V6.Application     # Business logic, interfaces, DTOs, services
├── OrbitAOS.V6.Infrastructure  # Data access, EF Core, Identity, repositories
├── OrbitAOS.V6.Web             # ASP.NET Core MVC presentation layer
└── OrbitAOS.V6.Tests           # Unit and integration tests (xUnit)
```

### Layer Dependencies
```
Web → Application + Infrastructure
Infrastructure → Application + Domain
Application → Domain
Domain → (none)
Tests → Web + Application + Infrastructure
```

## Technology Stack

| Component | Technology |
|-----------|-----------|
| Framework | ASP.NET Core MVC on .NET 8 |
| ORM | Entity Framework Core 8.0 |
| Database | SQL Server (LocalDB for development) |
| Authentication | ASP.NET Core Identity 8.0 |
| Mapping | AutoMapper 13.0.1 |
| Testing | xUnit 2.6.3 + Moq 4.20.70 + FluentAssertions 6.12.0 |
| Validation | FluentValidation 11.9.0 |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server or SQL Server LocalDB
- Visual Studio 2022 (17.8+) or VS Code

## Getting Started

### 1. Clone and Restore

```bash
git clone <repository-url>
cd OrbitAOSmup
dotnet restore OrbitAOS.V6.sln
```

### 2. Configure Database

Update the connection string in `OrbitAOS.V6.Web/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OrbitAOS.V6;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 3. Apply Database Migrations

```bash
cd OrbitAOS.V6.Web
dotnet ef database update --project ../OrbitAOS.V6.Infrastructure
```

### 4. Build the Solution

```bash
dotnet build OrbitAOS.V6.sln
```

### 5. Run Tests

```bash
dotnet test OrbitAOS.V6.sln
```

### 6. Run the Application

```bash
cd OrbitAOS.V6.Web
dotnet run
```

Navigate to `https://localhost:7000` or `http://localhost:5000`.

## Project Structure

```
OrbitAOS.V6.Domain/
├── Common/
│   └── BaseEntity.cs           # Base entity with audit properties
└── Entities/
    └── SampleEntity.cs         # Sample domain entity

OrbitAOS.V6.Application/
├── Common/
│   └── MappingProfile.cs       # AutoMapper profiles
├── DTOs/
│   └── SampleDto.cs            # Data Transfer Objects
├── Interfaces/
│   ├── IRepository.cs          # Generic repository interface
│   ├── ISampleService.cs       # Sample service interface
│   └── IUnitOfWork.cs          # Unit of Work interface
└── Services/
    └── SampleService.cs        # Sample business logic

OrbitAOS.V6.Infrastructure/
├── Data/
│   └── ApplicationDbContext.cs # EF Core DbContext with Identity
├── Identity/
│   └── ApplicationUser.cs      # Custom Identity user
└── Repositories/
    ├── Repository.cs           # Generic repository implementation
    └── UnitOfWork.cs           # Unit of Work implementation

OrbitAOS.V6.Web/
├── Areas/Identity/             # ASP.NET Core Identity UI pages
├── Controllers/
│   ├── HomeController.cs       # Home page controller
│   └── SampleController.cs    # Sample CRUD controller
├── Filters/
│   ├── ActionLoggingFilter.cs  # Global action logging filter
│   └── GlobalExceptionFilter.cs # Global exception handler
├── Middleware/
│   └── RequestLoggingMiddleware.cs # HTTP request logging
├── Models/
│   └── ErrorViewModel.cs       # Error page view model
├── Views/
│   ├── Home/                   # Home views
│   ├── Sample/                 # Sample CRUD views
│   └── Shared/                 # Layout and partial views
├── wwwroot/                    # Static files (CSS, JS, images)
├── Program.cs                  # Application entry point and DI
└── appsettings.json            # Configuration

OrbitAOS.V6.Tests/
├── Integration/
│   └── RepositoryIntegrationTests.cs  # EF Core InMemory tests
└── Unit/
    ├── HomeControllerTests.cs  # HomeController unit tests
    ├── SampleControllerTests.cs # SampleController unit tests
    └── SampleServiceTests.cs   # SampleService unit tests
```

## Migration Summary

This application was migrated from:
- **Source**: ASP.NET Core MVC 6.0 (net6.0)
- **Target**: ASP.NET Core MVC on .NET 8 (net8.0)
- **Architecture**: Restructured to Clean Architecture (4 layers)

### Key Changes

| Legacy | .NET 8 Equivalent |
|--------|-------------------|
| `net6.0` target framework | `net8.0` target framework |
| All packages v6.0.x | All packages v8.0.0 |
| Single-project structure | Clean Architecture (4 layers) |
| Direct DbContext in controllers | Repository + Unit of Work pattern |
| Manual property mapping | AutoMapper profiles |
| No global filters | GlobalExceptionFilter + ActionLoggingFilter |
| No request logging | RequestLoggingMiddleware |
| No unit tests | xUnit + Moq + FluentAssertions |

## EF Core Commands

```bash
# Create a new migration
dotnet ef migrations add <MigrationName> --project OrbitAOS.V6.Infrastructure --startup-project OrbitAOS.V6.Web

# Apply migrations
dotnet ef database update --project OrbitAOS.V6.Infrastructure --startup-project OrbitAOS.V6.Web

# Remove last migration
dotnet ef migrations remove --project OrbitAOS.V6.Infrastructure --startup-project OrbitAOS.V6.Web

# Generate SQL script
dotnet ef migrations script --output migration.sql --idempotent --project OrbitAOS.V6.Infrastructure --startup-project OrbitAOS.V6.Web
```

## Testing

```bash
# Run all tests
dotnet test OrbitAOS.V6.sln

# Run with coverage
dotnet test OrbitAOS.V6.sln /p:CollectCoverage=true /p:CoverletOutputFormat=lcov

# Run specific test project
dotnet test OrbitAOS.V6.Tests/OrbitAOS.V6.Tests.csproj
```

## License

This project is for demonstration purposes as part of the OrbitAOS migration.
