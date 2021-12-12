using Automatonymous;
using GreenPipes;
using MassTransit;
using Sample.Contracts.Dtos;
using Sample.Contracts.Events;

namespace Sample.Components.StateMachines.OrderStateMachineActivities
{
    /// <summary>
    /// Define actions to be taken when Order Saga changes state to <see cref="OrderAccepted"/>.
    /// </summary>
    /// <remarks>
    /// State Machine definition (defined in <see cref="OrderStateMachine"/>) should not have any dependencies - it should just describe the process.
    /// If you need to add any dependencies, like explicit database calls - that's what the activites are for. You add them in such cases
    /// and define some more complex logic in them, to keep State Machine definition as clean as possible.
    ///
    /// Important! Look out for MassTransit's naming. 'Activity' is related to State Machine (aka Sagas). 'IActivity' is related to Routing Slip (aka Courier).
    /// </remarks>
    public class AcceptOrderActivity :
        Activity<OrderState, OrderAccepted>
    {
        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <inheritdoc />
        public async Task Execute(BehaviorContext<OrderState, OrderAccepted> context, Behavior<OrderState, OrderAccepted> next)
        {
            Console.WriteLine("Hello, World. Order is {0}", context.Data.OrderId); // just for test purposes; usually you would put some logic here

            var consumeContext = context.GetPayload<ConsumeContext>();

            var sendEndpoint = await consumeContext.GetSendEndpoint(new Uri("exchange:fulfill-order"));

            await sendEndpoint.Send(new FulfillOrder
            {
                OrderId = context.Data.OrderId
            });

            await next.Execute(context);
        }

        /// <inheritdoc />
        public Task Faulted<TException>(BehaviorExceptionContext<OrderState, OrderAccepted, TException> context, Behavior<OrderState, OrderAccepted> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("accept-order");
        }
    }
}