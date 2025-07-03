using System;
using System.Threading.Tasks;
using Contracts.Events;
using MassTransit;

namespace OrderCreation.Worker;

public class OrderCanceledConsumer : IConsumer<OrderCanceled>
{
    public Task Consume(ConsumeContext<OrderCanceled> context)
    {
        Console.WriteLine($"Order cancelled: {context.Message.OrderId}");
        
        context.Publish(new OrderCompleted
        {
            OrderId = context.Message.OrderId
        });
        return Task.CompletedTask;
    }
}