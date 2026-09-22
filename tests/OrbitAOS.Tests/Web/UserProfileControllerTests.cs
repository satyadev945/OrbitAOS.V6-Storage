using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.Application.DTOs;
using OrbitAOS.Application.Interfaces;
using OrbitAOS.Web.Controllers;

namespace OrbitAOS.Tests.Web;

/// <summary>
/// Unit tests for <see cref="UserProfileController"/>.
/// Tests the ASP.NET Core MVC controller actions migrated from legacy ASP.NET MVC.
/// Uses Moq to mock the Application layer service (Clean Architecture boundary).
/// </summary>
public class UserProfileControllerTests
{
    private readonly Mock<IUserProfileService> _serviceMock;
    private readonly Mock<ILogger<UserProfileController>> _loggerMock;
    private readonly UserProfileController _controller;

    public UserProfileControllerTests()
    {
        _serviceMock = new Mock<IUserProfileService>();
        _loggerMock = new Mock<ILogger<UserProfileController>>();
        _controller = new UserProfileController(_serviceMock.Object, _loggerMock.Object);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ── Index ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Index_ShouldReturnViewWithProfiles()
    {
        // Arrange
        var profiles = new List<UserProfileDto>
        {
            new() { Id = 1, DisplayName = "Alice", Email = "alice@test.com", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, DisplayName = "Bob", Email = "bob@test.com", IsActive = true, CreatedAt = DateTime.UtcNow }
        }.AsReadOnly();
        _serviceMock.Setup(s => s.GetAllAsync(default)).ReturnsAsync(profiles);

        // Act
        var result = await _controller.Index(default);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IReadOnlyList<UserProfileDto>>(viewResult.Model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
    public async Task Index_ShouldReturnEmptyList_WhenNoProfiles()
    {
        // Arrange
        var emptyList = new List<UserProfileDto>().AsReadOnly();
        _serviceMock.Setup(s => s.GetAllAsync(default)).ReturnsAsync(emptyList);

        // Act
        var result = await _controller.Index(default);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IReadOnlyList<UserProfileDto>>(viewResult.Model);
        Assert.Empty(model);
    }

    // ── Details ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Details_ShouldReturnView_WhenProfileExists()
    {
        // Arrange
        var profile = new UserProfileDto { Id = 1, DisplayName = "Alice", Email = "alice@test.com", CreatedAt = DateTime.UtcNow };
        _serviceMock.Setup(s => s.GetByIdAsync(1, default)).ReturnsAsync(profile);

        // Act
        var result = await _controller.Details(1, default);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(profile, viewResult.Model);
    }

    [Fact]
    public async Task Details_ShouldReturnNotFound_WhenProfileNotExists()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetByIdAsync(99, default)).ReturnsAsync((UserProfileDto?)null);

        // Act
        var result = await _controller.Details(99, default);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public void Create_Get_ShouldReturnView()
    {
        // Act
        var result = _controller.Create();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<UserProfileDto>(viewResult.Model);
    }

    [Fact]
    public async Task Create_Post_ShouldRedirectToIndex_WhenModelIsValid()
    {
        // Arrange
        var dto = new UserProfileDto { IdentityUserId = "id-1", DisplayName = "New User", Email = "new@test.com" };
        _serviceMock.Setup(s => s.CreateAsync(dto, default)).ReturnsAsync(dto);

        // Act
        var result = await _controller.Create(dto, default);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task Create_Post_ShouldReturnView_WhenModelIsInvalid()
    {
        // Arrange
        var dto = new UserProfileDto();
        _controller.ModelState.AddModelError("DisplayName", "Required");

        // Act
        var result = await _controller.Create(dto, default);

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_Post_ShouldCallServiceCreate_WhenModelIsValid()
    {
        // Arrange
        var dto = new UserProfileDto { IdentityUserId = "id-1", DisplayName = "New User", Email = "new@test.com" };
        _serviceMock.Setup(s => s.CreateAsync(dto, default)).ReturnsAsync(dto);

        // Act
        await _controller.Create(dto, default);

        // Assert
        _serviceMock.Verify(s => s.CreateAsync(dto, default), Times.Once);
    }

    // ── Edit ──────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Edit_Get_ShouldReturnView_WhenProfileExists()
    {
        // Arrange
        var profile = new UserProfileDto { Id = 1, DisplayName = "Alice", Email = "alice@test.com", CreatedAt = DateTime.UtcNow };
        _serviceMock.Setup(s => s.GetByIdAsync(1, default)).ReturnsAsync(profile);

        // Act
        var result = await _controller.Edit(1, default);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(profile, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Get_ShouldReturnNotFound_WhenProfileNotExists()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetByIdAsync(99, default)).ReturnsAsync((UserProfileDto?)null);

        // Act
        var result = await _controller.Edit(99, default);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ShouldRedirectToIndex_WhenModelIsValid()
    {
        // Arrange
        var dto = new UserProfileDto { Id = 1, DisplayName = "Updated", Email = "updated@test.com" };
        _serviceMock.Setup(s => s.UpdateAsync(dto, default)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Edit(1, dto, default);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task Edit_Post_ShouldReturnBadRequest_WhenIdMismatch()
    {
        // Arrange
        var dto = new UserProfileDto { Id = 2, DisplayName = "Updated", Email = "updated@test.com" };

        // Act
        var result = await _controller.Edit(1, dto, default);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ShouldReturnView_WhenModelIsInvalid()
    {
        // Arrange
        var dto = new UserProfileDto { Id = 1 };
        _controller.ModelState.AddModelError("DisplayName", "Required");

        // Act
        var result = await _controller.Edit(1, dto, default);

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    // ── Delete ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_Get_ShouldReturnView_WhenProfileExists()
    {
        // Arrange
        var profile = new UserProfileDto { Id = 1, DisplayName = "Alice", Email = "alice@test.com", CreatedAt = DateTime.UtcNow };
        _serviceMock.Setup(s => s.GetByIdAsync(1, default)).ReturnsAsync(profile);

        // Act
        var result = await _controller.Delete(1, default);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(profile, viewResult.Model);
    }

    [Fact]
    public async Task Delete_Get_ShouldReturnNotFound_WhenProfileNotExists()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetByIdAsync(99, default)).ReturnsAsync((UserProfileDto?)null);

        // Act
        var result = await _controller.Delete(99, default);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_ShouldRedirectToIndex()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteAsync(1, default)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteConfirmed(1, default);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        _serviceMock.Verify(s => s.DeleteAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task DeleteConfirmed_ShouldCallServiceDelete()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteAsync(5, default)).Returns(Task.CompletedTask);

        // Act
        await _controller.DeleteConfirmed(5, default);

        // Assert
        _serviceMock.Verify(s => s.DeleteAsync(5, default), Times.Once);
    }
}
