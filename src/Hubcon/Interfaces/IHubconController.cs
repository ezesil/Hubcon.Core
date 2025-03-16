using Hubcon.Handlers;
using Hubcon.Interfaces.Communication;
using Hubcon.Tools;

namespace Hubcon.Interfaces
{
    public interface IHubconController
    {

    }

    public interface IHubconController<TCommunicationHandler> : IHubconController where TCommunicationHandler : class, ICommunicationHandler
    {
        TCommunicationHandler CommunicationHandler { get; set; }
        TCommunicationHandler GetCommunicationHandler();

        MethodHandler MethodHandler { get; set; }
        MethodHandler GetMethodHandler();

        protected void Initialize();
    }
}
