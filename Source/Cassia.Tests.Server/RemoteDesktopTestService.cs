using System;
using System.ServiceModel;
using Cassia.Tests.Model;

namespace Cassia.Tests.Server
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RemoteDesktopTestService : IRemoteDesktopTestService
    {
        private readonly ITerminalServicesManager _manager = new TerminalServicesManager();

        #region IRemoteDesktopTestService Members

        public void Disconnect(ConnectionDetails connection, int sessionId)
        {
            using (ImpersonationHelper.Impersonate(connection))
            {
                using (ITerminalServer server = GetServer(connection.Server))
                {
                    server.Open();
                    server.GetSession(sessionId).Disconnect();
                }
            }
        }

        public int GetLatestSessionId()
        {
            ITerminalServicesSession latest = null;
            foreach (ITerminalServicesSession session in _manager.GetLocalServer().GetSessions())
            {
                if (latest == null || latest.ConnectTime == null ||
                    (session.ConnectTime != null && session.ConnectTime > latest.ConnectTime))
                {
                    latest = session;
                }
            }
            return latest == null ? 0 : latest.SessionId;
        }

        public ConnectionState GetSessionState(int sessionId)
        {
            return _manager.GetLocalServer().GetSession(sessionId).ConnectionState;
        }

        public void Logoff(int sessionId)
        {
            _manager.GetLocalServer().GetSession(sessionId).Logoff();
        }

        public bool SessionExists(int sessionId)
        {
            try
            {
                ConnectionState state = _manager.GetLocalServer().GetSession(sessionId).ConnectionState;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        private ITerminalServer GetServer(string server)
        {
            return string.IsNullOrEmpty(server) ? _manager.GetLocalServer() : _manager.GetRemoteServer(server);
        }
    }
}