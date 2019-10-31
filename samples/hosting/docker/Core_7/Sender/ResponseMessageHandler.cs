using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

public class ResponseMessageHandler
    : IHandleMessages<ResponseMessage>
{
    static ILog log = LogManager.GetLogger<ResponseMessageHandler>();

    public Task Handle(ResponseMessage message, IMessageHandlerContext context)
    {
        log.Info($"(1.1.0) Response received with description: {message.Data}");
        return Task.CompletedTask;
    }
}