namespace Sample.Contracts.Events
{
    public record CustomerAccountClosed
    {
        public Guid CustomerId { get; init; } = Guid.Empty;
        public DateTimeOffset Timestamp { get; init; }
        public string CustomerNumber { get; init; } = string.Empty;
    }
}