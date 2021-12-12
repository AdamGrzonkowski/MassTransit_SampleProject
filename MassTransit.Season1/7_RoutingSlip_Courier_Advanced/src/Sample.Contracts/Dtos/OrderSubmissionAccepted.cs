namespace Sample.Contracts.Dtos
{
    public record OrderSubmissionAccepted
    {
        public Guid OrderId { get; init; } = Guid.Empty;
        public DateTimeOffset Timestamp { get; init; }
        public string CustomerNumber { get; init; } = string.Empty;
    }
}