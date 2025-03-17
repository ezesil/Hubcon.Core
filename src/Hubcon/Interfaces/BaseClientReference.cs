using Hubcon.Interfaces.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubcon.Interfaces
{
    public interface IClientReference
    {
        public string Id { get; }
        public object? ClientInfo { get; set; }
    }

    public interface IClientReference<TICommunicationContract> : IClientReference where TICommunicationContract : ICommunicationContract
    {
        public TICommunicationContract ClientController { get; init; }
    }
}
