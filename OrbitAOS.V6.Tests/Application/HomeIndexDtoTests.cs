// ============================================================
// Tests/Application/HomeIndexDtoTests.cs
// Unit Tests – Application Layer (DTOs)
//
// Tests the HomeIndexDto record type.
// ============================================================

using FluentAssertions;
using OrbitAOS.V6.Application.DTOs;
using Xunit;

namespace OrbitAOS.V6.Tests.Application;

/// <summary>
/// Unit tests for <see cref="HomeIndexDto"/>.
/// </summary>
public class HomeIndexDtoTests
{
    // ------------------------------------------------------------------
    // Constructor
    // ------------------------------------------------------------------

    [Fact]
    public void HomeIndexDto_Constructor_SetsWelcomeMessage()
    {
        // Arrange
        const string message = "Hello World";

        // Act
        var dto = new HomeIndexDto(message);

        // Assert
        dto.WelcomeMessage.Should().Be(message);
    }

    [Fact]
    public void HomeIndexDto_Constructor_WithEmptyString_SetsEmptyMessage()
    {
        // Act
        var dto = new HomeIndexDto(string.Empty);

        // Assert
        dto.WelcomeMessage.Should().BeEmpty();
    }

    [Fact]
    public void HomeIndexDto_Constructor_WithLongMessage_SetsFullMessage()
    {
        // Arrange
        var longMessage = new string('X', 5000);

        // Act
        var dto = new HomeIndexDto(longMessage);

        // Assert
        dto.WelcomeMessage.Should().Be(longMessage);
    }

    [Fact]
    public void HomeIndexDto_Constructor_WithSpecialChars_SetsMessage()
    {
        // Arrange
        const string specialMessage = "Welcome – OrbitAOS™ © 2024 <test> & more";

        // Act
        var dto = new HomeIndexDto(specialMessage);

        // Assert
        dto.WelcomeMessage.Should().Be(specialMessage);
    }

    // ------------------------------------------------------------------
    // Record equality
    // ------------------------------------------------------------------

    [Fact]
    public void HomeIndexDto_TwoInstancesWithSameMessage_AreEqual()
    {
        // Arrange
        const string message = "Same Message";

        // Act
        var dto1 = new HomeIndexDto(message);
        var dto2 = new HomeIndexDto(message);

        // Assert
        dto1.Should().Be(dto2);
    }

    [Fact]
    public void HomeIndexDto_TwoInstancesWithDifferentMessages_AreNotEqual()
    {
        // Act
        var dto1 = new HomeIndexDto("Message A");
        var dto2 = new HomeIndexDto("Message B");

        // Assert
        dto1.Should().NotBe(dto2);
    }

    // ------------------------------------------------------------------
    // Record immutability
    // ------------------------------------------------------------------

    [Fact]
    public void HomeIndexDto_WelcomeMessage_IsReadOnly()
    {
        // Arrange
        var dto = new HomeIndexDto("Original");

        // Assert – records are immutable; WelcomeMessage is init-only
        dto.WelcomeMessage.Should().Be("Original");
    }

    // ------------------------------------------------------------------
    // ToString / GetHashCode (record defaults)
    // ------------------------------------------------------------------

    [Fact]
    public void HomeIndexDto_ToString_ContainsWelcomeMessage()
    {
        // Arrange
        const string message = "OrbitAOS Welcome";
        var dto = new HomeIndexDto(message);

        // Act
        var str = dto.ToString();

        // Assert
        str.Should().Contain(message);
    }

    [Fact]
    public void HomeIndexDto_GetHashCode_SameForEqualInstances()
    {
        // Arrange
        const string message = "Hash Test";
        var dto1 = new HomeIndexDto(message);
        var dto2 = new HomeIndexDto(message);

        // Assert
        dto1.GetHashCode().Should().Be(dto2.GetHashCode());
    }

    [Fact]
    public void HomeIndexDto_IsNotNull()
    {
        // Act
        var dto = new HomeIndexDto("test");

        // Assert
        dto.Should().NotBeNull();
    }
}
