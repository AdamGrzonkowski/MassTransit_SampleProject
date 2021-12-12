namespace Sample.Contracts.Dtos
{
    public record OrderNotFound
    {
        public Guid OrderId { get; init; }
    }
}