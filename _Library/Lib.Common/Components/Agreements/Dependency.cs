using System.ServiceModel;

namespace Lib.Common.Components.Agreements
{
    [ServiceContract(Namespace = "http://entry.serviceengine.cross.digiwin.com")]
    public interface ISoapServer
    {
        [OperationContract]
        public string invokeSrv(string in0);
    }

    public interface IProtocol
    {
        public void Start();
    }
}
