namespace Sample.Contracts.Dtos
{
    public record OrderFulfillmentFaulted
    {
        public Guid OrderId { get; init; }

        public DateTimeOffset? Timestamp { get; init; }
    }
}