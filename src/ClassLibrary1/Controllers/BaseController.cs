using Hubcon.Core.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubcon.Core.Controllers
{
    public abstract class BaseController
    {
        protected abstract Task<object?> ExecuteInvocationAsync(IMethodInvokeInfo methodInfo, CancellationToken cancellationToken);

        public async Task<object?> InvokeAsync(IMethodInvokeInfo methodInfo, CancellationToken cancellationToken = default)
        {          


            // Llamar al método de la implementación hija
            return await ExecuteInvocationAsync(methodInfo, cancellationToken);          
        }
    }
}
