using MassTransit;
using MassTransit.Courier;
using Sample.Contracts.Dtos;

namespace Sample.Components.Consumers
{
    /// <summary>
    /// Executes RoutingSlip.
    /// </summary>
    public class FulfillOrderConsumer :
        IConsumer<FulfillOrder>
    {
        public async Task Consume(ConsumeContext<FulfillOrder> context)
        {
            // every RoutingSlip, just like every package in real world, must have unique tracking number, so we pass it in the constructor
            var builder = new RoutingSlipBuilder(NewId.NextGuid());

            // look up activity with below name and send it directly to the specified endpoint
            builder.AddActivity("AllocateInventory", new Uri("queue:allocate-inventory_execute"), new
            {
                ItemNumber = "Item123",
                Quantity = 10.0
            });

            // this is alternative approach to pass Input parameters to Courier's activity, instead of using Arguments class
            // it's recommended way to pass variables which are shared between multiple activities, as it reduces duplicating code
            builder.AddVariable("OrderId", context.Message.OrderId);

            var routingSlip = builder.Build();

            // trigger execution of RoutingSlip
            // activities are performed in order, meaning first added activity will execute first
            await context.Execute(routingSlip);
        }
    }
}