// ============================================================
// Tests/Application/DTOs/HomeViewModelTests.cs
// Unit tests for HomeViewModel DTO — xUnit
// ============================================================

using OrbitAOS.V6.Application.DTOs;
using Xunit;

namespace OrbitAOS.V6.Tests.Application.DTOs;

public class HomeViewModelTests
{
    // ── Default Values ───────────────────────────────────────────────────────

    [Fact]
    public void HomeViewModel_DefaultWelcomeMessage_IsEmptyString()
    {
        // Arrange & Act
        var vm = new HomeViewModel();

        // Assert
        Assert.Equal(string.Empty, vm.WelcomeMessage);
    }

    [Fact]
    public void HomeViewModel_DefaultGeneratedAt_IsRecentUtc()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var vm = new HomeViewModel();

        // Assert
        var after = DateTime.UtcNow.AddSeconds(1);
        Assert.True(vm.GeneratedAt >= before && vm.GeneratedAt <= after,
            $"GeneratedAt {vm.GeneratedAt} should be between {before} and {after}");
    }

    // ── Init Setters ─────────────────────────────────────────────────────────

    [Fact]
    public void HomeViewModel_SetWelcomeMessage_Persists()
    {
        // Arrange & Act
        var vm = new HomeViewModel { WelcomeMessage = "Hello World" };

        // Assert
        Assert.Equal("Hello World", vm.WelcomeMessage);
    }

    [Fact]
    public void HomeViewModel_SetGeneratedAt_Persists()
    {
        // Arrange
        var ts = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var vm = new HomeViewModel { GeneratedAt = ts };

        // Assert
        Assert.Equal(ts, vm.GeneratedAt);
    }

    [Fact]
    public void HomeViewModel_WelcomeMessage_CanBeSet_ToLongString()
    {
        // Arrange
        var longMessage = new string('A', 1000);

        // Act
        var vm = new HomeViewModel { WelcomeMessage = longMessage };

        // Assert
        Assert.Equal(longMessage, vm.WelcomeMessage);
    }

    [Theory]
    [InlineData("Welcome to OrbitAOS")]
    [InlineData("Hello, User!")]
    [InlineData("")]
    public void HomeViewModel_WelcomeMessage_VariousValues(string message)
    {
        // Arrange & Act
        var vm = new HomeViewModel { WelcomeMessage = message };

        // Assert
        Assert.Equal(message, vm.WelcomeMessage);
    }

    [Fact]
    public void HomeViewModel_IsSealed_CannotBeSubclassed()
    {
        // Assert — HomeViewModel is sealed, verify via type
        Assert.True(typeof(HomeViewModel).IsSealed);
    }

    [Fact]
    public void HomeViewModel_FullyPopulated_AllPropertiesCorrect()
    {
        // Arrange
        var ts = new DateTime(2024, 6, 1, 10, 30, 0, DateTimeKind.Utc);
        const string msg = "Welcome to OrbitAOS — running on .NET 8";

        // Act
        var vm = new HomeViewModel
        {
            WelcomeMessage = msg,
            GeneratedAt = ts
        };

        // Assert
        Assert.Equal(msg, vm.WelcomeMessage);
        Assert.Equal(ts, vm.GeneratedAt);
    }
}
