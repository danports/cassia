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

        public IEnumerable<ServerInfo> Servers
        {
            get { return _configuration.Servers; }
        }

        public IEnumerable<ServerConfiguration> Configurations
        {
            get
            {
                List<ServerConfiguration> configurations = new List<ServerConfiguration>();
                foreach (ServerInfo source in Servers)
                {
                    foreach (ServerInfo target in Servers)
                    {
                        if (source == target)
                        {
                            configurations.Add(new ServerConfiguration(source));
                        }
                        else
                        {
                            if (target.SupportsRemoteAdministration)
                            {
                                configurations.Add(new ServerConfiguration(source, target));
                            }
                        }
                    }
                }
                return configurations;
            }
        }
    }
}