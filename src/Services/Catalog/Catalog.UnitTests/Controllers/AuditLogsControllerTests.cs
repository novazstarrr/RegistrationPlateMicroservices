using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Catalog.API.Controllers;
using Catalog.API.Data.Interface;
using Catalog.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Catalog.UnitTests.Controllers
{
    public class AuditLogsControllerTests
    {
        private readonly Mock<IAuditLogRepository> _mockAuditLogRepository;
        private readonly AuditLogsController _controller;

        public AuditLogsControllerTests()
        {
            _mockAuditLogRepository = new Mock<IAuditLogRepository>();
            _controller = new AuditLogsController(_mockAuditLogRepository.Object);
        }

        [Fact]
        public async Task GetAuditLogs_ReturnsOkResult()
        {
            // Arrange
            var logs = new List<AuditLog>
            {
                new AuditLog(
                    plateId: Guid.NewGuid(),
                    registration: "ABC123",
                    action: "Created"
                ),
                new AuditLog(
                    plateId: Guid.NewGuid(),
                    registration: "DEF456",
                    action: "Updated"
                )
            };

            _mockAuditLogRepository
                .Setup(r => r.GetAllAsync())
                .Returns(Task.FromResult<IEnumerable<AuditLog>>(logs));

            // Act
            var result = await _controller.GetAuditLogs();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<AuditLog>>(okResult.Value);
            Assert.Equal(2, ((List<AuditLog>)returnValue).Count);
        }

        [Fact]
        public async Task GetAuditLogs_WhenEmpty_ReturnsEmptyList()
        {
            // Arrange
            var logs = new List<AuditLog>();

            _mockAuditLogRepository
                .Setup(r => r.GetAllAsync())
                .Returns(Task.FromResult<IEnumerable<AuditLog>>(logs));

            // Act
            var result = await _controller.GetAuditLogs();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<AuditLog>>(okResult.Value);
            Assert.Empty(returnValue);
        }
    }
}