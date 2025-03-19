using Hubcon.SignalR.Models.Interfaces;

namespace HubconTestClient
{
    internal class Program
    {
        #pragma warning disable S1075 // URIs should not be hardcoded
        private const string Url = "http://localhost:5056/clienthub";
        #pragma warning restore S1075 // URIs should not be hardcoded

        static async Task Main()
        {

            var hub = await new TestHubController(Url).StartInstanceAsync(Console.WriteLine);
            var connector = hub.GetConnector<ISignalRServerContract>();

            await connector.PrintMessage("Mensaje de prueba desde el cliente.");

            Console.ReadKey();
        }
    }
}
