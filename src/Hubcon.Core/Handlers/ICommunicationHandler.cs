using Hubcon.Core.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubcon.Core.Handlers
{
    public enum CommunicationState
    {
        Disconnected,
        Connecting,
        Connected,
        Disconnecting,
        Reconnecting
    }

    public interface ICommunicationsHandler
    {
        public Task StartAsync(CancellationToken cancellationToken = default);
        public Task StopAsync(CancellationToken cancellationToken = default);
        public CommunicationState State { get; }
        public IMethodHandler MethodHandler { get; }
        public Task<IMethodResponse> InvokeAsync<MethodResponse>(IMethodInvokeInfo methodInvokeInfo, CancellationToken cancellationToken = default);
    }

    public interface IMethodHandler
    {
        public void On(string v, Func<IMethodInvokeInfo, Task> handleWithoutResultAsync);
    }
}
