using System;
using MassTransit;
using Orders.Domain.StateMachine;

namespace OrderSaga.StateMachine;

public class OrderSagaDefinition : SagaDefinition<OrderStateData>
{
    public OrderSagaDefinition()
    {
        Endpoint(e =>
        {
            e.Name = "saga-queue";
        });
    }

    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderStateData> sagaConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(1)));
    }
}