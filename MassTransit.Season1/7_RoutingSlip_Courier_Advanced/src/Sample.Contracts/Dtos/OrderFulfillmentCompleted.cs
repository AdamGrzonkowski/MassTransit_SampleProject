namespace Sample.Contracts.Dtos
{
    public record OrderFulfillmentCompleted
    {
        public Guid OrderId { get; init; }

        public DateTimeOffset? Timestamp { get; init; }
    }
}