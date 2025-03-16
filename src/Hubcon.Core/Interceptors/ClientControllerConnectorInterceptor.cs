using Castle.DynamicProxy;
using Hubcon.Core.Extensions;
using Hubcon.Core.Models.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel;

namespace Hubcon.Core.Interceptors
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class ClientControllerConnectorInterceptor<THub> : AsyncInterceptorBase
        where THub : Hub
    {
        private protected string TargetClientId { get; private set; } = string.Empty;
        private protected Func<IHubContext<THub>>? HubContextFactory { get; private set; }
        private protected Func<THub>? HubFactory { get; private set; }

        public ClientControllerConnectorInterceptor(IHubContext<THub> hubContextFactory)
        {
            HubContextFactory = () => hubContextFactory;
        }

        public ClientControllerConnectorInterceptor(THub hubFactory)
        {
            HubFactory = () => hubFactory;
        }

        public void WithClientId(string clientId)
        {
            TargetClientId = clientId;
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            // Lógica antes de llamar al método original
            TResult? result;
            Hub? hub = HubFactory?.Invoke();
            IHubContext<Hub>? hubContext = HubContextFactory?.Invoke();


            //if (hub == null)
            //{
            //    result = await hubContext!.Clients
            //        .Client(TargetClientId)
            //        .InvokeMethodAsync<TResult?>(invocation.Method.GetMethodSignature(), new CancellationToken(), invocation.Arguments);
            //}
            //else
            //{
            //    result = await hub!.Clients
            //        .Client(TargetClientId)
            //        .InvokeMethodAsync<TResult?>(invocation.Method.GetMethodSignature(), new CancellationToken(), invocation.Arguments);
            //}

            // Convertir el resultado y devolverlo
            invocation.ReturnValue = null!;
            return default!;
        }

        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            Hub? hub = HubFactory?.Invoke();
            IHubContext<Hub>? hubContext = HubContextFactory?.Invoke();

            //if (hub == null)
            //{
            //    await hubContext!.Clients
            //        .Client(TargetClientId)
            //        .CallMethodAsync(invocation.Method.GetMethodSignature(), new CancellationToken(), invocation.Arguments);
            //}
            //else
            //{
            //    await hub.Clients
            //        .Client(TargetClientId)
            //        .CallMethodAsync(invocation.Method.GetMethodSignature(), new CancellationToken(), invocation.Arguments);
            //}
        }
    }
}
