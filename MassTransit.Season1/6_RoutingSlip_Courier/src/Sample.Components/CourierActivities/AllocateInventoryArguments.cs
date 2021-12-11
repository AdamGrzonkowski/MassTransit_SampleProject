namespace Sample.Components.CourierActivities
{
    /// <remarks>
    /// Represents arguments which Courier's Activity receives as input.
    /// </remarks>
    public record AllocateInventoryArguments
    {
        public Guid OrderId { get; init; }
        public string ItemNumber { get; init; } = string.Empty;
        public decimal Quantity { get; init; }
    }
}