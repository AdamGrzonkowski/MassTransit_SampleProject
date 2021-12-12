namespace Warehouse.Contracts
{
    public record AllocationReleaseRequested
    {
        public Guid AllocationId { get; init; }
        public string Reason { get; init; } = string.Empty;
    }
}