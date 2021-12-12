namespace Sample.Contracts.Events
{
    public record OrderAccepted
    {
        public Guid OrderId { get; init; }
        public DateTimeOffset Timestamp { get; init; }
    }
}