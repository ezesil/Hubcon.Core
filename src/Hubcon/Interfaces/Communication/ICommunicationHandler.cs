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
        public Task CallAsync();
        public Task<MethodResponse> InvokeAsync();
    }

    public interface IAsyncCommunicationHandler : ICommunicationHandler
    {
        public Task<MethodInvokeInfo> ReceiveAsync();
    }
}
