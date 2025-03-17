using Hubcon.Interfaces.Communication;
using Hubcon.Models.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Hubcon
{
    public interface IClientManager : IClientAccessor
    {
        TICommunicationContract CreateInstance<THub, TICommunicationContract>(string instanceId)
            where THub : ServerHub
            where TICommunicationContract : ICommunicationContract?;
        TICommunicationContract CreateInstance<TICommunicationContract>(Type hubType, string instanceId)
            where TICommunicationContract : ICommunicationContract?;

        void RemoveInstance(Type hubType, string instanceId);
    }

    public class ServerHub
    {
    }

    public interface IClientManager<TICommunicationContract, TICommunicationHandler> : IClientAccessor<TICommunicationContract, TICommunicationHandler>
        where TICommunicationContract : ICommunicationContract?
        where TICommunicationHandler : ICommunicationHandler
    {
        void RemoveInstance(string instanceId);
    }
}
