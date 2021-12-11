using Automatonymous;
using MassTransit;
using MassTransit.Saga;
using Sample.Contracts.Dtos;
using Sample.Contracts.Events;

namespace Sample.Components.StateMachines
{
    /// <remarks>
    /// State machines are created with two implicit states:
    /// 1. Initial - new instance is created for every consumed initial event where an existing instance with the same CorrelationId was not found
    /// 2. Final - when existing instance of state machine is in this state, you can remove it from saga repository.
    ///
    /// A saga repository is used to persist instances.
    /// Instances are classes, and must implement the SagaStateMachineInstance interface
    /// </remarks>
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            // This specifies that OrderId in OrderSubmitted event is used to correlate to the saga instance
            // if saga instance does not exist yet in saga repository, MassTransit will create it
            Event(() => OrderSubmitted, x => x.CorrelateById(x => x.Message.OrderId));
            Event(() => OrderStatusRequested, x =>
            {
                x.CorrelateById(x => x.Message.OrderId);
                // below is an example of an inline message consumer
                x.OnMissingInstance(x => x.ExecuteAsync(async context =>
                {
                    if (context.RequestId.HasValue)
                    {
                        await context.RespondAsync(new OrderNotFound
                        {
                            OrderId = context.Message.OrderId
                        });
                    }
                }));
            });

            // define which property will actually hold the state information
            InstanceState(x => x.CurrentState);

            // define state machine
            Initially(
                When(OrderSubmitted)
                    .Then(context =>
                    {
                        context.Instance.SubmitDate = context.Data.Timestamp; // store datetime when Order was submitted
                        context.Instance.CustomerNumber = context.Data.CustomerNumber;
                        context.Instance.Updated = DateTimeOffset.UtcNow; // store datetime of state's update
                    })
                    .TransitionTo(Submitted));

            During(Submitted,
                Ignore(OrderSubmitted)); // this ensures idempotency; ensures that OrderSubmitted event is not processed again for the same State, without raising an exception

            /* By default, MassTransit should throw exception when we try to process same State for the given Instance again, but there's a way to do it cleaner:
             * DuringAny never includes Initial or Final states. It executes however for any other states that those two.
             * Code below, after When(OrderSubmitted), ensures that if some later state, like OrderAccepted (which comes after OrderSubmitted) is actually
             * processed first (because in truly distributed systems we can't guarantee order of execution, so we must take such scenario into consideration)
             * we'll take some useful data, which is unique for OrderSubmitted state (and therefore it's not yet included in Saga Instance), like "SubmitDate" and store it in Saga instance, but without actually changing the State
             * from OrderAccepted back to OrderSubmitted, because that makes no sense;
             * State Machine is describing actions taking place in time. Since OrderAccepted happens after OrderSubmitted, it makes no logical sense to transition to earlier State
            */
            DuringAny(
                When(OrderSubmitted)
                    .Then(context =>
                    {
                        context.Instance.SubmitDate ??= context.Data.Timestamp; // copy SubmitDate, in case we don't have it
                        context.Instance.CustomerNumber ??= context.Data.CustomerNumber; // copy CustomerNumber, in case we don't have it
                    })
                );

            DuringAny(
                When(OrderStatusRequested)
                    // instead of using dedicated IConsumer, like in RequestReply (Mediator) pattern, in State Machine we can specify responses like this:
                    .RespondAsync(x => x.Init<OrderStatus>(new
                    {
                        OrderId = x.Instance.CorrelationId, // we specified that OrderId is correlationId for this machine state, hence why we can use it like this
                        State = x.Instance.CurrentState
                    }))
                );
        }

        public State? Submitted { get; private set; }

        public Event<OrderSubmitted>? OrderSubmitted { get; private set; }
        public Event<OrderStatus>? OrderStatusRequested { get; private set; }
    }
}