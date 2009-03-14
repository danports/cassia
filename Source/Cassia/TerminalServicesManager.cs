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
                using (ITerminalServer server = new TerminalServer())
                {
                    server.Open();
                    return
                        SessionHelper.GetSessionInfo(server, server.Handle,
                                                     SessionHelper.GetCurrentSessionId(server.Handle));
                }
            }
        }

        public IList<ITerminalServicesSession> GetSessions(string serverName)
        {
            using (ITerminalServer server = GetRemoteServer(serverName))
            {
                server.Open();
                return server.GetSessions();
            }
        }

        public IList<ITerminalServicesSession> GetSessions()
        {
            using (ITerminalServer server = GetLocalServer())
            {
                server.Open();
                return GetSessions(null);
            }
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