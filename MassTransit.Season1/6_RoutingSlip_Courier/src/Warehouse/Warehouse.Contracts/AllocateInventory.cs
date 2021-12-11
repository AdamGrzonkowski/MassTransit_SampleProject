namespace Warehouse.Contracts
{
    public record AllocateInventory
    {
        public Guid AllocationId { get; init; }
        public string ItemNumber { get; init; } = string.Empty;
        public decimal Quantity { get; init; }
    }
}