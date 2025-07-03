using System;
using System.Threading.Tasks;
using Contracts.Events;
using MassTransit;
using Orders.Domain.Entities;
using Orders.Service;

namespace AdminNotification.Worker;

public class OrderCreatedNotification : IConsumer<OrderCreated>
{
    private readonly IOrderService _orderService;

    public OrderCreatedNotification(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        await context.Publish(new InvoiceNeeded()
        {
            Id = context.Message.Id,
            OrderId = context.Message.OrderId,
            TotalAmount = context.Message.TotalAmount,
            VAT = context.Message.TotalAmount * 1.19m
        });
        
        var existingOrder = await _orderService.GetOrderAsync(int.Parse(context.Message.OrderId.ToString()));

        if (existingOrder != null)
        {
            existingOrder.Status = OrderStatus.Created;
            await _orderService.UpdateOrderAsync(existingOrder);
        }
        
        await Task.Delay(1000);
        Console.WriteLine($"Order created: {context.Message.OrderId}");
    }
}