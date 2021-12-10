namespace Sample.Contracts.Requests
{
    public record GetOrderStatusRequest
    {
        public Guid OrderId { get; init; }
    }
}