using Automatonymous;
using MassTransit.Saga;

namespace Sample.Components.StateMachines
{
    /// <summary>
    /// Defines data of Order.
    /// </summary>
    public record OrderState :
        SagaStateMachineInstance,
        ISagaVersion
    {
        /// <inheritdoc />
        public Guid CorrelationId { get; set; } = Guid.Empty;

        /// <summary>
        /// Current state of the saga.
        /// </summary>
        /// <remarks>
        /// MassTransit requires that property which stores CurrentState is nullable.
        /// </remarks>
        public string? CurrentState { get; set; }
        public string? CustomerNumber { get; set; }

        public string? PaymentCardNumber { get; set; }

        /// <summary>
        /// Timestamp when Order was submitted.
        /// </summary>
        public DateTimeOffset? SubmitDate { get; set; }

        public DateTimeOffset? Updated { get; set; }

        /// <summary>
        /// Needed for saga repositories that use an incrementing saga version, i.e. Optimistic Concurrency.
        /// </summary>
        public int Version { get; set; } = 1;
    }
}