using Contracts.Models;
using MassTransit;

namespace Contracts.Filters;

public class TenantPublishEmailFilter : IFilter<PublishContext<Email>>
{
    public Task Send(PublishContext<Email> context, IPipe<PublishContext<Email>> next)
    {
        Console.WriteLine($"Email filter");
        
        return next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        throw new NotImplementedException();
    }
}