using System;
using System.Threading.Tasks;
using Contracts.Events;
using MassTransit;

namespace OrderCreation.Worker;

public class CancelationRequestedConsumer : IConsumer<OrderCanceled>
{
    public Task Consume(ConsumeContext<OrderCanceled> context)
    {
        Console.WriteLine($"Order {context.Message.OrderId} cancelled");
        
        return Task.CompletedTask;
    }
}