namespace Cassia.Tests.Model
{
    public class ConnectionDetails
    {
        private string _domain;
        private string _password;
        private string _server;
        private string _username;

        public ConnectionDetails(string server, string username, string domain, string password)
        {
            _server = server;
            _username = username;
            _domain = domain;
            _password = password;
        }

        public ConnectionDetails() {}

        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Domain
        {
            get { return _domain; }
            set { _domain = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
    }
}