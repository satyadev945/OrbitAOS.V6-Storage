// =============================================================================
// Tests/Application/Common/Mappings/MappingProfileTests.cs
// xUnit Tests for MappingProfile class
// =============================================================================

using AutoMapper;
using OrbitAOS.V6.Application.Common.Mappings;
using Xunit;

namespace OrbitAOS.V6.Tests.Tests.Application.Common.Mappings
{
    public class MappingProfileTests
    {
        [Fact]
        public void MappingProfile_CanBeInstantiated()
        {
            // Arrange & Act
            var profile = new MappingProfile();

            // Assert
            Assert.NotNull(profile);
        }

        [Fact]
        public void MappingProfile_ConfigurationIsValid()
        {
            // Arrange
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            // Act & Assert
            config.AssertConfigurationIsValid();
        }

        [Fact]
        public void MappingProfile_CreateMapper_DoesNotThrow()
        {
            // Arrange
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            // Act
            var mapper = config.CreateMapper();

            // Assert
            Assert.NotNull(mapper);
        }

        [Fact]
        public void MappingProfile_IsProfileSubclass()
        {
            // Arrange & Act
            var profile = new MappingProfile();

            // Assert
            Assert.IsAssignableFrom<Profile>(profile);
        }

        [Fact]
        public void MapperConfiguration_WithMappingProfile_IsNotNull()
        {
            // Arrange & Act
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            // Assert
            Assert.NotNull(config);
        }
    }
}
