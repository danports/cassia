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

        /// <inheritdoc />
        public ITerminalServicesSession CurrentSession
        {
            get { return new TerminalServicesSession(GetLocalServer(), Process.GetCurrentProcess().SessionId); }
        }

        /// <inheritdoc />
        public ITerminalServicesSession ActiveConsoleSession
        {
            get
            {
                int? sessionId = NativeMethodsHelper.GetActiveConsoleSessionId();
                return sessionId == null ? null : new TerminalServicesSession(GetLocalServer(), sessionId.Value);
            }
        }

        /// <overloads><inheritdoc /></overloads>
        /// <inheritdoc />
        public IList<ITerminalServicesSession> GetSessions(string serverName)
        {
            using (ITerminalServer server = GetRemoteServer(serverName))
            {
                server.Open();
                return server.GetSessions();
            }
        }

        /// <inheritdoc />
        public IList<ITerminalServicesSession> GetSessions()
        {
            using (ITerminalServer server = GetLocalServer())
            {
                server.Open();
                return server.GetSessions();
            }
        }

        /// <inheritdoc />
        public ITerminalServer GetRemoteServer(string serverName)
        {
            return new TerminalServer(new RemoteServerHandle(serverName));
        }

        /// <inheritdoc />
        public ITerminalServer GetLocalServer()
        {
            return new TerminalServer(new LocalServerHandle());
        }

        /// <inheritdoc />
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