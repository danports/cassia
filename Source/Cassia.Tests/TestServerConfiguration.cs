using System.Collections.Generic;

namespace Cassia.Tests
{
    public class TestServerConfiguration
    {
        private readonly List<TestServer> _servers = new List<TestServer>();

        public IEnumerable<TestServer> Servers
        {
            get { return _servers; }
        }

        public void AddServer(TestServer server)
        {
            _servers.Add(server);
        }
    }
}