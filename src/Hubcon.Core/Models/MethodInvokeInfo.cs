using Hubcon.Core.Converters;
using System.ComponentModel;

namespace Hubcon.Core.Models
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class MethodInvokeInfo
    {
        public string MethodName { get; }

        public object?[] Args { get; private set; }

        public MethodInvokeInfo(string methodName, object?[]? args = null)
        {
            MethodName = methodName;
            Args = args ?? [];
        }

        public MethodInvokeInfo SerializeArgs()
        {
            Args = DynamicConverter.SerializeArgs(Args);
            return this;
        }

        public object?[] GetDeserializedArgs(Delegate del)
        {
            return DynamicConverter.DeserializedArgs(del, Args);
        }
    }
}
