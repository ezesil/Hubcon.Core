using Hubcon.Core.Converters;

namespace Hubcon.Core.Models.Messages
{
    public abstract class BaseMethodInvokeInfo : IMethodInvokeInfo
    {
        public string MethodName { get; }

        public object?[] Args { get; private set; }

        protected BaseMethodInvokeInfo(string methodName, object?[]? args = null)
        {
            MethodName = methodName;
            Args = args ?? [];
        }

        public virtual IMethodInvokeInfo SerializeArgs()
        {
            Args = DynamicConverter.SerializeArgs(Args);
            return this;
        }

        public virtual object?[] GetDeserializedArgs(Delegate del)
        {
            return DynamicConverter.DeserializedArgs(del, Args);
        }
    }
}
