using Automatonymous;
using MassTransit;
using Microsoft.Extensions.Logging;
using Warehouse.Contracts;

namespace Warehouse.Components.StateMachines
{
    public class AllocationStateMachine :
        MassTransitStateMachine<AllocationState>
    {
        public AllocationStateMachine()
        {
            Event(() => AllocationCreated, x => x.CorrelateById(m => m.Message.AllocationId));

            Schedule(() => HoldExpiration, x => x.HoldDurationToken, s =>
            {
                s.Delay = TimeSpan.FromHours(1);

                // specify how receive message should be correlated to Saga instance
                s.Received = x => x.CorrelateById(m => m.Message.AllocationId);
            });

            InstanceState(x => x.CurrentState);

            Initially(
                When(AllocationCreated)
                    // initialize the message which is gonna be sent
                    .Schedule(HoldExpiration, context => context.Init<AllocationHoldDurationExpired>(new { context.Data.AllocationId }),
                        context => context.Data.HoldDuration) // pass duration specified in the message
                    .TransitionTo(Allocated));

            During(Allocated,
                When(HoldExpiration.Received)
                .Finalize()); // this puts Saga in Final state, meaning all processes related to it, have finished

            // this tells Saga Repository to delete finalized Sagas
            SetCompletedWhenFinalized();
        }

        public State? Allocated { get; set; }
        public State? Released { get; set; }

        public Event<AllocationCreated>? AllocationCreated { get; set; }

        public Schedule<AllocationState, AllocationHoldDurationExpired>? HoldExpiration { get; set; }
    }
}