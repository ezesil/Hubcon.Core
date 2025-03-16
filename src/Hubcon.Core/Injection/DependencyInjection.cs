using Microsoft.Extensions.DependencyInjection;
using Hubcon.Core.Models.Interfaces;
using Hubcon.Core.Interceptors;
using Hubcon.Core.Connectors;
using Hubcon.Core.Tools;
using Hubcon.Core.HubControllers;

namespace Hubcon.Core.Injection
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Este metodo inicia la instancia de <typeparamref name="TController"/> que se conecta a la URL indicada.
        /// </summary>
        /// <typeparam name="TController"></typeparam>
        /// <param name="services"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static IServiceCollection AddHubconClientController<TController>(this IServiceCollection services, string url, Action<string>? consoleOutput = null)
            where TController : ClientController
        {
            ClientController controller = InstanceCreator.TryCreateInstance<TController>([url]);
            services.AddHostedService(x => controller);

            return services;
        }

        /// <summary>
        /// Este metodo inicia la instancia de <typeparamref name="TController"/> que se conecta a la URL indicada y
        /// agrega un cliente singleton de tipo <typeparamref name="TServerHubInterface"/> que utiliza <typeparamref name="TController"/> para comunicarse con el servidor.
        /// </summary>
        /// <typeparam name="TController"></typeparam>
        /// <typeparam name="TServerHubInterface"></typeparam>
        /// <param name="services"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static IServiceCollection AddHubconClientController<TController, TServerHubInterface>(this IServiceCollection services, string url, Action<string>? consoleOutput = null)
            where TController : ClientController
            where TServerHubInterface : IServerHubController
        {
            ClientController controller = InstanceCreator.TryCreateInstance<TController>([url]);
            services.AddHostedService(x => controller);

            TServerHubInterface connector = controller.GetConnector<TServerHubInterface>()!;
            services.AddSingleton(typeof(TServerHubInterface), provider => connector);

            return services;
        }

        /// <summary>
        /// Agrega un servicio IClientAccessor como servicio scoped que permite acceder a los clientes de un hub usando la interfaz IClientAccessor<THub, TIClientController>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddHubconClientAccessor(this IServiceCollection services)
        {
            services.AddScoped(typeof(ClientControllerConnectorInterceptor<>));
            services.AddScoped(typeof(IClientAccessor<,>), typeof(ClientConnector<,>));
            return services;
        }
    }
}
