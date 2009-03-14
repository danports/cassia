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
                    return new TerminalServicesSession(server, SessionHelper.GetCurrentSessionId(server.Handle));
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

        public IList<ITerminalServer> GetServers(string domainName)
        {
            List<ITerminalServer> servers = new List<ITerminalServer>();
            foreach (WTS_SERVER_INFO serverInfo in SessionHelper.EnumerateServers(domainName))
            {
                servers.Add(new TerminalServer(serverInfo.ServerName));
            }
            return servers;
        }

        #endregion
    }
}