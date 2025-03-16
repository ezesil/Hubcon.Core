//using Hubcon.Core.Handlers;
//using Hubcon.Core.Models.Interfaces;
//using Hubcon.Core.Models.Messages;
//using Microsoft.AspNetCore.SignalR.Client;
//using System.ComponentModel;

//namespace Hubcon.Core.HubControllers
//{
//    [EditorBrowsable(EditorBrowsableState.Never)]
//    public abstract class HubController
//    {
//        protected MethodHandler handler;

//        protected HubController()
//        {
//            handler = new MethodHandler();
//        }

//        protected void Build(HubConnection hubConnection)
//        {
//            Type derivedType = GetType();
//            if (!typeof(IHubController).IsAssignableFrom(derivedType))
//                throw new NotImplementedException($"El tipo {derivedType.FullName} no implementa la interfaz {nameof(IHubController)} o un tipo derivado.");

//            handler.BuildMethods(this, derivedType, (methodSignature, methodInfo, delegado) =>
//            {
//                if (methodInfo.ReturnType == typeof(void))
//                    hubConnection?.On($"{methodSignature}", (Func<BaseMethodInvokeInfo, Task>)handler.HandleWithoutResultAsync);
//                else if (methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
//                    hubConnection?.On($"{methodSignature}", (Func<BaseMethodInvokeInfo, Task<BaseMethodResponse>>)handler.HandleWithResultAsync);
//                else if (methodInfo.ReturnType == typeof(Task))
//                    hubConnection?.On($"{methodSignature}", (Func<BaseMethodInvokeInfo, Task>)handler.HandleWithoutResultAsync);
//                else
//                    hubConnection?.On($"{methodSignature}", (Func<BaseMethodInvokeInfo, Task<BaseMethodResponse>>)handler.HandleWithResultAsync);
//            });
//        }
//    }
//}
