namespace Sample.Components.CourierActivities
{
    /// <summary>
    /// Represents input parameters to be passed during payment activity.
    /// </summary>
    public record PaymentArguments
    {
        /// <summary>
        /// Id of Order for which this payment is being processed.
        /// </summary>
        public Guid OrderId { get; init; }

        /// <summary>
        /// Total amount to be paid.
        /// </summary>
        public decimal Amount { get; init; } = 0;

        /// <summary>
        /// Number of credit card from which <see cref="Amount"/> should be taken.
        /// </summary>
        public string CardNumber { get; init; } = string.Empty;
    }
}