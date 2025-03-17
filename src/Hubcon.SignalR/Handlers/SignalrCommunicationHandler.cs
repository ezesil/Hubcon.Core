using Castle.DynamicProxy;
using Hubcon.Interfaces.Communication;
using Hubcon.Models;
using Microsoft.AspNetCore.SignalR;

namespace Hubcon.SignalR.Handlers
{
    public class SignalrCommunicationHandler : ICommunicationHandler
    {
        private protected string TargetClientId { get; private set; } = string.Empty;
        private protected Func<IHubContext<Hub>>? HubContextFactory { get; private set; }
        private protected Func<Hub>? HubFactory { get; private set; }

        public SignalrCommunicationHandler(IHubContext<Hub> hubContextFactory)
        {
            HubContextFactory = () => hubContextFactory;
        }

        public SignalrCommunicationHandler(Hub hubFactory)
        {
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
    }
}
