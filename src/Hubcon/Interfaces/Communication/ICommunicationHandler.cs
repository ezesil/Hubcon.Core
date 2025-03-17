using Hubcon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubcon.Interfaces.Communication
{
    public interface ICommunicationHandler
    {
        public Task<MethodResponse> InvokeAsync(string method, object[] arguments, CancellationToken cancellationToken);
        public Task CallAsync(string method, object[] arguments, CancellationToken cancellationToken);
    }

    public interface IAsyncCommunicationHandler : ICommunicationHandler
    {
        public Task<MethodInvokeRequest> ReceiveAsync();
    }
}
