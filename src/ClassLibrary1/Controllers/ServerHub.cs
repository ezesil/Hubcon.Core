﻿//using Hubcon.Core.Handlers;
//using Hubcon.Core.Models;
//using Hubcon.Core.Models.Interfaces;
//using Hubcon.Core.Models.Messages;
//using Microsoft.AspNetCore.SignalR;

//namespace Hubcon.Core.HubControllers
//{
//    public abstract class ServerHub : Hub, IServerHubController
//    {
//        public static event OnClientConnectedEventHandler? OnClientConnected;
//        public static event OnClientDisconnectedEventHandler? OnClientDisconnected;

//        public delegate void OnClientConnectedEventHandler(Type hubType, string connectionId);
//        public delegate void OnClientDisconnectedEventHandler(Type hubType, string connectionId);

//        protected MethodHandler handler = new();

//        protected ServerHub() => Build();
//        private void Build()
//        {
//            handler.BuildMethods(this, GetType());
//            ClientReferences.TryAdd(GetType(), []);
//        }

//        public async Task<BaseMethodResponse> HandleTask(BaseMethodInvokeInfo info) => await handler.HandleWithResultAsync(info);
//        public async Task HandleVoid(BaseMethodInvokeInfo info) => await handler.HandleWithoutResultAsync(info);
//        protected static Dictionary<Type, Dictionary<string, ClientReference>> ClientReferences { get; } = [];
//        protected IEnumerable<ClientReference> GetClients() => ClientReferences[GetType()].Values;
//        public static IEnumerable<ClientReference> GetClients(Type hubType) => ClientReferences[hubType].Values;
//        public override Task OnConnectedAsync()
//        {
//            ClientReferences[GetType()].Add(Context.ConnectionId, new ClientReference(Context.ConnectionId));
//            OnClientConnected?.Invoke(GetType(), Context.ConnectionId);
//            return base.OnConnectedAsync();
//        }

//        public override Task OnDisconnectedAsync(Exception? exception)
//        {
//            ClientReferences[GetType()].Remove(Context.ConnectionId);
//            OnClientDisconnected?.Invoke(GetType(), Context.ConnectionId);
//            return base.OnDisconnectedAsync(exception);
//        }
//    }

//    public abstract class ServerHub<TIClientController> : ServerHub
//        where TIClientController : IClientController
//    {
//        protected TIClientController CurrentClient { get => ClientReferences[Context.ConnectionId].ClientController; }
//        protected new Dictionary<string, ClientReference<TIClientController>> ClientReferences { get; } = [];
//        protected new IEnumerable<ClientReference<TIClientController>> GetClients() => ClientReferences.Values;

//        protected ServerHub()
//        {
//        }
//    }
//}
