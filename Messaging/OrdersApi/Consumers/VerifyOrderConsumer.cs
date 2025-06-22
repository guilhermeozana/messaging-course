using Contracts.Response;
using MassTransit;
using Orders.Domain.Entities;
using Orders.Service;

namespace OrdersApi.Consumers;

public class VerifyOrderConsumer : IConsumer<VerifyOrder>
{
    private readonly IOrderService _orderService;

    public VerifyOrderConsumer(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task Consume(ConsumeContext<VerifyOrder> context)
    {
        if (!context.IsResponseAccepted<OrderResult>() && !context.IsResponseAccepted<OrderNotFoundResult>())
        {
            throw new ArgumentException(nameof(context));
        }
        
        var existingOrder = await _orderService.GetOrderAsync(context.Message.Id);

        if (existingOrder != null)
        {
            await context.RespondAsync<OrderResult>(new
            {
                Id = context.Message.Id,
                OrderDate = existingOrder.OrderDate,
                Status = existingOrder.Status,
            });
        }
        else
        {
            await context.RespondAsync<OrderNotFoundResult>(new OrderNotFoundResult()
            {
                ErrorMessage = "Sorry, we couldn't find an order with this id."
            });
        }
        
       
    }
}