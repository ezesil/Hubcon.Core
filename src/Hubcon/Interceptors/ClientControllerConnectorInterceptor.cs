﻿using Castle.DynamicProxy;
using Hubcon.Extensions;
using Hubcon.Interfaces.Communication;
using Hubcon.Models;
using System.ComponentModel;

namespace Hubcon
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ClientControllerConnectorInterceptor : AsyncInterceptorBase
    {
        private protected Func<ICommunicationHandler> HandlerFactory { get; private set; }

        public ClientControllerConnectorInterceptor(ICommunicationHandler handler) => HandlerFactory = () => handler;

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            Console.WriteLine($"[Server][MethodInterceptor] Calling {invocation.Method.Name} on CLIENT. Args: [{string.Join(",", invocation.Arguments.Select(x => $"{x}"))}]");

            MethodResponse response = await HandlerFactory
                .Invoke()
                .InvokeAsync(
                    invocation.Method.GetMethodSignature(),
                    invocation.Arguments,
                    new CancellationToken()
                );

            TResult? result = response.GetDeserializedData<TResult>();

            invocation.ReturnValue = result;
            return result!;
        }

        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            Console.WriteLine($"[Server][MethodInterceptor] Calling {invocation.Method.Name} on CLIENT. Args: [{string.Join(",", invocation.Arguments.Select(x => $"{x}"))}]");

            await HandlerFactory
                .Invoke()
                .CallAsync(
                    invocation.Method.GetMethodSignature(),
                    invocation.Arguments,
                    new CancellationToken()
                );
        }
    }
}
