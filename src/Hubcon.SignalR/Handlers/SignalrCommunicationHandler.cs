using Hubcon.Interfaces.Communication;
using Hubcon.Models;

namespace Hubcon.SignalR.Handlers
{
    public class SignalrCommunicationHandler : IAsyncCommunicationHandler
    {
        public Task CallAsync()
        {
            throw new NotImplementedException();
        }

        public Task<MethodResponse> InvokeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<MethodInvokeInfo> ReceiveAsync()
        {
            throw new NotImplementedException();
        }
    }
}
