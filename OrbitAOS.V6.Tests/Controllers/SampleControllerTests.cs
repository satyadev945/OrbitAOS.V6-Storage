using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Controllers;
using Xunit;

namespace OrbitAOS.V6.Tests.Controllers
{
    /// <summary>
    /// Unit tests for SampleController
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
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithSamples()
        {
            // Arrange
            var samples = new List<SampleDto>
            {
                new SampleDto { Id = 1, Name = "Sample 1", Description = "Description 1", IsActive = true },
                new SampleDto { Id = 2, Name = "Sample 2", Description = "Description 2", IsActive = true }
            };
            _mockSampleService.Setup(s => s.GetAllSamplesAsync()).ReturnsAsync(samples);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeAssignableTo<IEnumerable<SampleDto>>().Subject;
            model.Should().HaveCount(2);
            _mockSampleService.Verify(s => s.GetAllSamplesAsync(), Times.Once);
        }

        [Fact]
        public async Task Details_WithValidId_ShouldReturnViewWithSample()
        {
            // Arrange
            var sample = new SampleDto { Id = 1, Name = "Sample 1", Description = "Description 1", IsActive = true };
            _mockSampleService.Setup(s => s.GetSampleByIdAsync(1)).ReturnsAsync(sample);

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeAssignableTo<SampleDto>().Subject;
            model.Should().BeEquivalentTo(sample);
            _mockSampleService.Verify(s => s.GetSampleByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task Details_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            _mockSampleService.Setup(s => s.GetSampleByIdAsync(999)).ReturnsAsync((SampleDto?)null);

            // Act
            var result = await _controller.Details(999);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            _mockSampleService.Verify(s => s.GetSampleByIdAsync(999), Times.Once);
        }

        [Fact]
        public void Create_Get_ShouldReturnView()
        {
            // Act
            var result = _controller.Create();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Create_Post_WithValidModel_ShouldRedirectToIndex()
        {
            // Arrange
            var createDto = new CreateSampleDto { Name = "New Sample", Description = "New Description", IsActive = true };
            var createdDto = new SampleDto { Id = 1, Name = "New Sample", Description = "New Description", IsActive = true };
            _mockSampleService.Setup(s => s.CreateSampleAsync(createDto)).ReturnsAsync(createdDto);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
            _mockSampleService.Verify(s => s.CreateSampleAsync(createDto), Times.Once);
        }

        [Fact]
        public async Task Create_Post_WithInvalidModel_ShouldReturnViewWithModel()
        {
            // Arrange
            var createDto = new CreateSampleDto { Name = "", Description = "Description", IsActive = true };
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeAssignableTo<CreateSampleDto>().Subject;
            model.Should().BeEquivalentTo(createDto);
            _mockSampleService.Verify(s => s.CreateSampleAsync(It.IsAny<CreateSampleDto>()), Times.Never);
        }

        [Fact]
        public async Task Edit_Get_WithValidId_ShouldReturnViewWithDto()
        {
            // Arrange
            var sample = new SampleDto { Id = 1, Name = "Sample 1", Description = "Description 1", IsActive = true };
            _mockSampleService.Setup(s => s.GetSampleByIdAsync(1)).ReturnsAsync(sample);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeAssignableTo<UpdateSampleDto>().Subject;
            model.Name.Should().Be(sample.Name);
            model.Description.Should().Be(sample.Description);
            model.IsActive.Should().Be(sample.IsActive);
        }

        [Fact]
        public async Task Edit_Post_WithValidModel_ShouldRedirectToIndex()
        {
            // Arrange
            var updateDto = new UpdateSampleDto { Name = "Updated Sample", Description = "Updated Description", IsActive = false };

            // Act
            var result = await _controller.Edit(1, updateDto);

            // Assert
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
            _mockSampleService.Verify(s => s.UpdateSampleAsync(1, updateDto), Times.Once);
        }

        [Fact]
        public async Task Delete_Get_WithValidId_ShouldReturnViewWithSample()
        {
            // Arrange
            var sample = new SampleDto { Id = 1, Name = "Sample 1", Description = "Description 1", IsActive = true };
            _mockSampleService.Setup(s => s.GetSampleByIdAsync(1)).ReturnsAsync(sample);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeAssignableTo<SampleDto>().Subject;
            model.Should().BeEquivalentTo(sample);
        }

        [Fact]
        public async Task DeleteConfirmed_WithValidId_ShouldRedirectToIndex()
        {
            // Arrange
            _mockSampleService.Setup(s => s.DeleteSampleAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
            _mockSampleService.Verify(s => s.DeleteSampleAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteConfirmed_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            _mockSampleService.Setup(s => s.DeleteSampleAsync(999))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.DeleteConfirmed(999);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
