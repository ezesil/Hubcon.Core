using Hubcon.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;
using Hubcon;
using Hubcon.SignalR;
using HubconTest.Controllers;

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

            app.Run();
        }
    }
}
