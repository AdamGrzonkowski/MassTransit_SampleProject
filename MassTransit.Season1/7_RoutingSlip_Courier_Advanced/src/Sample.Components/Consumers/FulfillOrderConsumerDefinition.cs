using GreenPipes;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using MassTransit;

namespace Sample.Components.Consumers
{
    public class FulfillOrderConsumerDefinition :
        ConsumerDefinition<FulfillOrderConsumer>
    {
        public FulfillOrderConsumerDefinition()
        {
            ConcurrentMessageLimit = 20;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<FulfillOrderConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r =>
            {
                r.Ignore<InvalidOperationException>(); // since this error is not recoverable, do not retry on it - it won't change the result anyway

                r.Interval(3, 1000);
            });

            // do not save faulted messages to error queue; events will still be published to the chosen logging client
            endpointConfigurator.DiscardFaultedMessages();
        }
    }
}