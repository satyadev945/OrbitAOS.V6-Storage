// ============================================================
// Application/DTOs/HomeIndexDto.cs
// Clean Architecture – Application Layer
//
// Data Transfer Object returned by the Application layer to
// the Web (presentation) layer.  Controllers map DTOs to
// ViewModels; they never expose domain entities directly.
// ============================================================

namespace OrbitAOS.V6.Application.DTOs;

/// <summary>
/// DTO carrying data needed to render the Home/Index view.
/// </summary>
public sealed record HomeIndexDto(string WelcomeMessage);
