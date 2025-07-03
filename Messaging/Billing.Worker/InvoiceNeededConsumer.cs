using System;
using System.Threading.Tasks;
using Contracts.Events;
using MassTransit;

namespace AdminNotification.Worker;

public class InvoiceNeededConsumer : IConsumer<InvoiceNeeded>
{
    public Task Consume(ConsumeContext<InvoiceNeeded> context)
    {
        Console.WriteLine(context.ReceiveContext.InputAddress);
        Console.WriteLine("Invoice needed consumer");
        
        return Task.CompletedTask;
    }
}