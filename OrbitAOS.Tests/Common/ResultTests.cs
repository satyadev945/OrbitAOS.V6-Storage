using OrbitAOS.Application.Common.Models;
using Xunit;

namespace OrbitAOS.Tests.Common
{
    /// <summary>
    /// Unit tests for the Result pattern used across the Application layer.
    /// </summary>
    public class ResultTests
    {
        [Fact]
        public void Result_Success_IsSuccessTrue()
        {
            var result = Result.Success();
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public void Result_Failure_IsSuccessFalse()
        {
            var result = Result.Failure("Something went wrong");
            Assert.False(result.IsSuccess);
            Assert.Equal("Something went wrong", result.ErrorMessage);
        }

        [Fact]
        public void ResultT_Success_ContainsData()
        {
            var result = Result<string>.Success("hello");
            Assert.True(result.IsSuccess);
            Assert.Equal("hello", result.Data);
        }

        [Fact]
        public void ResultT_Failure_ContainsErrorMessage()
        {
            var result = Result<string>.Failure("error occurred");
            Assert.False(result.IsSuccess);
            Assert.Equal("error occurred", result.ErrorMessage);
            Assert.Null(result.Data);
        }

        [Fact]
        public void ResultT_Failure_WithMultipleErrors_JoinsMessages()
        {
            var errors = new[] { "Error 1", "Error 2" };
            var result = Result<int>.Failure(errors);
            Assert.False(result.IsSuccess);
            Assert.Contains("Error 1", result.ErrorMessage);
            Assert.Contains("Error 2", result.ErrorMessage);
        }

        [Fact]
        public void PaginatedList_Create_ReturnsCorrectPage()
        {
            var items = Enumerable.Range(1, 20).ToList();
            var page = PaginatedList<int>.Create(items, 2, 5);

            Assert.Equal(2, page.PageNumber);
            Assert.Equal(4, page.TotalPages);
            Assert.Equal(20, page.TotalCount);
            Assert.Equal(5, page.Items.Count);
            Assert.True(page.HasPreviousPage);
            Assert.True(page.HasNextPage);
        }
    }
}
