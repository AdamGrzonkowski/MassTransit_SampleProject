namespace Company.StateMachines
{
    using GreenPipes;
    using MassTransit;
    using MassTransit.Definition;

    public class OrderProcessingStateSagaDefinition :
        SagaDefinition<OrderProcessingState>
    {
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderProcessingState> sagaConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}