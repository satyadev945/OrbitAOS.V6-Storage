// ============================================================
// Application/Interfaces/IRepository.cs
// Re-export of Domain repository interface for Application layer use
// Clean Architecture: Application layer depends on Domain abstractions
// ============================================================

// This file intentionally re-exports the Domain interface so Application
// services can reference it without a direct Domain project dependency
// in a single-project Clean Architecture layout.

global using IRepository<T> = OrbitAOS.V6.Domain.Interfaces.IRepository<T>;
