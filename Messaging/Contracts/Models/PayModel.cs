namespace Contracts.Models;

public class PayModel
{
    public Guid OrderId { get; set; }
    public decimal AmountPaid { get; set; }
    public string PaymentMethod { get; set; }
}