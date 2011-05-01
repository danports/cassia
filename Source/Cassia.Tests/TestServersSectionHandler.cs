using System;
using System.Configuration;
using System.Xml;

namespace Cassia.Tests
{
    public class TestServersSectionHandler : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler Members

        public object Create(object parent, object configContext, XmlNode section)
        {
            var configuration = new TestServerConfiguration();
            foreach (XmlNode childNode in section.ChildNodes)
            {
                // TODO: XML comments in the section totally break this parsing.
                var name = childNode.Attributes["name"].Value;
                var domain = childNode.Attributes["domain"].Value;
                var username = childNode.Attributes["username"].Value;
                var password = childNode.Attributes["password"].Value;
                var remoteAdministration = false;
                var attribute = childNode.Attributes["supportsRemoteAdministration"];
                if (attribute != null)
                {
                    remoteAdministration = Convert.ToBoolean(attribute.Value);
                }
                var server = new ServerInfo(name, domain, username, password, remoteAdministration);
                configuration.AddServer(server);
            }
            return configuration;
        }

        #endregion
    }
}