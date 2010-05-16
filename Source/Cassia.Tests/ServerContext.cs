using System;
using System.ServiceModel;
using Cassia.Tests.Model;

namespace Cassia.Tests
{
    public class ServerContext : IDisposable
    {
        private readonly string _server;
        private ChannelFactory<IRemoteDesktopTestService> _channelFactory;
        private IRemoteDesktopTestService _testService;

        public ServerContext(string server)
        {
            _server = server;
        }

        public IRemoteDesktopTestService TestService
        {
            get
            {
                if (_testService == null)
                {
                    NetTcpBinding binding = new NetTcpBinding();
                    binding.Security.Mode = SecurityMode.None;
                    string remoteAddress = String.Format("net.tcp://{0}:17876/CassiaTestService", _server);
                    _channelFactory = new ChannelFactory<IRemoteDesktopTestService>(binding, remoteAddress);
                    _testService = _channelFactory.CreateChannel();
                }
                return _testService;
            }
        }

        public string ServerName
        {
            get { return _server; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _channelFactory.Close();
        }

        #endregion

        public RdpConnection OpenRdpConnection()
        {
            return new RdpConnection(this);
        }
    }
}