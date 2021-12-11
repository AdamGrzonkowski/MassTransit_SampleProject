namespace Warehouse.Contracts.Events
{
    public record InventoryAllocated
    {
        public Guid AllocationId { get; init; }
        public string ItemNumber { get; init; } = string.Empty;
        public decimal Quantity { get; init; }
    }
}