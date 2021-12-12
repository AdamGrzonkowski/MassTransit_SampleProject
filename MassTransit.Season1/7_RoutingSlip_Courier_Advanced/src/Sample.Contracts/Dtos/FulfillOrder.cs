namespace Sample.Contracts.Dtos
{
    public record FulfillOrder
    {
        public Guid OrderId { get; init; }
        public string CustomerNumber { get; init; } = string.Empty;
        public string PaymentCardNumber { get; init; } = string.Empty;
    }
}