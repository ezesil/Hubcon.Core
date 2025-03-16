using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubcon.Core.Models.Messages
{
    internal class MethodInvokeInfo : BaseMethodInvokeInfo
    {
        public MethodInvokeInfo(string methodName, object?[]? args = null) : base(methodName, args)
        {
        }
    }
}
