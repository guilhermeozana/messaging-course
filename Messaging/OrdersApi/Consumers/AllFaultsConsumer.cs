using MassTransit;

namespace OrdersApi.Consumers;

public class AllFaultsConsumer : IConsumer<Fault>
{
    public Task Consume(ConsumeContext<Fault> context)
    {
        Console.WriteLine("All Faults consumed");
        return Task.CompletedTask;
    }
}