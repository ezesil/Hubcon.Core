using Hubcon.Interfaces;
using Hubcon.Interfaces.Communication;

namespace Hubcon.Models.Interfaces
{
    public interface IClientAccessor
    {
        bool TryGetInstance<TICommunicationContract>(Type hubType, string instanceId, out TICommunicationContract? instance)
            where TICommunicationContract : ICommunicationContract?;
        bool GetAllClients<TICommunicationContract>(Type hubType, out IEnumerable<TICommunicationContract>? instance)
            where TICommunicationContract : ICommunicationContract?;
    }

#pragma warning disable S2326 // Unused type parameters should be removed
    public interface IClientAccessor<TICommunicationContract, TICommunicationHandler>
#pragma warning restore S2326 // Unused type parameters should be removed
        where TICommunicationContract : ICommunicationContract?
        where TICommunicationHandler : ICommunicationHandler
    {
        TICommunicationContract GetClient(string instanceId);
        List<string> GetAllClients();
    }

}
