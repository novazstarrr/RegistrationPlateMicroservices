using MassTransit;
using Microsoft.Extensions.Logging;
using IntegrationEvents;
using Catalog.Domain;
using Catalog.API.Data.Interface;

namespace Catalog.API.Consumers
{
    public class PlateReservationEventConsumer : IConsumer<PlateReservationEvent>
    {
        private readonly ILogger<PlateReservationEventConsumer> _logger;
        private readonly IAuditLogRepository _auditLogRepository;

        public PlateReservationEventConsumer(
            ILogger<PlateReservationEventConsumer> logger,
            IAuditLogRepository auditLogRepository)
        {
            _logger = logger;
            _auditLogRepository = auditLogRepository;
        }

        public async Task Consume(ConsumeContext<PlateReservationEvent> context)
        {
            var evt = context.Message;

            _logger.LogInformation(
                "Plate {Registration} (ID: {PlateId}) was {Action} at {Timestamp}",
                evt.Registration,
                evt.PlateId,
                evt.IsReserved ? "reserved" : "unreserved",
                evt.Timestamp);

            var auditLog = new AuditLog(
                plateId: evt.PlateId,
                registration: evt.Registration ?? string.Empty,
                action: evt.IsReserved ? "Reserved" : "Unreserved"
            );

            await _auditLogRepository.AddAsync(auditLog);
        }
    }
}