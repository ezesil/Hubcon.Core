using Hubcon.Core.Extensions;
using Hubcon.Core.Models;
using Hubcon.Core.Models.Interfaces;
using System.ComponentModel;

namespace Hubcon.Core.Connectors
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class HubconClientBuilder<TIHubController>
        where TIHubController : IHubController
    {
        protected Dictionary<string, MethodInvokeInfo> AvailableMethods { get; } = [];

        protected HubconClientBuilder()
        {
            BuildMethods();
        }

        protected void BuildMethods()
        {
            if (AvailableMethods.Count == 0)
            {
                var TType = typeof(TIHubController);

                if (!TType.IsInterface)
                    throw new ArgumentException($"El tipo {typeof(TIHubController).FullName} no es una interfaz.");

                if (!typeof(IHubController).IsAssignableFrom(TType))
                    throw new NotImplementedException($"El tipo {TType.FullName} no implementa la interfaz {nameof(IHubController)} ni es un tipo derivado.");

                foreach (var method in TType.GetMethods())
                {
                    var parameters = method.GetParameters();
                    var methodSignature = method.GetMethodSignature();
                    AvailableMethods.TryAdd(methodSignature, new MethodInvokeInfo(methodSignature, parameters));
                }
            }
        }
    }
}
