namespace Contracts.Events;

public class CancelationRequested
{
    public Guid OrderId { get; set; }
}