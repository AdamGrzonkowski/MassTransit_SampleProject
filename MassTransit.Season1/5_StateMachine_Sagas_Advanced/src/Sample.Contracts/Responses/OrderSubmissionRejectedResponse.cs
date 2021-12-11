namespace Sample.Contracts.Responses
{
    public record OrderSubmissionRejectedResponse
    {
        public Guid OrderId { get; init; } = Guid.Empty;
        public DateTimeOffset Timestamp { get; init; }
        public string CustomerNumber { get; init; } = string.Empty;
        public string Reason { get; init; } = string.Empty;
    }
}