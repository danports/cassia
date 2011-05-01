using System.Collections.Generic;
using System.Configuration;

namespace Cassia.Tests
{
    public static class TestSettings
    {
        private static readonly TestServerConfiguration _configuration;

        static TestSettings()
        {
            _configuration = (TestServerConfiguration) ConfigurationManager.GetSection("testServers") ??
                             new TestServerConfiguration();
        }

        public static IEnumerable<ServerConfiguration> Configurations
        {
            get
            {
                var configurations = new List<ServerConfiguration>();
                foreach (ServerInfo source in _configuration.Servers)
                {
                    foreach (ServerInfo target in _configuration.Servers)
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