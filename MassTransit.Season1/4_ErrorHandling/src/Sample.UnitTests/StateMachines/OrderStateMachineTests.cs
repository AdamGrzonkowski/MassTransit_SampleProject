using MassTransit.Testing;
using MassTransit;
using Moq;
using Sample.Components.Consumers;
using Sample.Components.StateMachines;
using Sample.Contracts.Dtos;
using System;
using System.Threading.Tasks;
using Xunit;
using Sample.Contracts.Events;
using FluentAssertions;

namespace Sample.UnitTests.StateMachines
{
    public class OrderStateMachineTests
    {
        [Fact]
        public async Task OrderStateMachine_CorrectPayload_CreatesStateInstance()
        {
            // Arrange
            using var harness = new InMemoryTestHarness();
            var orderStateMachine = new OrderStateMachine();
            var saga = harness.StateMachineSaga<OrderState, OrderStateMachine>(orderStateMachine);
            var orderId = NewId.NextGuid();

            await harness.Start();

            // Act
            await harness.Bus.Publish<OrderSubmitted>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = "12345"
            });

            // Assert
            saga.Created.Contains(orderId);
            var instanceId = await saga.Exists(orderId, x => x.Submitted);
            instanceId.Should().HaveValue();
            saga.Sagas.Contains(instanceId.Value).CustomerNumber.Should().BeEquivalentTo("12345");
        }
    }
}