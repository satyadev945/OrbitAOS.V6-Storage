using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Web.Controllers;
using Xunit;

namespace OrbitAOS.V6.Tests.Unit;

/// <summary>
/// Unit tests for SampleController.
/// Tests all CRUD action methods using Moq for service mocking.
/// Follows AAA (Arrange-Act-Assert) pattern.
/// </summary>
public class SampleControllerTests
{
    private readonly Mock<ISampleService> _mockSampleService;
    private readonly Mock<ILogger<SampleController>> _mockLogger;
    private readonly SampleController _controller;

    public SampleControllerTests()
    {
        _mockSampleService = new Mock<ISampleService>();
        _mockLogger = new Mock<ILogger<SampleController>>();
        _controller = new SampleController(_mockSampleService.Object, _mockLogger.Object);

        // Set up TempData
        _controller.TempData = new TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<ITempDataProvider>());
    }

    [Fact]
    public async Task Index_ShouldReturnViewWithSamples()
    {
        // Arrange
        var samples = new List<SampleDto>
        {
            new SampleDto { Id = 1, Name = "Sample 1", Description = "Desc 1", IsActive = true },
            new SampleDto { Id = 2, Name = "Sample 2", Description = "Desc 2", IsActive = false }
        };
        _mockSampleService.Setup(s => s.GetAllSamplesAsync()).ReturnsAsync(samples);

        // Act
        var result = await _controller.Index() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        var model = result!.Model as IEnumerable<SampleDto>;
        model.Should().HaveCount(2);
        _mockSampleService.Verify(s => s.GetAllSamplesAsync(), Times.Once);
    }

    [Fact]
    public async Task Details_WhenSampleExists_ShouldReturnViewWithSample()
    {
        // Arrange
        var sample = new SampleDto { Id = 1, Name = "Sample 1", Description = "Desc 1", IsActive = true };
        _mockSampleService.Setup(s => s.GetSampleByIdAsync(1)).ReturnsAsync(sample);

        // Act
        var result = await _controller.Details(1) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        var model = result!.Model as SampleDto;
        model.Should().NotBeNull();
        model!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Details_WhenSampleNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _mockSampleService.Setup(s => s.GetSampleByIdAsync(999)).ReturnsAsync((SampleDto?)null);

        // Act
        var result = await _controller.Details(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void Create_Get_ShouldReturnViewWithEmptyDto()
    {
        // Act
        var result = _controller.Create() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.Model.Should().BeOfType<SampleDto>();
    }

    [Fact]
    public async Task Create_Post_WhenModelValid_ShouldRedirectToIndex()
    {
        // Arrange
        var dto = new SampleDto { Name = "New Sample", Description = "New Desc", IsActive = true };
        var created = new SampleDto { Id = 1, Name = "New Sample", Description = "New Desc", IsActive = true };
        _mockSampleService.Setup(s => s.CreateSampleAsync(dto)).ReturnsAsync(created);

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be("Index");
        _mockSampleService.Verify(s => s.CreateSampleAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Create_Post_WhenModelInvalid_ShouldReturnView()
    {
        // Arrange
        var dto = new SampleDto { Name = "", Description = "Desc", IsActive = true };
        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.Create(dto) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        _mockSampleService.Verify(s => s.CreateSampleAsync(It.IsAny<SampleDto>()), Times.Never);
    }

    [Fact]
    public async Task Edit_Get_WhenSampleExists_ShouldReturnViewWithSample()
    {
        // Arrange
        var sample = new SampleDto { Id = 1, Name = "Sample 1", Description = "Desc 1", IsActive = true };
        _mockSampleService.Setup(s => s.GetSampleByIdAsync(1)).ReturnsAsync(sample);

        // Act
        var result = await _controller.Edit(1) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        var model = result!.Model as SampleDto;
        model!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Edit_Get_WhenSampleNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _mockSampleService.Setup(s => s.GetSampleByIdAsync(999)).ReturnsAsync((SampleDto?)null);

        // Act
        var result = await _controller.Edit(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Edit_Post_WhenModelValid_ShouldRedirectToIndex()
    {
        // Arrange
        var dto = new SampleDto { Id = 1, Name = "Updated", Description = "Updated Desc", IsActive = true };
        _mockSampleService.Setup(s => s.UpdateSampleAsync(1, dto)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Edit(1, dto);

        // Assert
        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be("Index");
        _mockSampleService.Verify(s => s.UpdateSampleAsync(1, dto), Times.Once);
    }

    [Fact]
    public async Task Edit_Post_WhenIdMismatch_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new SampleDto { Id = 2, Name = "Updated", Description = "Updated Desc", IsActive = true };

        // Act
        var result = await _controller.Edit(1, dto);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task Delete_Get_WhenSampleExists_ShouldReturnViewWithSample()
    {
        // Arrange
        var sample = new SampleDto { Id = 1, Name = "Sample 1", Description = "Desc 1", IsActive = true };
        _mockSampleService.Setup(s => s.GetSampleByIdAsync(1)).ReturnsAsync(sample);

        // Act
        var result = await _controller.Delete(1) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        var model = result!.Model as SampleDto;
        model!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Delete_Get_WhenSampleNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _mockSampleService.Setup(s => s.GetSampleByIdAsync(999)).ReturnsAsync((SampleDto?)null);

        // Act
        var result = await _controller.Delete(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteConfirmed_ShouldDeleteAndRedirectToIndex()
    {
        // Arrange
        _mockSampleService.Setup(s => s.DeleteSampleAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteConfirmed(1);

        // Assert
        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be("Index");
        _mockSampleService.Verify(s => s.DeleteSampleAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteConfirmed_WhenNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _mockSampleService.Setup(s => s.DeleteSampleAsync(999))
            .ThrowsAsync(new KeyNotFoundException("Sample not found"));

        // Act
        var result = await _controller.DeleteConfirmed(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
