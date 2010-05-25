namespace Cassia.Tests
{
    public class ServerConfiguration
    {
        private readonly ServerInfo _source;
        private readonly ServerInfo _target;

        public ServerConfiguration(ServerInfo localSource) : this(localSource, null) {}

        public ServerConfiguration(ServerInfo source, ServerInfo target)
        {
            _source = source;
            _target = target;
        }

        public ServerInfo Source
        {
            get { return _source; }
        }

        public ServerInfo Target
        {
            get { return _target; }
        }

        public bool Local
        {
            get { return _target == null; }
        }

        public override string ToString()
        {
            return Local ? _source.ToString() : _source + " => " + _target;
        }
    }
}