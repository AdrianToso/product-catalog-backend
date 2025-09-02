using ADR_T.ProductCatalog.Application.DTOs.Common;
using System.Collections.Generic;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Application.DTOs.Common;

public class PagedResponseTests
{
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var items = new List<string> { "item1", "item2", "item3" };
        int pageNumber = 2;
        int pageSize = 10;
        int totalCount = 25;

        // Act
        var response = new PagedResponse<string>(items, pageNumber, pageSize, totalCount);

        // Assert
        Assert.Equal(items, response.Items);
        Assert.Equal(pageNumber, response.PageNumber);
        Assert.Equal(pageSize, response.PageSize);
        Assert.Equal(totalCount, response.TotalCount);
        Assert.Equal(3, response.TotalPages); // 25/10 = 2.5 → ceil = 3
    }

    [Theory]
    [InlineData(1, 10, 25, false, true)]  // First page
    [InlineData(2, 10, 25, true, true)]   // Middle page
    [InlineData(3, 10, 25, true, false)]  // Last page
    [InlineData(1, 10, 10, false, false)] // Only one page
    public void HasPreviousAndNextPage_ShouldReturnCorrectValues(
        int pageNumber, int pageSize, int totalCount,
        bool expectedHasPrevious, bool expectedHasNext)
    {
        // Arrange
        var items = new List<string> { "item1" };
        var response = new PagedResponse<string>(items, pageNumber, pageSize, totalCount);

        // Act & Assert
        Assert.Equal(expectedHasPrevious, response.HasPreviousPage);
        Assert.Equal(expectedHasNext, response.HasNextPage);
    }

    [Theory]
    [InlineData(10, 10, 1)]  // Exact division
    [InlineData(11, 10, 2)]  // Needs rounding up
    [InlineData(9, 10, 1)]   // Less than page size
    [InlineData(0, 10, 0)]   // Empty
    public void TotalPages_ShouldCalculateCorrectly(int totalCount, int pageSize, int expectedTotalPages)
    {
        // Arrange
        var items = new List<string> { "item1" };
        int pageNumber = 1;

        // Act
        var response = new PagedResponse<string>(items, pageNumber, pageSize, totalCount);

        // Assert
        Assert.Equal(expectedTotalPages, response.TotalPages);
    }
}
