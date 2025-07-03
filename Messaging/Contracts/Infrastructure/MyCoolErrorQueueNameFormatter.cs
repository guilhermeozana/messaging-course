using MassTransit;

namespace Contracts.Infrastructure;

public class MyCoolErrorQueueNameFormatter : IErrorQueueNameFormatter
{
    public string FormatErrorQueueName(string queueName)
    {
        return "awesome-error";
    }
}