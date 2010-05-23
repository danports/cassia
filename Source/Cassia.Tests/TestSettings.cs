using System.Collections.Generic;
using System.Configuration;

namespace Cassia.Tests
{
    public class TestSettings
    {
        private static readonly TestServerConfiguration _configuration;

        static TestSettings()
        {
            _configuration = (TestServerConfiguration) ConfigurationManager.GetSection("testServers") ??
                             new TestServerConfiguration();
        }

        public IEnumerable<TestServer> Servers
        {
            get { return _configuration.Servers; }
        }
    }
}