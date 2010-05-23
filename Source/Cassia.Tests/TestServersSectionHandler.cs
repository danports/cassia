using System.Configuration;
using System.Xml;

namespace Cassia.Tests
{
    public class TestServersSectionHandler : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler Members

        public object Create(object parent, object configContext, XmlNode section)
        {
            TestServerConfiguration configuration = new TestServerConfiguration();
            foreach (XmlNode childNode in section.ChildNodes)
            {
                string name = childNode.Attributes["name"].Value;
                string domain = childNode.Attributes["domain"].Value;
                string username = childNode.Attributes["username"].Value;
                string password = childNode.Attributes["password"].Value;
                TestServer server = new TestServer(name, domain, username, password);
                configuration.AddServer(server);
            }
            return configuration;
        }

        #endregion
    }
}