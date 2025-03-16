using Hubcon.Core.Converters;

namespace Hubcon.Core.Models.Messages
{
    public class BaseMethodResponse : IMethodResponse
    {
        public bool Success { get; set; } = false;

        public object? Data { get; set; }

        public BaseMethodResponse(bool success, object? data = null)
        {
            Success = success;
            Data = data;
        }

        public IMethodResponse SerializeData()
        {
            if (Data == null) return this;
            Data = DynamicConverter.SerializeData(Data);
            return this;
        }

        public T? GetDeserializedData<T>()
        {
            return DynamicConverter.DeserializeData<T>(Data!);
        }
    }
}
