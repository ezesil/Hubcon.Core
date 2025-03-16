namespace Hubcon.Core.Models.Messages
{
    public interface IMethodInvokeInfo
    {
        public string MethodName { get; }

        public object?[] Args { get; }


        public IMethodInvokeInfo SerializeArgs();
        public object?[] GetDeserializedArgs(Delegate del);
    }
}
