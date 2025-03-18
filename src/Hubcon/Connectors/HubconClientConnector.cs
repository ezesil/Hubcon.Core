﻿using Castle.DynamicProxy;
using Hubcon.Interceptors;
using Hubcon.Interfaces;
using Hubcon.Interfaces.Communication;
using Hubcon.Models.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Hubcon.Connectors
{
    /// <summary>
    /// Allows a server to control a client given the client's interface type and a ServerHub type.
    /// This class can be injected.
    /// </summary>
    /// <typeparam name="TICommunicationHandler"></typeparam>
    /// <typeparam name="TICommunicationContract"></typeparam>
    public class HubconClientConnector<TICommunicationContract, TIHubconController> : IClientManager<TICommunicationContract, TIHubconController>
        where TICommunicationContract : ICommunicationContract?
        where TIHubconController : class, IHubconController
    {
#pragma warning disable S2743 // Static fields should not be used in generic types
        private readonly ProxyGenerator ProxyGen = new();
#pragma warning restore S2743 // Static fields should not be used in generic types

        protected Func<ICommunicationHandler> handlerFactory;
        protected Dictionary<string, TICommunicationContract>? clients = new();

        public HubconClientConnector(TIHubconController handler)
        {
            handlerFactory = () => handler.CommunicationHandler;
        }

        protected TICommunicationContract BuildInstance(string instanceId)
        {
            var interceptor = handlerFactory.Invoke();

            return (TICommunicationContract)ProxyGen.CreateInterfaceProxyWithTarget(
                typeof(TICommunicationContract),
                (TICommunicationContract)DynamicImplementationCreator.CreateImplementation(typeof(TICommunicationContract)),
                new ClientControllerConnectorInterceptor(handlerFactory.Invoke())
            );
        }

        public TICommunicationContract GetClient(string instanceId)
        {
            if (clients!.TryGetValue(instanceId, out TICommunicationContract? value))
                return value;

            var client = BuildInstance(instanceId);
            clients.TryAdd(instanceId, client);
            return client;
        }

        public void RemoveInstance(string instanceId)
        {
            clients?.Remove(instanceId, out _);
        }

        public List<string> GetAllClients()
        {
            return handlerFactory
                .Invoke()
                .GetAllClients()
                .Select(x => x.Id)
                .ToList();
        }
    }
}
