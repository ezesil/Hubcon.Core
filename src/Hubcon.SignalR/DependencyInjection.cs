using Hubcon.Connectors;
using Hubcon.HubControllers;
using Hubcon.Interfaces;
using Hubcon.Interfaces.Communication;
using Hubcon.SignalR.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Hubcon.SignalR
{
    public static class DependencyInjection
    {
        public static WebApplicationBuilder? UseHubconSignalR(this WebApplicationBuilder e)
        {
            e.Services.AddSignalR();
            e.Services.AddScoped<SignalRClientCommunicationHandler>();
            e.Services.AddScoped<SignalRServerCommunicationHandler>();
            return e;
        }

        
    }
}
