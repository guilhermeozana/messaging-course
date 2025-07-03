using MassTransit;

namespace Contracts.Filters;

public class TenantConsumeFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
    private readonly Tenant _tenant;

    public TenantConsumeFilter()
    {
        _tenant = new Tenant();
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("TenantConsume");
    }
    
    public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        var tenantString = context.Headers.Get<string>("Tenant");

        if (!string.IsNullOrEmpty(tenantString))
        {
            _tenant.TenantId = tenantString;
        }
        
        return next.Send(context);
    }
}