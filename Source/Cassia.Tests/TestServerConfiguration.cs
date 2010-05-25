using System.Collections.Generic;

namespace Cassia.Tests
{
    public class TestServerConfiguration
    {
        private readonly List<ServerInfo> _servers = new List<ServerInfo>();

        public IEnumerable<ServerInfo> Servers
        {
            get { return _servers; }
        }

        public void AddServer(ServerInfo server)
        {
            _servers.Add(server);
        }
    }
}