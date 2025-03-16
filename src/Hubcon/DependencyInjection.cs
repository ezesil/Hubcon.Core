using Hubcon.Interfaces;
using Hubcon.Interfaces.Communication;
using Hubcon.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Hubcon
{
    public static class DependencyInjection
    {
        public static WebApplication? UseHubcon(this WebApplication e)
        {
            StaticServiceProvider.Setup(e);
            return e;
        }

        public static IServiceCollection AddHubconController<T>(this IServiceCollection services)
            where T : class, IHubconController, ICommunicationContract
        {
            Type controllerType = typeof(T);
            List<Type> implementationTypes = controllerType
                .GetInterfaces()
                .Where(x => typeof(ICommunicationContract).IsAssignableFrom(x))
                .ToList();

            if (implementationTypes.Count == 0)
                throw new InvalidOperationException($"Controller {controllerType.Name} does not implement any communication contracts.");
            
            foreach (Type implementationType in implementationTypes)
            {
                services.AddScoped(controllerType, implementationType);
            }

            return services;
        }
    }
}
