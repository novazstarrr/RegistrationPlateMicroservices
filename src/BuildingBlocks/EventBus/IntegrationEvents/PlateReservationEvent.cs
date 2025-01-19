namespace IntegrationEvents
{
    public class PlateReservationEvent
    {
        public Guid PlateId { get; set; }
        public string? Registration { get; set; }
        public bool IsReserved { get; set; }
        public DateTime Timestamp { get; set; }

        public PlateReservationEvent(Guid plateId, string? registration, bool isReserved)
        {
            PlateId = plateId;
            Registration = registration;
            IsReserved = isReserved;
            Timestamp = DateTime.UtcNow;
        }

        
        public PlateReservationEvent() { }
    }
}