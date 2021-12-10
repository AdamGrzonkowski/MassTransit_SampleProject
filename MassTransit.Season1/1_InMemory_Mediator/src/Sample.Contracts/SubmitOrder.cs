namespace Sample.Contracts
{
    public record SubmitOrder
    {
        public string OrderId { get; init; } = string.Empty;
        public DateTimeOffset Timestamp { get; init; }
        public string CustomerNumber { get; init; } = string.Empty;
    }
}