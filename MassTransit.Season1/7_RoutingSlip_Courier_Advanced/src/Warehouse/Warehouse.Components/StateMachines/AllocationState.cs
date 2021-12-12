using Automatonymous;
using MassTransit.Saga;

namespace Warehouse.Components.StateMachines
{
    public record AllocationState :
        SagaStateMachineInstance,
        ISagaVersion
    {
        /// <inheritdoc />
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Current state of the saga.
        /// </summary>
        /// <remarks>
        /// MassTransit requires that property which stores CurrentState is nullable.
        /// </remarks>
        public string? CurrentState { get; set; }

        /// <summary>
        /// Unique TokenId used to cancel scheduled message.
        /// </summary>
        public Guid? HoldDurationToken { get; set; }

        /// <summary>
        /// Needed for saga repositories that use an incrementing saga version, i.e. Optimistic Concurrency.
        /// </summary>
        public int Version { get; set; } = 1;
    }
}