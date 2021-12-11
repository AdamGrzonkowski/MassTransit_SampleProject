namespace Sample.Contracts.Responses
{
    public record OrderSubmissionAcceptedResponse
    {
        public Guid OrderId { get; init; } = Guid.Empty;
        public DateTimeOffset Timestamp { get; init; }
        public string CustomerNumber { get; init; } = string.Empty;
    }
}