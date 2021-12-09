namespace Company.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Contracts;

    public class SubmitOrderConsumer :
        IConsumer<SubmitOrder>
    {
        public Task Consume(ConsumeContext<SubmitOrder> context)
        {
            return Task.CompletedTask;
        }
    }
}