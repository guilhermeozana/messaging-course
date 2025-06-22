using System;
using System.Threading.Tasks;
using Contracts.Events;
using MassTransit;

namespace AdminNotification.Worker;

public class OrderCreatedNotification : IConsumer<OrderCreated>
{
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        await Task.Delay(1000);
        Console.WriteLine($"Order created: {context.Message.OrderId}");
    }
}