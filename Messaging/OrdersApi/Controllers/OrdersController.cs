using AutoMapper;
using Contracts.Models;
using Contracts.Response;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Orders.Domain.Entities;
using Orders.Service;
using OrdersApi.Service.Clients;

namespace OrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IProductStockServiceClient productStockServiceClient;
        private readonly IMapper mapper;
        private readonly IPublishEndpoint publishEndpoint;
        private readonly ISendEndpointProvider sendEndpointProvider;
        private readonly IRequestClient<VerifyOrder> _requestClient;

        public OrdersController(IOrderService orderService,
            IProductStockServiceClient productStockServiceClient,
            IMapper mapper,
            IPublishEndpoint publishEndpoint, 
            ISendEndpointProvider sendEndpointProvider,
            IRequestClient<VerifyOrder> requestClient)
        {
            _orderService = orderService;
            this.productStockServiceClient = productStockServiceClient;
            this.mapper = mapper;
            this.publishEndpoint = publishEndpoint;
            this.sendEndpointProvider = sendEndpointProvider;
            _requestClient = requestClient;
        }


        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(OrderModel model)
        {
            var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:create-order-command"));
            await sendEndpoint.Send(model, context =>
            {
                context.Headers.Set("command-header", "value");
                context.TimeToLive = TimeSpan.FromMinutes(5);
            });
                
            return Accepted();
        }


        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var response = await _requestClient.GetResponse<OrderResult, OrderNotFoundResult>(new VerifyOrder()
            {
                Id = id
            });

            if (response.Is(out Response<OrderResult>? incomingMessage))
            {
                return Ok(incomingMessage.Message);
            }
            
            if (response.Is(out Response<OrderNotFoundResult>? notFound))
            {
                return Ok(notFound.Message);
            }
            
            // var order = await _orderService.GetOrderAsync(id);
            // if (order == null)
            // {
            //     return NotFound();
            // }
            //
            // return Ok(order);
            
            return BadRequest();
            
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            try
            {
                await _orderService.UpdateOrderAsync(order);
            }
            catch
            {
                if (!await _orderService.OrderExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _orderService.GetOrdersAsync();
            return Ok(orders);
        }



        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _orderService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            await _orderService.DeleteOrderAsync(id);
            return NoContent();
        }
    }
}
