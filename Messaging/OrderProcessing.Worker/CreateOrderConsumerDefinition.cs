using System;
using Contracts.Exceptions;
using MassTransit;

namespace OrderCreation.Worker;

public class CreateOrderConsumerDefinition : ConsumerDefinition<CreateOrderConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CreateOrderConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseDelayedRedelivery(r =>
        {
            r.Intervals(
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(60),
                TimeSpan.FromSeconds(80));
        });
        
        consumerConfigurator.UseMessageRetry(r =>
        {
            r.Immediate(3);
            r.Ignore(typeof(OrderTotalTooSmallException));
            r.Handle(typeof(HandleAllException));
        });
    }
}