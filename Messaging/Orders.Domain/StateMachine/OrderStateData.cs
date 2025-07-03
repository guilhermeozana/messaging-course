using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;
using Orders.Domain.Entities;

namespace Orders.Domain.StateMachine;

public class OrderStateData : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    
    public required string CurrentState { get; set; }

    public Guid OrderId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? PaidAt { get; set; }
    public bool IsBilled { get; set; }
    
    public OrderStatus OrderStatus { get; set; }
    public DateTime? CanceledAt { get; set; }
    
    public Guid? PaymentTimeoutTokenId { get; set; }

}