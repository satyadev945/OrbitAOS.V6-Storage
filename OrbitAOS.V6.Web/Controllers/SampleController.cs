using Microsoft.AspNetCore.Mvc;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Application.Interfaces;

namespace OrbitAOS.V6.Web.Controllers;

/// <summary>
/// Sample CRUD controller demonstrating Clean Architecture integration.
/// Uses ISampleService (Application layer) via constructor injection.
/// All actions are async following .NET 8 best practices.
/// Replaces legacy synchronous controller actions from ASP.NET MVC 5.
/// </summary>
public class SampleController : Controller
{
    private readonly ISampleService _sampleService;
    private readonly ILogger<SampleController> _logger;

    public SampleController(ISampleService sampleService, ILogger<SampleController> logger)
    {
        _sampleService = sampleService ?? throw new ArgumentNullException(nameof(sampleService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // GET: /Sample
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Fetching all samples.");
        var samples = await _sampleService.GetAllSamplesAsync();
        return View(samples);
    }

    // GET: /Sample/Details/5
    public async Task<IActionResult> Details(int id)
    {
        _logger.LogInformation("Fetching sample with ID {Id}.", id);
        var sample = await _sampleService.GetSampleByIdAsync(id);
        if (sample == null)
        {
            _logger.LogWarning("Sample with ID {Id} not found.", id);
            return NotFound();
        }
        return View(sample);
    }

    // GET: /Sample/Create
    public IActionResult Create()
    {
        return View(new SampleDto());
    }

    // POST: /Sample/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description,IsActive")] SampleDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            var created = await _sampleService.CreateSampleAsync(dto);
            _logger.LogInformation("Created sample with ID {Id}.", created.Id);
            TempData["SuccessMessage"] = "Sample created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sample.");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the sample.");
            return View(dto);
        }
    }

    // GET: /Sample/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var sample = await _sampleService.GetSampleByIdAsync(id);
        if (sample == null)
        {
            return NotFound();
        }
        return View(sample);
    }

    // POST: /Sample/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,IsActive")] SampleDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await _sampleService.UpdateSampleAsync(id, dto);
            _logger.LogInformation("Updated sample with ID {Id}.", id);
            TempData["SuccessMessage"] = "Sample updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sample with ID {Id}.", id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the sample.");
            return View(dto);
        }
    }

    // GET: /Sample/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var sample = await _sampleService.GetSampleByIdAsync(id);
        if (sample == null)
        {
            return NotFound();
        }
        return View(sample);
    }

    // POST: /Sample/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _sampleService.DeleteSampleAsync(id);
            _logger.LogInformation("Deleted sample with ID {Id}.", id);
            TempData["SuccessMessage"] = "Sample deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting sample with ID {Id}.", id);
            TempData["ErrorMessage"] = "An error occurred while deleting the sample.";
            return RedirectToAction(nameof(Index));
        }
    }
}
