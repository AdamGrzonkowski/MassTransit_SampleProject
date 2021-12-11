using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace Sample.Components.Consumers
{
    public class SubmitOrderConsumerDefinition :
        ConsumerDefinition<SubmitOrderConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<SubmitOrderConsumer> consumerConfigurator)
        {
            // define Retry pattern
            endpointConfigurator.UseMessageRetry(x => x.Intervals(3, 1000));
        }
    }
}