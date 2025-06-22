using Orders.Domain.Entities;

namespace Contracts.Response;

public class OrderResult
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
}