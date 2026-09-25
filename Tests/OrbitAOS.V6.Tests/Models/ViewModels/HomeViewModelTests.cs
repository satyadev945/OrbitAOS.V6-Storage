using FluentAssertions;
using OrbitAOS.V6.Models.ViewModels;
using Xunit;

namespace OrbitAOS.V6.Tests.Models.ViewModels
{
    /// <summary>
    /// Unit tests for HomeViewModel – Web layer view model.
    /// Covers default values, property assignment, and all fields.
    /// </summary>
    public class HomeViewModelTests
    {
        // ─── Default State ──────────────────────────────────────────────────────

        [Fact]
        public void HomeViewModel_DefaultConstructor_WelcomeMessageIsEmpty()
        {
            // Arrange & Act
            var vm = new HomeViewModel();

            // Assert
            vm.WelcomeMessage.Should().BeEmpty();
        }

        [Fact]
        public void HomeViewModel_DefaultConstructor_IsAuthenticatedIsFalse()
        {
            // Arrange & Act
            var vm = new HomeViewModel();

            // Assert
            vm.IsAuthenticated.Should().BeFalse();
        }

        [Fact]
        public void HomeViewModel_DefaultConstructor_UserNameIsNull()
        {
            // Arrange & Act
            var vm = new HomeViewModel();

            // Assert
            vm.UserName.Should().BeNull();
        }

        // ─── Property assignment ────────────────────────────────────────────────

        [Fact]
        public void WelcomeMessage_CanBeSetAndRetrieved()
        {
            // Arrange
            var vm = new HomeViewModel();
            const string expected = "Welcome to OrbitAOS!";

            // Act
            vm.WelcomeMessage = expected;

            // Assert
            vm.WelcomeMessage.Should().Be(expected);
        }

        [Fact]
        public void IsAuthenticated_CanBeSetToTrue()
        {
            // Arrange
            var vm = new HomeViewModel();

            // Act
            vm.IsAuthenticated = true;

            // Assert
            vm.IsAuthenticated.Should().BeTrue();
        }

        [Fact]
        public void UserName_CanBeSetAndRetrieved()
        {
            // Arrange
            var vm = new HomeViewModel();
            const string expected = "john.doe";

            // Act
            vm.UserName = expected;

            // Assert
            vm.UserName.Should().Be(expected);
        }

        [Fact]
        public void UserName_CanBeSetToNull()
        {
            // Arrange
            var vm = new HomeViewModel { UserName = "someone" };

            // Act
            vm.UserName = null;

            // Assert
            vm.UserName.Should().BeNull();
        }

        // ─── Object initializer ─────────────────────────────────────────────────

        [Fact]
        public void HomeViewModel_ObjectInitializer_SetsAllProperties()
        {
            // Arrange & Act
            var vm = new HomeViewModel
            {
                WelcomeMessage = "Hello World",
                IsAuthenticated = true,
                UserName = "admin"
            };

            // Assert
            vm.WelcomeMessage.Should().Be("Hello World");
            vm.IsAuthenticated.Should().BeTrue();
            vm.UserName.Should().Be("admin");
        }

        [Theory]
        [InlineData("Welcome!", true, "alice")]
        [InlineData("Hello", false, null)]
        [InlineData("", true, "bob")]
        public void HomeViewModel_VariousInitializations_PropertiesMatchExpected(
            string welcomeMessage, bool isAuthenticated, string? userName)
        {
            // Arrange & Act
            var vm = new HomeViewModel
            {
                WelcomeMessage = welcomeMessage,
                IsAuthenticated = isAuthenticated,
                UserName = userName
            };

            // Assert
            vm.WelcomeMessage.Should().Be(welcomeMessage);
            vm.IsAuthenticated.Should().Be(isAuthenticated);
            vm.UserName.Should().Be(userName);
        }
    }
}
