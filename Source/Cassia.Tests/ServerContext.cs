using System;
using Cassia.Tests.Model;

namespace Cassia.Tests
{
    public class ServerContext : IDisposable
    {
        private readonly ServerConnection _source;
        private readonly ServerConnection _target;

        public ServerContext(ServerConfiguration config)
        {
            _source = new ServerConnection(config.Source);
            _target = config.Local ? _source : new ServerConnection(config.Target);
        }

        public IRemoteDesktopTestService Source
        {
            get { return _source.TestService; }
        }

        public string TargetName
        {
            get { return _target.Server.Name; }
        }

        public ConnectionDetails TargetConnection
        {
            get { return _target.Server.ConnectionDetails; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _source.Dispose();
            if (_target != _source)
            {
                _target.Dispose();
            }
        }

        #endregion

        public RdpConnection OpenRdpConnection()
        {
            return _target.OpenRdpConnection();
        }
    }
}