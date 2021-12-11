using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Sample.Components.Consumers;
using Sample.Contracts.Dtos;
using System;
using System.Threading.Tasks;
using Xunit;
using Sample.Contracts.Events;

namespace Sample.UnitTests.Consumers
{
    public class SubmitOrderConsumerTests
    {
        [Fact]
        public async Task Consume_CorrectPayload_RespondsWithOrderSubmissionAccepted()
        {
            // Arrange
            using var harness = new InMemoryTestHarness();
            var submitOrderConsumer = harness.Consumer<SubmitOrderConsumer>();
            var requestClient = await harness.ConnectRequestClient<SubmitOrder>();
            var orderId = NewId.NextGuid();

            await harness.Start();

            // Act
            var response = await requestClient.GetResponse<OrderSubmissionAccepted>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = "12345"
            });

            // Assert
            response.Message.OrderId.Should().Be(orderId);
            submitOrderConsumer.Consumed.Select<SubmitOrder>().Should().NotBeNullOrEmpty();
            (await harness.Sent.Any<OrderSubmissionAccepted>()).Should().BeTrue();
        }

        [Fact]
        public async Task Consume_CustomerNumberIsTest_RespondsWithOrderSubmissionRejected()
        {
            // Arrange
            using var harness = new InMemoryTestHarness();
            var submitOrderConsumer = harness.Consumer<SubmitOrderConsumer>();
            var requestClient = await harness.ConnectRequestClient<SubmitOrder>();
            var orderId = NewId.NextGuid();

            await harness.Start();

            // Act
            var response = await requestClient.GetResponse<OrderSubmissionRejected>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = "TEST"
            });

            // Assert
            response.Message.OrderId.Should().Be(orderId);
            submitOrderConsumer.Consumed.Select<SubmitOrder>().Should().NotBeNullOrEmpty();
            (await harness.Sent.Any<OrderSubmissionRejected>()).Should().BeTrue();
        }

        [Fact]
        public async Task Consume_RequestIdIsNull_DoNotSendAnyResponses()
        {
            // Arrange
            // you can adjust Timeouts directly here, to make tests run faster, if you test whether timeout will occur
            using var harness = new InMemoryTestHarness
            {
                TestTimeout = TimeSpan.FromSeconds(5)
            };

            var submitOrderConsumer = harness.Consumer<SubmitOrderConsumer>();
            var requestClient = await harness.ConnectRequestClient<SubmitOrder>();
            var orderId = NewId.NextGuid();

            await harness.Start();

            // Act
            await harness.InputQueueSendEndpoint.Send<SubmitOrder>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = "12345"
            });

            // Assert
            submitOrderConsumer.Consumed.Select<SubmitOrder>().Should().NotBeNullOrEmpty();
            (await harness.Sent.Any<OrderSubmissionRejected>()).Should().BeFalse();
            (await harness.Sent.Any<OrderSubmissionRejected>()).Should().BeFalse();
        }

        [Fact]
        public async Task Consume_CorrectPayload_ConsumesSubmitOrderCommand()
        {
            // Arrange
            using var harness = new InMemoryTestHarness();
            var submitOrderConsumer = harness.Consumer<SubmitOrderConsumer>();
            var orderId = NewId.NextGuid();

            await harness.Start();

            // Act
            await harness.InputQueueSendEndpoint.Send<SubmitOrder>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = "12345"
            });

            // Assert
            submitOrderConsumer.Consumed.Select<SubmitOrder>().Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Consume_CorrectPayload_PublishesOrderSubmittedEvent()
        {
            // Arrange
            using var harness = new InMemoryTestHarness();
            var submitOrderConsumer = harness.Consumer<SubmitOrderConsumer>();
            var orderId = NewId.NextGuid();

            await harness.Start();

            // Act
            await harness.InputQueueSendEndpoint.Send<SubmitOrder>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = "12345"
            });

            // Assert
            submitOrderConsumer.Consumed.Select<SubmitOrder>().Should().NotBeNullOrEmpty();
            (await harness.Published.Any<OrderSubmitted>()).Should().BeTrue();
        }

        [Fact]
        public async Task Consume_CustomerNumberIsTest_DoesNotPublishOrderSubmittedEvent()
        {
            // Arrange
            using var harness = new InMemoryTestHarness();
            var submitOrderConsumer = harness.Consumer<SubmitOrderConsumer>();
            var orderId = NewId.NextGuid();

            await harness.Start();

            // Act
            await harness.InputQueueSendEndpoint.Send<SubmitOrder>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = "TEST12345"
            });

            // Assert
            submitOrderConsumer.Consumed.Select<SubmitOrder>().Should().NotBeNullOrEmpty();
            (await harness.Published.Any<OrderSubmitted>()).Should().BeFalse();
        }
    }
}