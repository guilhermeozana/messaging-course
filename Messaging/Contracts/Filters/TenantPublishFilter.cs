using MassTransit;

namespace Contracts.Filters;

public class TenantPublishFilter<T> : IFilter<PublishContext<T>> where T : class
{
    private readonly Tenant _tenant;

    public TenantPublishFilter(Tenant tenant)
    {
        _tenant = tenant;
    }

    public void Probe(ProbeContext context)
    {
        throw new NotImplementedException();
    }

    public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
    {
        if (!string.IsNullOrEmpty(_tenant.TenantId))
        {
            context.Headers.Set(_tenant.TenantId, _tenant.TenantId);
        }
        
        return next.Send(context);
    }
}