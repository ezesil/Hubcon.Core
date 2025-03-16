using Hubcon.Core.Connectors;
using Hubcon.Core.Handlers;
using Hubcon.Core.Models.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Hubcon.Core.HubControllers
{
    public abstract class ClientController : HubController, IHostedService
    {
        protected Func<ICommunicationsHandler> handlerFactory;
        protected CancellationToken _token;
        protected string _url;
        protected Task? runningTask;

        protected ClientController(string url, Func<ICommunicationsHandler> handlerFactory)
        {
            _url = url;

            this.handlerFactory = handlerFactory;

            Build(handlerFactory.Invoke());
        }

        public TIServerHubController GetConnector<TIServerHubController>() where TIServerHubController : IServerHubController
        {
            return new ServerHubConnector<TIServerHubController>(handlerFactory.Invoke()).Instance;
        }

        public ClientController StartInstanceAsync(Action<string>? consoleOutput = null, CancellationToken cancellationToken = default)
        {
            _ = StartAsync(consoleOutput, cancellationToken);
            return this;
        }

        public async Task StartAsync(Action<string>? consoleOutput = null, CancellationToken cancellationToken = default)
        {
            var handler = handlerFactory.Invoke();
            try
            {
                _token = cancellationToken;

                bool connectedInvoked = false;
                while (true)
                {
                    await Task.Delay(1000, cancellationToken);
                    if (handler.State == CommunicationState.Connecting)
                    {
                        consoleOutput?.Invoke($"Connecting to {_url}...");
                        _ = handler.StartAsync(_token);
                        connectedInvoked = false;
                    }
                    else if (handler.State == CommunicationState.Disconnected)
                    {
                        consoleOutput?.Invoke($"Failed connectin to {_url}. Retrying...");
                        _ = handler.StartAsync(_token);
                        connectedInvoked = false;
                    }
                    else if (handler.State == CommunicationState.Reconnecting)
                    {
                        consoleOutput?.Invoke($"Connection lost, reconnecting to {_url}...");
                        _ = handler.StartAsync(_token);
                        connectedInvoked = false;
                    }
                    else if (handler.State == CommunicationState.Connected && !connectedInvoked)
                    {
                        consoleOutput?.Invoke($"Successfully connected to {_url}.");
                        connectedInvoked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                consoleOutput?.Invoke($"Error: {ex.Message}");

                if (_token.IsCancellationRequested)
                {
                    consoleOutput?.Invoke("Cancelado.");
                }
            }

            _ = handler?.StopAsync(_token);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            runningTask = Task.Run(async () => await StartAsync(Console.WriteLine, cancellationToken));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return runningTask ?? Task.CompletedTask;
        }
    }

    public class ClientController<TIServerHubController> : ClientController
        where TIServerHubController : IServerHubController
    {
        public TIServerHubController Server { get; private set; }
        public ClientController(string url, Func<ICommunicationsHandler> handlerFactory) : base(url, handlerFactory) => Server = GetConnector<TIServerHubController>();
    }
}
