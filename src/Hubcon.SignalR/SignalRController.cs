using Hubcon;
using Hubcon.Handlers;
using Hubcon.Interfaces;
using Hubcon.Interfaces.Communication;
using Hubcon.Models;
using Hubcon.SignalR.Models;
using Hubcon.Tools;
using Microsoft.AspNetCore.SignalR;

namespace Hubcon.SignalR
{
    public class SignalRServerCommunicationHandler : ICommunicationHandler
    {
        public SignalRServerCommunicationHandler(Hub hub)
        {
            
        }

        public Task CallAsync()
        {
            
        }

        public Task<MethodResponse> InvokeAsync()
        {
            
        }
    }

    public abstract class BaseSignalRHubController : Hub, IHubconController<SignalRServerCommunicationHandler>
    {
        // Events
        public static event OnClientConnectedEventHandler? OnClientConnected;
        public static event OnClientDisconnectedEventHandler? OnClientDisconnected;

        public delegate void OnClientConnectedEventHandler(Type hubType, string connectionId);
        public delegate void OnClientDisconnectedEventHandler(Type hubType, string connectionId);

        // Handlers
        SignalRServerCommunicationHandler IHubconController<SignalRServerCommunicationHandler>.CommunicationHandler { get; set; }
        public SignalRServerCommunicationHandler GetCommunicationHandler() => ((IHubconController<SignalRServerCommunicationHandler>)this).CommunicationHandler;
        MethodHandler IHubconController<SignalRServerCommunicationHandler>.MethodHandler { get; set; }
        public MethodHandler GetMethodHandler() => ((IHubconController<SignalRServerCommunicationHandler>)this).MethodHandler;

        // Clients
        protected static Dictionary<Type, Dictionary<string, ClientReference>> ClientReferences { get; } = new();

        protected BaseSignalRHubController() => Initialize();

        public void Initialize()
        {
            var commHandlerRef = InstanceCreator.TryCreateInstance<SignalRServerCommunicationHandler>(this);
            ((IHubconController<SignalRServerCommunicationHandler>)this).CommunicationHandler = commHandlerRef;

            var methodHandlerRef = new MethodHandler(this, GetType());
            ((IHubconController<SignalRServerCommunicationHandler>)this).MethodHandler = methodHandlerRef;
        }

        public async Task<MethodResponse> HandleTask(MethodInvokeInfo info) => await GetMethodHandler().HandleWithResultAsync(info);
        public async Task HandleVoid(MethodInvokeInfo info) => await GetMethodHandler().HandleWithoutResultAsync(info);

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
}
