using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Commands;
using Contracts.Events;
using Contracts.Exceptions;
using Contracts.Models;
using MassTransit;
using Orders.Domain.Entities;
using Orders.Service;

namespace OrderCreation.Worker;

public class CreateOrderConsumer : IConsumer<OrderModel>
{
    private readonly IMapper _mapper;
    private readonly IOrderService _orderService;

    public CreateOrderConsumer(IMapper mapper, IOrderService orderService)
    {
        _mapper = mapper;
        _orderService = orderService;
    }

    public async Task Consume(ConsumeContext<OrderModel> context)
    {
        Console.WriteLine($"I got a command to create an order from {context.Message.OrderId}");
        
        var retryAttemptsNo = context.GetRetryAttempt();
        Console.WriteLine($"Retry attempt: {retryAttemptsNo}, from the {context.GetRedeliveryCount()}");
        //throw new HandleAllException();
        // if (context.Message.OrderItems.Sum(x => x.Price * x.Quantity) < 100)
        // {
        //     if (retryAttemptsNo <= 1)
        //     {
        //         throw new OrderTotalTooSmallException();
        //     }
        // }
        
        var orderToAdd = _mapper.Map<Order>(context.Message);
        var createdOrder = await _orderService.AddOrderAsync(orderToAdd);
        
        await context.Publish(new OrderCreated()
        {
            CreatedAt = createdOrder.OrderDate,
            Id = createdOrder.Id,
            OrderId = createdOrder.OrderId,
            TotalAmount = createdOrder.OrderItems.Sum(oi => oi.Price * oi.Quantity)
        }, context =>
        {
            context.Headers.Set("my-custom-header", "value");
            context.TimeToLive = TimeSpan.FromMinutes(5);
        });
        
        await Task.CompletedTask;
    }
}