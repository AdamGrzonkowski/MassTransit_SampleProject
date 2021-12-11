using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.Extensions.Logging;

namespace Sample.Components.Consumers
{
    /// <summary>
    /// Consumes event data from Routing Slip.
    /// </summary>
    /// <remarks>
    /// RoutingSlip has built in event publishing. Defining Consumer which listens to these events is considered good practice,
    /// as we can log them.
    /// </remarks>
    public class RoutingSlipEventConsumer :
        IConsumer<RoutingSlipCompleted>,
        IConsumer<RoutingSlipActivityCompleted>,
        IConsumer<RoutingSlipFaulted>
    {
        private readonly ILogger<RoutingSlipEventConsumer>? _logger;

        public RoutingSlipEventConsumer(ILogger<RoutingSlipEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<RoutingSlipCompleted> context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.Log(LogLevel.Information, "Routing Slip Completed: {TrackingNumber}", context.Message.TrackingNumber);
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RoutingSlipActivityCompleted> context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.Log(LogLevel.Information, "Routing Slip Activity Completed: {TrackingNumber} {ActivityName}", context.Message.TrackingNumber,
                    context.Message.ActivityName);
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RoutingSlipFaulted> context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.Log(LogLevel.Information, "Routing Slip Faulted: {TrackingNumber} {ExceptionInfo}", context.Message.TrackingNumber,
                    context.Message.ActivityExceptions.FirstOrDefault());
            }

            return Task.CompletedTask;
        }
    }
}