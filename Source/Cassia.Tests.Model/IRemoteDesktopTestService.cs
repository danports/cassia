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

        [OperationContract]
        ConnectionState GetSessionState(int sessionId);

        [OperationContract]
        void Logoff(int sessionId);

        [OperationContract]
        bool SessionExists(int sessionId);
    }
}