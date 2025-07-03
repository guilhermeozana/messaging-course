using System;
using System.Threading.Tasks;
using Contracts.Commands;
using MassTransit;
using MassTransit.Testing.Implementations;

namespace Billing.Worker;

public class RefundOrderConsumer : IConsumer<RefundOrder>
{
    public Task Consume(ConsumeContext<RefundOrder> context)
    {
        Console.WriteLine($"Refund Order: {context.Message.OrderId}");
        
        return Task.CompletedTask;
    }
}