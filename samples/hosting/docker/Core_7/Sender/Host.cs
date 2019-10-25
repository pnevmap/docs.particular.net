using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

namespace Sender
{
    class Host
    {
        static readonly ILog log = LogManager.GetLogger<Host>();

        IEndpointInstance endpoint;

        public string EndpointName => "Samples.Docker.Sender";

        public async Task Start()
        {
            try
            {
                var endpointConfiguration = new EndpointConfiguration(EndpointName);
                var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
                transport.ConnectionString(Environment.GetEnvironmentVariable("ASB_CONNECTION_STRING"));
                endpointConfiguration.EnableInstallers();

                endpoint = await Endpoint.Start(endpointConfiguration);

                Console.WriteLine("Sending a message...");

                while (true) {
                    var guid = Guid.NewGuid();
                    Console.WriteLine($"Requesting to get data by id: {guid:N}");

                    var message = new RequestMessage
                    {
                        Id = guid,
                        Data = "String property value: " + guid.ToString()
                    };

                    await endpoint.Send("Samples.Docker.Receiver", message)
                        .ConfigureAwait(false);
                    await Task.Delay(5000);
                }
            }
            catch (Exception ex)
            {
                FailFast("Failed to start.", ex);
            }
        }

        public async Task Stop()
        {
            try
            {
                await endpoint?.Stop();
            }
            catch (Exception ex)
            {
                FailFast("Failed to stop correctly.", ex);
            }
        }

        async Task OnCriticalError(ICriticalErrorContext context)
        {
            try
            {
                await context.Stop();
            }
            finally
            {
                FailFast($"Critical error, shutting down: {context.Error}", context.Exception);
            }
        }

        void FailFast(string message, Exception exception)
        {
            try
            {
                log.Fatal(message, exception);
            }
            finally
            {
                Environment.FailFast(message, exception);
            }
        }
    }
}
