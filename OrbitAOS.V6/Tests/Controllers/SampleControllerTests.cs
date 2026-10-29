using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Controllers;
using Xunit;

namespace OrbitAOS.V6.Tests.Controllers
{
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
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            // Arrange & Act
            var controller = new SampleController(_mockSampleService.Object, _mockLogger.Object);

            // Assert
            Assert.NotNull(controller);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfSamples()
        {
            // Arrange
            var samples = new List<SampleDto>
            {
                new SampleDto { Id = 1, Name = "Sample 1" },
                new SampleDto { Id = 2, Name = "Sample 2" }
            };
            _mockSampleService.Setup(s => s.GetAllSamplesAsync()).ReturnsAsync(samples);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<SampleDto>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Index_ReturnsErrorView_WhenExceptionThrown()
        {
            // Arrange
            _mockSampleService.Setup(s => s.GetAllSamplesAsync()).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsViewResult()
        {
            // Arrange
            var sample = new SampleDto { Id = 1, Name = "Sample 1" };
            _mockSampleService.Setup(s => s.GetSampleByIdAsync(1)).ReturnsAsync(sample);

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SampleDto>(viewResult.Model);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockSampleService.Setup(s => s.GetSampleByIdAsync(999)).ReturnsAsync((SampleDto?)null);

            // Act
            var result = await _controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_WhenExceptionThrown_ReturnsErrorView()
        {
            // Arrange
            _mockSampleService.Setup(s => s.GetSampleByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public void Create_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            var dto = new CreateSampleDto { Name = "New Sample", Description = "Description" };
            _mockSampleService.Setup(s => s.CreateSampleAsync(dto)).Returns(Task.FromResult(new SampleDto()));

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_Post_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var dto = new CreateSampleDto { Name = "New Sample" };
            _controller.ModelState.AddModelError("Description", "Required");

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(dto, viewResult.Model);
        }

        [Fact]
        public async Task Create_Post_WhenExceptionThrown_ReturnsViewWithError()
        {
            // Arrange
            var dto = new CreateSampleDto { Name = "New Sample" };
            _mockSampleService.Setup(s => s.CreateSampleAsync(dto))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Equal(dto, viewResult.Model);
        }

        [Fact]
        public async Task Edit_Get_WithValidId_ReturnsViewResult()
        {
            // Arrange
            var sample = new SampleDto { Id = 1, Name = "Sample 1", Description = "Desc", IsActive = true };
            _mockSampleService.Setup(s => s.GetSampleByIdAsync(1)).ReturnsAsync(sample);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UpdateSampleDto>(viewResult.Model);
            Assert.Equal("Sample 1", model.Name);
        }

        [Fact]
        public async Task Edit_Get_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockSampleService.Setup(s => s.GetSampleByIdAsync(999)).ReturnsAsync((SampleDto?)null);

            // Act
            var result = await _controller.Edit(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Get_WhenExceptionThrown_ReturnsErrorView()
        {
            // Arrange
            _mockSampleService.Setup(s => s.GetSampleByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_Post_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            var dto = new UpdateSampleDto { Name = "Updated Sample", Description = "Updated Desc" };
            _mockSampleService.Setup(s => s.UpdateSampleAsync(1, dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Edit(1, dto);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);
        }

        [Fact]
        public async Task Edit_Post_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var dto = new UpdateSampleDto { Name = "Updated Sample" };
            _controller.ModelState.AddModelError("Description", "Required");

            // Act
            var result = await _controller.Edit(1, dto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(dto, viewResult.Model);
        }

        [Fact]
        public async Task Edit_Post_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var dto = new UpdateSampleDto { Name = "Updated Sample" };
            _mockSampleService.Setup(s => s.UpdateSampleAsync(999, dto))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.Edit(999, dto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_WhenExceptionThrown_ReturnsViewWithError()
        {
            // Arrange
            var dto = new UpdateSampleDto { Name = "Updated Sample" };
            _mockSampleService.Setup(s => s.UpdateSampleAsync(1, dto))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.Edit(1, dto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Delete_Get_WithValidId_ReturnsViewResult()
        {
            // Arrange
            var sample = new SampleDto { Id = 1, Name = "Sample 1" };
            _mockSampleService.Setup(s => s.GetSampleByIdAsync(1)).ReturnsAsync(sample);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SampleDto>(viewResult.Model);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public async Task Delete_Get_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockSampleService.Setup(s => s.GetSampleByIdAsync(999)).ReturnsAsync((SampleDto?)null);

            // Act
            var result = await _controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Get_WhenExceptionThrown_ReturnsErrorView()
        {
            // Arrange
            _mockSampleService.Setup(s => s.GetSampleByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task DeleteConfirmed_WithValidId_RedirectsToIndex()
        {
            // Arrange
            _mockSampleService.Setup(s => s.DeleteSampleAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);
        }

        [Fact]
        public async Task DeleteConfirmed_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            _mockSampleService.Setup(s => s.DeleteSampleAsync(999))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.DeleteConfirmed(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_WhenExceptionThrown_RedirectsToIndexWithError()
        {
            // Arrange
            _mockSampleService.Setup(s => s.DeleteSampleAsync(1))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);
        }
    }
}
