namespace Contracts.Events;

public class PaymentTimeoutExpired
{
    public Guid OrderId { get; set; }
}