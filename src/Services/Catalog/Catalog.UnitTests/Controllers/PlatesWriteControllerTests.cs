using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Catalog.API.Controllers;
using Catalog.API.Services;
using Catalog.Domain;
using Catalog.API.DTOs.Requests;
using Catalog.API.DTOs.Responses;
using Catalog.API.Exceptions;
using MassTransit;
using IntegrationEvents;
using System;
using System.Threading.Tasks;

namespace Catalog.UnitTests.Controllers
{
    public class PlatesWriteControllerTests
    {
        private readonly Mock<IPlateService> _mockPlateService;
        private readonly Mock<ILogger<PlatesWriteController>> _mockLogger;
        private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
        private readonly PlatesWriteController _controller;

        public PlatesWriteControllerTests()
        {
            _mockPlateService = new Mock<IPlateService>();
            _mockLogger = new Mock<ILogger<PlatesWriteController>>();
            _mockPublishEndpoint = new Mock<IPublishEndpoint>();
            _controller = new PlatesWriteController(
                _mockPlateService.Object,
                _mockLogger.Object,
                _mockPublishEndpoint.Object);
        }

        [Fact]
        public async Task CreatePlate_ReturnsCreatedAtAction()
        {
            // Arrange
            var createDto = new CreatePlateDto
            {
                Registration = "ABC123",
                SalePrice = 1000m
            };
            var plate = new PlateDto
            {
                Id = Guid.NewGuid(),
                Registration = "ABC123",
                Status = (int)PlateStatus.ForSale,
                SalePrice = 1000m
            };

            _mockPlateService
                .Setup(s => s.CreatePlateAsync(createDto))
                .Returns(Task.FromResult(plate));

            // Act
            var result = await _controller.CreatePlate(createDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(PlatesReadController.GetPlate), createdAtActionResult.ActionName);
            Assert.Equal("PlatesRead", createdAtActionResult.ControllerName);
            var returnValue = Assert.IsType<PlateDto>(createdAtActionResult.Value);
            Assert.Equal(plate.Id, returnValue.Id);
        }

        [Fact]
        public async Task UpdateStatus_WhenPlateExists_ReturnsOkResult()
        {
            // Arrange
            var plateId = Guid.NewGuid();
            var updateDto = new UpdatePlateStatusDto { Status = (int)PlateStatus.Reserved };
            var plate = new PlateDto
            {
                Id = plateId,
                Registration = "ABC123",
                Status = (int)PlateStatus.Reserved,
                SalePrice = 1000m
            };

            _mockPlateService
                .Setup(s => s.UpdateStatusAsync(plateId, PlateStatus.Reserved))
                .Returns(Task.FromResult(plate));

            // Act
            var result = await _controller.UpdateStatus(plateId, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPlate = Assert.IsType<PlateDto>(okResult.Value);
            Assert.Equal(plateId, returnedPlate.Id);
            Assert.Equal((int)PlateStatus.Reserved, returnedPlate.Status);

            // Verify event was published for Reserved status
            _mockPublishEndpoint.Verify(p => p.Publish(
                It.Is<PlateReservationEvent>(e =>
                    e.PlateId == plateId &&
                    e.Registration == plate.Registration &&
                    e.IsReserved == true),
                default),
                Times.Once);
        }

        [Fact]
        public async Task UpdateStatus_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            var plateId = Guid.NewGuid();
            var updateDto = new UpdatePlateStatusDto { Status = (int)PlateStatus.Reserved };

            _mockPlateService
                .Setup(s => s.UpdateStatusAsync(plateId, PlateStatus.Reserved))
                .Returns(Task.FromResult<PlateDto>(null!));

            // Act
            var result = await _controller.UpdateStatus(plateId, updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
            _mockPublishEndpoint.Verify(p => p.Publish(It.IsAny<PlateReservationEvent>(), default), Times.Never);
        }

        [Fact]
        public async Task UpdateStatus_WhenNotReserved_DoesNotPublishEvent()
        {
            // Arrange
            var plateId = Guid.NewGuid();
            var updateDto = new UpdatePlateStatusDto { Status = (int)PlateStatus.Sold };
            var plate = new PlateDto
            {
                Id = plateId,
                Registration = "ABC123",
                Status = (int)PlateStatus.Sold,
                SalePrice = 1000m
            };

            _mockPlateService
                .Setup(s => s.UpdateStatusAsync(plateId, PlateStatus.Sold))
                .Returns(Task.FromResult(plate));

            // Act
            var result = await _controller.UpdateStatus(plateId, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            _mockPublishEndpoint.Verify(p => p.Publish(It.IsAny<PlateReservationEvent>(), default), Times.Never);
        }
    }
}