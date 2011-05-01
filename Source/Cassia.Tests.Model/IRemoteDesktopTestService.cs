using System;
using System.ServiceModel;

namespace Cassia.Tests.Model
{
    [ServiceContract]
    public interface IRemoteDesktopTestService
    {
        [OperationContract]
        void Disconnect(ConnectionDetails connection, int sessionId);

        [OperationContract]
        int GetLatestSessionId();

        [OperationContract]
        ConnectionState GetSessionState(ConnectionDetails connection, int sessionId);

        [OperationContract]
        void Logoff(ConnectionDetails connection, int sessionId);

        [OperationContract]
        bool SessionExists(ConnectionDetails connection, int sessionId);

        [OperationContract]
        RemoteMessageBoxResult GetLatestMessageBoxResponse();

        [OperationContract]
        void ClickButtonInWindow(int sessionId, string windowTitle, string button);

        [OperationContract]
        bool WindowWithTitleExists(int sessionId, string windowTitle);

        [OperationContract]
        void StartShowingMessageBox(ConnectionDetails connection, int sessionId, string windowTitle, string text,
                                    RemoteMessageBoxButtons buttons, TimeSpan timeout);
    }
}