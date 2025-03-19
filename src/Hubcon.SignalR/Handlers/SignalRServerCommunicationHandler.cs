﻿using Hubcon.Interfaces;
using Hubcon.Interfaces.Communication;
using Hubcon.Models;
using Hubcon.SignalR.Server;
using Microsoft.AspNetCore.SignalR;
using Hubcon.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Hubcon.SignalR.Handlers
{
    public class SignalRServerCommunicationHandler : ICommunicationHandler
    {
        private protected string TargetClientId { get; private set; } = string.Empty;
        private protected Func<IHubContext<Hub>>? HubContextFactory { get; private set; }
        private protected Func<BaseHubController>? HubFactory { get; private set; }
        private protected Type HubType { get; private set; }

        public SignalRServerCommunicationHandler(Type hubType)
        {
            HubType = hubType;
            Type hubContextType = typeof(IHubContext<>).MakeGenericType(hubType);
            var hubContext = (IHubContext<Hub>)StaticServiceProvider.Services.GetRequiredService(hubContextType);
            HubContextFactory = () => hubContext;
        }

        public SignalRServerCommunicationHandler(BaseHubController hubFactory, Type hubType)
        {
            HubType = hubType;
            HubFactory = () => hubFactory;
        }

        public async Task<MethodResponse> InvokeAsync(string method, object[] arguments, CancellationToken cancellationToken) 
        { 
            MethodResponse result;
            Hub? hub = HubFactory?.Invoke();
            IHubContext<Hub>? hubContext = HubContextFactory?.Invoke();
      
            var client = hubContext?.Clients.Client(TargetClientId) ?? hub!.Clients.Client(TargetClientId);

            MethodInvokeRequest request = new MethodInvokeRequest(method, arguments).SerializeArgs();
            result = await client.InvokeAsync<MethodResponse>(method, request, cancellationToken);
            return result;                   
        }

        public async Task CallAsync(string method, object[] arguments, CancellationToken cancellationToken)
        {
            Hub? hub = HubFactory?.Invoke();
            IHubContext<Hub>? hubContext = HubContextFactory?.Invoke();

            var client = hubContext?.Clients.Client(TargetClientId) ?? hub!.Clients.Client(TargetClientId);

            MethodInvokeRequest request = new MethodInvokeRequest(method, arguments).SerializeArgs();
            await client.SendAsync(method, request, cancellationToken);
        }

        public List<IClientReference> GetAllClients()
        {
            return BaseHubController.GetClients(HubType).ToList();
        }

        public ICommunicationHandler WithUserId(string id)
        {
            TargetClientId = id;
            return this;
        }
    }
}
