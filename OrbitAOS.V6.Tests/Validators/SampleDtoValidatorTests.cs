using FluentAssertions;
using FluentValidation.TestHelper;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Application.Validators;
using Xunit;

namespace OrbitAOS.V6.Tests.Validators
{
    /// <summary>
    /// Unit tests for DTO validators
    /// </summary>
    public class SampleDtoValidatorTests
    {
        private readonly CreateSampleDtoValidator _createValidator;
        private readonly UpdateSampleDtoValidator _updateValidator;

        public SampleDtoValidatorTests()
        {
            _createValidator = new CreateSampleDtoValidator();
            _updateValidator = new UpdateSampleDtoValidator();
        }

        [Fact]
        public void CreateSampleDto_WithValidData_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var dto = new CreateSampleDto
            {
                Name = "Valid Name",
                Description = "Valid Description",
                IsActive = true
            };

            // Act
            var result = _createValidator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void CreateSampleDto_WithEmptyName_ShouldHaveValidationError()
        {
            // Arrange
            var dto = new CreateSampleDto
            {
                Name = "",
                Description = "Valid Description",
                IsActive = true
            };

            // Act
            var result = _createValidator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Name is required");
        }

        [Fact]
        public void CreateSampleDto_WithNameTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var dto = new CreateSampleDto
            {
                Name = new string('A', 201), // 201 characters
                Description = "Valid Description",
                IsActive = true
            };

            // Act
            var result = _createValidator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Name must not exceed 200 characters");
        }

        [Fact]
        public void CreateSampleDto_WithDescriptionTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var dto = new CreateSampleDto
            {
                Name = "Valid Name",
                Description = new string('A', 1001), // 1001 characters
                IsActive = true
            };

            // Act
            var result = _createValidator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Description must not exceed 1000 characters");
        }

        [Fact]
        public void UpdateSampleDto_WithValidData_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var dto = new UpdateSampleDto
            {
                Name = "Valid Name",
                Description = "Valid Description",
                IsActive = false
            };

            // Act
            var result = _updateValidator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void UpdateSampleDto_WithEmptyName_ShouldHaveValidationError()
        {
            // Arrange
            var dto = new UpdateSampleDto
            {
                Name = "",
                Description = "Valid Description",
                IsActive = true
            };

            // Act
            var result = _updateValidator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Name is required");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateSampleDto_WithInvalidName_ShouldHaveValidationError(string? name)
        {
            // Arrange
            var dto = new CreateSampleDto
            {
                Name = name!,
                Description = "Valid Description",
                IsActive = true
            };

            // Act
            var result = _createValidator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("Valid Name")]
        [InlineData("This is a longer but still valid name")]
        public void CreateSampleDto_WithValidNameLengths_ShouldNotHaveValidationError(string name)
        {
            // Arrange
            var dto = new CreateSampleDto
            {
                Name = name,
                Description = "Valid Description",
                IsActive = true
            };

            // Act
            var result = _createValidator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }
    }
}
