using Catalog.Domain;

namespace IntegrationEvents
{
    public record PlateStatusChangedEvent(
        Guid PlateId,
        string Registration,
        PlateStatus Status
    );
}