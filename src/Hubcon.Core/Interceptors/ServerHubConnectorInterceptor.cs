using Castle.DynamicProxy;
using Hubcon.Core.Extensions;
using Hubcon.Core.Handlers;
using Hubcon.Core.Models.Messages;
using System.Threading;

namespace Hubcon.Core.Interceptors
{
    internal class ServerConnectorInterceptor(ICommunicationsHandler handler) : AsyncInterceptorBase
    {
        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            if (handler.State != CommunicationState.Connected)
                await handler.StartAsync();

            IMethodInvokeInfo request = new MethodInvokeInfo(invocation.Method.GetMethodSignature(), invocation.Arguments).SerializeArgs();
            IMethodResponse? result = await handler.InvokeAsync<IMethodResponse>(request, new CancellationToken());

            // Convertir el resultado y devolverlo
            invocation.ReturnValue = result;
            return result!.GetDeserializedData<TResult>()!;
        }

        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            if (handler.State != CommunicationState.Connected)
                await handler.StartAsync();

            IMethodInvokeInfo request = new MethodInvokeInfo(invocation.Method.GetMethodSignature(), invocation.Arguments).SerializeArgs();
            await handler.InvokeAsync<IMethodResponse>(request, new CancellationToken());
        }
    }
}
