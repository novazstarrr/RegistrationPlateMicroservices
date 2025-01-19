using Xunit;
using Catalog.API.DTOs.Requests;
using Catalog.Domain;

namespace Catalog.UnitTests.DTOs.Requests
{
    public class UpdatePlateStatusDtoTests
    {
        [Theory]
        [InlineData(PlateStatus.ForSale)]
        [InlineData(PlateStatus.Reserved)]
        [InlineData(PlateStatus.Sold)]
        public void Status_ShouldAcceptValidValues(PlateStatus status)
        {
            // Arrange & Act
            var dto = new UpdatePlateStatusDto
            {
                Status = (int)status
            };

            // Assert
            Assert.Equal((int)status, dto.Status);
        }

        [Fact]
        public void Status_ShouldDefaultToAvailable()
        {
            // Arrange & Act
            var dto = new UpdatePlateStatusDto();

            // Assert
            Assert.Equal((int)PlateStatus.ForSale, dto.Status);
        }
    }
}