using Castle.DynamicProxy;
using Hubcon.Core.Handlers;
using Hubcon.Core.Interceptors;
using Hubcon.Core.Models.Interfaces;

namespace Hubcon.Core.Connectors
{
    /// <summary>
    /// The ServerHubConnector allows a client to connect itself to a ServerHub and control its methods given its URL and
    /// the server's interface.
    /// </summary>
    /// <typeparam name="TIServerHubController"></typeparam>
    public class ServerHubConnector<TIServerHubController> : HubconClientBuilder<TIServerHubController>, IConnector
        where TIServerHubController : IServerHubController
    {
        private TIServerHubController? _client;
        private readonly ICommunicationsHandler _commsHandler;

        public TIServerHubController Instance
        {
            get
            {
                _client ??= GetInstance();
                return _client;
            }
        }

        public ICommunicationsHandler Connection { get => _commsHandler; }

        public ServerHubConnector(ICommunicationsHandler handler) : base()
        {
            _commsHandler = handler;
        }

        public async Task StartAsync()
        {
            await _commsHandler.StartAsync();
        }

        public async Task StopAsync()
        {
            await _commsHandler.StopAsync();
        }

        private TIServerHubController GetInstance()
        {
            var proxyGenerator = new ProxyGenerator();
            return (TIServerHubController)proxyGenerator.CreateInterfaceProxyWithTarget(
                typeof(TIServerHubController),
                (TIServerHubController)DynamicImplementationCreator.CreateImplementation(typeof(TIServerHubController)),
                new ServerHubConnectorInterceptor(Connection)
            );
        }
    }
}
