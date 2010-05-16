using System.ServiceModel;

namespace Cassia.Tests.Model
{
    [ServiceContract]
    public interface IRemoteDesktopTestService
    {
        [OperationContract]
        void Disconnect(int sessionId);

        [OperationContract]
        int GetLatestSessionId();
    }
}