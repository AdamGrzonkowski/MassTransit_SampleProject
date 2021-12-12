namespace Warehouse.Contracts
{
    public record AllocationCreated
    {
        public Guid AllocationId { get; init; }

        /// <summary>
        /// Defines for how long allocation is held. After this Timespan, Allocation is released.
        /// </summary>
        public TimeSpan HoldDuration { get; init; }
    }
}