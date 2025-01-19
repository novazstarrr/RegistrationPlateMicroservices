using Xunit;
using Catalog.API.DTOs.Common;
using System.Collections.Generic;

namespace Catalog.UnitTests.DTOs.Common
{
    public class PagedListDtoTests
    {
        [Fact]
        public void Constructor_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var items = new List<string> { "item1", "item2" };
            var totalCount = 10;
            var pageNumber = 1;
            var pageSize = 2;
            var sortOrder = "asc";

            // Act
            var pagedList = new PagedListDto<string>(items, totalCount, pageNumber, pageSize, sortOrder);

            // Assert
            Assert.Equal(items, pagedList.Items);
            Assert.Equal(pageNumber, pagedList.PageNumber);
            Assert.Equal(totalCount, pagedList.TotalCount);
            Assert.Equal(5, pagedList.TotalPages);
            Assert.Equal(sortOrder, pagedList.SortOrder);
            Assert.False(pagedList.HasPreviousPage);
            Assert.True(pagedList.HasNextPage);
        }

        [Fact]
        public void HasPreviousPage_ShouldBeTrueForPageGreaterThanOne()
        {
            // Arrange & Act
            var pagedList = new PagedListDto<string>(
                new List<string>(),
                totalCount: 10,
                pageNumber: 2,
                pageSize: 2);

            // Assert
            Assert.True(pagedList.HasPreviousPage);
        }

        [Fact]
        public void HasNextPage_ShouldBeFalseForLastPage()
        {
            // Arrange & Act
            var pagedList = new PagedListDto<string>(
                new List<string>(),
                totalCount: 10,
                pageNumber: 5,
                pageSize: 2);

            // Assert
            Assert.False(pagedList.HasNextPage);
        }
    }
}