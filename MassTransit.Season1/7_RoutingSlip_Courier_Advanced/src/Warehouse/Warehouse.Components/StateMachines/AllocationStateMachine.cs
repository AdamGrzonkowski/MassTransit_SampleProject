using Automatonymous;
using MassTransit;
using Warehouse.Contracts;

namespace Warehouse.Components.StateMachines
{
    public class AllocationStateMachine :
        MassTransitStateMachine<AllocationState>
    {
        public AllocationStateMachine()
        {
            Event(() => AllocationCreated, x => x.CorrelateById(m => m.Message.AllocationId));
            Event(() => ReleaseRequested, x => x.CorrelateById(m => m.Message.AllocationId));

            Schedule(() => HoldExpiration, x => x.HoldDurationToken, s =>
            {
                // specify how scheduled message should be correlated to the Saga instance once it's received
                s.Received = x => x.CorrelateById(m => m.Message.AllocationId);
            });

            InstanceState(x => x.CurrentState);

            Initially(
                When(AllocationCreated)
                    .Schedule(
                        HoldExpiration, // specifies which Schedule object we're allocating / executing
                        context => context.Init<AllocationHoldDurationExpired>(new { context.Data.AllocationId }), // initialize the scheduled message which is gonna be sent
                        context => context.Data.HoldDuration) // pass duration, after which, scheduled message is gonna be sent
                    .TransitionTo(Allocated),
                When(ReleaseRequested)
                    .TransitionTo(Released));

            /* This handles case when Saga was already saved (committed) to Saga Repository, but RabbitMQ went down
             * in such case, RabbitMQ will act as it should and will try to re-deliver messages as soon as it's up and running again
             * but in Saga Repository there will already be info that Saga already is in 'Allocated' state (because it was already committed)
             * to handle such case, we need to do the behavior When(AllocationCreated) without the state TransitionTo(Allocated) again
             * */
            During(Allocated,
                When(AllocationCreated)
                    .Schedule(
                        HoldExpiration, // specifies which Schedule object we're allocating / executing
                        context => context.Init<AllocationHoldDurationExpired>(new { context.Data.AllocationId }), // initialize the scheduled message which is gonna be sent
                        context => context.Data.HoldDuration) // pass duration, after which, scheduled message is gonna be sent
            );

            During(Released,
                When(AllocationCreated)
                    .Finalize()); // if we're already in Released state, simply finalzie

            During(Allocated,
                When(HoldExpiration.Received)
                    .Finalize(), // this puts Saga in Final state, meaning all processes related to it, have finished
                When(ReleaseRequested)
                    .Unschedule(HoldExpiration) // since allocation is released, we no longer need to check its HoldExpiration time
                    .Finalize());
            // this tells Saga Repository to delete finalized Sagas
            SetCompletedWhenFinalized();
        }

        public State? Allocated { get; private set; }
        public State? Released { get; private set; }

        public Event<AllocationCreated>? AllocationCreated { get; private set; }
        public Event<AllocationReleaseRequested>? ReleaseRequested { get; private set; }

        public Schedule<AllocationState, AllocationHoldDurationExpired>? HoldExpiration { get; private set; }
    }
}