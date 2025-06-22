using MassTransit;

namespace OrdersApi.Consumers;

public class OrderCreatedConsumerDefinition : ConsumerDefinition<OrderCreatedConsumer>
{
    public OrderCreatedConsumerDefinition()
    {
        EndpointName = "my-named-order";
        
        Endpoint(e=>
        {
            e.Name = "my-named-order";
            e.ConcurrentMessageLimit = 10; 
        });
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator receiveEndpointConfigurator, 
        IConsumerConfigurator<OrderCreatedConsumer> consumerConfigurator, IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(r => { r.Immediate(5); });
    }
}