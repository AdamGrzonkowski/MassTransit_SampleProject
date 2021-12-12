using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using Sample.Components.CourierActivities;
using Sample.Contracts.Dtos;

namespace Sample.Components.Consumers
{
    /// <summary>
    /// Executes RoutingSlip, related to fulfilling order.
    /// Triggers allocating items from warehouse, payment process etc.
    /// </summary>
    public class FulfillOrderConsumer :
        IConsumer<FulfillOrder>
    {
        public async Task Consume(ConsumeContext<FulfillOrder> context)
        {
            // every RoutingSlip, just like every package in real world, must have unique tracking number, so we pass it in the constructor
            var builder = new RoutingSlipBuilder(NewId.NextGuid());

            // look up activity with below name and send it directly to the specified endpoint
            // the way MassTransit chooses names for theses queues depends on the EndpointNameFormatter chosen (in this example app KebabCaseEndpointNameFormatter was chosen)
            // this formatter strips word "Activity" from the end, lowercases everything and puts '-' chars between words starting with capital letters, adding '_execute_ suffix at the end
            builder.AddActivity("AllocateInventoryActivity", new Uri("exchange:allocate-inventory_execute"), new AllocateInventoryArguments
            {
                ItemNumber = "Item123",
                Quantity = 10.0m
            });

            builder.AddActivity("PaymentActivity", new Uri("exchange:payment_execute"), new PaymentArguments
            {
                // we check for null/empty here, because this property was added after contract has been published, so it must be optional, to be backward-compatible
                CardNumber = !string.IsNullOrEmpty(context.Message.PaymentCardNumber) ? context.Message.PaymentCardNumber : "5999-1234-5678-9012", // CardNumber starting with '5999' will trigger exception and Compensation mechanism
                Amount = 99.95m
            });

            // this is alternative approach to pass Input parameters to Courier's activity, instead of using Arguments class
            // it's recommended way to pass variables which are shared between multiple activities, as it reduces duplicating code
            builder.AddVariable("OrderId", context.Message.OrderId);

            // subscribes to Routing Slip, which will come back to the source (OrderStateMachine) and deliver message to an endpoint when some error happens
            await builder.AddSubscription(context.SourceAddress,
                 RoutingSlipEvents.Faulted | RoutingSlipEvents.Supplemental,
                 RoutingSlipEventContents.None, x => x.Send<OrderFulfillmentFaulted>(new { context.Message.OrderId }));

            // subscribes to Routing Slip, which will come back to the source (OrderStateMachine) and deliver message to an endpoint when routing slip is completed
            await builder.AddSubscription(context.SourceAddress,
                 RoutingSlipEvents.Completed | RoutingSlipEvents.Supplemental,
                 RoutingSlipEventContents.None, x => x.Send<OrderFulfillmentCompleted>(new { context.Message.OrderId }));

            var routingSlip = builder.Build();

            // trigger execution of RoutingSlip
            // activities are performed in order, meaning first added activity will execute first
            await context.Execute(routingSlip);
        }
    }
}