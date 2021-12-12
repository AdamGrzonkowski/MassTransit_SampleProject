using MassTransit.Courier;

namespace Sample.Components.CourierActivities
{
    public class PaymentActivity :
            IActivity<PaymentArguments, PaymentLog>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<PaymentArguments> context)
        {
            string cardNumber = context.Arguments.CardNumber;
            if (string.IsNullOrEmpty(cardNumber))
            {
                throw new ArgumentNullException(nameof(cardNumber));
            }

            // this is added to test Compenstation mechanism (forces exception throw)
            if (cardNumber.StartsWith("5999"))
            {
                throw new InvalidOperationException("The card number was invalid");
            }

            return context.Completed(new { AuthorizationCode = "77777" });
        }

        public async Task<CompensationResult> Compensate(CompensateContext<PaymentLog> context)
        {
            return context.Compensated();
        }
    }
}