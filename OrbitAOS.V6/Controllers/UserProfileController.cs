using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitAOS.V6.Models;
using OrbitAOS.V6.Models.DTOs;
using OrbitAOS.V6.Services;

namespace OrbitAOS.V6.Controllers
{
    /// <summary>
    /// Controller for managing user profiles.
    /// Demonstrates service layer integration within the MVC single-project architecture.
    /// </summary>
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly IUserProfileService _userProfileService;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(
            IUserProfileService userProfileService,
            ILogger<UserProfileController> logger)
        {
            _userProfileService = userProfileService;
            _logger = logger;
        }

        /// <summary>
        /// Lists all user profiles.
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var profiles = await _userProfileService.GetAllAsync(cancellationToken);
            return View(profiles);
        }

        /// <summary>
        /// Displays details for a specific user profile.
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var profile = await _userProfileService.GetByIdAsync(id, cancellationToken);
            if (profile is null)
            {
                return NotFound();
            }
            return View(profile);
        }

        /// <summary>
        /// Displays the create user profile form.
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Handles the create user profile form submission.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserProfileDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var created = await _userProfileService.CreateAsync(dto, cancellationToken);
                TempData["SuccessMessage"] = "User profile created successfully.";
                return RedirectToAction(nameof(Details), new { id = created.Id });
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Domain exception creating user profile");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        /// <summary>
        /// Displays the edit user profile form.
        /// </summary>
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var profile = await _userProfileService.GetByIdAsync(id, cancellationToken);
            if (profile is null)
            {
                return NotFound();
            }

            var dto = new CreateUserProfileDto
            {
                UserId = profile.UserId,
                DisplayName = profile.DisplayName,
                Bio = profile.Bio,
                AvatarUrl = profile.AvatarUrl
            };

            ViewBag.ProfileId = id;
            return View(dto);
        }

        /// <summary>
        /// Handles the edit user profile form submission.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateUserProfileDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ProfileId = id;
                return View(dto);
            }

            try
            {
                await _userProfileService.UpdateAsync(id, dto, cancellationToken);
                TempData["SuccessMessage"] = "User profile updated successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Domain exception updating user profile {Id}", id);
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.ProfileId = id;
                return View(dto);
            }
        }

        /// <summary>
        /// Displays the delete confirmation page.
        /// </summary>
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var profile = await _userProfileService.GetByIdAsync(id, cancellationToken);
            if (profile is null)
            {
                return NotFound();
            }
            return View(profile);
        }

        /// <summary>
        /// Handles the delete confirmation form submission.
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _userProfileService.DeleteAsync(id, cancellationToken);
                TempData["SuccessMessage"] = "User profile deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Domain exception deleting user profile {Id}", id);
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
