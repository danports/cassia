using Cassia.Tests.Model;

namespace Cassia.Tests.Server
{
    public class RemoteDesktopTestService : IRemoteDesktopTestService
    {
        private readonly ITerminalServicesManager _manager = new TerminalServicesManager();

        #region IRemoteDesktopTestService Members

        public void Disconnect(int sessionId)
        {
            _manager.GetLocalServer().GetSession(sessionId).Disconnect();
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

        #endregion
    }
}