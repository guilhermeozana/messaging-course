using Contracts.Events;
using MassTransit;

namespace OrdersApi.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        Console.WriteLine($"Order Created with ID: {context.Message.OrderId}");
        await Task.Delay(1000);
    }
}