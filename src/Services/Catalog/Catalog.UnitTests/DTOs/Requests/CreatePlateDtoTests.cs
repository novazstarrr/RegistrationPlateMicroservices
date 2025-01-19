using Xunit;
using Catalog.API.DTOs.Requests;

namespace Catalog.UnitTests.DTOs.Requests
{
    public class CreatePlateDtoTests
    {
        [Fact]
        public void Constructor_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var registration = "ABC123";
            var salePrice = 1000m;

            // Act
            var dto = new CreatePlateDto
            {
                Registration = registration,
                SalePrice = salePrice
            };

            // Assert
            Assert.Equal(registration, dto.Registration);
            Assert.Equal(salePrice, dto.SalePrice);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Registration_ShouldAllowNullOrWhitespace(string registration)
        {
            // Arrange
            var dto = new CreatePlateDto
            {
                Registration = registration,
                SalePrice = 1000m
            };

            // Assert
            Assert.Equal(registration, dto.Registration);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public void SalePrice_ShouldAllowNegativeValues(decimal price)
        {
            // Arrange
            var dto = new CreatePlateDto
            {
                Registration = "ABC123",
                SalePrice = price
            };

            // Assert
            Assert.Equal(price, dto.SalePrice);
        }
    }
}