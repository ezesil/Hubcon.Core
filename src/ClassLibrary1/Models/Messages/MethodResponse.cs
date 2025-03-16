using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubcon.Core.Models.Messages
{
    internal class MethodResponse : BaseMethodResponse
    {
        public MethodResponse(bool success, object? data = null) : base(success, data)
        {
        }
    }
}
