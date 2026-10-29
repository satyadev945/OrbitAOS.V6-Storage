using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using OrbitAOS.V6.Filters;
using Xunit;

namespace OrbitAOS.V6.Tests.Filters
{
    public class ValidationFilterTests
    {
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly ValidationFilter _filter;

        public ValidationFilterTests()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
            _filter = new ValidationFilter(_mockServiceProvider.Object);
        }

        [Fact]
        public void Constructor_WithValidServiceProvider_CreatesInstance()
        {
            // Arrange & Act
            var filter = new ValidationFilter(_mockServiceProvider.Object);

            // Assert
            Assert.NotNull(filter);
        }

        [Fact]
        public async Task OnActionExecutionAsync_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var context = CreateActionExecutingContext();
            context.ModelState.AddModelError("TestKey", "Test error");
            ActionExecutionDelegate next = () => Task.FromResult(CreateActionExecutedContext(context));

            // Act
            await _filter.OnActionExecutionAsync(context, next);

            // Assert
            var result = Assert.IsType<BadRequestObjectResult>(context.Result);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public async Task OnActionExecutionAsync_WithValidModelState_CallsNext()
        {
            // Arrange
            var context = CreateActionExecutingContext();
            var nextCalled = false;
            ActionExecutionDelegate next = () =>
            {
                nextCalled = true;
                return Task.FromResult(CreateActionExecutedContext(context));
            };

            // Act
            await _filter.OnActionExecutionAsync(context, next);

            // Assert
            Assert.True(nextCalled);
        }

        [Fact]
        public async Task OnActionExecutionAsync_WithNullArgument_SkipsValidation()
        {
            // Arrange
            var context = CreateActionExecutingContext();
            context.ActionArguments["test"] = null!;
            var nextCalled = false;
            ActionExecutionDelegate next = () =>
            {
                nextCalled = true;
                return Task.FromResult(CreateActionExecutedContext(context));
            };

            // Act
            await _filter.OnActionExecutionAsync(context, next);

            // Assert
            Assert.True(nextCalled);
        }

        [Fact]
        public async Task OnActionExecutionAsync_WithValidatorAndValidData_CallsNext()
        {
            // Arrange
            var testModel = new TestModel { Name = "Valid" };
            var context = CreateActionExecutingContext();
            context.ActionArguments["model"] = testModel;

            var mockValidator = new Mock<IValidator<TestModel>>();
            var validationResult = new ValidationResult();
            mockValidator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestModel>>(), default))
                .ReturnsAsync(validationResult);

            _mockServiceProvider.Setup(sp => sp.GetService(typeof(IValidator<TestModel>)))
                .Returns(mockValidator.Object);

            var nextCalled = false;
            ActionExecutionDelegate next = () =>
            {
                nextCalled = true;
                return Task.FromResult(CreateActionExecutedContext(context));
            };

            // Act
            await _filter.OnActionExecutionAsync(context, next);

            // Assert
            Assert.True(nextCalled);
        }

        [Fact]
        public async Task OnActionExecutionAsync_WithValidatorAndInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var testModel = new TestModel { Name = "" };
            var context = CreateActionExecutingContext();
            context.ActionArguments["model"] = testModel;

            var mockValidator = new Mock<IValidator<TestModel>>();
            var validationResult = new ValidationResult(new[]
            {
                new ValidationFailure("Name", "Name is required")
            });
            mockValidator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestModel>>(), default))
                .ReturnsAsync(validationResult);

            _mockServiceProvider.Setup(sp => sp.GetService(typeof(IValidator<TestModel>)))
                .Returns(mockValidator.Object);

            ActionExecutionDelegate next = () => Task.FromResult(CreateActionExecutedContext(context));

            // Act
            await _filter.OnActionExecutionAsync(context, next);

            // Assert
            var result = Assert.IsType<BadRequestObjectResult>(context.Result);
            Assert.False(context.ModelState.IsValid);
            Assert.True(context.ModelState.ContainsKey("Name"));
        }

        [Fact]
        public async Task OnActionExecutionAsync_WithNoValidator_CallsNext()
        {
            // Arrange
            var testModel = new TestModel { Name = "Test" };
            var context = CreateActionExecutingContext();
            context.ActionArguments["model"] = testModel;

            _mockServiceProvider.Setup(sp => sp.GetService(It.IsAny<Type>()))
                .Returns(null);

            var nextCalled = false;
            ActionExecutionDelegate next = () =>
            {
                nextCalled = true;
                return Task.FromResult(CreateActionExecutedContext(context));
            };

            // Act
            await _filter.OnActionExecutionAsync(context, next);

            // Assert
            Assert.True(nextCalled);
        }

        [Fact]
        public async Task OnActionExecutionAsync_WithMultipleArguments_ValidatesAll()
        {
            // Arrange
            var testModel1 = new TestModel { Name = "Test1" };
            var testModel2 = new TestModel { Name = "Test2" };
            var context = CreateActionExecutingContext();
            context.ActionArguments["model1"] = testModel1;
            context.ActionArguments["model2"] = testModel2;

            _mockServiceProvider.Setup(sp => sp.GetService(It.IsAny<Type>()))
                .Returns(null);

            var nextCalled = false;
            ActionExecutionDelegate next = () =>
            {
                nextCalled = true;
                return Task.FromResult(CreateActionExecutedContext(context));
            };

            // Act
            await _filter.OnActionExecutionAsync(context, next);

            // Assert
            Assert.True(nextCalled);
        }

        [Fact]
        public async Task OnActionExecutionAsync_WithMultipleValidationErrors_AddsAllToModelState()
        {
            // Arrange
            var testModel = new TestModel { Name = "" };
            var context = CreateActionExecutingContext();
            context.ActionArguments["model"] = testModel;

            var mockValidator = new Mock<IValidator<TestModel>>();
            var validationResult = new ValidationResult(new[]
            {
                new ValidationFailure("Name", "Name is required"),
                new ValidationFailure("Name", "Name must be at least 3 characters")
            });
            mockValidator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestModel>>(), default))
                .ReturnsAsync(validationResult);

            _mockServiceProvider.Setup(sp => sp.GetService(typeof(IValidator<TestModel>)))
                .Returns(mockValidator.Object);

            ActionExecutionDelegate next = () => Task.FromResult(CreateActionExecutedContext(context));

            // Act
            await _filter.OnActionExecutionAsync(context, next);

            // Assert
            var result = Assert.IsType<BadRequestObjectResult>(context.Result);
            Assert.False(context.ModelState.IsValid);
            Assert.Equal(2, context.ModelState["Name"]!.Errors.Count);
        }

        private ActionExecutingContext CreateActionExecutingContext()
        {
            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new ActionDescriptor(),
                new ModelStateDictionary()
            );

            return new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?>(),
                new object()
            );
        }

        private ActionExecutedContext CreateActionExecutedContext(ActionExecutingContext executingContext)
        {
            return new ActionExecutedContext(
                executingContext,
                new List<IFilterMetadata>(),
                new object()
            );
        }

        private class TestModel
        {
            public string Name { get; set; } = string.Empty;
        }
    }
}
