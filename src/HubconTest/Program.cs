using Hubcon;
using Hubcon.Connectors;
using Hubcon.HubControllers;
using Hubcon.SignalR.Handlers;
using Hubcon.SignalR.Models.Interfaces;
using Hubcon.SignalR.Server;
using HubconTest.Controllers;
using Scalar.AspNetCore;
using Hubcon.SignalR;
using Hubcon.Models.Interfaces;
using HubconTestDomain;


namespace HubconTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.UseHubconSignalR();
            builder.Services.AddHubconClientAccessor();
            builder.Services.AddHubconController<TestSignalRController>();


            var app = builder.Build();

            app.UseHubcon();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<TestSignalRController>("/clienthub");

            //Just a test endpoint, it can also be injected in a controller.
            app.MapGet("/test", async (IClientAccessor<ITestClientController, TestSignalRController> clientAccessor) =>
            {
                var clientId = clientAccessor.GetAllClients().FirstOrDefault()!;
                // Getting some connected clientId

                // Gets a client instance
                var instance = clientAccessor.GetClient(clientId);

                // Using some methods
                string message = "Mensaje de prueba";
                Console.WriteLine($"Servidor: Mensaje enviado a cliente de Hubcon SignalR: {message}");
                var item = await instance.ShowAndReturnMessage(message);
                Console.WriteLine($"Servidor: Mensaje recibido desde el cliente de Hubcon SignalR: {message}");


                return item;
            });

            app.Run();
        }
    }
}
