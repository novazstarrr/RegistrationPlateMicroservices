using Xunit;
using Catalog.API.DTOs.Responses;
using Catalog.Domain;
using System;

namespace Catalog.UnitTests.DTOs.Responses
{
    public class PlateDtoTests
    {
        [Fact]
        public void Constructor_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();
            var registration = "ABC123";
            var salePrice = 1000m;
            var status = (int)PlateStatus.ForSale;

            // Act
            var dto = new PlateDto
            {
                Id = id,
                Registration = registration,
                SalePrice = salePrice,
                Status = status
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(registration, dto.Registration);
            Assert.Equal(salePrice, dto.SalePrice);
            Assert.Equal(status, dto.Status);
        }

        [Fact]
        public void Properties_ShouldMatchPlateEntity()
        {
            // Arrange
            var plate = new Plate
            {
                Id = Guid.NewGuid(),
                Registration = "ABC123",
                SalePrice = 1000m,
                Status = PlateStatus.ForSale
            };

            // Act
            var dto = new PlateDto
            {
                Id = plate.Id,
                Registration = plate.Registration,
                SalePrice = plate.SalePrice,
                Status = (int)plate.Status
            };

            // Assert
            Assert.Equal(plate.Id, dto.Id);
            Assert.Equal(plate.Registration, dto.Registration);
            Assert.Equal(plate.SalePrice, dto.SalePrice);
            Assert.Equal((int)plate.Status, dto.Status);
        }
    }
}