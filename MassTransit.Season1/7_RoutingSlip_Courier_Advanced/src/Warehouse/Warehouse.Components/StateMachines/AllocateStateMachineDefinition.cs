using GreenPipes;
using MassTransit.Definition;
using MassTransit;

namespace Warehouse.Components.StateMachines
{
    public class AllocateStateMachineDefinition :
        SagaDefinition<AllocationState>
    {
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<AllocationState> sagaConfigurator)
        {
            // define Retry pattern
            endpointConfigurator.UseMessageRetry(x => x.Intervals(3, 1000));

            // holds published and sent messages in memory until the message is processed successfully (such as the saga being saved to the database)
            // if saving Saga to Saga Repository fails for some reason, messages will not be sent
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}