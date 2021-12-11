﻿using MassTransit.Testing;
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
using Automatonymous.Graphing;
using System.IO;
using Automatonymous.Visualizer;

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

        /// <summary>
        /// Generates .dot file with State Machine diagram.
        /// </summary>
        /// <remarks>
        /// Copy contents of file generated by this and paste them on the page: <see href="https://dreampuf.github.io/GraphvizOnline/"/>
        /// It will give you visualization of OrderStateMachine
        /// </remarks>
        [Fact]
        public async Task GenerateStateMachineGraph()
        {
            var orderStateMachine = new OrderStateMachine();

            var graph = orderStateMachine.GetGraph();

            var generator = new StateMachineGraphvizGenerator(graph);

            string dots = generator.CreateDotFile();
            var filePath = Path.Combine(Environment.CurrentDirectory, "OrderStateMachine_graph.dot");
            await File.WriteAllTextAsync(filePath, dots);
        }
    }
}