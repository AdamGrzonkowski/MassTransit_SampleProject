namespace Sample.Components.CourierActivities
{
    /// <remarks>
    /// Represents a log class, used by the Courier to compensate (rollback) changes made, in case of failure.
    /// This class should have enough info to compensate all changes made during activity's Execute method.
    /// </remarks>
    public record AllocateInventoryLog
    {
        public Guid AllocationId { get; init; }
    }
}