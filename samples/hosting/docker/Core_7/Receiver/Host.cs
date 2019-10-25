using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

namespace Receiver
{
    class Host
    {
        static readonly ILog log = LogManager.GetLogger<Host>();

        IEndpointInstance endpoint;

        public string EndpointName => "Samples.Docker.Receiver";

        public async Task Start()
        {
            try
            {
                var endpointConfiguration = new EndpointConfiguration(EndpointName);
                #region TransportConfiguration

                var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
                transport.ConnectionString(Environment.GetEnvironmentVariable("ASB_CONNECTION_STRING"));
                endpointConfiguration.EnableInstallers();

                #endregion

                endpointConfiguration.EnableInstallers();

                endpoint = await Endpoint.Start(endpointConfiguration);

                Console.WriteLine("Use 'docker-compose down' to stop containers.");
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
