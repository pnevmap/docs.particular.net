using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

public class RequestMessageHandler
    : IHandleMessages<RequestMessage>
{
    static ILog log = LogManager.GetLogger<RequestMessageHandler>();

    public Task Handle(RequestMessage message, IMessageHandlerContext context)
    {
        log.Info($"(1.2.0) Request received with description: {message.Data}");

        if (new Random().Next(3) == 0)
        {
            log.Fatal("Endpoint is crashing!");
            Environment.Exit(1);
        }

        var response = new ResponseMessage
        {
            Id = message.Id,
            Data = message.Data
        };
        return context.Reply(response);
    }
}