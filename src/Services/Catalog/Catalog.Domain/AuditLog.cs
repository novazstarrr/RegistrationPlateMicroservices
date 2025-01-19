using System;

namespace Catalog.Domain
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid PlateId { get; set; }
        public string? Registration { get; set; }
        public string? Action { get; set; }
        public DateTime Timestamp { get; set; }

        public AuditLog(Guid plateId, string registration, string action)
        {
            Id = Guid.NewGuid();
            PlateId = plateId;
            Registration = registration ?? throw new ArgumentNullException(nameof(registration));
            Action = action ?? throw new ArgumentNullException(nameof(action));
            Timestamp = DateTime.UtcNow;
        }

        protected AuditLog() { }
    }
}