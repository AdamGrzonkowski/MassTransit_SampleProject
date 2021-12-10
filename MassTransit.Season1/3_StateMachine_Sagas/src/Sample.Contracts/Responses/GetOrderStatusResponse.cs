namespace Sample.Contracts.Responses
{
    public record GetOrderStatusResponse
    {
        public Guid? OrderId { get; init; }
        public string? State { get; init; }
    }
}