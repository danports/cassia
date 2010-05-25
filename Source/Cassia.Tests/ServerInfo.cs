using Cassia.Tests.Model;

namespace Cassia.Tests
{
    public class ServerInfo
    {
        private readonly string _domain;
        private readonly string _name;
        private readonly string _password;
        private readonly bool _supportsRemoteAdministration;
        private readonly string _username;

        public ServerInfo(string name, string domain, string username, string password,
                          bool supportsRemoteAdministration)
        {
            _name = name;
            _domain = domain;
            _username = username;
            _password = password;
            _supportsRemoteAdministration = supportsRemoteAdministration;
        }

        public bool SupportsRemoteAdministration
        {
            get { return _supportsRemoteAdministration; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Domain
        {
            get { return _domain; }
        }

        public string Username
        {
            get { return _username; }
        }

        public string Password
        {
            get { return _password; }
        }

        public ConnectionDetails ConnectionDetails
        {
            get { return new ConnectionDetails(_name, _username, _domain, _password); }
        }

        public override string ToString()
        {
            return _name;
        }
    }
}