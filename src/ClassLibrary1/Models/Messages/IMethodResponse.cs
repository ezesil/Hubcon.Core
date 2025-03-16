using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubcon.Core.Models.Messages
{
    public interface IMethodResponse
    {
        public bool Success { get; set; }
        public object? Data { get; set; }

        public IMethodResponse SerializeData();
        public T? GetDeserializedData<T>();
    }
}
