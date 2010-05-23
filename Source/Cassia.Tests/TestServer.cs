namespace Cassia.Tests
{
    public class TestServer
    {
        private readonly string _domain;
        private readonly string _name;
        private readonly string _password;
        private readonly string _username;

        public TestServer(string name, string domain, string username, string password)
        {
            _name = name;
            _domain = domain;
            _username = username;
            _password = password;
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

        public override string ToString()
        {
            return _name;
        }
    }
}