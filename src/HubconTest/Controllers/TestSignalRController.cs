using Hubcon.Interfaces.Communication;
using Hubcon.SignalR;
using Hubcon.SignalR.Interfaces;

namespace HubconTest.Controllers
{
    public class TestSignalRController : BaseSignalRHubController, ISignalRContract
    {
        public Task PrintMessage(string message)
        {
            throw new NotImplementedException();
        }

        public Task<string> PrintMessageWithReturn(string message)
        {
            throw new NotImplementedException();
        }

        public void VoidPrintMessage(string message)
        {
            throw new NotImplementedException();
        }

        public string VoidPrintMessageWithReturn(string message)
        {
            throw new NotImplementedException();
        }
    }
}
