using Hubcon.Interfaces.Communication;

namespace Hubcon.SignalR.Interfaces
{
    public interface ISignalRContract : ICommunicationContract
    {
        public Task PrintMessage(string message);
        public void VoidPrintMessage(string message);
        public Task<string> PrintMessageWithReturn(string message);
        public string VoidPrintMessageWithReturn(string message);
    }
}
