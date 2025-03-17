using Hubcon.SignalR.Models.Interfaces;

namespace HubconTestClient
{
    internal class Program
    {
        #pragma warning disable S1075 // URIs should not be hardcoded
        private const string Url = "http://localhost:5237/clienthub";
        #pragma warning restore S1075 // URIs should not be hardcoded

        static async Task Main()
        {

            var connector = new TestHubController(Url).GetConnector<ISignalRServerContract>();

            await connector.PrintMessage("a1");
            var serverData = await connector.PrintMessageWithReturn("a2");

            Console.WriteLine(serverData);
            Console.ReadKey();
        }
    }
}
