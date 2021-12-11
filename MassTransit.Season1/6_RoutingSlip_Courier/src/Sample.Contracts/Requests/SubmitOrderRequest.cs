namespace Sample.Contracts.Requests
{
    public record SubmitOrderRequest
    {
        public Guid OrderId { get; init; } = Guid.Empty;
        public string CustomerNumber { get; init; } = string.Empty;
    }
}