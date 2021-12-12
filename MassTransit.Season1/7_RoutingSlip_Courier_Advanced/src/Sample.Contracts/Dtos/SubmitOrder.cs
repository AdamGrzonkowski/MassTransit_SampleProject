namespace Sample.Contracts.Dtos
{
    public record SubmitOrder
    {
        public Guid OrderId { get; init; } = Guid.Empty;
        public DateTimeOffset Timestamp { get; init; }
        public string CustomerNumber { get; init; } = string.Empty;
        public string PaymentCardNumber { get; init; } = string.Empty;
    }
}