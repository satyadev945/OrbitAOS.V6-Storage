using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitAOS.Application.DTOs;
using OrbitAOS.Application.Interfaces;

namespace OrbitAOS.Web.Controllers;

/// <summary>
/// Controller for managing user profiles.
/// Demonstrates Clean Architecture wiring: Web → Application → Domain/Infrastructure.
/// Migrated from legacy ASP.NET MVC to ASP.NET Core MVC on .NET 8.
/// </summary>
[Authorize]
public class UserProfileController : Controller
{
    private readonly IUserProfileService _userProfileService;
    private readonly ILogger<UserProfileController> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="UserProfileController"/>.
    /// </summary>
    /// <param name="userProfileService">The user profile service for business operations.</param>
    /// <param name="logger">The logger instance for diagnostic output.</param>
    public UserProfileController(
        IUserProfileService userProfileService,
        ILogger<UserProfileController> logger)
    {
        _userProfileService = userProfileService;
        _logger = logger;
    }

    /// <summary>Lists all user profiles.</summary>
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Loading user profiles list");
        var profiles = await _userProfileService.GetAllAsync(cancellationToken);
        return View(profiles);
    }

    /// <summary>Shows details for a specific user profile.</summary>
    /// <param name="id">The unique identifier of the user profile.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Loading details for user profile {Id}", id);
        var profile = await _userProfileService.GetByIdAsync(id, cancellationToken);
        if (profile is null)
        {
            _logger.LogWarning("User profile {Id} not found", id);
            return NotFound();
        }
        return View(profile);
    }

    /// <summary>Shows the create user profile form.</summary>
    [HttpGet]
    public IActionResult Create()
    {
        return View(new UserProfileDto());
    }

    /// <summary>Handles the create user profile form submission.</summary>
    /// <param name="dto">The user profile data transfer object.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserProfileDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        await _userProfileService.CreateAsync(dto, cancellationToken);
        _logger.LogInformation("Created user profile for {IdentityUserId}", dto.IdentityUserId);
        TempData["SuccessMessage"] = "User profile created successfully.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Shows the edit user profile form.</summary>
    /// <param name="id">The unique identifier of the user profile to edit.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Loading edit form for user profile {Id}", id);
        var profile = await _userProfileService.GetByIdAsync(id, cancellationToken);
        if (profile is null)
        {
            _logger.LogWarning("User profile {Id} not found for editing", id);
            return NotFound();
        }
        return View(profile);
    }

    /// <summary>Handles the edit user profile form submission.</summary>
    /// <param name="id">The unique identifier of the user profile.</param>
    /// <param name="dto">The updated user profile data transfer object.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserProfileDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
        {
            _logger.LogWarning("ID mismatch in Edit: route id={RouteId}, dto id={DtoId}", id, dto.Id);
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        await _userProfileService.UpdateAsync(dto, cancellationToken);
        _logger.LogInformation("Updated user profile {Id}", id);
        TempData["SuccessMessage"] = "User profile updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Shows the delete confirmation page.</summary>
    /// <param name="id">The unique identifier of the user profile to delete.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Loading delete confirmation for user profile {Id}", id);
        var profile = await _userProfileService.GetByIdAsync(id, cancellationToken);
        if (profile is null)
        {
            _logger.LogWarning("User profile {Id} not found for deletion", id);
            return NotFound();
        }
        return View(profile);
    }

    /// <summary>Handles the delete confirmation.</summary>
    /// <param name="id">The unique identifier of the user profile to delete.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await _userProfileService.DeleteAsync(id, cancellationToken);
        _logger.LogInformation("Deleted user profile {Id}", id);
        TempData["SuccessMessage"] = "User profile deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
