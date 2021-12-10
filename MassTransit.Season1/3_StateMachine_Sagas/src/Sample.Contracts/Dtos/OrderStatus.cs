namespace Sample.Contracts.Dtos
{
    public record OrderStatus
    {
        public Guid OrderId { get; init; }
        public string? State { get; init; }
    }
}