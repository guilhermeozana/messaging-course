using MassTransit;

namespace Contracts.Filters;

public class TenantSendFilter<T>: IFilter<SendContext<T>> where T : class
{
    private readonly Tenant _tenant;

    public TenantSendFilter(Tenant tenant)
    {
        _tenant = tenant;
    }

    public void Probe(ProbeContext context)
    {
        throw new NotImplementedException();
    }
    
    public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
    {
        if (!string.IsNullOrEmpty(_tenant.TenantId))
        {
            context.Headers.Set("Tenant-From-Send", _tenant.TenantId);
        }
        
        return next.Send(context);
    }

    
}