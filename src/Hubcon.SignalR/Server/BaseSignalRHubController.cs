using Hubcon.Handlers;
using Hubcon.Interfaces;
using Hubcon.Interfaces.Communication;
using Hubcon.Models;
using Hubcon.SignalR.Handlers;
using Hubcon.SignalR.Models;
using Hubcon.Tools;
using Microsoft.AspNetCore.SignalR;

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
        public ICommunicationHandler GetCommunicationHandler() => CommunicationHandler;
        public MethodHandler GetMethodHandler() => ((IHubconController)this).MethodHandler;

        public MethodHandler MethodHandler { get; set; }
        public ICommunicationHandler CommunicationHandler { get; set; }

        // Clients
        protected static Dictionary<Type, Dictionary<string, ClientReference>> ClientReferences { get; } = new();

        protected BaseHubController()
        {
            ClientReferences.TryAdd(GetType(), new Dictionary<string, ClientReference>());

            CommunicationHandler = new SignalRServerCommunicationHandler(this, GetType());

            MethodHandler = new MethodHandler();
            MethodHandler.BuildMethods(this, GetType());
        }

        public async Task<MethodResponse> HandleTask(MethodInvokeRequest info) => await GetMethodHandler().HandleWithResultAsync(info);
        public async Task HandleVoid(MethodInvokeRequest info) => await GetMethodHandler().HandleWithoutResultAsync(info);

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

    public abstract class BaseSignalRHubController<TICommunicationContract> : BaseHubController
        where TICommunicationContract : ICommunicationContract
    {
        protected TICommunicationContract CurrentClient { get => ClientReferences[Context.ConnectionId].ClientController; }
        protected new Dictionary<string, ClientReference<TICommunicationContract>> ClientReferences { get; } = new();
        protected new IEnumerable<ClientReference<TICommunicationContract>> GetClients() => ClientReferences.Values;

        protected BaseSignalRHubController()
        {
        }
    }
}
