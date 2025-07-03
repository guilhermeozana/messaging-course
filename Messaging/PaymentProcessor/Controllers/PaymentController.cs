using Contracts.Events;
using Contracts.Models;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orders.Domain.Entities;
using PaymentProcessor.Services;

namespace PaymentProcessor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public PaymentController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost("/pay")]
        public async Task<IActionResult> Post(PayModel model)
        {
            await _publishEndpoint.Publish(new OrderPaid()
            {
                AmountPaid = model.AmountPaid,
                OrderId = model.OrderId,
                PaymentMethod = model.PaymentMethod
            });
            
            return Ok("Payment Processor API");
        }
        
        [HttpGet("/requestcancelation/{id}")]
        public async Task<ActionResult<Order>> RequestCancelation(Guid id)
        {
            await _publishEndpoint.Publish(new CancelationRequested()
            {
                OrderId = id
            });
            return Accepted();
        }

    }
}
