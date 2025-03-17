using Hubcon;
using Hubcon.Handlers;
using Hubcon.Interfaces;
using Hubcon.Interfaces.Communication;
using Hubcon.Models;
using Hubcon.SignalR.Handlers;
using Hubcon.SignalR.Models;
using Hubcon.Tools;
using Microsoft.AspNetCore.SignalR;

namespace Hubcon.SignalR
{
    public abstract class BaseSignalRHubController : Hub, IHubconController<SignalrCommunicationHandler>
    {
        // Events
        public static event OnClientConnectedEventHandler? OnClientConnected;
        public static event OnClientDisconnectedEventHandler? OnClientDisconnected;

        public delegate void OnClientConnectedEventHandler(Type hubType, string connectionId);
        public delegate void OnClientDisconnectedEventHandler(Type hubType, string connectionId);

        // Handlers
        SignalrCommunicationHandler IHubconController<SignalrCommunicationHandler>.CommunicationHandler { get; set; }
        public SignalrCommunicationHandler GetCommunicationHandler() => ((IHubconController<SignalrCommunicationHandler>)this).CommunicationHandler;
        MethodHandler IHubconController<SignalrCommunicationHandler>.MethodHandler { get; set; }
        public MethodHandler GetMethodHandler() => ((IHubconController<SignalrCommunicationHandler>)this).MethodHandler;

        // Clients
        protected static Dictionary<Type, Dictionary<string, ClientReference>> ClientReferences { get; } = new();

        protected BaseSignalRHubController()
        {
            var commHandlerRef = InstanceCreator.TryCreateInstance<SignalrCommunicationHandler>(this);
            ((IHubconController<SignalrCommunicationHandler>)this).CommunicationHandler = commHandlerRef;

            var methodHandlerRef = new MethodHandler(this, GetType());
            ((IHubconController<SignalrCommunicationHandler>)this).MethodHandler = methodHandlerRef;
        }

        public async Task<MethodResponse> HandleTask(MethodInvokeRequest info) => await GetMethodHandler().HandleWithResultAsync(info);
        public async Task HandleVoid(MethodInvokeRequest info) => await GetMethodHandler().HandleWithoutResultAsync(info);

        protected IEnumerable<ClientReference> GetClients() => ClientReferences[GetType()].Values;
        public static IEnumerable<ClientReference> GetClients(Type hubType) => ClientReferences[hubType].Values;

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

    public abstract class BaseSignalRHubController<TICommunicationContract> : BaseSignalRHubController
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
