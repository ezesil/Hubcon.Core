using Hubcon.Core.HubControllers;
using Microsoft.AspNetCore.SignalR;

namespace Hubcon.Core.Models.Interfaces
{
    public interface IClientManager : IClientAccessor
    {
        TIClientController CreateInstance<THub, TIClientController>(string instanceId)
            where THub : ServerHub
            where TIClientController : IClientController?;
        TIClientController CreateInstance<TIClientController>(Type hubType, string instanceId)
            where TIClientController : IClientController?;

        void RemoveInstance(Type hubType, string instanceId);
    }

    public class ServerHub
    {
    }

    public interface IClientManager<THub, TIClientController> : IClientAccessor<THub, TIClientController>
        where THub : Hub
        where TIClientController : IClientController?
    {
        void RemoveInstance(string instanceId);
    }
}
