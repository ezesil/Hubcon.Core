﻿using Castle.DynamicProxy;
using Hubcon.Extensions;
using Hubcon.Interfaces.Communication;
using Hubcon.Models;


namespace Hubcon.Interceptors
{
    internal class ServerConnectorInterceptor : AsyncInterceptorBase
    {
        private readonly ICommunicationHandler handler;

        public ServerConnectorInterceptor(ICommunicationHandler handler)
        {
            this.handler = handler;
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            Console.WriteLine($"[Client][MethodInterceptor] Calling {invocation.Method.Name} on SERVER. Args: [{string.Join(",", invocation.Arguments.Select(x => $"{x}"))}]");

            MethodResponse? result = await handler.InvokeAsync(
                invocation.Method.GetMethodSignature(),
                invocation.Arguments,
                new CancellationToken()
            );

            // Convertir el resultado y devolverlo
            invocation.ReturnValue = result;
            return result!.GetDeserializedData<TResult>()!;
        }

        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            Console.WriteLine($"[Client][MethodInterceptor] Calling {invocation.Method.Name} on SERVER. Args: [{string.Join(",", invocation.Arguments.Select(x => $"{x}"))}]");

            await handler.CallAsync(
                invocation.Method.GetMethodSignature(),
                invocation.Arguments,
                new CancellationToken()
            );
        }
    }
}
