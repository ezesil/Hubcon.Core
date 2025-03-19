using Hubcon.Connectors;
using Hubcon.Handlers;
using Hubcon.Interfaces;
using Hubcon.Interfaces.Communication;
using Hubcon.Models;
using Hubcon.SignalR.Handlers;
using Hubcon.SignalR.Models;
using Hubcon.Tools;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Hubcon.SignalR.Server
{
    public abstract class BaseHubController : Hub, IHubconController
    {
        // Events
        public static event OnClientConnectedEventHandler? OnClientConnected;
        public static event OnClientDisconnectedEventHandler? OnClientDisconnected;

        public delegate void OnClientConnectedEventHandler(Type hubType, string connectionId);
        public delegate void OnClientDisconnectedEventHandler(Type hubType, string connectionId);

        // Handlers
        public MethodHandler MethodHandler { get; set; }
        public ICommunicationHandler CommunicationHandler { get; set; }

        // Clients
        protected static Dictionary<Type, Dictionary<string, ClientReference>> ClientReferences { get; } = new();

        protected BaseHubController()
        {
            Build();
        }

        protected void Build()
        {
            ClientReferences.TryAdd(GetType(), new Dictionary<string, ClientReference>());

            CommunicationHandler = new SignalRServerCommunicationHandler(GetType());

            MethodHandler = new MethodHandler();
            MethodHandler.BuildMethods(this, GetType());
        }

        public async Task<MethodResponse> HandleTask(MethodInvokeRequest info) => await MethodHandler.HandleWithResultAsync(info);
        public async Task HandleVoid(MethodInvokeRequest info) => await MethodHandler.HandleWithoutResultAsync(info);

        protected IEnumerable<IClientReference> GetClients() => ClientReferences[GetType()].Values;
        public static IEnumerable<IClientReference> GetClients(Type hubType) => ClientReferences[hubType].Values;

        public override Task OnConnectedAsync()
        {
            ClientReferences[GetType()].Add(Context.ConnectionId, new ClientReference(Context.ConnectionId));
            OnClientConnected?.Invoke(GetType(), Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            ClientReferences[GetType()].Remove(Context.ConnectionId);
            OnClientDisconnected?.Invoke(GetType(), Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }

    public abstract class BaseHubController<TICommunicationContract> : BaseHubController
        where TICommunicationContract : ICommunicationContract
    {

        private IClientManager _clientManager;
        protected IClientManager clientManager 
        { 
            get
            {
                if (_clientManager == null)
                {
                    Type clientManagerType = typeof(IClientManager<,>).MakeGenericType(typeof(TICommunicationContract), GetType());
                    using (var scope = StaticServiceProvider.Services.CreateScope())
                    {
                        var scopedProvider = scope.ServiceProvider;
                        _clientManager = (IClientManager)scopedProvider.GetRequiredService(clientManagerType);
                    }
                }

                return _clientManager;
            } 
        }
        protected TICommunicationContract? CurrentClient { get => clientManager.GetClient<TICommunicationContract>(Context.ConnectionId); }
        protected TICommunicationContract? GetClient(string connectionId) => clientManager.GetClient<TICommunicationContract>(connectionId);

        protected BaseHubController()
        {
        }
    }
}
