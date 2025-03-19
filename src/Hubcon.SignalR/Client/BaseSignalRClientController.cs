using Castle.DynamicProxy;
using Hubcon.Connectors;
using Hubcon.Handlers;
using Hubcon.Interfaces;
using Hubcon.Interfaces.Communication;
using Hubcon.Models;
using Hubcon.SignalR.Server;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace Hubcon.HubControllers
{
    public class SignalRClientCommunicationHandler : ICommunicationHandler
    {
        protected Func<HubConnection> _hubFactory;

        public SignalRClientCommunicationHandler(Func<HubConnection> hubFactory)
        {
            _hubFactory = hubFactory;
        }

        public async Task<MethodResponse> InvokeAsync(string method, object[] arguments, CancellationToken cancellationToken)
        {
            var client = _hubFactory.Invoke();

            if (client.State != HubConnectionState.Connected) await client.StartAsync();

            MethodInvokeRequest request = new MethodInvokeRequest(method, arguments).SerializeArgs();

            return await client.InvokeAsync<MethodResponse>(nameof(IHubconController.HandleTask), request, cancellationToken);
        }

        public async Task CallAsync(string method, object[] arguments, CancellationToken cancellationToken)
        {
            var client = _hubFactory.Invoke();

            MethodInvokeRequest request = new MethodInvokeRequest(method, arguments).SerializeArgs();
            await client.SendAsync(nameof(IHubconController.HandleVoid), request, cancellationToken);
        }

        public List<IClientReference> GetAllClients()
        {
            return Array.Empty<IClientReference>().ToList();
        }

        public ICommunicationHandler WithUserId(string id)
        {
            return this;
        }
    }

    public abstract class BaseSignalRClientController : IHubconController, IHostedService
    {
        protected string _url;
        protected Func<HubConnection> _hubFactory;
        protected CancellationToken _token;
        protected Task? runningTask;
        protected HubConnection hub;

        public ICommunicationHandler CommunicationHandler { get; set; }
        public MethodHandler MethodHandler { get; set; }

        protected BaseSignalRClientController(string url)
        {
            Type derivedType = GetType();
            if (!typeof(IHubconController).IsAssignableFrom(derivedType))
                throw new NotImplementedException($"El tipo {derivedType.FullName} no implementa la interfaz {nameof(IHubconController)} o un tipo derivado.");

            _url = url;
            var hub = new HubConnectionBuilder()
                .WithUrl(url)
                .AddMessagePackProtocol()
                .WithAutomaticReconnect()
                .Build();

            _hubFactory = () => hub;

            CommunicationHandler = new SignalRClientCommunicationHandler(_hubFactory);
            MethodHandler = new MethodHandler();

            MethodHandler.BuildMethods(this, GetType(), (methodSignature, methodInfo, handler) =>
            {
                if (methodInfo.ReturnType == typeof(void))
                    hub?.On($"{methodSignature}", (Func<MethodInvokeRequest, Task>)handler.HandleWithoutResultAsync);
                else if (methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                    hub?.On($"{methodSignature}", (Func<MethodInvokeRequest, Task<MethodResponse>>)handler.HandleWithResultAsync);
                else if (methodInfo.ReturnType == typeof(Task))
                    hub?.On($"{methodSignature}", (Func<MethodInvokeRequest, Task>)handler.HandleWithoutResultAsync);
                else
                    hub?.On($"{methodSignature}", (Func<MethodInvokeRequest, Task<MethodResponse>>)handler.HandleWithResultAsync);
            });


        }

        public TICommunicationContract GetConnector<TICommunicationContract>() 
            where TICommunicationContract : ICommunicationContract

        {
            return new HubconServerConnector<TICommunicationContract, ICommunicationHandler>(CommunicationHandler).GetClient()!;
        }

        public async Task<BaseSignalRClientController> StartInstanceAsync(Action<string>? consoleOutput = null, CancellationToken cancellationToken = default)
        {
            _ = StartAsync(consoleOutput, cancellationToken);

            while (true) 
            { 
                if(hub.State == HubConnectionState.Connected)
                {
                    return this;
                }

                await Task.Delay(500, cancellationToken);
            }          
        }

        public async Task StartAsync(Action<string>? consoleOutput = null, CancellationToken cancellationToken = default)
        {
            hub = _hubFactory.Invoke();
            try
            {
                _token = cancellationToken;

                bool connectedInvoked = false;
                while (true)
                {
                    await Task.Delay(3000, cancellationToken);
                    if (hub.State == HubConnectionState.Connecting)
                    {
                        consoleOutput?.Invoke($"Connecting to {_url}...");
                        _ = hub.StartAsync(_token);
                        connectedInvoked = false;
                    }
                    else if (hub.State == HubConnectionState.Disconnected)
                    {
                        consoleOutput?.Invoke($"Failed connecting to {_url}. Retrying...");
                        _ = hub.StartAsync(_token);
                        connectedInvoked = false;
                    }
                    else if (hub.State == HubConnectionState.Reconnecting)
                    {
                        consoleOutput?.Invoke($"Connection lost, reconnecting to {_url}...");
                        _ = hub.StartAsync(_token);
                        connectedInvoked = false;
                    }
                    else if (hub.State == HubConnectionState.Connected && !connectedInvoked)
                    {
                        consoleOutput?.Invoke($"Successfully connected to {_url}.");
                        connectedInvoked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                consoleOutput?.Invoke($"Error: {ex.Message}");

                if (_token.IsCancellationRequested)
                {
                    consoleOutput?.Invoke("Cancelado.");
                }
            }

            _ = hub?.StopAsync(_token);
        }
        public void Stop()
        {
            var hub = _hubFactory.Invoke();
            _ = hub?.StopAsync(_token);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            runningTask = Task.Run(async () => await StartAsync(Console.WriteLine, cancellationToken));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return runningTask ?? Task.CompletedTask;
        }

        public Task<MethodResponse> HandleTask(MethodInvokeRequest info) => MethodHandler.HandleWithResultAsync(info);
        public Task HandleVoid(MethodInvokeRequest info) => MethodHandler.HandleWithoutResultAsync(info);
    }

    public class BaseSignalRClientController<TICommunicationContract> : BaseSignalRClientController
        where TICommunicationContract : ICommunicationContract
    {
        public TICommunicationContract Server { get; private set; }
        public BaseSignalRClientController(string url) : base(url) => Server = GetConnector<TICommunicationContract>();
    }
}
