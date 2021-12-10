﻿using MassTransit;
using Microsoft.Extensions.Logging;
using Sample.Contracts;

namespace Sample.Components.Consumers
{
    public class SubmitOrderConsumer : IConsumer<SubmitOrder>
    {
        private readonly ILogger _logger;

        public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            _logger.Log(LogLevel.Debug, "SubmitOrderConsumer: {CustomerNumber}", context.Message.CustomerNumber);

            if (context.Message.CustomerNumber.Contains("TEST"))
            {
                await context.RespondAsync(new OrderSubmissionRejected
                {
                    CustomerNumber = context.Message.CustomerNumber,
                    OrderId = context.Message.OrderId,
                    Reason = $"Test customer cannot submit orders: {context.Message.CustomerNumber}",
                    Timestamp = context.Message.Timestamp
                });

                return;
            }

            await context.RespondAsync(new OrderSubmissionAccepted
            {
                CustomerNumber = context.Message.CustomerNumber,
                OrderId = context.Message.OrderId,
                Timestamp = context.Message.Timestamp
            });
        }
    }
}