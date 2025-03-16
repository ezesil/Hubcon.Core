using Hubcon.Core.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubcon.Core.CommunicationHandlers
{
    public interface ICommunicationHandler
    {
        bool IsConnected { get; }
        Task OpenAsync(CancellationToken cancellationToken = default);
        Task Close();
        Task InvokeMethod(IMethodInvokeInfo methodInvokeInfo, CancellationToken cancellationToken = default);
        Task<IMethodResponse> InvokeMethod<T>(IMethodInvokeInfo methodInvokeInfo, CancellationToken cancellationToken = default);
        void RegisterHandler(string method, Func<object?, Task<object?>> handler);
    }
}
