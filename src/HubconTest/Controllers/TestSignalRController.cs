using Hubcon.SignalR.Models.Interfaces;
using Hubcon.SignalR.Server;

namespace HubconTest.Controllers
{
    public class TestSignalRController : BaseHubController, ISignalRServerContract
    {
        public async Task PrintMessage(string message)
        {
            Console.WriteLine(message);
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
