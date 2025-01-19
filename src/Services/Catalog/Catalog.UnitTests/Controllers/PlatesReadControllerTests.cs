using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Catalog.API.Controllers;
using Catalog.API.Services;
using Catalog.Domain;
using Catalog.API.DTOs.Common;
using Catalog.API.DTOs.Responses;
using AutoMapper;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Catalog.UnitTests.Controllers
{
    public class PlatesReadControllerTests
    {
        private readonly Mock<IPlateService> _mockPlateService;
        private readonly Mock<ILogger<PlatesReadController>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PlatesReadController _controller;

        public PlatesReadControllerTests()
        {
            _mockPlateService = new Mock<IPlateService>();
            _mockLogger = new Mock<ILogger<PlatesReadController>>();
            _mockMapper = new Mock<IMapper>();
            _controller = new PlatesReadController(
                _mockPlateService.Object,
                _mockLogger.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task GetPlates_ReturnsOkResult()
        {
            // Arrange
            var plates = new List<PlateDto>
            {
                new PlateDto
                {
                    Id = Guid.NewGuid(),
                    Registration = "ABC123",
                    Status = (int)PlateStatus.ForSale,
                    SalePrice = 1000m,
                    PurchasePrice = 500m,
                    Letters = "ABC",
                    Numbers = 123,
                    StatusDisplay = "For Sale"
                },
                new PlateDto
                {
                    Id = Guid.NewGuid(),
                    Registration = "DEF456",
                    Status = (int)PlateStatus.ForSale,
                    SalePrice = 2000m,
                    PurchasePrice = 1000m,
                    Letters = "DEF",
                    Numbers = 456,
                    StatusDisplay = "For Sale"
                }
            };

            var pagedResult = new PagedListDto<PlateDto>(plates, 2, 1, 20, "asc");

            _mockPlateService
                .Setup(s => s.GetPlatesAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(pagedResult));

            // Act
            var result = await _controller.GetPlates();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<PagedListDto<PlateDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Items.Count);
            Assert.Equal(2, returnValue.TotalCount);
        }

        [Fact]
        public async Task GetPlate_WhenExists_ReturnsOkResult()
        {
            // Arrange
            var plateId = Guid.NewGuid();
            var plate = new PlateDto
            {
                Id = plateId,
                Registration = "ABC123",
                Status = (int)PlateStatus.ForSale,
                SalePrice = 1000m,
                PurchasePrice = 500m,
                Letters = "ABC",
                Numbers = 123,
                StatusDisplay = "For Sale"
            };

            _mockPlateService
                .Setup(s => s.GetByIdAsync(plateId))
                .Returns(Task.FromResult(plate));

            // Act
            var result = await _controller.GetPlate(plateId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<PlateDto>(okResult.Value);
            Assert.Equal(plateId, returnValue.Id);
        }

        [Fact]
        public async Task GetPlate_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            var plateId = Guid.NewGuid();

            _mockPlateService
                .Setup(s => s.GetByIdAsync(plateId))
                .Returns(Task.FromResult<PlateDto>(null!));

            // Act
            var result = await _controller.GetPlate(plateId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Theory]
        [InlineData(1, 20, 0, 0, "asc", null)]
        [InlineData(2, 10, 1000, 2000, "desc", "ABC")]
        public async Task GetPlates_WithParameters_PassesCorrectValuesToService(
            int pageNumber, int pageSize, decimal minPrice, decimal maxPrice,
            string sortOrder, string? nameMatch)
        {
            // Arrange
            var pagedResult = new PagedListDto<PlateDto>(
                new List<PlateDto>(),
                0,
                pageNumber,
                pageSize,
                sortOrder);

            _mockPlateService
                .Setup(s => s.GetPlatesAsync(
                    pageNumber,
                    pageSize,
                    minPrice,
                    maxPrice,
                    sortOrder,
                    nameMatch))
                .Returns(Task.FromResult(pagedResult))
                .Verifiable();

            // Act
            await _controller.GetPlates(pageNumber, pageSize, minPrice, maxPrice, sortOrder, nameMatch);

            // Assert
            _mockPlateService.Verify();
        }
    }
}