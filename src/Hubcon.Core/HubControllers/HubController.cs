using Hubcon.Core.Handlers;
using Hubcon.Core.Models.Interfaces;
using Hubcon.Core.Models.Messages;
using System.ComponentModel;

namespace Hubcon.Core.HubControllers
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class HubController : IHubController
    {
        protected MethodHandler handler;

        protected HubController()
        {
            handler = new MethodHandler();
        }

        protected void Build(ICommunicationsHandler handler)
        {
            Type derivedType = GetType();
            if (!typeof(IHubController).IsAssignableFrom(derivedType))
                throw new NotImplementedException($"El tipo {derivedType.FullName} no implementa la interfaz {nameof(IHubController)} o un tipo derivado.");

            this.handler.BuildMethods(this, derivedType, (methodSignature, methodInfo, delegado) =>
            {
                if (methodInfo.ReturnType == typeof(void))
                    handler?.MethodHandler.On($"{methodSignature}", this.handler.HandleWithoutResultAsync);
                else if (methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                    handler?.MethodHandler.On($"{methodSignature}", (Func<IMethodInvokeInfo, Task<IMethodResponse>>)this.handler.HandleWithResultAsync);
                else if (methodInfo.ReturnType == typeof(Task))
                    handler?.MethodHandler.On($"{methodSignature}", this.handler.HandleWithoutResultAsync);
                else
                    handler?.MethodHandler.On($"{methodSignature}", (Func<IMethodInvokeInfo, Task<IMethodResponse>>)this.handler.HandleWithResultAsync);
            });
        }
    }
}
