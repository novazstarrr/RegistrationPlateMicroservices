using Xunit;
using Catalog.API.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Catalog.UnitTests.Data.Common
{
    public class PagedListTests
    {
        private class TestItem
        {
            public int Id { get; set; }
            public string Value { get; set; } = string.Empty;
        }

        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            var items = new List<string> { "item1", "item2" };
            var totalCount = 10;
            var pageNumber = 1;
            var pageSize = 2;
            var sortOrder = "asc";

            // Act
            var pagedList = new PagedList<string>(items, totalCount, pageNumber, pageSize, sortOrder);

            // Assert
            Assert.Equal(items, pagedList.Items);
            Assert.Equal(pageNumber, pagedList.PageNumber);
            Assert.Equal(totalCount, pagedList.TotalCount);
            Assert.Equal(5, pagedList.TotalPages);
            Assert.Equal(sortOrder, pagedList.SortOrder);
        }

        [Theory]
        [InlineData(1, 2, 2)]
        [InlineData(2, 2, 0)]
        [InlineData(3, 2, 0)]
        public async Task CreateAsync_ReturnsCorrectItems(int pageNumber, int pageSize, int expectedItemCount)
        {
            // Arrange
            var items = new List<TestItem>
            {
                new TestItem { Id = 1, Value = "item1" },
                new TestItem { Id = 2, Value = "item2" }
            };
            var dbContext = CreateDbContext(items);
            var query = dbContext.Set<TestItem>().AsQueryable();

            // Act
            var result = await PagedList<TestItem>.CreateAsync(query, pageNumber, pageSize);

            // Assert
            Assert.Equal(expectedItemCount, result.Items.Count);
            Assert.Equal(items.Count, result.TotalCount);
            Assert.Equal(pageNumber, result.PageNumber);
        }

        [Fact]
        public void Create_WithEnumerable_ReturnsCorrectList()
        {
            // Arrange
            var items = new List<string> { "item1", "item2", "item3", "item4" };
            var pageNumber = 2;
            var pageSize = 2;
            var totalCount = items.Count;

            // Act
            var result = PagedList<string>.Create(
                items.Skip((pageNumber - 1) * pageSize).Take(pageSize),
                pageNumber,
                pageSize,
                totalCount);

            // Assert
            Assert.Equal(2, result.Items.Count);
            Assert.Equal(totalCount, result.TotalCount);
            Assert.Equal(pageNumber, result.PageNumber);
            Assert.Equal("item3", result.Items[0]);
            Assert.Equal("item4", result.Items[1]);
        }

        [Theory]
        [InlineData(10, 3, 4)]
        [InlineData(10, 4, 3)]
        [InlineData(10, 5, 2)]
        public void Constructor_CalculatesTotalPagesCorrectly(int totalCount, int pageSize, int expectedTotalPages)
        {
            // Arrange
            var items = new List<string>();

            // Act
            var pagedList = new PagedList<string>(items, totalCount, 1, pageSize);

            // Assert
            Assert.Equal(expectedTotalPages, pagedList.TotalPages);
        }

        private DbContext CreateDbContext(List<TestItem> items)
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb" + Guid.NewGuid())
                .Options;

            var context = new TestDbContext(options);
            context.TestItems.AddRange(items);
            context.SaveChanges();

            return context;
        }

        private class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions options) : base(options) { }
            public DbSet<TestItem> TestItems { get; set; } = null!;

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<TestItem>().HasKey(x => x.Id);
            }
        }
    }
}