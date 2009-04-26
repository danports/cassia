using System.Collections.Generic;
using System.Diagnostics;
using Cassia.Impl;

namespace Cassia
{
    /// <summary>
    /// The main Cassia class, and the only class you should directly construct from your application code.
    /// Provides a default implementation of <see cref="ITerminalServicesManager" />.
    /// </summary>
    public class TerminalServicesManager : ITerminalServicesManager
    {
        #region ITerminalServicesManager Members

        public ITerminalServicesSession CurrentSession
        {
            get { return new TerminalServicesSession(GetLocalServer(), Process.GetCurrentProcess().SessionId); }
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
            return new TerminalServer(new RemoteServerHandle(serverName));
        }

        public ITerminalServer GetLocalServer()
        {
            return new TerminalServer(new LocalServerHandle());
        }

        public IList<ITerminalServer> GetServers(string domainName)
        {
            List<ITerminalServer> servers = new List<ITerminalServer>();
            foreach (WTS_SERVER_INFO serverInfo in NativeMethodsHelper.EnumerateServers(domainName))
            {
                servers.Add(new TerminalServer(new RemoteServerHandle(serverInfo.ServerName)));
            }
            return servers;
        }

        #endregion
    }
}