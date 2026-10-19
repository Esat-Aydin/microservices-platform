namespace EventBus.Messages.Events
{
    public class BaseIntegrationEvent
    {
        public Guid CorrelationId { get; set; }
        public DateTime CreationDate { get; set; }
        public BaseIntegrationEvent()
        {
            this.CorrelationId = Guid.NewGuid();
            this.CreationDate = DateTime.UtcNow;
        }

        public BaseIntegrationEvent(Guid correlationId, DateTime creationDate)
        {
            this.CorrelationId = correlationId;
            this.CreationDate = creationDate;
        }
    }
}
