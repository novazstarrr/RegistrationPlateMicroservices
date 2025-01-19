using Xunit;
using Moq;
using MassTransit;
using Microsoft.Extensions.Logging;
using Catalog.API.Consumers;
using Catalog.API.Data.Interface;
using Catalog.Domain;
using IntegrationEvents;
using System;
using System.Threading.Tasks;

namespace Catalog.UnitTests.Consumers
{
    public class PlateReservationEventConsumerTests
    {
        private readonly Mock<ILogger<PlateReservationEventConsumer>> _mockLogger;
        private readonly Mock<IAuditLogRepository> _mockAuditLogRepository;
        private readonly PlateReservationEventConsumer _consumer;
        private readonly Mock<ConsumeContext<PlateReservationEvent>> _mockConsumeContext;

        public PlateReservationEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<PlateReservationEventConsumer>>();
            _mockAuditLogRepository = new Mock<IAuditLogRepository>();
            _mockConsumeContext = new Mock<ConsumeContext<PlateReservationEvent>>();
            _consumer = new PlateReservationEventConsumer(
                _mockLogger.Object,
                _mockAuditLogRepository.Object);
        }

        [Fact]
        public async Task Consume_WhenReservationReceived_CreatesAuditLog()
        {
            // Arrange
            var plateId = Guid.NewGuid();
            var message = new PlateReservationEvent(
                plateId: plateId,
                registration: "ABC123",
                isReserved: true
            );

            _mockConsumeContext
                .Setup(x => x.Message)
                .Returns(message);

            // Act
            await _consumer.Consume(_mockConsumeContext.Object);

            // Assert
            _mockAuditLogRepository.Verify(
                r => r.AddAsync(It.Is<AuditLog>(log =>
                    log.PlateId == plateId &&
                    log.Registration == "ABC123" &&
                    log.Action == "Reserved")),
                Times.Once);
        }

        [Fact]
        public async Task Consume_WhenUnreservationReceived_CreatesAuditLog()
        {
            // Arrange
            var plateId = Guid.NewGuid();
            var message = new PlateReservationEvent(
                plateId: plateId,
                registration: "ABC123",
                isReserved: false
            );

            _mockConsumeContext
                .Setup(x => x.Message)
                .Returns(message);

            // Act
            await _consumer.Consume(_mockConsumeContext.Object);

            // Assert
            _mockAuditLogRepository.Verify(
                r => r.AddAsync(It.Is<AuditLog>(log =>
                    log.PlateId == plateId &&
                    log.Registration == "ABC123" &&
                    log.Action == "Unreserved")),
                Times.Once);
        }

        [Fact]
        public async Task Consume_WhenRegistrationIsNull_UsesEmptyString()
        {
            // Arrange
            var plateId = Guid.NewGuid();
            var message = new PlateReservationEvent(
                plateId: plateId,
                registration: null,
                isReserved: true
            );

            _mockConsumeContext
                .Setup(x => x.Message)
                .Returns(message);

            // Act
            await _consumer.Consume(_mockConsumeContext.Object);

            // Assert
            _mockAuditLogRepository.Verify(
                r => r.AddAsync(It.Is<AuditLog>(log =>
                    log.PlateId == plateId &&
                    log.Registration == string.Empty &&
                    log.Action == "Reserved")),
                Times.Once);
        }
    }
}