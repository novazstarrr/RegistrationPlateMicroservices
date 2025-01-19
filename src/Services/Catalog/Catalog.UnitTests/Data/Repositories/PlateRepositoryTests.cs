using Xunit;
using Microsoft.EntityFrameworkCore;
using Catalog.API.Data;
using Catalog.API.Data.Repositories;
using Catalog.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.UnitTests.Data.Repositories
{
    public class PlateRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly PlateRepository _repository;

        public PlateRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb" + Guid.NewGuid())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new PlateRepository(_context);
        }



        [Fact]
        public async Task GetByIdAsync_WhenExists_ReturnsPlate()
        {
            // Arrange
            var plate = new Plate { Registration = "ABC123", PurchasePrice = 1000m, SalePrice = 2000m };
            await _context.Plates.AddAsync(plate);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(plate.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(plate.Registration!, result!.Registration!);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_WithNewPlate_AddsSuccessfully()
        {
            // Arrange
            var plate = new Plate { Registration = "ABC123", PurchasePrice = 1000m, SalePrice = 2000m };

            // Act
            var result = await _repository.AddAsync(plate);

            // Assert
            Assert.NotNull(result);
            var dbPlate = await _context.Plates.FindAsync(plate.Id);
            Assert.NotNull(dbPlate);
            Assert.Equal(plate.Registration!, dbPlate!.Registration!);
        }

        [Fact]
        public async Task AddAsync_WithDuplicateRegistration_ReturnsNull()
        {
            // Arrange
            var plate1 = new Plate { Registration = "ABC123", PurchasePrice = 1000m, SalePrice = 2000m };
            await _context.Plates.AddAsync(plate1);
            await _context.SaveChangesAsync();

            var plate2 = new Plate { Registration = "ABC123", PurchasePrice = 2000m, SalePrice = 3000m };

            // Act
            var result = await _repository.AddAsync(plate2);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateStatusAsync_WhenExists_UpdatesStatus()
        {
            // Arrange
            var plate = new Plate { Registration = "ABC123", PurchasePrice = 1000m, SalePrice = 2000m };
            await _context.Plates.AddAsync(plate);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.UpdateStatusAsync(plate.Id, PlateStatus.Reserved);

            // Assert
            Assert.NotNull(result);
            var dbPlate = await _context.Plates.FindAsync(plate.Id);
            Assert.NotNull(dbPlate);
            Assert.NotNull(plate.Registration);
        }

        [Fact]
        public async Task UpdateStatusAsync_WhenNotExists_ReturnsNull()
        {
            // Act
            var result = await _repository.UpdateStatusAsync(Guid.NewGuid(), PlateStatus.Reserved);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData("ABC", "ABC123")]
        [InlineData("123", "ABC123")]
        [InlineData("BC1", "ABC123")]
        public async Task GetPlatesAsync_WithNameMatch_ReturnsMatchingPlates(string pattern, string registration)
        {
            // Arrange
            var plate = new Plate { Registration = registration, PurchasePrice = 1000m, SalePrice = 2000m };
            await _context.Plates.AddAsync(plate);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetPlatesAsync(nameMatch: pattern);

            // Assert
            Assert.NotNull(result.Items);
            var items = result.Items;
            Assert.NotEmpty(items);
            var firstItem = items.First();
            Assert.NotNull(firstItem);
            Assert.NotNull(firstItem.Registration);
            Assert.Equal(registration, firstItem.Registration);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}