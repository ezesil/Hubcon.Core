using Hubcon.Handlers;
using Hubcon.Interfaces.Communication;
using Hubcon.Models;
using Hubcon.Tools;

namespace Hubcon.Interfaces
{
    public interface IHubconController
    {

    }

    public interface IHubconController<TCommunicationHandler> : IHubconController where TCommunicationHandler : ICommunicationHandler
    {
        TCommunicationHandler CommunicationHandler { get; set; }
        MethodHandler MethodHandler { get; set; }
        Task<MethodResponse> HandleTask(MethodInvokeRequest info);
        Task HandleVoid(MethodInvokeRequest info);
    }
}
