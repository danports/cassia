using System.Collections.Generic;

namespace Cassia
{
    public class TerminalServicesManager : ITerminalServicesManager
    {
        #region ITerminalServicesManager Members

        public ITerminalServicesSession CurrentSession
        {
            get
            {
                using (TerminalServerHandle server = new TerminalServerHandle(null))
                {
                    return SessionHelper.GetSessionInfo(server, SessionHelper.GetCurrentSessionId(server));
                }
            }
        }

        public IList<ITerminalServicesSession> GetSessions(string serverName)
        {
            return new TerminalServer(serverName).GetSessions();
        }

        public IList<ITerminalServicesSession> GetSessions()
        {
            return GetSessions(null);
        }

        public ITerminalServer GetRemoteServer(string serverName)
        {
            return new TerminalServer(serverName);
        }

        public ITerminalServer GetLocalServer()
        {
            return new TerminalServer();
        }

        #endregion
    }
}