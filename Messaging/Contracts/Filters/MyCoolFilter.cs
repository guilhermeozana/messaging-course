using MassTransit;

namespace Contracts.Filters;

public class MyCoolFilter : IFilter<ConsumeContext>
{
    public Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
    {
        var variable = context.ReceiveContext.TransportHeaders.TryGetHeader("Tenant-From-Send", out var tenant);
        
        Console.WriteLine("MyCoolFilter send");
        
        return next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        throw new NotImplementedException();
    }
}