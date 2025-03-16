//using Hubcon.Core.HubControllers;
//using Hubcon.Core.Models.Messages;
//using Microsoft.AspNetCore.SignalR;
//using System.ComponentModel;
//using System.Reflection;

//namespace Hubcon.Core.Extensions
//{
//    [EditorBrowsable(EditorBrowsableState.Never)]
//    internal static class HubconExtensions
//    {
//        public static string GetMethodSignature(this MethodInfo method)
//        {
//            List<string> identifiers = [];
//            identifiers.Add(method.ReturnType.Name);
//            identifiers.Add(method.Name);
//            identifiers.AddRange(method.GetParameters().Select(p => p.ParameterType.Name));
//            var result = string.Join("_", identifiers);
//            return result;
//        }

//        public static async Task<T?> InvokeMethodAsync<T>(this ISingleClientProxy client, string method, CancellationToken cancellationToken = default, params object?[] args)
//        {
//            BaseMethodInvokeInfo request = new BaseMethodInvokeInfo(method, args).SerializeArgs();
//            var result = await client.InvokeAsync<BaseMethodResponse>(method, request, cancellationToken);
//            return result.GetDeserializedData<T?>();
//        }

//        public static async Task CallMethodAsync(this ISingleClientProxy client, string method, CancellationToken cancellationToken = default, params object?[] args)
//        {
//            BaseMethodInvokeInfo request = new BaseMethodInvokeInfo(method, args).SerializeArgs();
//            await client.SendAsync(method, request, cancellationToken);
//        }

//        public static async Task<T?> InvokeServerMethodAsync<T>(this HubConnection client, string method, CancellationToken cancellationToken = default, params object?[] args)
//        {
//            BaseMethodInvokeInfo request = new BaseMethodInvokeInfo(method, args).SerializeArgs();
//            var result = await client.InvokeAsync<BaseMethodResponse>(nameof(ServerHub.HandleTask), request, cancellationToken);
//            return result.GetDeserializedData<T?>();
//        }

//        public static async Task CallServerMethodAsync(this HubConnection client, string method, CancellationToken cancellationToken = default, params object?[] args)
//        {
//            BaseMethodInvokeInfo request = new BaseMethodInvokeInfo(method, args).SerializeArgs();
//            await client.SendAsync(nameof(ServerHub.HandleVoid), request, cancellationToken);
//        }
//    }
//}
