using GreenPipes;
using MassTransit;
using MassTransit.Definition;

namespace Sample.Components.StateMachines
{
    public class OrderStateMachineDefinition
        : SagaDefinition<OrderState>
    {
        public OrderStateMachineDefinition()
        {
            /* By specifying a concurrent message limit, MassTransit limits the number of messages delivered to a consumer at the same time.
             * At the same time, since a consumer factory is used to create consumers, it also limits the number of concurrent consumers that exist at the same time
            */
            ConcurrentMessageLimit = 4;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
        {
            // define Retry pattern
            endpointConfigurator.UseMessageRetry(x => x.Intervals(500, 5000, 10000));

            // holds published and sent messages in memory until the message is processed successfully (such as the saga being saved to the database).
            // in other words: this feature ensures that the transaction with updates to Saga state only commits when all the messages that were sent or published from the consumer context are actually sent to the message bus
            // if the transport fails to send or publish messages, there will be no commit to saga repository, so the transaction will rollback - data integrity is preserved that way
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}