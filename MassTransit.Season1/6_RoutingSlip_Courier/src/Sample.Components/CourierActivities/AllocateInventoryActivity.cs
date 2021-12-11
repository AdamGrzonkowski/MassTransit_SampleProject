using MassTransit;
using MassTransit.Courier;
using Warehouse.Contracts;
using Warehouse.Contracts.Events;

namespace Sample.Components.CourierActivities
{
    /// <remarks>
    /// Activity performed by Courier. Supports rolling back changes (compensating) in case of failure.
    /// Courier activities are completely stand-alone, they are not coupled with Sagas etc.
    ///
    /// Note: Activities hosted on the endpoints need 2 queues to run:
    /// 1. To run Execute command
    /// 2. To run Compensate command
    ///
    /// these queues are created/maintained automatically by MassTransit
    /// </remarks>
    public class AllocateInventoryActivity :
        IActivity<AllocateInventoryArguments, AllocateInventoryLog>
    {
        private readonly IRequestClient<AllocateInventory> _client;

        public AllocateInventoryActivity(IRequestClient<AllocateInventory> client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public async Task<ExecutionResult> Execute(ExecuteContext<AllocateInventoryArguments> context)
        {
            var orderId = context.Arguments.OrderId;

            var itemNumber = context.Arguments.ItemNumber;
            if (string.IsNullOrEmpty(itemNumber))
            {
                throw new ArgumentNullException(nameof(itemNumber));
            }

            var quantity = context.Arguments.Quantity;
            if (quantity <= 0.0m)
            {
                throw new ArgumentNullException(nameof(quantity));
            }

            var allocationId = NewId.NextGuid();

            var response = await _client.GetResponse<InventoryAllocated>(new
            {
                AllocationId = allocationId,
                ItemNumber = itemNumber,
                Quantity = quantity
            });

            return context.Completed(new AllocateInventoryLog
            {
                AllocationId = allocationId
            });
        }

        /// <inheritdoc />
        public async Task<CompensationResult> Compensate(CompensateContext<AllocateInventoryLog> context)
        {
            await context.Publish<AllocationReleaseRequested>(new
            {
                context.Log.AllocationId,
                Reason = "Order Faulted"
            });

            return context.Compensated();
        }
    }
}