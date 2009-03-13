using System.Collections.Generic;

namespace Cassia
{
    public class TerminalServer : ITerminalServer
    {
        private readonly string _serverName;

        public TerminalServer() {}

        public TerminalServer(string serverName)
        {
            _serverName = serverName;
        }

        #region ITerminalServer Members

        public IList<ITerminalServicesSession> GetSessions()
        {
            using (TerminalServerHandle server = new TerminalServerHandle(_serverName))
            {
                List<ITerminalServicesSession> results = new List<ITerminalServicesSession>();
                IList<WTS_SESSION_INFO> sessionInfos = SessionHelper.GetSessionInfos(server);
                foreach (WTS_SESSION_INFO sessionInfo in sessionInfos)
                {
                    results.Add(SessionHelper.GetSessionInfo(server, sessionInfo.SessionID));
                }
                return results;
            }
        }

        public ITerminalServicesSession GetSession(int sessionId)
        {
            using (TerminalServerHandle server = new TerminalServerHandle(_serverName))
            {
                return SessionHelper.GetSessionInfo(server, (uint) sessionId);
            }
        }

        #endregion
    }
}